using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace PointsServer.Points.Dtos;

public class GetRankingListInput : PagedAndSortedResultRequestDto
{
    public string Keyword { get; set; }
    public string DappName { get; set; }
    public SortingKeywordType SortingKeyWord { get; set; }

    [Required] public override int SkipCount { get; set; }
    [Required] public override int MaxResultCount { get; set; }
    
    public override string Sorting { get; set; } = "DESC";
}