using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignePattern.Ioc._1getStarted.step1
{
    /// <summary>
    /// http://www.cnblogs.com/showjan/p/3950989.html
    /// </summary>
    class LogicController
    {
        public static void Run() {
            string message = "新年快乐！过节费5000.";
            GreetMessageService service = new GreetMessageService();
            service.Greet(message);
        }
    }

    public class EmailHelper
    {
        public void Send(string message)
        {
            Console.Write("Frome email: " + message);
        }
    }
    public class GreetMessageService
    {
        EmailHelper greetTool;
        public GreetMessageService()
        {
            greetTool = new EmailHelper();
        }
        public void Greet(string message)
        {
            greetTool.Send(message);
        }
    }
}
