using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFix
{
    internal class Fund
    {
        public Dictionary<string,float> buy = new Dictionary<string,float>();
        public Dictionary<string, float> sell = new Dictionary<string, float>();
        public string accountName = "";

        public Fund(string account)
        {
            accountName = account;
        }

        public Fund()
        {

        }


    }
}
