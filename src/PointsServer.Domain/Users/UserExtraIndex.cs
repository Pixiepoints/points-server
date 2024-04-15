using System;
using System.Collections.Generic;
using AElf.Indexing.Elasticsearch;
using Nest;
using PointsServer.Entities;

namespace PointsServer.Users;

public class UserExtraIndex : PointsServerEntity<Guid>, IIndexBuild
{
    [Keyword] public string UserName { get; set; }
    [Keyword] public string AelfAddress { get; set; }
    [Keyword] public string CaHash { get; set; }
    [Keyword] public string CaAddressMain { get; set; }

    [Nested(Name = "CaAddressListSide", Enabled = true, IncludeInParent = true, IncludeInRoot = true)]
    public List<UserAddress> CaAddressListSide { get; set; }

    public DateTime CreateTime { get; set; }
}