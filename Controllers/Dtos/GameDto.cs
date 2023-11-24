using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaWolfAggr.Controllers.Dtos
{
    public class GameDto
    {
        public IEnumerable<CellDto> OwnField { get; set; }
        public IEnumerable<CellDto> EnemyField { get; set; }
        public IEnumerable<CellDto[]> Ships { get; set; }
        public Guid CurrentPlayerId { get; set; }
        public Guid WinPlayerId { get; set; }
    }
}