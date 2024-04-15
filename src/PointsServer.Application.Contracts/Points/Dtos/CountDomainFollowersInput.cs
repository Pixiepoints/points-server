using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointsServer.Points.Dtos;

public class CountDomainFollowersInput
{
    [Required] public List<string> DomainList  { get; set; }
}