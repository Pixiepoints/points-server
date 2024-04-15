using Orleans.TestingHost;
using Volo.Abp.Modularity;
using Xunit.Abstractions;

namespace PointsServer;

public abstract class PointsServerOrleansTestBase<TStartupModule> : 
    PointsServerTestBase<TStartupModule> where TStartupModule : IAbpModule
{

    protected readonly TestCluster Cluster;
    
    public PointsServerOrleansTestBase(ITestOutputHelper output) : base(output)
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}