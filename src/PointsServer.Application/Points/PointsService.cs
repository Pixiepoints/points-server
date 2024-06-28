using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using PointsServer.Common;
using PointsServer.DApps;
using PointsServer.DApps.Dtos;
using PointsServer.DApps.Provider;
using PointsServer.Options;
using PointsServer.Points.Dtos;
using PointsServer.Points.Provider;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Constants = PointsServer.Common.Constants;

namespace PointsServer.Points;

public class PointsService : IPointsService, ISingletonDependency
{
    private readonly IObjectMapper _objectMapper;
    private readonly IPointsRulesProvider _pointsRulesProvider;
    private readonly IPointsProvider _pointsProvider;
    private readonly ILogger<PointsService> _logger;
    private readonly IOperatorDomainProvider _operatorDomainProvider;
    private readonly IDAppService _dAppService;
    private const int SplitSize = 1;

    public PointsService(IObjectMapper objectMapper, IPointsProvider pointsProvider,
        IPointsRulesProvider pointsRulesProvider, IOperatorDomainProvider operatorDomainProvider,
        ILogger<PointsService> logger, IDAppService dAppService)
    {
        _objectMapper = objectMapper;
        _pointsRulesProvider = pointsRulesProvider;
        _pointsProvider = pointsProvider;
        _operatorDomainProvider = operatorDomainProvider;
        _logger = logger;
        _dAppService = dAppService;
    }

    public async Task<PagedResultDto<RankingListDto>> GetRankingListAsync(GetRankingListInput input)
    {
        _logger.LogInformation("GetRankingListAsync, req:{req}", JsonConvert.SerializeObject(input));
        if (input != null && !CollectionUtilities.IsNullOrEmpty(input.Keyword))
        {
            input.SkipCount = 0;
        }

        var pointsList = await
            _pointsProvider.GetOperatorPointsSumIndexListAsync(
                _objectMapper.Map<GetRankingListInput, GetOperatorPointsSumIndexListInput>(input));

        var resp = new PagedResultDto<RankingListDto>();
        if (pointsList.TotalCount == 0)
        {
            return resp;
        }

        var pointsRules =
            await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, CommonConstant.SelfIncreaseAction);

        var domains = pointsList.Data
            .Select(p => p.Domain).Distinct()
            .ToList();
        var kolFollowersCountDic = await GetKolFollowersCountDicAsync(domains);

        var inviteeAddressList = kolFollowersCountDic.Values
            .SelectMany(l => l.Select(d => d.Address))
            .Distinct()
            .ToList();

        var inviteFollowersCountDic = await GetInviteFollowersCountDicAsync(inviteeAddressList);

        var domainInviteFollowersCountDic = new Dictionary<string, long>();
        foreach (var keyValuePair in kolFollowersCountDic)
        {
            long sum = 0;
            foreach (var domainUserRelationShipIndexerDto in keyValuePair.Value)
            {
                if (inviteFollowersCountDic.TryGetValue(domainUserRelationShipIndexerDto.Address, out var num))
                {
                    sum += num;
                }
            }

            domainInviteFollowersCountDic.Add(keyValuePair.Key, sum);
        }

        var items = new List<RankingListDto>();
        foreach (var index in pointsList.Data)
        {
            var dto = _objectMapper.Map<PointsSumIndexerDto, RankingListDto>(index);
            if (kolFollowersCountDic.TryGetValue(index.Domain, out var followersNumber))
            {
                dto.FollowersNumber = followersNumber.Count;
            }

            if (domainInviteFollowersCountDic.TryGetValue(index.Domain, out var inviteFollowersNumber))
            {
                dto.InviteFollowersNumber = inviteFollowersNumber;
            }

            dto.InviteRate = pointsRules.KOLThirdLevelUserAmount;
            dto.Rate = pointsRules.KolAmount;
            dto.Decimal = pointsRules.Decimal;
            items.Add(dto);
        }

