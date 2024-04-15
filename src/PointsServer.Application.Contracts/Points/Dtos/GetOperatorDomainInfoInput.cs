using System.ComponentModel.DataAnnotations;
using PointsServer.Common;

namespace PointsServer.Points.Dtos;

public class GetOperatorDomainInfoInput 
{
    [Required] public string Domain  { get; set; }
}