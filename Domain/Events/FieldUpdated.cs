using System.Collections.Generic;

namespace SeaWolfAggr
{
    public abstract class FieldUpdated : GameDomainEvent
    {
        public IEnumerable<Cell> Cells { get; set; }
    }
}