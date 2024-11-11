using System.Collections.Generic;

namespace PointsServer.Options;

public class PointsRulesOption
{
    public List<PointsRules> PointsRulesList { get; set; }
}

public class PointsRules
{
    public string DappName { get; set; } = "";
    public string DappId { get; set; }
    public string Action { get; set; } = "";
    public string Symbol { get; set; } = "";
    public decimal UserAmount { get; set; } = 0;
    public decimal KolAmount { get; set; } = 0;
    public decimal SecondLevelUserAmount { get; set; } = 0;
    public decimal ThirdLevelUserAmount { get; set; } = 0;
    
    public decimal KOLThirdLevelUserAmount{ get; set; } = 0;
    public decimal InviterAmount { get; set; } = 0;
    public int Decimal { get; set; } = 0;
    public string DisplayNamePattern { get; set; } = "";
}