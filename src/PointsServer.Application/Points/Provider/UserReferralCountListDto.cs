using System.Collections.Generic;

namespace PointsServer.Points.Provider;

public class UserReferralCountListDto
{
    public long TotalRecordCount { get; set; }
    public List<UserReferralCountDto> Data { get; set; }
}

public class UserReferralCountDto
{
    public string Domain { get; set; }

    public string DappId { get; set; }

    public string Referrer { get; set; }

    public long InviteeNumber { get; set; }

    public long CreateTime { get; set; }

    public long UpdateTime { get; set; }
}

public class UserReferralCountResultDto
{
    public UserReferralCountListDto GetUserReferralCounts { get; set; }
}
