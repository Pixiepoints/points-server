using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.Logging;
using PointsServer.Common;
using PointsServer.Common.GraphQL;
using PointsServer.Points.Dtos;
using PointsServer.Worker.Provider.Dtos;
using Volo.Abp.DependencyInjection;

namespace PointsServer.Points.Provider;

public interface IPointsProvider
{
    public Task<PointsSumIndexerListDto> GetOperatorPointsSumIndexListAsync(GetOperatorPointsSumIndexListInput input);

    public Task<PointsSumIndexerListDto> GetOperatorPointsSumIndexListByAddressAsync(
        GetOperatorPointsSumIndexListByAddressInput input);

    public Task<Dictionary<string, List<DomainUserRelationShipIndexerDto>>> GetKolFollowersCountDicAsync(List<string> domainList);

    Task<RankingDetailIndexerListDto> GetOperatorPointsActionSumAsync(GetOperatorPointsActionSumInput queryInput);

    public Task<OperatorDomainDto> GetOperatorDomainInfoAsync(GetOperatorDomainInfoInput queryInput);

    Task<List<UserReferralCountDto>> GetUserReferralCountAsync(List<string> addressList, int skipCount = 0,
        int maxResultCount = 1000);

    Task<string> GetUserRegisterDomainByAddressAsync(string address);

    Task<List<UserReferralDto>> GetUserReferralRecordsAsync(List<string> addressList, long skipCount = 0,
        long maxResultCount = 1000);
}

public class PointsProvider : IPointsProvider, ISingletonDependency
{
    private readonly IGraphQlHelper _graphQlHelper;
    private readonly ILogger<PointsProvider> _logger;

    public PointsProvider(IGraphQlHelper graphQlHelper, ILogger<PointsProvider> logger)
    {
        _graphQlHelper = graphQlHelper;
        _logger = logger;
    }

