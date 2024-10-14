using AElf.ExceptionHandler;

namespace PointsServer.Grains.Exceptions;

public class ExceptionHandlingService
{
    public static async Task<FlowBehavior> HandleException(Exception ex, int i)
    {
        return new FlowBehavior
        {
            ExceptionHandlingStrategy = ExceptionHandlingStrategy.Return
        };
    }
}