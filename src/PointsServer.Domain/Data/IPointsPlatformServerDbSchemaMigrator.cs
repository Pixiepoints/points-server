using System.Threading.Tasks;

namespace PointsServer.Data;

public interface IPointsServerDbSchemaMigrator
{
    Task MigrateAsync();
}
