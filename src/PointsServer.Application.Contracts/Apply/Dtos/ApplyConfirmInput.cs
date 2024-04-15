namespace PointsServer.Apply.Dtos;

public class ApplyConfirmInput
{
    public string RawTransaction { get; set; }
    public string Describe { get; set; }
    public string PublicKey { get; set; }
    public string ChainId { get; set; }
}