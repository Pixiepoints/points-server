using System.Collections.Generic;

namespace PointsServer.Options;

public class PointsRulesOption
{
    public List<PointsRules> PointsRulesList { get; set; }
}

public class PointsRules
{
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Action { get; set; }
    public string Symbol { get; set; }
    public decimal UserAmount { get; set; }
    public decimal KolAmount { get; set; }
    public decimal SecondLevelUserAmount { get; set; }
    public decimal ThirdLevelUserAmount { get; set; }
    
    public decimal KOLThirdLevelUserAmount{ get; set; }
    public decimal InviterAmount { get; set; }
    public int Decimal { get; set; }
    public string DisplayNamePattern { get; set; }
}