using System;

namespace SeaWolfAggr.Controllers
{
    public class GameDto
    {
        public Guid GameId { get; set; }
        public Guid FirstPlayerId { get; set; }
        public Guid SecondPlayerId { get; set; }
    }
}
