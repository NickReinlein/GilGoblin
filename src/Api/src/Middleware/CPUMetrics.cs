using System;
using System.Diagnostics;
using System.Threading;

public class CpuMetrics
{
    public static float GetCpuUsage()
    {
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
        Thread.Sleep(100);
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsage = (endCpuUsage - startCpuUsage).TotalMilliseconds / Environment.ProcessorCount / 100;
        return (float)Math.Round(cpuUsage, 2);
    }
}