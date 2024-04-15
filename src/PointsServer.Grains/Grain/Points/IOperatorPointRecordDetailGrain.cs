using Orleans;

namespace PointsServer.Grains.Grain.Points;

public interface IOperatorPointRecordDetailGrain : IGrainWithStringKey
{
    Task<GrainResultDto<PointRecordGrainDto>> PointsRecordAsync(PointRecordGrainDto grainDto);
}