using AutoMapper;
using PointsServer.Grains.Grain.InvitationRelationships;
using PointsServer.Grains.Grain.Operator;
using PointsServer.Grains.State.InvitationRelationships;
using PointsServer.Grains.State.Operator;
using PointsServer.Grains.State.Users;
using PointsServer.Users;
using PointsServer.Users.Etos;

namespace PointsServer.Grains;

public class SymbolMarketGrainsAutoMapperProfile : Profile
{
    public SymbolMarketGrainsAutoMapperProfile()
    {
        CreateMap<UserGrainDto, UserState>().ReverseMap();
        CreateMap<UserGrainDto, UserInformationEto>().ReverseMap();
        CreateMap<InvitationRelationshipsState, InvitationRelationshipsGrainDto>().ReverseMap();
        CreateMap<OperatorDomainState, OperatorDomainGrainDto>().ReverseMap();
    }
}