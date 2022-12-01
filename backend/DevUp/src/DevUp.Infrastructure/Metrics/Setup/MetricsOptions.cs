namespace DevUp.Infrastructure.Metrics.Setup
{
    internal sealed class MetricsOptions
    {
        public string DefaultContextLabel { get; set; }
        public string AppTag { get; set; }
        public string EnvTag { get; set; }
        public string ServerTag { get; set; }
    }
}
