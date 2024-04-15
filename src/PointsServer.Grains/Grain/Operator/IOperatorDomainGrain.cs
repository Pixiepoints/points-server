using Orleans;
using PointsServer.Common;

namespace PointsServer.Grains.Grain.Operator;

public interface IOperatorDomainGrain : IGrainWithStringKey
{
    Task<GrainResultDto<OperatorDomainGrainDto>> AddOperatorDomainAsync(OperatorDomainGrainDto dto);
    Task UpdateApplyStatusAsync(ApplyStatus status);
}