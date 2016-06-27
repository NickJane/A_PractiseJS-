using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program2
    {
        void Main(string[] args)
        {

            Console.Read();

            var task1 = new Task(() =>
            {
                Console.WriteLine("Task 1 Begin");
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Task 1 Finish");
            });
            var task2 = new Task(() =>
            {
                Console.WriteLine("Task 2 Begin");
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("Task 2 Finish");
            });

            
            task1.Start();
            task2.Start();
            var result = task1.ContinueWith<string>(task =>
            {
                Console.WriteLine("task1 finished!");
                return "This is task result!";
            });
            
            Console.WriteLine(result.Result.ToString());

            Console.Read();

            Console.WriteLine("current thread : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            var task11 = new Task(() =>
            {
                Console.WriteLine("task thread : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                //TODO you code
            });
            task1.Start();

            var task22 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Hello,task started by task factory, task thread : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            });

            //List<int> list = new List<int>();
            //Parallel.For(0, 10000, item =>
            //{
            //    list.Add(item);
            //});
            //Console.WriteLine("List's count is {0}", list.Count());

            /*
            Console.WriteLine("当前线程是main, id是 : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Parallel.For(0, 100, (i, state) =>
            {
                Console.WriteLine("当前线程, id是 : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                Console.Write(i + "\t");
            });

            Console.ReadKey();
            var a = new ParallelDemo();
            a.ParallelForMethod();
            a.ParallelInvokeMethod();
            */

            Console.Read();
        }
       

    }
    public class ParallelDemo
    {
        private Stopwatch stopWatch = new Stopwatch();

        public void Run1()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Run1当前线程, id是 : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Task 1 is cost 2 sec");
        }
        public void Run2()
        {
            Thread.Sleep(3000);
            Console.WriteLine("Run2当前线程, id是 : {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Task 2 is cost 3 sec");
        }

        public void ParallelInvokeMethod()
        {
            stopWatch.Start();
            Parallel.Invoke(Run1, Run2);
            stopWatch.Stop();
            Console.WriteLine("Parallel run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Restart();
            Run1();
            Run2();
            stopWatch.Stop();
            Console.WriteLine("Normal run " + stopWatch.ElapsedMilliseconds + " ms.");
        }
        public void ParallelForMethod()
        {
            
            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += i;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("NormalFor run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += item;
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");

        }
    }
}
