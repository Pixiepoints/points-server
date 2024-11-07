using System;
using System.Collections.Generic;
using PointsServer.Common;

namespace PointsServer.Points.Provider;

public class PointsSumIndexerListDto
{
    public long TotalCount { get; set; }
    public List<PointsSumIndexerDto> Data { get; set; }
}

public class PointsSumIndexerDto
{
    public string Domain { get; set; }
    public string Address { get; set; }
    public string FirstSymbolAmount { get; set; }
    public string SecondSymbolAmount { get; set; }
    public string ThirdSymbolAmount { get; set; }
    public string FourSymbolAmount { get; set; }
    public string FiveSymbolAmount { get; set; }
    public string SixSymbolAmount { get; set; }
    public string SevenSymbolAmount { get; set; }
    public string EightSymbolAmount { get; set; }
    public string NineSymbolAmount { get; set; }
    public string TenSymbolAmount { get; set; } 
    public string ElevenSymbolAmount { get; set; } 
    public string TwelveSymbolAmount { get; set; } 
    public long UpdateTime { get; set; }
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Icon { get; set; }
    public OperatorRole Role { get; set; }
}

public class IndexerRankingListQueryDto
{
    public PointsSumIndexerListDto GetRankingList { get; set; }
}

public class IndexerPointsEarnedListQueryDto
{
    public PointsSumIndexerListDto GetPointsEarnedList { get; set; }
}

public class IndexerPointsSumListQueryDto
{
    public PointsSumAllIndexerListDto GetPointsSumBySymbol { get; set; }
}

public class IndexerAllPointsListQueryDto
{
    public AllPointsIndexerListDto GetAllPointsList { get; set; }
}

public class PointsSumAllIndexerListDto
{
    public long TotalCount { get; set; }
    public List<PointsSumAllIndexerDto> Data { get; set; }
}

public class AllPointsIndexerListDto
{
    public long TotalCount { get; set; }
    public List<PointsSumAllIndexerDto> Data { get; set; }
}


public class PointsSumAllIndexerDto
{
    public string Id { get; set; }
    public long BlockHeight { get; set; }
    public string Domain { get; set; }
    public string Address { get; set; }
    public string FirstSymbolAmount { get; set; }
    public string SecondSymbolAmount { get; set; }
    public string ThirdSymbolAmount { get; set; }
    public string FourSymbolAmount { get; set; }
    public string FiveSymbolAmount { get; set; }
    public string SixSymbolAmount { get; set; }
    public string SevenSymbolAmount { get; set; }
    public string EightSymbolAmount { get; set; }
    public string NineSymbolAmount { get; set; }
    public string TenSymbolAmount { get; set; } 
    public string ElevenSymbolAmount { get; set; } 
    public string TwelveSymbolAmount { get; set; } 
    public DateTime UpdateTime { get; set; }
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Icon { get; set; }
    public OperatorRole Role { get; set; }
}