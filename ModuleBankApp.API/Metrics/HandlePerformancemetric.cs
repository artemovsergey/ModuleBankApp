using System.Diagnostics.Metrics;

namespace ModuleBankApp.API.Metrics;

public class HandlePerformancemetric
{
    private readonly Counter<long> _millisecondElapced;

    public HandlePerformancemetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("ModuleBankApp.API");
        _millisecondElapced = meter.CreateCounter<long>("request-handler-counter");
    }

    public void MilliSecondElapsed(long millisecond) =>
        _millisecondElapced.Add(millisecond);
}