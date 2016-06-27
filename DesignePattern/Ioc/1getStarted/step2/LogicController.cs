using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignePattern.Ioc._1getStarted.step2
{
    /// <summary>
    /// http://www.cnblogs.com/showjan/p/3950989.html
    /// </summary>
    class LogicController
    {
        public static void Run() {
            string message = "新年快乐！过节费5000.";
            GreetMessageService service = new GreetMessageService(SendToolType.Email);
            service.Greet(message);
        }
    }

    /// <summary>
    /// 现在有邮件和短信两种问候方式
    /// 把发送抽象成接口
    /// </summary>
    public interface ISendable {
        void Send(string message);
    }
    #region 实现接口 EmailHelper TelephoneHelper
    public class EmailHelper:ISendable
    {
        public void Send(string message)
        {
            Console.Write("Frome email: " + message);
        }
    }
    public class TelephoneHelper : ISendable
    {
        public void Send(string message)
        {
            Console.Write("Frome TelephoneHelper : " + message);
        }
    }
    #endregion


    #region 服务块
    public enum SendToolType
    {
        Email,
        Telephone,
    }
    public class GreetMessageService
    {
        ISendable greetTool;
        public GreetMessageService(SendToolType sendToolType)
        {
            //构造函数内实例化响应的实现
            if (sendToolType == SendToolType.Email)
            {
                greetTool = new EmailHelper();
            }
            else if (sendToolType == SendToolType.Telephone)
            {
                greetTool = new TelephoneHelper();
            }
        }

        public void Greet(string message)
        {
            greetTool.Send(message);
        }
    }
#endregion
}
