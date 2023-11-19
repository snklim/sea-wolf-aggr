using System.Collections.Generic;
using System.Linq;

namespace SeaWolfAggr
{
    public class Player : Entity
    {
        public Field OwnField { get; private set; }
        public Field EnemyField { get; private set; }
        public Dictionary<int, int> Ships { get; private set; }

        public void AddOwnField(Field field)
        {
            OwnField = field;
            Ships = Enumerable.Range(1, 10).Zip(new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 },
                (shipIndex, shipLength) => (shipIndex, shipLength)).ToDictionary(x => x.shipIndex, x => x.shipLength);
        }

        public void AddEnemyField(Field field)
        {
            EnemyField = field;
        }

        public void SetCellAsDestroyed(int shipIndex)
        {
            Ships[shipIndex]--;
        }
    }
}