using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AElf;
using AElf.Cryptography;
using AElf.ExceptionHandler;
using AElf.Types;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using PointsServer.Common;
using PointsServer.Common.HttpClient;
using PointsServer.Dto;
using PointsServer.Exceptions;
using PointsServer.Options;
using PointsServer.Users;
using PointsServer.Users.Provider;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace PointsServer;

public class SignatureGrantHandler : ITokenExtensionGrant, ITransientDependency
{
    private readonly IUserInformationProvider _userInformationProvider;
    private readonly ILogger<SignatureGrantHandler> _logger;
    private readonly IAbpDistributedLock _distributedLock;
    private readonly IdentityUserManager _userManager;
    private readonly IOptionsMonitor<TimeRangeOption> _timeRangeOption;
    private readonly IOptionsMonitor<GraphQLOption> _graphQlOption;
    private readonly IHttpProvider _httpProvider;

    private const string LockKeyPrefix = "PointsServer:Auth:SignatureGrantHandler:";
    private const string SourcePortkey = "portkey";
    private const string SourceNightAElf = "nightElf";

    public SignatureGrantHandler(IUserInformationProvider userInformationProvider,
        ILogger<SignatureGrantHandler> logger, IAbpDistributedLock distributedLock,
        IdentityUserManager userManager, IOptionsMonitor<TimeRangeOption> timeRangeOption,
        IOptionsMonitor<GraphQLOption> graphQlOption, IHttpProvider httpProvider)
    {
        _userInformationProvider = userInformationProvider;
        _logger = logger;
        _distributedLock = distributedLock;
        _userManager = userManager;
        _timeRangeOption = timeRangeOption;
        _graphQlOption = graphQlOption;
        _httpProvider = httpProvider;
    }

    public string Name { get; } = "signature";

