using AElf.ExceptionHandler;
using Microsoft.Extensions.Logging;
using PointsServer.Grains.Exceptions;
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

    // public override async Task OnActivateAsync()
    // {
    //     await ReadStateAsync();
    //     await base.OnActivateAsync();
    // }
    //
    // public override async Task OnDeactivateAsync()
    // {
    //     await WriteStateAsync();
    //     await base.OnDeactivateAsync();
    // }

    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), ReturnDefault = ReturnDefault.New,
        LogTargets = ["grainDto"], Message = "PointsRecordAsync error")]
    public async Task<GrainResultDto<PointRecordGrainDto>> PointsRecordAsync(PointRecordGrainDto grainDto)
    {
        var result = new GrainResultDto<PointRecordGrainDto>();

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
}