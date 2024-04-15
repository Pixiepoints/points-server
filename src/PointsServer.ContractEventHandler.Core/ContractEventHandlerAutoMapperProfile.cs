using AutoMapper;
using PointsServer.Users.Etos;
using PointsServer.Users.Index;

namespace PointsServer.ContractEventHandler
{
    public class ContractEventHandlerAutoMapperProfile : Profile
    {
        public ContractEventHandlerAutoMapperProfile()
        {
            CreateMap<UserInformationEto, UserIndex>();
        }
    }
}