    [ExceptionHandler([typeof(UserFriendlyException)], TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleExceptionWithInvalidRequest), LogTargets = ["context"],
        Message = "SignatureGrantHandler error")]
    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleExceptionWithServerError), LogTargets = ["context"],
        Message = "SignatureGrantHandler error")]
    public virtual async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        var publicKeyVal = context.Request.GetParameter("publickey").ToString();
        var signatureVal = context.Request.GetParameter("signature").ToString();
        var timestampVal = context.Request.GetParameter("timestamp").ToString();
        var address = context.Request.GetParameter("address").ToString();
        var source = context.Request.GetParameter("source").ToString();

        AssertHelper.NotEmpty(source, "invalid parameter source.");
        AssertHelper.NotEmpty(publicKeyVal, "invalid parameter publickey.");
        AssertHelper.NotEmpty(signatureVal, "invalid parameter signature.");
        AssertHelper.NotEmpty(timestampVal, "invalid parameter timestamp.");
        AssertHelper.NotEmpty(address, "invalid parameter address.");
        AssertHelper.IsTrue(long.TryParse(timestampVal, out var timestamp) && timestamp > 0,
            "invalid parameter timestamp value.");

        var publicKey = ByteArrayHelper.HexStringToByteArray(publicKeyVal);
        var signature = ByteArrayHelper.HexStringToByteArray(signatureVal);
        var signAddress = Address.FromPublicKey(publicKey);

            var newSignText = """
                              Welcome to PixiePoints! Click to connect wallet to and accept its Terms of Service and Privacy Policy. This request will not trigger a blockchain transaction or cost any gas fees.

                              signature: 
                              """ + string.Join("-", address, timestampVal);

            var managerAddress = Address.FromPublicKey(publicKey);
            var userName = string.Empty;
            var caHash = string.Empty;
            var caAddressMain = string.Empty;
            var caAddressSide = new Dictionary<string, string>();

            AssertHelper.IsTrue(CryptoHelper.RecoverPublicKey(signature,
                HashHelper.ComputeFrom(Encoding.UTF8.GetBytes(newSignText).ToHex()).ToByteArray(),
                out var managerPublicKey), "Invalid signature.");

            AssertHelper.IsTrue(CryptoHelper.RecoverPublicKey(signature,
                HashHelper.ComputeFrom(string.Join("-", address, timestampVal)).ToByteArray(),
                out var managerPublicKeyOld), "Invalid signature.");
            AssertHelper.IsTrue(managerPublicKey.ToHex() == publicKeyVal || managerPublicKeyOld.ToHex() == publicKeyVal,
                "Invalid publicKey or signature.");


            var time = DateTime.UnixEpoch.AddMilliseconds(timestamp);
            AssertHelper.IsTrue(
                time > DateTime.UtcNow.AddMinutes(-_timeRangeOption.CurrentValue.TimeRange) &&
                time < DateTime.UtcNow.AddMinutes(_timeRangeOption.CurrentValue.TimeRange),
                "The time should be {} minutes before and after the current time.",
                _timeRangeOption.CurrentValue.TimeRange);

            if (source == SourcePortkey)
            {
                var manager = managerAddress.ToBase58();
                var portkeyUrl = _graphQlOption.CurrentValue.PortkeyUrl;
                var caHolderInfos = await GetCaHolderInfo(portkeyUrl, manager);
                AssertHelper.NotNull(caHolderInfos, "CaHolder not found.");
                AssertHelper.NotEmpty(caHolderInfos.CaHolderManagerInfo, "CaHolder manager not found.");
                if (caHolderInfos.CaHolderManagerInfo.Select(m => m.CaAddress).All(add => add != address))
                {
                    var caHolderManagerInfo = await GetCaHolderManagerInfoAsync(manager);
                    AssertHelper.IsTrue(caHolderManagerInfo != null && caHolderManagerInfo.CaAddress == address,
                        "PublicKey not manager of address");
                }

                //Find caHash by caAddress
                foreach (var account in caHolderInfos.CaHolderManagerInfo)
                {
                    caHash = caHolderInfos.CaHolderManagerInfo[0].CaHash;
                    if (account.ChainId == CommonConstant.MainChainId)
                    {
                        caAddressMain = account.CaAddress;
                    }
                    else
                    {
                        caAddressSide.TryAdd(account.ChainId, account.CaAddress);
                    }
                }

                userName = caHash;
            }
            else if (source == SourceNightAElf)
            {
                AssertHelper.IsTrue(address == signAddress.ToBase58(), "Invalid address or pubkey");
                userName = address;
            }
            else
            {
                throw new UserFriendlyException("Source not support.");
            }

            var user = await _userManager.FindByNameAsync(userName!);
            if (user == null)
            {
                var userId = Guid.NewGuid();
                var createUserResult = await CreateUserAsync(_userManager, _userInformationProvider, userId,
                    address!, caHash, caAddressMain, caAddressSide);
                AssertHelper.IsTrue(createUserResult, "Create user failed.");
                user = await _userManager.GetByIdAsync(userId);
            }
            else
            {
                var userSourceInput = new UserGrainDto
                {
                    Id = user.Id,
                    AelfAddress = caAddressMain,
                    CaHash = caHash,
                    CaAddressMain = caAddressMain,
                    CaAddressSide = caAddressSide,
                };
                await _userInformationProvider.SaveUserSourceAsync(userSourceInput);
            }

            var userClaimsPrincipalFactory = context.HttpContext.RequestServices
                .GetRequiredService<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<IdentityUser>>();
            var signInManager = context.HttpContext.RequestServices
                .GetRequiredService<Microsoft.AspNetCore.Identity.SignInManager<IdentityUser>>();
            var principal = await signInManager.CreateUserPrincipalAsync(user);
            var claimsPrincipal = await userClaimsPrincipalFactory.CreateAsync(user);
            claimsPrincipal.SetScopes("PointsServer");
            claimsPrincipal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));
            claimsPrincipal.SetAudiences("PointsServer");
            // await context.HttpContext.RequestServices.GetRequiredService<AbpOpenIddictClaimDestinationsManager>()
            //     .SetAsync(principal);
            return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal); 
    }



    private async Task<IndexerCAHolderInfos> GetCaHolderInfo(string url, string managerAddress, string? chainId = null)
    {
        using var graphQlClient = new GraphQLHttpClient(url, new NewtonsoftJsonSerializer());

        // It should just one item
        var graphQlRequest = new GraphQLRequest
        {
            Query = @"query(
                    $manager:String
                    $skipCount:Int!,
                    $maxResultCount:Int!
                ) {
                    caHolderManagerInfo(dto: {
                        manager:$manager,
                        skipCount:$skipCount,
                        maxResultCount:$maxResultCount
                    }){
                        chainId,
                        caHash,
                        caAddress,
                        managerInfos{ address }
                    }
                }",
            Variables = new
            {
                chainId = chainId, manager = managerAddress, skipCount = 0, maxResultCount = 10
            }
        };

        var graphQlResponse = await graphQlClient.SendQueryAsync<IndexerCAHolderInfos>(graphQlRequest);
        var indexerCaHolderInfos = graphQlResponse.Data;
        if (!indexerCaHolderInfos.CaHolderManagerInfo.IsNullOrEmpty())
        {
            return indexerCaHolderInfos;
        }

        var caHolderManagerInfo = await GetCaHolderManagerInfoAsync(managerAddress);
        if (caHolderManagerInfo != null)
        {
            indexerCaHolderInfos.CaHolderManagerInfo.Add(caHolderManagerInfo);
        }

        return indexerCaHolderInfos;
    }

    private async Task<CAHolderManager?> GetCaHolderManagerInfoAsync(string manager)
    {
        var portkeyCaHolderInfoUrl = _graphQlOption.CurrentValue.PortkeyCaHolderInfoUrl;

        var apiInfo = new ApiInfo(HttpMethod.Get, "/api/app/account/manager/check");
        var param = new Dictionary<string, string> { { "manager", manager } };
        try
        {
            var resp = await _httpProvider.InvokeAsync<CAHolderManager>(portkeyCaHolderInfoUrl, apiInfo, param: param);
            return resp;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "get ca holder manager info fail.");
            return null;
        }
    }

    private async Task<bool> CreateUserAsync(IdentityUserManager userManager,
        IUserInformationProvider userInformationProvider, Guid userId, string address,
        string caHash, string caAddressMain, Dictionary<string, string> caAddressSide)
    {
        var result = false;
        await using var handle =
            await _distributedLock.TryAcquireAsync(name: LockKeyPrefix + caHash + address);

        //get shared lock
        if (handle == null)
        {
            _logger.LogError("do not get lock, keys already exits. userId: {UserId}", userId.ToString());
            return result;
        }

        var userName = string.IsNullOrEmpty(caHash) ? address : caHash;
        var user = new IdentityUser(userId, userName: userName,
            email: Guid.NewGuid().ToString("N") + "@schrodingernft.ai");
        var identityResult = await userManager.CreateAsync(user);

        if (!identityResult.Succeeded)
        {
            return identityResult.Succeeded;
        }

        _logger.LogDebug("Save user extend info...");
        var userSourceInput = new UserGrainDto()
        {
            Id = userId,
            AelfAddress = address,
            CaHash = caHash,
            CaAddressMain = caAddressMain,
            CaAddressSide = caAddressSide,
        };
        var userGrainDto = await userInformationProvider.SaveUserSourceAsync(userSourceInput);
        _logger.LogDebug("create user success: {UserId}", userId.ToString());

        return identityResult.Succeeded;
    }

    private async Task<IEnumerable<string>> GetResourcesAsync(ExtensionGrantContext context,
        ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in context.HttpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>()
                           .ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }
}