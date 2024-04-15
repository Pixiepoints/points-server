using System;
using System.Collections.Generic;
using PointsServer.Common;

namespace PointsServer.Points.Provider;

public class RankingDetailIndexerQueryDto
{
    public RankingDetailIndexerListDto GetPointsSumByAction { get; set; }
}

public class OperatorDomainIndexerQueryDto
{
    public OperatorDomainDto OperatorDomainInfo { get; set; }
}

public class RankingDetailIndexerListDto
{
    public List<RankingDetailIndexerDto> Data { get; set; }
    public long TotalRecordCount { get; set; }
}

public class OperatorDomainDto
{
    public  string Id { get; set; }  
    
    public string Domain { get; set; }
    
    public string DepositAddress { get; set; }
    
    public string InviterAddress { get; set; }
    
    public string DappId { get; set; }  
    
    public DateTime CreateTime { get; set; } 
    public string Descibe { get; set; }
}

public class RankingDetailIndexerDto
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
    public OperatorRole Role { get; set; }
    public string DappId { get; set; }
    public string PointsName { get; set; }
    public string ActionName { get; set; }
    public string Amount { get; set; }
    public string SymbolName { get; set; }

    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}