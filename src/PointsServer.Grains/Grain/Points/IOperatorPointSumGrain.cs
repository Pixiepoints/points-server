using Orleans;

namespace PointsServer.Grains.Grain.Points;

public interface IOperatorPointSumGrain : IGrainWithStringKey
{
    Task<GrainResultDto<OperatorPointSumGrainDto>> UpdatePointsSumAsync(OperatorPointSumGrainDto grainDto);
}                             