    public async Task<PointsSumIndexerListDto> GetOperatorPointsSumIndexListAsync(
        GetOperatorPointsSumIndexListInput input)
    {
        try
        {
            var indexerResult = await _graphQlHelper.QueryAsync<IndexerRankingListQueryDto>(new GraphQLRequest
            {
                Query =
                    @"query($keyword:String!, $dappId:String!, $sortingKeyWord:SortingKeywordType!, $sorting:String!, $skipCount:Int!,$maxResultCount:Int!){
                    getRankingList(input: {keyword:$keyword,dappId:$dappId,sortingKeyWord:$sortingKeyWord,sorting:$sorting,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalCount,
                        data{
                        domain,
                        address,
                        firstSymbolAmount,
                        secondSymbolAmount,
                        thirdSymbolAmount,
    					fourSymbolAmount,
    					fiveSymbolAmount,
    					sixSymbolAmount,
    					sevenSymbolAmount,
    					eightSymbolAmount,
    					nineSymbolAmount,
    					updateTime,
    					dappName,
    					role,
                    }
                }
            }",
                Variables = new
                {
                    keyword = input.Keyword ?? "", dappId = input.DappName ?? "", sortingKeyWord = input.SortingKeyWord,
                    sorting = input.Sorting, skipCount = input.SkipCount, maxResultCount = input.MaxResultCount,
                }
            });

            return indexerResult.GetRankingList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "getRankingList Indexer error");
            return new PointsSumIndexerListDto();
        }
    }

    public async Task<PointsSumIndexerListDto> GetOperatorPointsSumIndexListByAddressAsync(
        GetOperatorPointsSumIndexListByAddressInput input)
    {
        try
        {
            var indexerResult = await _graphQlHelper.QueryAsync<IndexerPointsEarnedListQueryDto>(new GraphQLRequest
            {
                Query =
                    @"query($address:String!, $dappId:String!, $type:OperatorRole!, $sortingKeyWord:SortingKeywordType!, $sorting:String!, $skipCount:Int!,$maxResultCount:Int!){
                    getPointsEarnedList(input: {address:$address,dappId:$dappId,type:$type,sortingKeyWord:$sortingKeyWord,sorting:$sorting,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalCount,
                        data{
                        domain,
                        address,
                        firstSymbolAmount,
                        secondSymbolAmount,
                        thirdSymbolAmount,
    					fourSymbolAmount,
    					fiveSymbolAmount,
    					sixSymbolAmount,
    					sevenSymbolAmount,
    					eightSymbolAmount,
    					nineSymbolAmount,
    					updateTime,
    					dappName,
    					role,
                    }
                }
            }",
                Variables = new
                {
                    address = input.Address, dappId = input.DappName, type = input.Type,
                    sortingKeyWord = input.SortingKeyWord,
                    sorting = input.Sorting, skipCount = input.SkipCount, maxResultCount = input.MaxResultCount,
                }
            });

            return indexerResult.GetPointsEarnedList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "getPointsEarnedList Indexer error");
            return new PointsSumIndexerListDto();
        }
    }

    public async Task<Dictionary<string, List<DomainUserRelationShipIndexerDto>>> GetKolFollowersCountDicAsync(List<string> domainList)
    {
        var indexerResult = await _graphQlHelper.QueryAsync<DomainUserRelationShipIndexerQuery>(new GraphQLRequest
        {
            Query =
                @"query($domainIn:[String!]!,$addressIn:[String!]!,$dappNameIn:[String!]!,$skipCount:Int!,$maxResultCount:Int!){
                    queryUserAsync(input: {domainIn:$domainIn,addressIn:$addressIn,dappNameIn:$dappNameIn,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalRecordCount
                        data {
                          id
                          domain
                          address
                          dappName
                          createTime
                        }
                }
            }",
            Variables = new
            {
                domainIn = domainList, addressIn = new List<string>(), dappNameIn = new List<string>(), skipCount = 0,
                maxResultCount = Constants.MaxQuerySize
            }
        });

        var result = indexerResult.QueryUserAsync.Data;
        if (result.IsNullOrEmpty())
        {
            return new Dictionary<string, List<DomainUserRelationShipIndexerDto>>();
        }

        return result
            .GroupBy(a => a.Domain)
            .ToDictionary(a => a.Key, a => a.ToList());
    }

    public async Task<RankingDetailIndexerListDto> GetOperatorPointsActionSumAsync(
        GetOperatorPointsActionSumInput queryInput)
    {
        var indexerResult = await _graphQlHelper.QueryAsync<RankingDetailIndexerQueryDto>(new GraphQLRequest
        {
            Query =
                @"query($dappId:String!, $address:String!, $domain:String!, $role:IncomeSourceType){
                    getPointsSumByAction(input: {dappId:$dappId,address:$address,domain:$domain,role:$role}){
                        totalRecordCount,
                        data{
                        id,
                        address,
                        domain,
                        role,
                        dappId,
    					pointsName,
    					actionName,
    					amount,
    					createTime,
    					updateTime
                    }
                }
            }",
            Variables = new
            {
                dappId = queryInput.DappName, address = queryInput.Address, domain = queryInput.Domain,
                role = queryInput.Role
            }
        });
        
        if (indexerResult.GetPointsSumByAction.Data.IsNullOrEmpty())
        {
            return indexerResult.GetPointsSumByAction;
        }
        
        var selfIncreaseActionRecord = indexerResult.GetPointsSumByAction.Data.FirstOrDefault(x => x.ActionName == Constants.SelfIncreaseAction);
        if (selfIncreaseActionRecord != null)
        {
            return indexerResult.GetPointsSumByAction;
        }

        var updateTime = indexerResult.GetPointsSumByAction.Data.MaxBy(x => x.UpdateTime).UpdateTime;
        selfIncreaseActionRecord = new RankingDetailIndexerDto
        {
            Address = queryInput.Address,
            Domain = queryInput.Domain,
            DappId = queryInput.DappName,
            PointsName = "XPSGR-2",
            ActionName = Constants.SelfIncreaseAction,
            Amount = "0",
            CreateTime = updateTime,
            UpdateTime = updateTime
        };
        if (queryInput.Role != null)
        {
            selfIncreaseActionRecord.Role = queryInput.Role.Value;
        }
        indexerResult.GetPointsSumByAction.Data.Add(selfIncreaseActionRecord);

        return indexerResult.GetPointsSumByAction;
    }

    public async Task<OperatorDomainDto> GetOperatorDomainInfoAsync(GetOperatorDomainInfoInput queryInput)
    {
        try
        {
            var indexerResult = await _graphQlHelper.QueryAsync<OperatorDomainIndexerQueryDto>(new GraphQLRequest
            {
                Query =
                    @"query($domain:String!){
                    operatorDomainInfo(input: {domain:$domain}){
                        id,
                        domain,
                        depositAddress,
                        inviterAddress,
    					dappId               
                }
            }",
                Variables = new
                {
                    domain = queryInput.Domain
                }
            });

            return indexerResult?.OperatorDomainInfo;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetOperatorDomainInfoAsync Exception domain:{Domain}", queryInput.Domain);
            return null;
        }
    }

    public async Task<List<UserReferralCountDto>> GetUserReferralCountAsync(List<string> addressList, int skipCount,
        int maxResultCount)
    {
        try
        {
            var indexerResult = await _graphQlHelper.QueryAsync<UserReferralCountResultDto>(new GraphQLRequest
            {
                Query =
                    @"query($referrerList:[String!]!,$skipCount:Int!,$maxResultCount:Int!){
                    getUserReferralCounts(input: {referrerList:$referrerList,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalRecordCount
                        data{referrer,inviteeNumber}
                }
            }",
                Variables = new
                {
                    referrerList = addressList, skipCount = skipCount, maxResultCount = maxResultCount
                }
            });
            return indexerResult.GetUserReferralCounts.Data;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetUserReferralCountAsync error");
            return new List<UserReferralCountDto>();
        }
    }

    public async Task<string> GetUserRegisterDomainByAddressAsync(string address)
    {
        var indexerResult = await _graphQlHelper.QueryAsync<DomainUserRelationShipQuery>(new GraphQLRequest
        {
            Query =
                @"query($domainIn:[String!]!,$addressIn:[String!]!,$dappNameIn:[String!]!,$skipCount:Int!,$maxResultCount:Int!){
                    queryUserAsync(input: {domainIn:$domainIn,addressIn:$addressIn,dappNameIn:$dappNameIn,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalRecordCount
                        data {
                          id
                          domain
                          address
                          dappName
                          createTime
                        }
                }
            }",
            Variables = new
            {
                domainIn = new List<string>(), dappNameIn = new List<string>(),
                addressIn = new List<string>() { address }, skipCount = 0, maxResultCount = 1
            }
        });
        var ans = indexerResult.QueryUserAsync.Data;
        if (ans == null || ans.Count == 0)
        {
            return "";
        }

        return ans[0].Domain;
    }

    public async Task<List<UserReferralDto>> GetUserReferralRecordsAsync(List<string> addressList, long skipCount = 0,
        long maxResultCount = 1000)
    {
        try
        {
            var indexerResult = await _graphQlHelper.QueryAsync<UserReferralQueryResultDto>(new GraphQLRequest
            {
                Query =
                    @"query($referrerList:[String!]!,$skipCount:Int!,$maxResultCount:Int!){
                    getUserReferralRecords(input: {referrerList:$referrerList,skipCount:$skipCount,maxResultCount:$maxResultCount}){
                        totalRecordCount
                        data{
                          domain
                          dappId
                          referrer
                          invitee
                          inviter
                          createTime
                    }
                }
            }",
                Variables = new
                {
                    referrerList = addressList, skipCount = skipCount, maxResultCount = maxResultCount
                }
            });
            return indexerResult.GetUserReferralRecords.Data;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetUserReferralRecordsAsync error");
            return new List<UserReferralDto>();
        }
    }
}