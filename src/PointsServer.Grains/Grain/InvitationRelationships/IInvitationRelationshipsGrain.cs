using Orleans;

namespace PointsServer.Grains.Grain.InvitationRelationships;

public interface IInvitationRelationshipsGrain : IGrainWithStringKey
{
    Task<GrainResultDto<InvitationRelationshipsGrainDto>> AddInvitationRelationshipsAsync(
        InvitationRelationshipsGrainDto dto);
}