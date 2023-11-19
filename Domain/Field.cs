using System.Collections.Generic;

namespace SeaWolfAggr
{
    public class Field : Entity
    {
        public IEnumerable<Cell> Cells { get; private set; }
        public Field AddCells(IEnumerable<Cell> cells)
        {
            Cells = cells;
            return this;
        }
    }
}