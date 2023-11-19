namespace SeaWolfAggr
{
    public class SecondPlayerCreated : GameDomainEvent
    {
        public System.Guid PlayerId { get; set; }
    }
}