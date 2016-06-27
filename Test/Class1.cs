using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public abstract class Animal
    {
        public abstract void ShowType();

        public void Eat() { Console.WriteLine("animal can eat;"); }
    }

    public class Bird : Animal {
        private string type = "Bird";
        public override void ShowType()
        {
            Console.WriteLine("type is {0}", type);
        }

        private string color;
        public string Color { get; set; }
    }

    public class Chicken : Bird {
        private string type = "chicken";
        public void ShowType()
        {
            Console.WriteLine("type is {0}", type);
        }

        public void ShowColor() {
            Console.WriteLine("color is {0}", Color);
        }
    }
}
