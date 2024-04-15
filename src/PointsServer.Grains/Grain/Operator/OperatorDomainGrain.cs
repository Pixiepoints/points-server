using Microsoft.Extensions.Logging;
using Orleans;
using PointsServer.Common;
using PointsServer.Grains.State.Operator;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Grains.Grain.Operator;

public class OperatorDomainGrain : Grain<OperatorDomainState>, IOperatorDomainGrain
{
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<OperatorDomainGrain> _logger;

    public OperatorDomainGrain(IObjectMapper objectMapper, ILogger<OperatorDomainGrain> logger)
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

    public async Task<GrainResultDto<OperatorDomainGrainDto>> AddOperatorDomainAsync(OperatorDomainGrainDto dto)
    {
        var result = new GrainResultDto<OperatorDomainGrainDto>();

        try
        {
            if (!State.Id.IsNullOrEmpty())
            {
                result.Message = "OperatorDomain already exists.";
                return result;
            }

            State.Id = this.GetPrimaryKeyString();
            State.Address = dto.Address;
            State.InviterAddress = dto.InviterAddress;
            State.Role = dto.Role;
            State.Status = dto.Status;
            State.Domain = dto.Domain;
            State.Icon = dto.Icon;
            State.DappName = dto.DappName;
            State.Descibe = dto.Descibe;
            State.ApplyTime = dto.ApplyTime;

            await WriteStateAsync();

            result.Success = true;
            result.Data = _objectMapper.Map<OperatorDomainState, OperatorDomainGrainDto>(State);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "add InvitationRelationships error, Address:{Address}", dto.Address);
            result.Message = e.Message;
            return result;
        }
    }

    public async Task UpdateApplyStatusAsync(ApplyStatus status)
    {
        State.Status = status;
        await WriteStateAsync();
    }
}