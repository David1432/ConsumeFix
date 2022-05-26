using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFix
{
    internal class Orders
    {
        public float price = 0;
        public string symbol = "";
        public string side = "";

        public Orders()
        {

        }

        public Orders(float price, string symbol, string side)
        {
            this.price = price;
            this.symbol = symbol;
            this.side = side;
        }
    }
}
