using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
namespace Test.Ninject
{
    public class RegisterIoc
    {
        private StandardKernel _kernel = new StandardKernel();

        public RegisterIoc() {
            _kernel.Bind<IDataAccess>().To<MySqlDataOrder>();
        }

        public T Get<T>() {
            return _kernel.Get<T>();
        }

        public void Dispose() {
            _kernel.Dispose();
        }
    }
}
