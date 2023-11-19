using System;

namespace SeaWolfAggr
{

    public class MovePlayerCommand
    {
        public Guid PlayerId { get; set; }
        public Pos Pos { get; set; }
    }
}