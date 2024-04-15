using System.ComponentModel.DataAnnotations;

namespace PointsServer.Points.Dtos;

public class GetRankingDetailInput
{
    [Required] public string DappName  { get; set; }
    [Required] public string Domain  { get; set; }
}