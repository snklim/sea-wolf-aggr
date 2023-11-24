using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaWolfAggr
{
    public class Player : Entity
    {
        public Field OwnField { get; private set; }
        public Field EnemyField { get; private set; }
        public Dictionary<int, Cell[]> Ships { get; private set; }

        public void AddOwnField(Field field)
        {
            OwnField = field;
            Ships = Enumerable.Range(1, 10)
                .Zip(new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }, (shipIndex, shipLength) => (shipIndex, shipLength))
                .ToDictionary(x => x.shipIndex, x => Enumerable.Range(0, x.shipLength).Select(x => new Cell(new Pos(0, 0))
                {
                    CellType = CellType.Ship
                }).ToArray());
        }

        public void AddEnemyField(Field field)
        {
            EnemyField = field;
        }

        public void SetCellAsDestroyed(int shipIndex)
        {
            var ship = Ships[shipIndex];
            var cell = ship.Last(x => !x.IsDestroyed);
            cell.IsDestroyed = true;
        }

        public int AliveCells(int shipIndex)
        {
            var ship = Ships[shipIndex];
            return ship.Count(x => !x.IsDestroyed);
        }
    }
}