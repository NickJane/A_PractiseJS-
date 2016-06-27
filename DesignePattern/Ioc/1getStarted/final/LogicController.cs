using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignePattern.Ioc._1getStarted.step4
{

    /// <summary>
    /// http://www.cnblogs.com/showjan/p/3950989.html
    /// </summary>
    class LogicController
    {
        public static void Run() {
            

            string message = "新年快乐！过节费5000.";
            ISendable greetTool = SendToolFactory.GetInstance();
            GreetMessageService service = new GreetMessageService(greetTool);
            service.Greet(message);
        }
    }
    /// <summary>
    /// 创建发送实例的工厂. 
    /// </summary>
    public abstract class SendToolFactory
    {
        public static ISendable GetInstance()
        {
            string currentStyle = "DesignePattern.Ioc._1getStarted.step4.EmailHelper";
            try
            {
                //Assembly assembly = Assembly.LoadFile(GetAssembly()); // 加载程序集
                //object obj = assembly.CreateInstance(GetObjectType()); // 创建类的实例 
                object obj = Activator.CreateInstance(System.Type.GetType(currentStyle));
                return obj as ISendable;
            }
            catch
            {
                return null;
            }
        }

        //static string GetAssembly()
        //{
        //    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["AssemblyString"]);
        //}

        //static string GetObjectType()
        //{
        //    return ConfigurationManager.AppSettings["TypeString"];
        //}
    }


    /// <summary>
    /// 现在有邮件和短信两种问候方式
    /// 把发送抽象成接口
    /// </summary>
    public interface ISendable {
        void Send(string message);
    }
    #region 实现接口 EmailHelper TelephoneHelper.  只继承接口实现其他的. 稳定了
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


    #region 服务块.  稳定

    public class GreetMessageService
    {
        ISendable greetTool;
        public GreetMessageService(ISendable _greetTool)
        {
            greetTool = _greetTool;
        }

        public void Greet(string message)
        {
            greetTool.Send(message);
        }
    }
#endregion
}
