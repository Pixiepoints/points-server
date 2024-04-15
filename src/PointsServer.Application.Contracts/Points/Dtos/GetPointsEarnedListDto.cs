using Volo.Abp.Application.Dtos;

namespace PointsServer.Points.Dtos;

public class GetPointsEarnedListDto : PagedResultDto<PointsEarnedListDto>
{
    public decimal TotalEarned { get; set; }
}