using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.ExceptionHandler;
using AElf.Indexing.Elasticsearch;
using GraphQL;
using Microsoft.Extensions.Logging;
using Nest;
using PointsServer.Common.GraphQL;
using PointsServer.Exceptions;
using PointsServer.Operator;
using PointsServer.Points.Provider;
using Volo.Abp.DependencyInjection;

namespace PointsServer.DApps.Provider;

public interface IOperatorDomainProvider
{
    Task<OperatorDomainDto> GetOperatorDomainIndexAsync(string domain, bool getDescribe = false);
}

public class OperatorDomainProvider : IOperatorDomainProvider, ISingletonDependency
{
    private readonly INESTRepository<OperatorDomainInfoIndex, string> _repository;
    private readonly IGraphQlHelper _graphQlHelper;
    private readonly ILogger<OperatorDomainProvider> _logger;

    public OperatorDomainProvider(INESTRepository<OperatorDomainInfoIndex, string> repository,
        IGraphQlHelper graphQlHelper,
        ILogger<OperatorDomainProvider> logger)
    {
        _repository = repository;
        _graphQlHelper = graphQlHelper;
        _logger = logger;
    }

    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), ReturnDefault = ReturnDefault.New,
        Message = "GetOperatorDomainIndexAsync error")]
    public virtual async Task<OperatorDomainDto> GetOperatorDomainIndexAsync(string domain, bool getDescribe)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            return null;
        }

        var indexerOperatorDomainInfo = await GetIndexerOperatorDomainInfoAsync(domain);

        if (indexerOperatorDomainInfo == null)
        {
            return null;
        }

        if (!getDescribe)
        {
            return indexerOperatorDomainInfo;
        }

        var operatorDomainIndex = await GetOperatorDomainIndexAsync(domain);
        indexerOperatorDomainInfo.Descibe = operatorDomainIndex?.Descibe;

        return indexerOperatorDomainInfo;
    }

    private async Task<OperatorDomainInfoIndex> GetOperatorDomainIndexAsync(string domain)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<OperatorDomainInfoIndex>, QueryContainer>>();
        mustQuery.Add(q => q.Terms(i => i.Field(f => f.Domain)
            .Terms(domain)));

        QueryContainer Filter(QueryContainerDescriptor<OperatorDomainInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        return await _repository.GetAsync(Filter);
    }

    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), ReturnDefault = ReturnDefault.New,
        Message = "GetIndexerOperatorDomainInfoAsync error")]
    protected virtual async Task<OperatorDomainDto> GetIndexerOperatorDomainInfoAsync(string domain)
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
                domain = domain
            }
        });

        return indexerResult?.OperatorDomainInfo;
    }
}