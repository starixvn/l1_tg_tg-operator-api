namespace TgOperator.Shared.Configuration
{
    public class SentrySettings
    {
        public string Dsn { get; set; }
        public bool Debug { get; set; }
        public double TracesSampleRate { get; set; }
        public string MinimumEventLevel { get; set; }
        public bool SendDefaultPii { get; set; }
        public bool AttachStacktrace { get; set; }
        public string Environment { get; set; }
    }
} 