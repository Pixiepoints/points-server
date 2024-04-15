using System;
using System.Threading.Tasks;
using AElf.Indexing.Elasticsearch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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


    public async Task HandleEventAsync(UserInformationEto eventData)
    {
        if (eventData == null)
        {
            return;
        }

        try
        {
            var contact = _objectMapper.Map<UserInformationEto, UserIndex>(eventData);
            await _userRepository.AddAsync(contact);
            _logger.LogDebug("HandleEventAsync UserInformationEto success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", JsonConvert.SerializeObject(eventData));
        }
    }
}