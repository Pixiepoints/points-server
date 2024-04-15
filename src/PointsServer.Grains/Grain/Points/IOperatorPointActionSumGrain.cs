using Orleans;

namespace PointsServer.Grains.Grain.Points;

public interface IOperatorPointActionSumGrain : IGrainWithStringKey
{
    Task<GrainResultDto<OperatorPointActionSumGrainDto>> UpdatePointsActionSumAsync(OperatorPointActionSumGrainDto grainDto);
}