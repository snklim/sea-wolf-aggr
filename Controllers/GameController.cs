using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SeaWolfAggr.Controllers.Dtos;

namespace SeaWolfAggr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static GameAggr _gameAggr;
        private static Dictionary<Guid, GameAggr> _games = new Dictionary<Guid, GameAggr>();

        [HttpGet("getall")]
        public IEnumerable<GameItemDto> GetAll()
        {
            return _games.Select(x => new GameItemDto { GameId = x.Key, FirstPlayerId = x.Value.Game.FirstPlayer.Id, SecondPlayerId = x.Value.Game.SecondPlayer.Id });
        }

        [HttpPost("getgame")]
        public object GetGame(GameRequestDto request)
        {
            var gameAggr = _games.FirstOrDefault(x => x.Key == request.GameId).Value;
            return request.PlayerId == gameAggr.Game.FirstPlayer.Id
                ? new GameDto
                {
                    CurrentPlayerId = gameAggr.Game.CurrentPlayerId,
                    OwnField = gameAggr.Game.FirstPlayer.OwnField.Cells.Select(CellToCellDto).ToArray(),
                    EnemyField = gameAggr.Game.FirstPlayer.EnemyField.Cells.Select(CellToCellDto).ToArray(),
                    Ships = gameAggr.Game.SecondPlayer.Ships.Values.Select(x => x.Select(CellToCellDto).ToArray()),
                    WinPlayerId = GetWinPlayerId(gameAggr.Game)
                }
                : new GameDto
                {
                    CurrentPlayerId = gameAggr.Game.CurrentPlayerId,
                    OwnField = gameAggr.Game.SecondPlayer.OwnField.Cells.Select(CellToCellDto).ToArray(),
                    EnemyField = gameAggr.Game.SecondPlayer.EnemyField.Cells.Select(CellToCellDto).ToArray(),
                    Ships = gameAggr.Game.FirstPlayer.Ships.Values.Select(x => x.Select(CellToCellDto).ToArray()),
                    WinPlayerId = GetWinPlayerId(gameAggr.Game)
                };
        }

        [HttpPost]
        public GameDto Post()
        {
            _gameAggr = new GameAggr();
            var game = _gameAggr.CreateGame();
            _games.Add(_gameAggr.Game.Id, _gameAggr);
            return new GameDto
            {
                OwnField = game.FirstPlayer.OwnField.Cells.Select(CellToCellDto).ToArray(),
                EnemyField = game.FirstPlayer.EnemyField.Cells.Select(CellToCellDto).ToArray()
            };
        }

        [HttpPut]
        public GameDto Put(MovePlayerCommand cmd)
        {
            var gameAggr = _games.FirstOrDefault(x => x.Key == cmd.GameId).Value;
            var game = gameAggr.MovePlayer(cmd);
            return null;
        }

        private static Guid GetWinPlayerId(Game game)
        {
            return game.FirstPlayer.IsAlive && game.SecondPlayer.IsAlive
                                ? Guid.Empty
                                : game.FirstPlayer.IsAlive
                                ? game.FirstPlayer.Id
                                : game.SecondPlayer.Id;
        }

        private static CellDto CellToCellDto(Cell cell)
        {
            return new CellDto
            {
                CellType = cell.CellType == CellType.Ship ? "ship" : "",
                IsDestroyed = cell.IsDestroyed,
                Pos = new PosDto
                {
                    Col = cell.Pos.Col,
                    Row = cell.Pos.Row
                }
            };
        }
    }
}
