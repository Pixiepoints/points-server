using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AElf;
using AElf.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Points.Contracts.Point;
using PointsServer.Apply.Dtos;
using PointsServer.Apply.Etos;
using PointsServer.Common;
using PointsServer.Common.AElfSdk;
using PointsServer.DApps.Provider;
using PointsServer.Grains.Grain.Operator;
using PointsServer.Grains.Grain.Worker;
using PointsServer.Grains.State.Worker;
using PointsServer.Options;
using PointsServer.Points.Dtos;
using PointsServer.Points.Provider;
using PointsServer.Users.Provider;
using Portkey.Contracts.CA;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace PointsServer.Apply;

public class ApplyService : PointsPlatformAppService, IApplyService
{
    private readonly IOperatorDomainProvider _operatorDomainProvider;
    private readonly IClusterClient _clusterClient;
    private readonly IDistributedEventBus _distributedEventBus;
    private readonly IObjectMapper _objectMapper;
    private readonly IUserInformationProvider _userInformationProvider;
    private readonly IContractProvider _contractProvider;
    private readonly ApplyConfirmOptions _applyConfirmOptions;
    private readonly InternalWhiteListOptions _internalWhiteListOptions;
    private readonly ILogger<ApplyService> _logger;
    private readonly IPointsProvider _pointsProvider;

    public ApplyService(IOperatorDomainProvider operatorDomainProvider, IClusterClient clusterClient,
        IDistributedEventBus distributedEventBus, IObjectMapper objectMapper,
        IUserInformationProvider userInformationProvider, IContractProvider contractProvider,
        IOptionsSnapshot<ApplyConfirmOptions> applyConfirmOptions,IPointsProvider pointsProvider, ILogger<ApplyService> logger, 
        IOptionsSnapshot<InternalWhiteListOptions> internalWhiteListOptions)
    {
        _operatorDomainProvider = operatorDomainProvider;
        _clusterClient = clusterClient;
        _distributedEventBus = distributedEventBus;
        _objectMapper = objectMapper;
        _userInformationProvider = userInformationProvider;
        _contractProvider = contractProvider;
        _applyConfirmOptions = applyConfirmOptions.Value;
        _logger = logger;
        _internalWhiteListOptions = internalWhiteListOptions.Value;
        _pointsProvider = pointsProvider;
    }

    public async Task<ApplyCheckResultDto> ApplyCheckAsync(ApplyCheckInput input)
    {
        var result = new ApplyCheckResultDto();

        if (await _operatorDomainProvider.GetOperatorDomainIndexAsync(input.Domain) != null)
        {
            result.DomainCheck = "this domain already existed";
        }

        return result;
    }

    public async Task<ApplyConfirmDto> ApplyConfirmAsync(ApplyConfirmInput input)
    {
        var transaction =
            Transaction.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(input.RawTransaction));
        

        var applyToOperatorInput = new ApplyToBeAdvocateInput();
        if (transaction.To.ToBase58() == _applyConfirmOptions.CAContractAddress &&
            transaction.MethodName == "ManagerForwardCall")
        {
            var managerForwardCallInput = ManagerForwardCallInput.Parser.ParseFrom(transaction.Params);
            if (managerForwardCallInput.MethodName == "ApplyToBeAdvocate" &&
                managerForwardCallInput.ContractAddress.ToBase58() == _applyConfirmOptions.PointContractAddress)
            {
                applyToOperatorInput = ApplyToBeAdvocateInput.Parser.ParseFrom(managerForwardCallInput.Args);
            }
        }
        else if (transaction.To.ToBase58() == _applyConfirmOptions.PointContractAddress &&
                 transaction.MethodName == "ApplyToBeAdvocate")
        {
            applyToOperatorInput = ApplyToBeAdvocateInput.Parser.ParseFrom(transaction.Params);
        }
        else
        {
            throw new UserFriendlyException("Invalid transaction");
        }

        var userInfo = await _userInformationProvider.GetUserById(CurrentUser.GetId());

        var dto = new OperatorDomainGrainDto()
        {
            Address = applyToOperatorInput.Invitee.ToBase58(),
            InviterAddress = userInfo.CaAddressMain,
            Role = applyToOperatorInput.Invitee.ToBase58() == userInfo.CaAddressMain
                ? OperatorRole.Kol
                : OperatorRole.Inviter,
            Status = ApplyStatus.Applying,
            Domain = applyToOperatorInput.Domain,
            DappName = applyToOperatorInput.DappId.ToHex(),
            Descibe = input.Describe,
            ApplyTime = DateTime.UtcNow.ToUtcMilliSeconds()
        };

        var operatorDomainGrain = _clusterClient.GetGrain<IOperatorDomainGrain>(dto.Domain);
        var result =
            await operatorDomainGrain.AddOperatorDomainAsync(dto);

        if (!result.Success)
        {
            throw new UserFriendlyException(result.Message);
        }

        await _distributedEventBus.PublishAsync(
            _objectMapper.Map<OperatorDomainGrainDto, OperatorDomainCreateEto>(result.Data));

        var transactionOutput = await _contractProvider.SendTransactionAsync(input.ChainId, transaction);

        return new ApplyConfirmDto
        {
            TransactionId = transactionOutput.TransactionId
        };
    }

    public async Task<DomainCheckDto> DomainCheckAsync(ApplyCheckInput input)
    {
        var domainCheckDto = new DomainCheckDto();
        _logger.LogInformation("DomainCheckAsync:" + input.Domain);
        if (await _operatorDomainProvider.GetOperatorDomainIndexAsync(input.Domain) != null)
        {
            domainCheckDto.Exists = true;
        }

        if (!domainCheckDto.Exists)
        {
            //find indexer
            var operatorDomainDto = await _pointsProvider.GetOperatorDomainInfoAsync(new GetOperatorDomainInfoInput()
            {
                Domain = input.Domain
            });
            if (operatorDomainDto != null)
            {
                domainCheckDto.Exists = true;
                _logger.LogInformation(
                    "DomainCheckAsync:local Es not find,to indexer find, domain: {domain}", operatorDomainDto.Domain);
            }
        }


        return domainCheckDto;
    }

    public async Task<bool> InternalChangeWorkerTimeAsync(long milliseconds)
    {
        var userInfo = await _userInformationProvider.GetUserById(CurrentUser.GetId());

        if (!_internalWhiteListOptions.WhiteList.Contains(userInfo.CaAddressMain))
        {
            throw new Exception("invalid address");
        }
        
        var dto = new WorkerOptionState
        {
            Type = CommonConstant.PointsSumWorker,
            LatestExecuteTime = milliseconds
        };
        var workerOptionGrain = _clusterClient.GetGrain<IWorkerOptionGrain>(dto.Type);
        
        var result = await workerOptionGrain.UpdateLatestExecuteTimeAsync(dto);

        return result.Success;
    }

    public async Task<long> InternalGetWorkerTimeAsync()
    {
        var userInfo = await _userInformationProvider.GetUserById(CurrentUser.GetId());

        if (!_internalWhiteListOptions.WhiteList.Contains(userInfo.CaAddressMain))
        {
            throw new Exception("invalid address");
        }
        
        
        var workerOptionGrain = _clusterClient.GetGrain<IWorkerOptionGrain>(CommonConstant.PointsSumWorker);

        var result = await workerOptionGrain.GetLatestExecuteTimeAsync();

        if (!result.Success)
        {
            throw new UserFriendlyException(result.Message);
        }

        return result.Data;
    }
}