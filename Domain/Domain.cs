using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaWolfAggr
{
    public class GameAggr
    {
        private List<DomainEvent> _events = new List<DomainEvent>();

        private Game _game;

        public Game CreateGame()
        {
            if (_game != null) throw new ArgumentException();

            var gameId = System.Guid.NewGuid();

            _events.Add(ApplyEvent(new GameCreated
            {
                GameId = gameId
            }));

            _events.Add(ApplyEvent(new FirstPlayerCreated
            {
                GameId = gameId,
                PlayerId = System.Guid.NewGuid()
            }));

            _events.Add(ApplyEvent(new SecondPlayerCreated
            {
                GameId = gameId,
                PlayerId = System.Guid.NewGuid()
            }));

            _events.Add(ApplyEvent(new FirstPlayerOwnFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateOwnCells()
            }));

            _events.Add(ApplyEvent(new FirstPlayerEnemyFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateEnemyCells()
            }));

            _events.Add(ApplyEvent(new SecondPlayerOwnFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateOwnCells()
            }));

            _events.Add(ApplyEvent(new SecondPlayerEnemyFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateEnemyCells()
            }));

            return _game;
        }

        private IEnumerable<Cell> GenerateOwnCells()
        {
            var cells = Enumerable.Range(0, 10)
                .SelectMany(col => Enumerable.Range(0, 10)
                    .Select(row => new Cell(new Pos(col, row))))
                .ToArray();

            var rnd = new Random();

            foreach (var ship in new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 })
            {
                while (true)
                {
                    var col = rnd.Next(10);
                    var row = rnd.Next(10);
                    var isCol = rnd.Next(100) % 2 == 0;
                    var deltaCol = isCol ? 1 : 0;
                    var deltaRow = isCol ? 0 : 1;
                    var canFit = true;

                    var shipCells = new List<Cell>();

                    for (var i = 0; i < ship; i++)
                    {
                        var pos = new Pos(col, row);
                        var cell = cells.FirstOrDefault(c => c.Pos == pos && c.CellType == CellType.Empty);
                        if (cell == null)
                        {
                            canFit = false;
                            break;
                        }
                        var neighbors = new[]{
                            new Pos(col-1, row-1),new Pos(col-1, row),new Pos(col-1, row+1),
                            new Pos(col, row-1),new Pos(col, row+1),
                            new Pos(col+1, row-1),new Pos(col+1, row),new Pos(col+1, row+1),
                            };
                        foreach (var neighbor in neighbors)
                        {
                            var neighborCell = cells.FirstOrDefault(c => c.Pos == neighbor);
                            if (neighborCell != null && neighborCell.CellType == CellType.Ship)
                            {
                                canFit = false;
                                break;
                            }
                        }
                        if (cell == null)
                        {
                            canFit = false;
                            break;
                        }
                        shipCells.Add(cell);
                        col += deltaCol;
                        row += deltaRow;
                    }

                    if (!canFit) continue;

                    foreach (var cell in shipCells)
                    {
                        cell.CellType = CellType.Ship;
                    }

                    break;
                }
            }

            return cells;
        }

        private IEnumerable<Cell> GenerateEnemyCells()
        {
            var cells = Enumerable.Range(0, 10)
                .SelectMany(col => Enumerable.Range(0, 10)
                    .Select(row => new Cell(new Pos(col, row))))
                .ToArray();

            return cells;
        }

        private DomainEvent ApplyEvent(FirstPlayerOwnFieldCreated @event)
        {
            _game.AddFirstPlayerOwnField(
                new Field
                {
                    Id = @event.FieldId
                }.AddCells(@event.Cells)
            );
            return @event;
        }

        private DomainEvent ApplyEvent(FirstPlayerEnemyFieldCreated @event)
        {
            _game.AddFirstPlayerEnemyField(
                new Field
                {
                    Id = @event.FieldId
                }.AddCells(@event.Cells)
            );
            return @event;
        }

        private DomainEvent ApplyEvent(SecondPlayerOwnFieldCreated @event)
        {
            _game.AddSecondPlayerOwnField(
                new Field
                {
                    Id = @event.FieldId
                }.AddCells(@event.Cells)
            );
            return @event;
        }

        private DomainEvent ApplyEvent(SecondPlayerEnemyFieldCreated @event)
        {
            _game.AddSecondPlayerEnemyField(
                new Field
                {
                    Id = @event.FieldId
                }.AddCells(@event.Cells)
            );
            return @event;
        }

        private DomainEvent ApplyEvent(GameCreated @event)
        {
            _game = new Game
            {
                Id = @event.GameId
            };
            return @event;
        }

        private DomainEvent ApplyEvent(FirstPlayerCreated @event)
        {
            _game.AddFirstPlayer(new Player
            {
                Id = @event.PlayerId
            });
            return @event;
        }

        private DomainEvent ApplyEvent(SecondPlayerCreated @event)
        {
            _game.AddSecondPlayer(new Player
            {
                Id = @event.PlayerId
            });
            return @event;
        }
    }

    public abstract class Entity
    {
        public System.Guid Id { get; set; }
    }

    public class Game : Entity
    {
        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

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
    }

    public class Player : Entity
    {
        public Field OwnFields { get; private set; }

        public Field EnemyField { get; private set; }

        public void AddOwnField(Field field)
        {
            OwnFields = field;
        }

        public void AddEnemyField(Field field)
        {
            EnemyField = field;
        }
    }

    public class Field : Entity
    {
        public IEnumerable<Cell> Cells { get; private set; }
        public Field AddCells(IEnumerable<Cell> cells)
        {
            Cells = cells;
            return this;
        }
    }

    public class Pos
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public Pos(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public static bool operator ==(Pos pos1, Pos pos2)
        {
            if (pos1 is null || pos2 is null) return false;
            return pos1.Col == pos2.Col && pos1.Row == pos2.Row;
        }

        public static bool operator !=(Pos pos1, Pos pos2)
        {
            return !(pos1 == pos2);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Pos;
        }

        public override int GetHashCode()
        {
            return Col * 100 + Row;
        }
    }

    public class Cell
    {
        public Pos Pos { get; private set; }
        public CellType CellType { get; set; } = CellType.Empty;
        public bool IsDestroyed { get; set; } = false;
        public Cell(Pos pos)
        {
            Pos = pos;
        }
    }

    public enum CellType
    {
        Empty,
        Ship
    }

    public abstract class DomainEvent
    {

    }

    public abstract class GameDomainEvent : DomainEvent
    {
        public System.Guid GameId { get; set; }
    }

    public class GameCreated : GameDomainEvent
    {
    }

    public class FirstPlayerCreated : GameDomainEvent
    {
        public System.Guid PlayerId { get; set; }
    }

    public class SecondPlayerCreated : GameDomainEvent
    {
        public System.Guid PlayerId { get; set; }
    }

    public abstract class FieldCreated : GameDomainEvent
    {
        public System.Guid FieldId { get; set; }
        public IEnumerable<Cell> Cells { get; set; }
    }

    public class FirstPlayerOwnFieldCreated : FieldCreated
    {
    }

    public class FirstPlayerEnemyFieldCreated : FieldCreated
    {
    }

    public class SecondPlayerOwnFieldCreated : FieldCreated
    {
    }

    public class SecondPlayerEnemyFieldCreated : FieldCreated
    {
    }
}