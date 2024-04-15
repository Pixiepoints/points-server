using System.Collections.Generic;

namespace PointsServer.Options;

public class DappOption
{
    public List<DappInfo> DappInfos { get; set; }
}

public class DappInfo
{
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Icon { get; set; }
    public string Category { get; set; }
    public string SecondLevelDomain { get; set; }
    public string FirstLevelDomain { get; set; }
    public string Link { get; set; }
    public bool SupportsApply { get; set; }
}