namespace HomeLibrary.Service
{
    public class MassTransitConfigConstants
    {
        public string     HostAddress                { get; set; }
        public long       BusStartStopTimeoutSeconds { get; set; }
        public string     Username                   { get; set; }
        public string     Password                   { get; set; }
        public QueueNames QueueNames                 { get; set; }
        public int        RetryLimitCount            { get; set; }
        public int        InitialIntervalSeconds     { get; set; }
        public int        IntervalIncrementSeconds   { get; set; }
        public int        ConcurrencyLimit           { get; set; }
    }

    public class QueueNames
    {
        public string BookCreatedEvent { get; set; }
    }
}