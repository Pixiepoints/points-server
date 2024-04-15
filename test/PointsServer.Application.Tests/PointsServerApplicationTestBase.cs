using Xunit.Abstractions;

namespace PointsServer;

public abstract partial class PointsServerApplicationTestBase : PointsServerOrleansTestBase<PointsServerApplicationTestModule>
{

    public  readonly ITestOutputHelper Output;
    protected PointsServerApplicationTestBase(ITestOutputHelper output) : base(output)
    {
        Output = output;
    }
}