using AElf.ExceptionHandler;
using Microsoft.Extensions.Logging;
using PointsServer.Grains.Exceptions;
using PointsServer.Grains.State.Worker;
using Volo.Abp.ObjectMapping;

namespace PointsServer.Grains.Grain.Worker;

public interface IWorkerOptionGrain : IGrainWithStringKey
{
    Task<GrainResultDto<long>> GetLatestExecuteTimeAsync();
    Task<GrainResultDto<WorkerOptionGrainDto>> UpdateLatestExecuteTimeAsync(WorkerOptionGrainDto dto);
}

public class WorkerOptionGrain : Grain<WorkerOptionState>, IWorkerOptionGrain
{
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<WorkerOptionGrain> _logger;

    public WorkerOptionGrain(IObjectMapper objectMapper, ILogger<WorkerOptionGrain> logger)
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

    public Task<GrainResultDto<long>> GetLatestExecuteTimeAsync()
    {
        var result = new GrainResultDto<long>
        {
            Success = true,
            Data = State.LatestExecuteTime
        };
        return Task.FromResult(result);
    }

    [ExceptionHandler(typeof(Exception), TargetType = typeof(ExceptionHandlingService),
        MethodName = nameof(ExceptionHandlingService.HandleException), ReturnDefault = ReturnDefault.New,
        LogTargets = ["dto"], Message = "UpdateLatestExecuteTimeAsync error")]
    public async Task<GrainResultDto<WorkerOptionGrainDto>> UpdateLatestExecuteTimeAsync(WorkerOptionGrainDto dto)
    {
        var result = new GrainResultDto<WorkerOptionGrainDto>();

        State.Id = this.GetPrimaryKeyString();
        State.Type = dto.Type;
        State.LatestExecuteTime = dto.LatestExecuteTime;

        await WriteStateAsync();

        result.Success = true;
        result.Data = _objectMapper.Map<WorkerOptionState, WorkerOptionGrainDto>(State);
        return result;
    }
}