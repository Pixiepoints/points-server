using PointsServer.Common;

namespace PointsServer.Points.Dtos;

public class PointsEarnedListDto
{
    public string Domain { get; set; }
    public string Address { get; set; }
    public string DappName { get; set; }
    public string Icon { get; set; }
    public OperatorRole Role { get; set; }
    public string FirstSymbolAmount { get; set; }
    public string SecondSymbolAmount { get; set; }
    public string ThirdSymbolAmount { get; set; }
    public string FourSymbolAmount { get; set; }
    public string FiveSymbolAmount { get; set; }
    public string SixSymbolAmount { get; set; } 
    public string SevenSymbolAmount { get; set; } 
    public string EightSymbolAmount { get; set; } 
    public string NineSymbolAmount { get; set; } 
    public long FollowersNumber { get; set; }
    public long UpdateTime { get; set; }
    public decimal Rate { get; set; }
    public int Decimal { get; set; }
    public decimal InviteRate { get; set; }
    public long InviteFollowersNumber { get; set; }
}