using AElf.ExceptionHandler;
using PointsServer.Grains.Grain;
using PointsServer.Grains.Grain.Worker;

namespace PointsServer.Grains.Exceptions;

public class ExceptionHandlingService
{
    public static async Task<FlowBehavior> HandleException(Exception ex)
    {
        return new FlowBehavior
        {
            ExceptionHandlingStrategy = ExceptionHandlingStrategy.Return
        };
    }
}