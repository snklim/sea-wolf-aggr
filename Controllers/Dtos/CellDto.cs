using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaWolfAggr.Controllers.Dtos
{
    public class CellDto
    {
        public string CellType { get; set; }
        public PosDto Pos { get; set; }
        public bool IsDestroyed { get; set; }
    }
}