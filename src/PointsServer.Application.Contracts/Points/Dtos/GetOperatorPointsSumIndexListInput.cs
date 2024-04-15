using Volo.Abp.Application.Dtos;

namespace PointsServer.Points.Dtos;

public class GetOperatorPointsSumIndexListInput : PagedAndSortedResultRequestDto
{
    public string Keyword { get; set; }
    
    public string DappName { get; set; }
    public SortingKeywordType SortingKeyWord { get; set; }

    public override string Sorting { get; set; } = "DESC";
}