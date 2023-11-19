namespace SeaWolfAggr
{
    public abstract class GameDomainEvent : DomainEvent
    {
        public System.Guid GameId { get; set; }
    }
}