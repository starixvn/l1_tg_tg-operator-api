namespace TgOperator.Shared.Configuration
{
    public class AppSettings
    {
        public required MongoDbSettings MongoDbSettings { get; set; }        
        public required SentrySettings SentrySettings { get; set; }
    }
} 