using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Ninject
{
    public class SqlServerDataOrder : IDataAccess
    {
        public string QueryOrder()
        {
            return "sqlserver查询Order";
        }
    }
}
