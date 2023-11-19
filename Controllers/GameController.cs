using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SeaWolfAggr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static GameAggr _gameAggr;
        private static Dictionary<Guid, GameAggr> _games = new Dictionary<Guid, GameAggr>();

        [HttpGet("getall")]
        public IEnumerable<GameDto> GetAll()
        {
            return _games.Select(x => new GameDto { GameId = x.Key, FirstPlayerId = x.Value.Game.FirstPlayer.Id, SecondPlayerId = x.Value.Game.SecondPlayer.Id });
        }

        [HttpPost("getgame")]
        public object GetGame(GameRequestDto request)
        {
            var gameAggr = _games.FirstOrDefault(x => x.Key == request.GameId).Value;
            return request.Player == "first"
                ? new[] { gameAggr.Game.FirstPlayer.OwnField.Cells, gameAggr.Game.FirstPlayer.EnemyField.Cells }
                : new[] { gameAggr.Game.SecondPlayer.OwnField.Cells, gameAggr.Game.SecondPlayer.EnemyField.Cells };
        }

        [HttpPost]
        public object Post()
        {
            _gameAggr = new GameAggr();
            var game = _gameAggr.CreateGame();
            _games.Add(_gameAggr.Game.Id, _gameAggr);
            return new[] { game.FirstPlayer.OwnField.Cells, game.FirstPlayer.EnemyField.Cells };
        }

        [HttpPut]
        public object Put(MovePlayerCommand cmd)
        {
            var gameAggr = _games.FirstOrDefault(x => x.Key == cmd.GameId).Value;
            var game = gameAggr.MovePlayer(cmd);
            return new[] { game.FirstPlayer.OwnField.Cells, game.FirstPlayer.EnemyField.Cells };
        }
    }
}