        resp.TotalCount = pointsList.TotalCount;
        resp.Items = items;
        return resp;
    }

    public async Task<RankingDetailDto> GetRankingDetailAsync(GetRankingDetailInput input)
    {
        _logger.LogInformation("GetRankingDetailAsync, req:{req}", JsonConvert.SerializeObject(input));
        var queryInput = _objectMapper.Map<GetRankingDetailInput, GetOperatorPointsActionSumInput>(input);
        queryInput.Role = OperatorRole.Kol;
        var actionRecordPoints = await _pointsProvider.GetOperatorPointsActionSumAsync(queryInput);

        var resp = new RankingDetailDto();
        if (actionRecordPoints.TotalRecordCount == 0)
        {
            return resp;
        }

        var actionPointList =
            _objectMapper.Map<List<RankingDetailIndexerDto>, List<ActionPoints>>(actionRecordPoints.Data)
                .OrderBy(o => o.Symbol).ToList();

        var kolFollowersCountDic =
            await _pointsProvider.GetKolFollowersCountDicAsync(new List<string> { input.Domain });
        var pointsRules =
            await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, CommonConstant.SelfIncreaseAction);
        var inviteeAddressList = kolFollowersCountDic.Values
            .SelectMany(l => l.Select(d => d.Address))
            .Distinct()
            .ToList();
        var inviteFollowersCountDic =
            await GetInviteFollowersCountDicAsync(inviteeAddressList);

        var joinRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.JoinAction);
        var acceptRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.AcceptReferral);
        if (joinRecord != null && acceptRecord != null)
        {
            joinRecord.Amount =
                (BigInteger.Parse(joinRecord.Amount) + BigInteger.Parse(acceptRecord.Amount)).ToString();
            actionPointList.Remove(acceptRecord);
        }

        foreach (var actionPoints in actionPointList)
        {
            if (actionPoints.Action == CommonConstant.SelfIncreaseAction)
            {
                if (kolFollowersCountDic.TryGetValue(input.Domain, out var followersNumber))
                {
                    actionPoints.FollowersNumber = followersNumber.Count;
                }

                actionPoints.InviteFollowersNumber = inviteFollowersCountDic.Values.Sum();
                actionPoints.Rate = pointsRules.KolAmount;
                actionPoints.InviteRate = pointsRules.KOLThirdLevelUserAmount;
            }

            actionPoints.Decimal = pointsRules.Decimal;
            actionPoints.DisplayName = await GetDisplayNameAsync(input.DappName, actionPoints);
        }
        
        actionPointList = actionPointList.OrderByDescending(o =>
        {
            var symbolData = o.Symbol.Split("-");
            if (symbolData.Length != 2)
            {
                return 0;
            }

            return int.Parse(symbolData[1]);
        }).ToList();

        resp.PointDetails = actionPointList;

        var domainInfo = await _operatorDomainProvider.GetOperatorDomainIndexAsync(input.Domain, true);
        if (domainInfo != null)
        {
            resp.Describe = domainInfo.Descibe;
            resp.Icon = GetDappDto(domainInfo.DappId).Icon;
            resp.DappName = GetDappDto(domainInfo.DappId).DappName;
            resp.Domain = domainInfo.Domain;
        }

        return resp;
    }

    public async Task<GetPointsEarnedListDto> GetPointsEarnedListAsync(GetPointsEarnedListInput input)
    {
        _logger.LogInformation("GetPointsEarnedListAsync, req:{req}", JsonConvert.SerializeObject(input));
        var queryInput =
            _objectMapper.Map<GetPointsEarnedListInput, GetOperatorPointsSumIndexListByAddressInput>(input);
        var pointsList = await _pointsProvider.GetOperatorPointsSumIndexListByAddressAsync(queryInput);

        var resp = new GetPointsEarnedListDto();
        if (pointsList.TotalCount == 0)
        {
            return resp;
        }

        var pointsRules =
            await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, CommonConstant.SelfIncreaseAction);
        var domains = pointsList.Data
            .Select(p => p.Domain).Distinct()
            .ToList();
        var kolFollowersCountDic = await GetKolFollowersCountDicAsync(domains);

        var addressList = kolFollowersCountDic.Values
            .SelectMany(p => p.Select(x => x.Address)).Distinct()
            .ToList();
        var inviteFollowersCountDic = await GetInviteFollowersCountDicAsync(addressList);

        var domainInviteFollowersCountDic = new Dictionary<string, long>();
        foreach (var keyValuePair in kolFollowersCountDic)
        {
            long sum = 0;
            foreach (var domainUserRelationShipIndexerDto in keyValuePair.Value)
            {
                if (inviteFollowersCountDic.TryGetValue(domainUserRelationShipIndexerDto.Address, out var num))
                {
                    sum += num;
                }
            }

            domainInviteFollowersCountDic.Add(keyValuePair.Key, sum);
        }

        var items = new List<PointsEarnedListDto>();
        foreach (var operatorPointSumIndex in pointsList.Data)
        {
            var pointsEarnedListDto =
                _objectMapper.Map<PointsSumIndexerDto, PointsEarnedListDto>(operatorPointSumIndex);

            if (kolFollowersCountDic.TryGetValue(operatorPointSumIndex.Domain, out var followersNumber))
            {
                pointsEarnedListDto.FollowersNumber = followersNumber.Count;
            }

            if (pointsEarnedListDto.Role == OperatorRole.Kol)
            {
                if (domainInviteFollowersCountDic.TryGetValue(operatorPointSumIndex.Domain,
                        out var inviteFollowersNumber))
                {
                    pointsEarnedListDto.InviteFollowersNumber = inviteFollowersNumber;
                }

                pointsEarnedListDto.InviteRate = pointsRules.KOLThirdLevelUserAmount;
            }

            pointsEarnedListDto.Rate = pointsEarnedListDto.Role == OperatorRole.Kol
                ? pointsRules.KolAmount
                : pointsRules.InviterAmount;
            pointsEarnedListDto.Decimal = pointsRules.Decimal;

            pointsEarnedListDto.DappName = GetDappDto(operatorPointSumIndex.DappName).DappName;
            pointsEarnedListDto.Icon = GetDappDto(operatorPointSumIndex.DappName).Icon;
            items.Add(pointsEarnedListDto);
        }

        resp.TotalCount = pointsList.TotalCount;
        resp.Items = items;
        return resp;
    }

    public async Task<PointsEarnedDetailDto> GetPointsEarnedDetailAsync(GetPointsEarnedDetailInput input)
    {
        _logger.LogInformation("GetPointsEarnedDetailAsync, req:{req}", JsonConvert.SerializeObject(input));
        var queryInput = _objectMapper.Map<GetPointsEarnedDetailInput, GetOperatorPointsActionSumInput>(input);
        var actionRecordPoints = await _pointsProvider.GetOperatorPointsActionSumAsync(queryInput);

        var resp = new PointsEarnedDetailDto();
        if (actionRecordPoints.TotalRecordCount == 0)
        {
            return resp;
        }

        var actionPointList =
            _objectMapper.Map<List<RankingDetailIndexerDto>, List<ActionPoints>>(actionRecordPoints.Data)
                .OrderBy(o => o.Symbol).ToList();
        var kolFollowersCountDic =
            await _pointsProvider.GetKolFollowersCountDicAsync(new List<string> { input.Domain });
        var pointsRules =
            await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, CommonConstant.SelfIncreaseAction);

        var inviteeAddressList = kolFollowersCountDic.Values
            .SelectMany(l => l.Select(d => d.Address))
            .Distinct()
            .ToList();
        var inviteFollowersCountDic =
            await GetInviteFollowersCountDicAsync(inviteeAddressList);

        var joinRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.JoinAction);
        var acceptRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.AcceptReferral);
        if (joinRecord != null && acceptRecord != null)
        {
            joinRecord.Amount =
                (BigInteger.Parse(joinRecord.Amount) + BigInteger.Parse(acceptRecord.Amount)).ToString();
            actionPointList.Remove(acceptRecord);
        }

        foreach (var actionPoints in actionPointList)
        {
            if (actionPoints.Action == CommonConstant.SelfIncreaseAction)
            {
                if (kolFollowersCountDic.TryGetValue(input.Domain, out var followersNumber))
                {
                    actionPoints.FollowersNumber = followersNumber.Count;
                }

                if (input.Role == OperatorRole.Kol)
                {
                    actionPoints.InviteFollowersNumber = inviteFollowersCountDic.Values.Sum();

                    actionPoints.InviteRate = pointsRules.KOLThirdLevelUserAmount;
                }

                actionPoints.Rate = input.Role == OperatorRole.Kol ? pointsRules.KolAmount : pointsRules.InviterAmount;
            }

            actionPoints.Decimal = pointsRules.Decimal;
            actionPoints.DisplayName = await GetDisplayNameAsync(input.DappName, actionPoints);
        }
        
        actionPointList = actionPointList.OrderByDescending(o =>
        {
            var symbolData = o.Symbol.Split("-");
            if (symbolData.Length != 2)
            {
                return 0;
            }

            return int.Parse(symbolData[1]);
        }).ToList();

        resp.PointDetails = actionPointList;

        var domainInfo = await _operatorDomainProvider.GetOperatorDomainIndexAsync(input.Domain, true);
        if (domainInfo != null)
        {
            resp.Describe = domainInfo.Descibe;
            resp.Icon = GetDappDto(domainInfo.DappId).Icon;
            resp.DappName = GetDappDto(domainInfo.DappId).DappName;
            resp.Domain = domainInfo.Domain;
        }

        return resp;
    }

    private async Task<string> GetDisplayNameAsync(string dappName, ActionPoints actionPoints)
    {
        PointsRules pointsRules;
        switch (actionPoints.Action)
        {
            case Constants.JoinAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.SelfIncreaseAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;

            case Constants.ApplyToBeAdvocateAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.CommunityInteractionAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.AdoptAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
                break;
            case Constants.RerollAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.TradeAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.TradeGen0Action:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.SGRHoldingAction:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.AwakenLpHolding:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            case Constants.UniswapLpHolding:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, actionPoints.Action);
                if (pointsRules == null) break;
                return pointsRules.DisplayNamePattern;
            default:
                pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(dappName, Constants.DefaultAction);
                if (pointsRules == null) break;
                return Strings.Format(pointsRules.DisplayNamePattern, actionPoints.Action);
        }

        return "";
    }

    public async Task<MyPointDetailsDto> GetMyPointsAsync(GetMyPointsInput input)
    {
        var domain = await _pointsProvider.GetUserRegisterDomainByAddressAsync(input.Address);
        if (domain == null)
        {
            return new MyPointDetailsDto();
        }

        input.Domain = domain;
        _logger.LogInformation("GetMyPointsAsync, req:{req}", JsonConvert.SerializeObject(input));
        var queryInput = _objectMapper.Map<GetMyPointsInput, GetOperatorPointsActionSumInput>(input);
        queryInput.Role = OperatorRole.User;
        var actionRecordPoints = await _pointsProvider.GetOperatorPointsActionSumAsync(queryInput);
        var resp = new MyPointDetailsDto();
    

        //query master domain
        var schrodingerDAppDto = GetDappDto(input.DappName);
        var schrodingerDomain = schrodingerDAppDto?.Link.Replace("https://", "");
        if (!CollectionUtilities.IsNullOrEmpty(schrodingerDomain) && !schrodingerDomain.Equals(domain) &&
            !CollectionUtilities.IsNullOrEmpty(queryInput.Domain))
        {
            queryInput.Domain = schrodingerDomain;
            var schrodingerRecordPoints = await _pointsProvider.GetOperatorPointsActionSumAsync(queryInput);
            if (schrodingerRecordPoints != null && schrodingerRecordPoints.TotalRecordCount > 0)
            {
                actionRecordPoints.TotalRecordCount += schrodingerRecordPoints.TotalRecordCount;
                actionRecordPoints.Data.AddRange(schrodingerRecordPoints.Data);
            }
        }

        //merge same actionName
        actionRecordPoints.Data = actionRecordPoints.Data.GroupBy(x => x.ActionName)
            .Select(group => new RankingDetailIndexerDto
            {
                ActionName = group.Key,
                Amount = group.Aggregate(BigInteger.Zero, (sum, x) => sum + BigInteger.Parse(x.Amount)).ToString(),
                Id = group.First().Id,
                Address = group.First().Address,
                Domain = group.First().Domain,
                Role = group.First().Role,
                DappId = group.First().DappId,
                PointsName = group.First().PointsName,
                SymbolName = group.First().SymbolName,
                CreateTime = group.First().CreateTime,
                UpdateTime = group.First().UpdateTime
            })
            .ToList();

        var actionPointList =
            _objectMapper.Map<List<RankingDetailIndexerDto>, List<EarnedPointDto>>(actionRecordPoints.Data)
                .OrderBy(o => o.Symbol).ToList();

        var joinRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.JoinAction);
        var acceptRecord = actionPointList.FirstOrDefault(x => x.Action == Constants.AcceptReferral);
        if (joinRecord != null && acceptRecord != null)
        {
            joinRecord.Amount =
                (BigInteger.Parse(joinRecord.Amount) + BigInteger.Parse(acceptRecord.Amount)).ToString();
            actionPointList.Remove(acceptRecord);
        }
        
        var tenSymbolList = actionPointList.Where(x => x.Action == Constants.UniswapLpHolding)
            .ToList();
        if (tenSymbolList.Count == 0)
        {
            var tenPoint = new EarnedPointDto
            {
                Amount = "0",
                Action = Constants.UniswapLpHolding,
            };
            actionPointList.Add(tenPoint);
        }
        
        foreach (var earnedPointDto in actionPointList)
        {
            PointsRules pointsRules;
            switch (earnedPointDto.Action)
            {
                case Constants.JoinAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.SelfIncreaseAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Rate = pointsRules.UserAmount;
                    await SetUserLowerLevelRate(input.Address, earnedPointDto, pointsRules);
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.ApplyToBeAdvocateAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.CommunityInteractionAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.AdoptAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.RerollAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.TradeAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.TradeGen0Action:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.SGRHoldingAction:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.AwakenLpHolding:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    break;
                case Constants.UniswapLpHolding:
                    pointsRules = await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, earnedPointDto.Action);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = pointsRules.DisplayNamePattern;
                    earnedPointDto.Symbol = pointsRules.Symbol;
                    break;
                default:
                    pointsRules =
                        await _pointsRulesProvider.GetPointsRulesAsync(input.DappName, Constants.DefaultAction);
                    if (pointsRules == null) continue;
                    earnedPointDto.Decimal = pointsRules.Decimal;
                    earnedPointDto.DisplayName = Strings.Format(pointsRules.DisplayNamePattern, earnedPointDto.Action);
                    break;
            }
        }
        
        actionPointList = await _pointsRulesProvider.AddAllPoints(actionPointList, input.DappName);
        resp.PointDetails.AddRange(actionPointList);

        _logger.LogInformation("GetMyPointsAsync, resp:{resp}", JsonConvert.SerializeObject(resp));
        return resp;
    }

    public async Task<List<PointsListDto>> GetPointsListAsync(GetPointsListInput input)
    {
        var pointsSumIndexerDtos = await _pointsProvider.GetPointsSumListAsync(input);
        return _objectMapper.Map<List<PointsSumAllIndexerDto>, List<PointsListDto>>(pointsSumIndexerDtos);
    }

    public async Task<List<RelationshipDto>> GetRelationshipListAsync(GetRelationshipInput input)
    {
        //inviter -> kol -> user
        var inviterKolFollowerNumDic = await GetInviterKolFollowerNumDic(input);
        //kol -> user -> user
        var kolFollowerNumDic = await GetKolFollowerNumDic(input);
        //user -> user -> user
        var inviteeNumDic = await GetInviteeNumDic(input);

        return input.AddressList.Select(address => new RelationshipDto
        {
            Address = address,
            InviterKolFollowerNum = inviterKolFollowerNumDic.GetOrDefault(address),
            KolFollowerNum = kolFollowerNumDic.GetOrDefault(address).KolFollowerNum,
            KolFollowerInviteeNum = kolFollowerNumDic.GetOrDefault(address).KolFollowerInviteeNum,
            InviteeNum = inviteeNumDic.GetOrDefault(address).InviteeNum,
            SecondInviteeNum = inviteeNumDic.GetOrDefault(address).SecondInviteeNum,
        }).ToList();
    }

    private async Task<Dictionary<string, InviteeNumDto>> GetInviteeNumDic(GetRelationshipInput input)
    {
        var secondLevelFollowersCountDic =
            await GetNextLevelUserAddressList(input.AddressList);
        var thirdFollowerAddress = secondLevelFollowersCountDic.Values
            .SelectMany(p => p.Select(x => x.Referrer))
            .Distinct()
            .ToList();

        var thirdLevelFollowersCountDic =
            await GetNextLevelUserAddressList(thirdFollowerAddress);

        var inviteeNumDic = new Dictionary<string, InviteeNumDto>();
        foreach (var keyValuePair in secondLevelFollowersCountDic)
        {
            long secondInviteeNum = 0;
            foreach (var userReferralDto in keyValuePair.Value)
            {
                if (thirdLevelFollowersCountDic.TryGetValue(userReferralDto.Referrer, out var list))
                {
                    secondInviteeNum += list.Count;
                }
            }

            var inviteeNumDto = new InviteeNumDto
            {
                InviteeNum = keyValuePair.Value.Count,
                SecondInviteeNum = secondInviteeNum
            };
            inviteeNumDic.Add(keyValuePair.Key, inviteeNumDto);
        }

        return inviteeNumDic;
    }

    private async Task<Dictionary<string, KolFollowerNumDto>> GetKolFollowerNumDic(GetRelationshipInput input)
    {
        var domainInfoList = await GetOperatorDomainListAsync(input.ChainId, input.AddressList, false);
        var inviterFollowerDomainDic = domainInfoList
            .GroupBy(a => a.InviterAddress)
            .ToDictionary(a => a.Key, a => a.ToList());
        var domains = inviterFollowerDomainDic.Values
            .SelectMany(p => p.Select(x => x.Domain)).Distinct()
            .ToList();
        var domainFollowersDic = await GetKolFollowersCountDicAsync(domains);

        var domainFollowerInviteeAddressList = domainFollowersDic.Values
            .SelectMany(l => l.Select(d => d.Address))
            .Distinct()
            .ToList();

        var domainFollowerInviteeCountDic = await GetInviteFollowersCountDicAsync(domainFollowerInviteeAddressList);

        var kolFollowerNumDic = new Dictionary<string, KolFollowerNumDto>();
        foreach (var keyValuePair in inviterFollowerDomainDic)
        {
            long kolFollowerNum = 0;
            long kolFollowerInviteeNum = 0;
            foreach (var operatorDomainDto in keyValuePair.Value)
            {
                if (!domainFollowersDic.TryGetValue(operatorDomainDto.Domain, out var list))
                {
                    continue;
                }

                kolFollowerNum += list.Count;
                foreach (var dto in list)
                {
                    if (domainFollowerInviteeCountDic.TryGetValue(dto.Address, out var inviteeNum))
                    {
                        kolFollowerInviteeNum += inviteeNum;
                    }
                }
            }

            var kolFollowerNumDto = new KolFollowerNumDto
            {
                KolFollowerNum = kolFollowerNum,
                KolFollowerInviteeNum = kolFollowerInviteeNum
            };
            kolFollowerNumDic.Add(keyValuePair.Key, kolFollowerNumDto);
        }

        return kolFollowerNumDic;
    }

    private async Task<Dictionary<string, long>> GetInviterKolFollowerNumDic(GetRelationshipInput input)
    {
        var domainInfoList = await GetOperatorDomainListAsync(input.ChainId, input.AddressList);
        var inviterFollowerDomainDic = domainInfoList
            .GroupBy(a => a.InviterAddress)
            .ToDictionary(a => a.Key, a => a.ToList());
        var domains = inviterFollowerDomainDic.Values
            .SelectMany(p => p.Select(x => x.Domain)).Distinct()
            .ToList();
        var domainFollowersDic = await GetKolFollowersCountDicAsync(domains);
        var inviterKolFollowerNumDic = new Dictionary<string, long>();
        foreach (var keyValuePair in inviterFollowerDomainDic)
        {
            long sum = 0;
            foreach (var operatorDomainDto in keyValuePair.Value)
            {
                if (domainFollowersDic.TryGetValue(operatorDomainDto.Domain, out var list))
                {
                    sum += list.Count;
                }
            }

            inviterKolFollowerNumDic.Add(keyValuePair.Key, sum);
        }

        return inviterKolFollowerNumDic;
    }

    private async Task<List<OperatorDomainDto>> GetOperatorDomainListAsync(string chainId, List<string> addressList,
        bool isInviter = true)
    {
        var res = new List<OperatorDomainDto>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<OperatorDomainDto> list;
        do
        {
            list = await _pointsProvider.GetOperatorDomainListAsync(chainId, addressList);
            var count = list.Count;
            res.AddRange(list);
            if (list.IsNullOrEmpty() || count < maxResultCount)
            {
                break;
            }

            skipCount += count;
        } while (!list.IsNullOrEmpty());

        return isInviter
            ? res.Where(x => x.DepositAddress != x.InviterAddress).ToList()
            : res.Where(x => x.DepositAddress == x.InviterAddress).ToList();
    }

    private async Task SetUserLowerLevelRate(string address, EarnedPointDto earnedPointDto, PointsRules pointsRules)
    {
        var secondLevelFollowersCountDic =
            await GetNextLevelUserAddressList(new List<string> { address });
        var inviteFollowersNumber = secondLevelFollowersCountDic.TryGetValue(address, out var secondLevelFollowers)
            ? secondLevelFollowers.Count
            : 0;
        earnedPointDto.InviteFollowersNumber = inviteFollowersNumber;
        earnedPointDto.InviteRate = pointsRules.SecondLevelUserAmount;

        if (secondLevelFollowers is { Count: > 0 })
        {
            var thirdLevelAddress = secondLevelFollowers.Select(s => s.Invitee).ToList();
            var thirdLevelFollowersCountDic =
                await GetNextLevelUserAddressList(thirdLevelAddress);
            earnedPointDto.ThirdFollowersNumber = thirdLevelFollowersCountDic.Values.Sum(list => list.Count);
            earnedPointDto.ThirdRate = pointsRules.ThirdLevelUserAmount;
        }
    }


    private async Task<Dictionary<string, List<UserReferralDto>>> GetNextLevelUserAddressList(List<string> addressList)
    {
        var res = new List<UserReferralDto>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<UserReferralDto> list;
        do
        {
            list = await _pointsProvider.GetUserReferralRecordsAsync(addressList, skipCount, maxResultCount);
            var count = list.Count;
            res.AddRange(list);
            if (list.IsNullOrEmpty() || count < maxResultCount)
            {
                break;
            }

            skipCount += count;
        } while (!list.IsNullOrEmpty());

        return res.GroupBy(u => u.Referrer)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<Dictionary<string, long>> GetInviteFollowersCountDicAsync(List<string> addressList)
    {
        var res = new List<UserReferralCountDto>();
        var skipCount = 0;
        var maxResultCount = 5000;
        List<UserReferralCountDto> list;
        do
        {
            list = await _pointsProvider.GetUserReferralCountAsync(addressList, skipCount, maxResultCount);
            var count = list.Count;
            res.AddRange(list);
            if (list.IsNullOrEmpty() || count < maxResultCount)
            {
                break;
            }

            skipCount += count;
        } while (!list.IsNullOrEmpty());

        return res.GroupBy(u => u.Referrer)
            .ToDictionary(g => g.Key, g => g.First().InviteeNumber);
    }

    private async Task<Dictionary<string, List<DomainUserRelationShipIndexerDto>>> GetKolFollowersCountDicAsync(
        List<string> domains)
    {
        var splitDomainList = SplitDomainList(domains);
        var kolFollowersCountDic = new Dictionary<string, List<DomainUserRelationShipIndexerDto>>();
        var tasks = splitDomainList.Select(domainList => _pointsProvider.GetKolFollowersCountDicAsync(domainList));
        var taskResults = await Task.WhenAll(tasks);
        foreach (var result in taskResults)
        {
            kolFollowersCountDic.AddIfNotContains(result);
        }

        return kolFollowersCountDic;
    }

    private static List<List<string>> SplitDomainList(List<string> domains)
    {
        var splitList = new List<List<string>>();

        for (var i = 0; i < domains.Count; i += SplitSize)
        {
            var sublist = domains.Skip(i).Take(SplitSize).ToList();
            splitList.Add(sublist);
        }

        return splitList;
    }

    private DAppDto GetDappDto(string dappId)
    {
        var dappIdDic = _dAppService.GetDappIdDic();
        return dappIdDic[dappId];
    }
}