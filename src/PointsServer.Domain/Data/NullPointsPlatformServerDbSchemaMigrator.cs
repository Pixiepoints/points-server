using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PointsServer.Data;

/* This is used if database provider does't define
 * IPointsServerDbSchemaMigrator implementation.
 */
public class NullPointsServerDbSchemaMigrator : IPointsServerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
