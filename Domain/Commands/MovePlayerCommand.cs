using System;

namespace SeaWolfAggr
{

    public class MovePlayerCommand
    {
        public Guid GameId { get; set; }
        public string Player { get; set; }
        public Pos Pos { get; set; }
    }
}