using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaWolfAggr
{
    public class Cell
    {
        public Pos Pos { get; private set; }
        public CellType CellType { get; set; } = CellType.Empty;
        public bool IsDestroyed { get; set; } = false;
        public Guid PlayerId { get; set; }
        public int ShipLength { get; set; }
        public int ShipIndex { get; set; }
        public IEnumerable<Pos> Border { get; set; } = Enumerable.Empty<Pos>();
        public Cell(Pos pos)
        {
            Pos = pos;
        }
    }
}