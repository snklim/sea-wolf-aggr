using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaWolfAggr.Domain.Events
{
    public class CurrentPlayerChanged : DomainEvent
    {
        public Guid CurrentPlayerId { get; set; }
    }
}