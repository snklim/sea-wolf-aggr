using System.Collections.Generic;

namespace SeaWolfAggr
{
    public abstract class FieldCreated : GameDomainEvent
    {
        public System.Guid FieldId { get; set; }
        public IEnumerable<Cell> Cells { get; set; }
    }
}