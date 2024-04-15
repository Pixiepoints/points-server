using System.ComponentModel.DataAnnotations;
using PointsServer.Common;
using Volo.Abp.Application.Dtos;

namespace PointsServer.Points.Dtos;

public class GetPointsEarnedListInput : PagedAndSortedResultRequestDto
{
    [Required] public string DappName  { get; set; }
    [Required] public string Address  { get; set; }
    [Required] public OperatorRole Role { get; set; }
    public SortingKeywordType SortingKeyWord { get; set; }

    [Required] public override int SkipCount { get; set; }
    [Required] public override int MaxResultCount { get; set; }

    public override string Sorting { get; set; } = "DESC";
}
