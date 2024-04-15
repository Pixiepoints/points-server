using System.ComponentModel.DataAnnotations;
using PointsServer.Common;

namespace PointsServer.Points.Dtos;

public class GetPointsEarnedDetailInput
{
    [Required] public string DappName  { get; set; }
    [Required] public string Domain  { get; set; }
    [Required] public string Address  { get; set; }
    [Required] public OperatorRole Role { get; set; }
}

public class GetMyPointsInput
{
    [Required] public string DappName  { get; set; }
    [Required] public string Domain  { get; set; }
    [Required] public string Address  { get; set; }
    [Required] public OperatorRole Role { get; set; }
}