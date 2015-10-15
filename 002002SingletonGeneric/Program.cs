using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00202SingletonGeneric
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //4.10基于类型参数的Generic Singleton
    //相当与规范化Singleton，便于减少代码的编写，便于客户程序使用

    //*********C#定义抽象部分**********
    //定义一个非泛型的抽象基础
    //否则类型约束上，只能T：class,new(),相对而言约束不够严谨
    public interface ISingleton { }

    public abstract class SingletonBase<T>:ISingleton where T:ISingleton,new()
    {
        protected static T instance = new T();
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
    }

    //C#批量加工出一批访问点很“规矩”的Singleton类型
    //利用现有基础可以快速地构造出一批具有public static T Instance
    //类型特增的准Singleton类型，从整体上统一Singleton方式访问的入口
    class SingletonA : SingletonBase<SingletonA>
    {

    }


    //4.11有工厂类型协助Singleton实例管理

}
