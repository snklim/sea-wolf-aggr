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
            var firstPlayerId = System.Guid.NewGuid();
            var secondPlayerId = System.Guid.NewGuid();

            _events.Add(ApplyEvent(new GameCreated
            {
                GameId = gameId
            }));

            _events.Add(ApplyEvent(new FirstPlayerCreated
            {
                GameId = gameId,
                PlayerId = firstPlayerId
            }));

            _events.Add(ApplyEvent(new SecondPlayerCreated
            {
                GameId = gameId,
                PlayerId = secondPlayerId
            }));

            _events.Add(ApplyEvent(new FirstPlayerOwnFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateOwnCells(firstPlayerId)
            }));

            _events.Add(ApplyEvent(new FirstPlayerEnemyFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateCells(secondPlayerId)
            }));

            _events.Add(ApplyEvent(new SecondPlayerOwnFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateOwnCells(secondPlayerId)
            }));

            _events.Add(ApplyEvent(new SecondPlayerEnemyFieldCreated
            {
                GameId = gameId,
                FieldId = System.Guid.NewGuid(),
                Cells = GenerateCells(firstPlayerId)
            }));

            _events.Add(ApplyEvent(new CurrentPlayerSet
            {
                PlayerId = firstPlayerId
            }));

            return _game;
        }

        public Game MovePlayer(MovePlayerCommand cmd)
        {
            if (cmd.PlayerId == _game.CurrentPlayerId) throw new ArgumentException();

            _events.Clear();

            var player = new[]
            {
                 _game.FirstPlayer,
                 _game.SecondPlayer
            }.First(p => p.Id == cmd.PlayerId);

            var cell = player.OwnField.Cells.First(c => c.Pos == cmd.Pos);
            var affectedCells = new List<Cell>() { cell };

            if (cell.CellType == CellType.Ship && player.Ships[cell.ShipIndex] == 1)
            {
                affectedCells.AddRange(player.OwnField.Cells.Where(c => cell.Border.Any(b => b == c.Pos)));
            }

            affectedCells.ForEach(c => c.IsDestroyed = true);

            if (_game.CurrentPlayerId == _game.FirstPlayer.Id)
            {
                _events.Add(ApplyEvent(new FirstPlayerEnemyFieldUpdated
                {
                    Cells = affectedCells.ToArray()
                }));

                _events.Add(ApplyEvent(new SecondPlayerOwnFieldUpdated
                {
                    Cells = affectedCells.ToArray(),
                    ShipIndex = cell.ShipIndex
                }));
            }
            else
            {
                _events.Add(ApplyEvent(new SecondPlayerEnemyFieldUpdated
                {
                    Cells = affectedCells.ToArray()
                }));

                _events.Add(ApplyEvent(new FirstPlayerOwnFieldUpdated
                {
                    Cells = affectedCells.ToArray(),
                    ShipIndex = cell.ShipIndex
                }));
            }

            return _game;
        }

        private IEnumerable<Cell> GenerateOwnCells(Guid playerId)
        {
            var cells = GenerateCells(playerId);

            var rnd = new Random();
            var shipIndex = 1;

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
                    var border = new List<Pos>();

                    for (var i = 0; i < ship; i++)
                    {
                        var pos = new Pos(col, row);
                        var cell = cells.FirstOrDefault(c => c.Pos == pos && c.CellType == CellType.Empty);

                        if (cell == null)
                        {
                            canFit = false;
                            break;
                        }

                        var neighbors = new[]
                        {
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
                        border.AddRange(neighbors);
                        col += deltaCol;
                        row += deltaRow;
                    }

                    if (!canFit) continue;

                    border = border.Where(b => shipCells.All(c => c.Pos != b)).Where(b => cells.Any(c => c.Pos == b)).ToList();

                    foreach (var cell in shipCells)
                    {
                        cell.CellType = CellType.Ship;
                        cell.ShipLength = ship;
                        cell.ShipIndex = shipIndex;
                        cell.Border = border;
                    }

                    break;
                }
                shipIndex++;
            }

            return cells;
        }

        private IEnumerable<Cell> GenerateCells(Guid playerId)
        {
            var cells = Enumerable.Range(0, 10)
                .SelectMany(col => Enumerable.Range(0, 10)
                    .Select(row => new Cell(new Pos(col, row)) { PlayerId = playerId }))
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

        private DomainEvent ApplyEvent(CurrentPlayerSet @event)
        {
            _game.SetCurrentPlayer(@event.PlayerId);
            return @event;
        }

        private DomainEvent ApplyEvent(FirstPlayerOwnFieldUpdated @event)
        {
            _game.UpdateFirstPlayerOwnField(@event.Cells);
            return @event;
        }

        private DomainEvent ApplyEvent(FirstPlayerEnemyFieldUpdated @event)
        {
            _game.UpdateFirstPlayerEnemyField(@event.Cells);
            return @event;
        }

        private DomainEvent ApplyEvent(SecondPlayerOwnFieldUpdated @event)
        {
            _game.UpdateSecondPlayerOwnField(@event.Cells);
            return @event;
        }

        private DomainEvent ApplyEvent(SecondPlayerEnemyFieldUpdated @event)
        {
            _game.UpdateSecondPlayerEnemyField(@event.Cells);
            return @event;
        }
    }

    public class ShipDetails
    {
        public int AliveCells { get; set; }
    }
}