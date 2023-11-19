using System;

namespace SeaWolfAggr
{
    public class CurrentPlayerSet : GameDomainEvent
    {
        public Guid PlayerId { get; set; }
    }
}