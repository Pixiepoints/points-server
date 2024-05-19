using System.Collections.Generic;

namespace PointsServer.Points.Dtos;

public class GetRelationshipInput
{
    public List<string> AddressList { get; set; }
    public string ChainId { get; set; }
    public string DappId { get; set; }
}