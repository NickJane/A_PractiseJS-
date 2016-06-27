using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Ninject
{
    public class MySqlDataOrder : IDataAccess
    {
        public string QueryOrder()
        {
            return "MySql查询Order";
        }
    }
}
