using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaWolfAggr
{
    public class Game : Entity
    {
        public Guid CurrentPlayerId { get; private set; }
        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

        public void ChangeCurrentPlayer(Guid currentPlayerId)
        {
            CurrentPlayerId = currentPlayerId;
        }

        public void AddFirstPlayer(Player player)
        {
            FirstPlayer = player;
        }

        public void AddSecondPlayer(Player player)
        {
            SecondPlayer = player;
        }

        public void AddFirstPlayerOwnField(Field field)
        {
            FirstPlayer.AddOwnField(field);
        }

        public void AddFirstPlayerEnemyField(Field field)
        {
            FirstPlayer.AddEnemyField(field);
        }

        public void AddSecondPlayerOwnField(Field field)
        {
            SecondPlayer.AddOwnField(field);
        }

        public void AddSecondPlayerEnemyField(Field field)
        {
            SecondPlayer.AddEnemyField(field);
        }

        public void UpdateFirstPlayerOwnField(IEnumerable<Cell> cells)
        {
            UpdatePlayerField(FirstPlayer.OwnField, cells);
            SetCellsAsDestroyed(FirstPlayer, cells);
        }

        public void UpdateFirstPlayerEnemyField(IEnumerable<Cell> cells)
        {
            UpdatePlayerField(FirstPlayer.EnemyField, cells);
        }

        public void UpdateSecondPlayerOwnField(IEnumerable<Cell> cells)
        {
            UpdatePlayerField(SecondPlayer.OwnField, cells);
            SetCellsAsDestroyed(SecondPlayer, cells);
        }

        public void UpdateSecondPlayerEnemyField(IEnumerable<Cell> cells)
        {
            UpdatePlayerField(SecondPlayer.EnemyField, cells);
        }

        private void SetCellsAsDestroyed(Player player, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells.Where(c => c.CellType == CellType.Ship))
            {
                player.SetCellAsDestroyed(cell.ShipIndex);
            }
        }

        private void UpdatePlayerField(Field field, IEnumerable<Cell> cells)
        {
            foreach (var s in cells)
            {
                var t = field.Cells.First(c => c.Pos == s.Pos);
                t.IsDestroyed = s.IsDestroyed;
                t.CellType = s.CellType;
            }
        }
    }
}