using Xunit.Abstractions;

namespace PointsServer;

public abstract class PointsServerDomainTestBase : PointsServerTestBase<PointsServerDomainTestModule>
{
    protected PointsServerDomainTestBase(ITestOutputHelper output) : base(output)
    {
    }
}