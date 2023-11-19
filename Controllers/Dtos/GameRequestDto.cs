using System;

namespace SeaWolfAggr.Controllers
{
    public class GameRequestDto
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
