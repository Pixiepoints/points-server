using System.Collections.Generic;

namespace PointsServer.Options;

public class InternalWhiteListOptions
{
    public List<string> WhiteList { get; set; }
    public Dictionary<string, string> ElevenAmountSubDic { get; set; }
}