using Microsoft.Extensions.Logging;
using Orleans;
using PointsServer.Grains.State.Points;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Grains.Grain.Points;

public class OperatorPointRecordDetailGrain : Grain<OperatorPointRecordDetailState>, IOperatorPointRecordDetailGrain
{
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<OperatorPointRecordDetailGrain> _logger;

    public OperatorPointRecordDetailGrain(IObjectMapper objectMapper, ILogger<OperatorPointRecordDetailGrain> logger)
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

    public async Task<GrainResultDto<PointRecordGrainDto>> PointsRecordAsync(PointRecordGrainDto grainDto)
    {
        var result = new GrainResultDto<PointRecordGrainDto>();

        try
        {
            if (!State.Id.IsNullOrEmpty())
            {
                result.Message = "InvitationRelationships already exists.";
                return result;
            }

            State.Id = this.GetPrimaryKeyString();
            State.Address = grainDto.Address;
            State.Role = grainDto.Role;
            State.Domain = grainDto.Domain;
            State.DappName = grainDto.DappName;
            State.RecordAction = grainDto.RecordAction;
            State.Amount = grainDto.Amount;
            State.PointSymbol = grainDto.PointSymbol;
            State.RecordTime = grainDto.RecordTime;

            await WriteStateAsync();

            result.Success = true;
            result.Data = _objectMapper.Map<OperatorPointRecordDetailState, PointRecordGrainDto>(State);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "add PointsRecord error, Address:{Address}", grainDto.Address);
            result.Message = e.Message;
            return result;
        }
    }
}