using AElf.ExceptionHandler;
using Microsoft.Extensions.Logging;
using PointsServer.Common;
using PointsServer.Grains.Exceptions;
using PointsServer.Grains.State.Points;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Grains.Grain.Points;

public class OperatorPointActionSumGrain : Grain<OperatorPointActionSumState>, IOperatorPointActionSumGrain
{
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<OperatorPointActionSumGrain> _logger;

    public OperatorPointActionSumGrain(IObjectMapper objectMapper, ILogger<OperatorPointActionSumGrain> logger)
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
        LogTargets = ["grainDto"], Message = "UpdatePointsActionSumAsync error")]
    public async Task<GrainResultDto<OperatorPointActionSumGrainDto>> UpdatePointsActionSumAsync(
        OperatorPointActionSumGrainDto grainDto)
    {
        var result = new GrainResultDto<OperatorPointActionSumGrainDto>();

        if (!State.Id.IsNullOrEmpty())
        {
            _logger.LogInformation("Add point");
            State.Amount += grainDto.Amount;
            State.UpdateTime = grainDto.UpdateTime;
        }
        else
        {
            var nowMillisecond = DateTime.UtcNow.ToUtcMilliSeconds();
            State.Id = this.GetPrimaryKeyString();
            State.Domain = grainDto.Domain;
            State.DappName = grainDto.DappName;
            State.Address = grainDto.Address;
            State.Role = grainDto.Role;
            State.RecordAction = grainDto.RecordAction;
            State.Amount = grainDto.Amount;
            State.CreateTime = nowMillisecond;
            State.UpdateTime = nowMillisecond;
        }


        await WriteStateAsync();

        result.Success = true;
        result.Data = _objectMapper.Map<OperatorPointActionSumState, OperatorPointActionSumGrainDto>(State);
        return result;
    }
}