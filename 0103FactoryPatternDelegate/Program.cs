using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00103FactoryPatternDelegate
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public interface IFactory<T>
    {
        T Create();
    }


    //委托工厂类型
    //相对而言，前面的实现（抽象工厂，参数化批量化工厂），毕竟加工的是一个类型对象，但是在C#实现设计模式的时候，很多行为型和结构型模式需要的内容就是类的方法。受到语言的限制，JAVA等很多语言必须用一个接口，然后由工厂把接口加工出来，但是C#又更经济的解决办法——委托。


    //C#委托本质上就是对具体执行方法的抽象，相当于Product的角色
    public delegate int CalculateHandler(params int[] items);
    
    class Calculator
    {
        //这个方法相当于Delegate Factory看到的Concrete Product
        public int Add(params int[] items)
        {
            int result = 0;
            foreach (int item in items)
                result += item;
            return result;
        }
    }

    //Concrete Factory
    public class CalculateHandlerFactory:IFactory<CalculateHandler>
    {
        public CalculateHandler Create()
        {
            return (new Calculator()).Add;
        }
    }
}
