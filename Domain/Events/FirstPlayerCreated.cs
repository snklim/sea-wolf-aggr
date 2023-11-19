namespace SeaWolfAggr
{
    public class FirstPlayerCreated : GameDomainEvent
    {
        public System.Guid PlayerId { get; set; }
    }
}