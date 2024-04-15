using Microsoft.Extensions.Logging;
using Orleans;
using PointsServer.Grains.State.InvitationRelationships;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Grains.Grain.InvitationRelationships;

public class InvitationRelationshipsGrain : Grain<InvitationRelationshipsState>, IInvitationRelationshipsGrain
{
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<InvitationRelationshipsGrain> _logger;

    public InvitationRelationshipsGrain(IObjectMapper objectMapper, ILogger<InvitationRelationshipsGrain> logger)
    {
        _objectMapper = objectMapper;
        _logger = logger;
    }

    public override async Task OnActivateAsync()
    {
        await ReadStateAsync();
        await base.OnActivateAsync();
    }

    public override async Task OnDeactivateAsync()
    {
        await WriteStateAsync();
        await base.OnDeactivateAsync();
    }

    public async Task<GrainResultDto<InvitationRelationshipsGrainDto>> AddInvitationRelationshipsAsync(
        InvitationRelationshipsGrainDto dto)
    {
        var result = new GrainResultDto<InvitationRelationshipsGrainDto>();

        try
        {
            if (!State.Id.IsNullOrEmpty())
            {
                result.Message = "InvitationRelationships already exists.";
                return result;
            }

            State.Id = this.GetPrimaryKeyString();
            State.Address = dto.Address;
            State.OperatorAddress = dto.OperatorAddress;
            State.InviterAddress = dto.InviterAddress;
            State.DappName = dto.DappName;
            State.Domain = dto.Domain;
            State.InviteTime = dto.InviteTime.Millisecond;

            await WriteStateAsync();

            result.Success = true;
            result.Data = _objectMapper.Map<InvitationRelationshipsState, InvitationRelationshipsGrainDto>(State);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "add InvitationRelationships error, Address:{Address}", dto.Address);
            result.Message = e.Message;
            return result;
        }
    }
}