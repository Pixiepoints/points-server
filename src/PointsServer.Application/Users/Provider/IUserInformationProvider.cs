using System;
using System.Threading.Tasks;

namespace PointsServer.Users.Provider;

public interface IUserInformationProvider
{
    Task<UserGrainDto> SaveUserSourceAsync(UserGrainDto userSourceInput);
    Task<UserGrainDto> GetUserById(Guid id);
}