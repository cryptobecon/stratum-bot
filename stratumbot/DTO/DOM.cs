using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    public class Depth
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Cost { get; set; }
    }

    public class DepthOfMarket
    {
        public Depth[] DOM { get; set; }
    }
}
