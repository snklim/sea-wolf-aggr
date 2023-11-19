using System;
using Microsoft.AspNetCore.Mvc;

namespace SeaWolfAggr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static GameAggr _gameAggr;

        [HttpGet]
        public object Get()
        {
            var gameAggr = new GameAggr();
            var game = gameAggr.CreateGame();
            return game.FirstPlayer.OwnField.Cells;
        }

        [HttpPost]
        public object Post()
        {
            _gameAggr = new GameAggr();
            var game = _gameAggr.CreateGame();
            return new[] { game.FirstPlayer.OwnField.Cells, game.FirstPlayer.EnemyField.Cells };
        }

        [HttpPut]
        public object Put(MovePlayerCommand cmd)
        {
            var game = _gameAggr.MovePlayer(cmd);
            return new[] { game.FirstPlayer.OwnField.Cells, game.FirstPlayer.EnemyField.Cells };
        }
    }
}
