using System;
using System.Threading.Tasks;
using AElf.ExceptionHandler;
using AElf.Indexing.Elasticsearch;
using Microsoft.Extensions.Logging;
using PointsServer.Grains.Exceptions;
using PointsServer.Users.Etos;
using PointsServer.Users.Index;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;

namespace PointsServer.EntityEventHandler.Core.IndexHandler;

public class UserInformationHandler : IDistributedEventHandler<UserInformationEto>,
    ITransientDependency
{
    private readonly INESTRepository<UserIndex, Guid> _userRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<UserInformationHandler> _logger;

    public UserInformationHandler(INESTRepository<UserIndex, Guid> userRepository, IObjectMapper objectMapper,
        ILogger<UserInformationHandler> logger)
    {
        _userRepository = userRepository;
        _objectMapper = objectMapper;
        _logger = logger;
    }


    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), LogOnly = true,
        LogTargets = ["eventData"], Message = "UserInformationHandler error")]
    public async Task HandleEventAsync(UserInformationEto eventData)
    {
        if (eventData == null)
        {
            return;
        }

        var contact = _objectMapper.Map<UserInformationEto, UserIndex>(eventData);
        await _userRepository.AddAsync(contact);
        _logger.LogDebug("HandleEventAsync UserInformationEto success");
    }
}