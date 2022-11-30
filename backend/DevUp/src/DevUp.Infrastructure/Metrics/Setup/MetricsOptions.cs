using System;

namespace DevUp.Infrastructure.Metrics.Setup
{
    internal sealed class MetricsOptions
    {
        public TimeSpan Interval { get; set; }
        public string DefaultContextLabel { get; set; }
        public string AppTag { get; set; }
    }
}
