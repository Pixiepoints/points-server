using System;
using System.Threading.Tasks;
using AElf.ExceptionHandler;

namespace PointsServer.Exceptions;

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