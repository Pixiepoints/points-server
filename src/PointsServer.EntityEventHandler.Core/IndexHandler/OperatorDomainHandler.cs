using System;
using System.Threading.Tasks;
using AElf.ExceptionHandler;
using AElf.Indexing.Elasticsearch;
using Microsoft.Extensions.Logging;
using PointsServer.Apply.Etos;
using PointsServer.Grains.Exceptions;
using PointsServer.Operator;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;

namespace PointsServer.EntityEventHandler.Core.IndexHandler;

public class OperatorDomainHandler : IDistributedEventHandler<OperatorDomainCreateEto>,
    IDistributedEventHandler<OperatorDomainUpdateEto>, ITransientDependency
{
    private readonly INESTRepository<OperatorDomainInfoIndex, string> _operatorDomainRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<OperatorDomainHandler> _logger;

    public OperatorDomainHandler(INESTRepository<OperatorDomainInfoIndex, string> operatorDomainRepository,
        IObjectMapper objectMapper,
        ILogger<OperatorDomainHandler> logger)
    {
        _operatorDomainRepository = operatorDomainRepository;
        _objectMapper = objectMapper;
        _logger = logger;
    }

    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), LogOnly = true,
        LogTargets = ["eventData"], Message = "OperatorDomainHandler create error")]
    public async Task HandleEventAsync(OperatorDomainCreateEto eventData)
    {
        if (eventData == null)
        {
            return;
        }

        var operatorDomainIndex =
            _objectMapper.Map<OperatorDomainCreateEto, OperatorDomainInfoIndex>(eventData);

        await _operatorDomainRepository.AddAsync(operatorDomainIndex);

        _logger.LogDebug("OperatorDomain information add success: {domain}", operatorDomainIndex.Domain);
    }


    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), LogOnly = true,
        LogTargets = ["eventData"], Message = "OperatorDomainHandler update error")]
    public async Task HandleEventAsync(OperatorDomainUpdateEto eventData)
    {
        if (eventData == null)
        {
            return;
        }

        var operatorDomainIndex =
            _objectMapper.Map<OperatorDomainUpdateEto, OperatorDomainInfoIndex>(eventData);

        await _operatorDomainRepository.UpdateAsync(operatorDomainIndex);

        _logger.LogDebug("OperatorDomain information update success: {domain}", operatorDomainIndex.Domain);
    }
}