using System;
using System.Threading.Tasks;
using Orleans;
using PointsServer.Grains.Grain.Users;
using PointsServer.Users.Etos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Users.Provider;

public class UserInformationProvider : IUserInformationProvider, ISingletonDependency

{
    private readonly IClusterClient _clusterClient;
    private readonly IObjectMapper _objectMapper;
    private readonly IDistributedEventBus _distributedEventBus;

    public UserInformationProvider(IClusterClient clusterClient, IObjectMapper objectMapper,
        IDistributedEventBus distributedEventBus)
    {
        _clusterClient = clusterClient;
        _objectMapper = objectMapper;
        _distributedEventBus = distributedEventBus;
    }

    public async Task<UserGrainDto> GetUserById(Guid id)
    {
        var userGrain = _clusterClient.GetGrain<IUserGrain>(id);
        var resp = await userGrain.GetUserAsync();
        return resp.Success ? resp.Data : null;
    }

    public async Task<UserGrainDto> SaveUserSourceAsync(UserGrainDto userSourceInput)
    {
        var userGrain = _clusterClient.GetGrain<IUserGrain>(userSourceInput.Id);
        var result = await userGrain.UpdateUserAsync(userSourceInput);
        await _distributedEventBus.PublishAsync(
            _objectMapper.Map<UserGrainDto, UserInformationEto>(result.Data));
        return result.Data;
    }
}