using System.ComponentModel.DataAnnotations;
using PointsServer.Common;

namespace PointsServer.Points.Dtos;

public class GetOperatorPointsActionSumInput 
{
    [Required] public string DappName  { get; set; }
    [Required] public string Domain  { get; set; }
    public OperatorRole? Role  { get; set; }

    public string Address { get; set; } = "";
}