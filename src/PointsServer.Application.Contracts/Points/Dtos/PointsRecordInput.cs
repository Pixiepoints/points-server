namespace PointsServer.Points.Dtos;

public class PointsRecordInput
{
    public string DappName { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
    public string RecordAction { get; set; }
    public string Signature { get; set; }
}