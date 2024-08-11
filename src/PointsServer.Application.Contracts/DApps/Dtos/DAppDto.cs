using System.Collections.Generic;

namespace PointsServer.DApps.Dtos;

public class DAppDto
{
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Icon { get; set; }
    public string Category { get; set; }
    public string Link { get; set; }
    public string SecondLevelDomain { get; set; }
    public string FirstLevelDomain { get; set; }
    public bool SupportsApply { get; set; }
    public List<string> PointsRule { get; set; }
    public List<RankingColumn> RankingColumns { get; set; }
}

public class DAppFilterDto
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Suffix { get; set; }
}

public class RankingColumn
{
    public string DataIndex { get; set; }
    public string SortingKeyWord { get; set; }
    public string Label { get; set; }
    public string DefaultSortOrder { get; set; }
    public string TipText { get; set; }
}