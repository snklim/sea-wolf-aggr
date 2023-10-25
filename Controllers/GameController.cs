using Microsoft.AspNetCore.Mvc;

namespace SeaWolfAggr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            var gameAggr = new GameAggr();
            var game = gameAggr.CreateGame();
            return game.FirstPlayer.OwnFields.Cells;
        }

        [HttpPost]
        public object Post()
        {
            var gameAggr = new GameAggr();
            var game = gameAggr.CreateGame();
            return new[]{game.FirstPlayer.OwnFields.Cells,game.FirstPlayer.EnemyField.Cells };
        }
    }
}
