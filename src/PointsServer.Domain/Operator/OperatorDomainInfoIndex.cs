using AElf.Indexing.Elasticsearch;
using Nest;
using PointsServer.Common;
using PointsServer.Entities;

namespace PointsServer.Operator;

public class OperatorDomainInfoIndex : PointsServerEntity<string>, IIndexBuild
{
    [Keyword] public string Address { get; set; }
    [Keyword] public string InviterAddress { get; set; }
    public OperatorRole Role { get; set; }
    public ApplyStatus Status { get; set; }
    [Keyword] public string Domain { get; set; }
    [Keyword] public string Icon { get; set; }
    [Keyword] public string DappName { get; set; }
    [Keyword] public string Descibe { get; set; }
    public long ApplyTime { get; set; }
}