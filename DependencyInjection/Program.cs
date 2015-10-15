using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * 依赖注入 
 * Constructor构造函数注入
 * setter属性索引器注入
 * 接口注入
 * Attribute属性注入
 * 
 */

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {

            ITimeProvider timeProvider = (new Assembler()).Create<ITimeProvider>();
            Client mm = new Client(timeProvider);
            Console.WriteLine(mm.GetYear());
            Console.ReadKey();
            
        }
    }


    interface ITimeProvider
    {
        DateTime CurrentDate { get; }
    }

    class TimeProvider : ITimeProvider
    {
        public DateTime CurrentDate { get { return DateTime.Now; } }
    }

    //public class Client
    //{
    //    public int GetYear()
    //    {
    //        ITimeProvider timeProvider = new TimeProvider();
    //        return timeProvider.CurrentDate.Year;
    //    }
    //}

    public class Assembler
    {
        //保存抽象类型实体类型对应关系的字典
        private static Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();

        //******************************************************************
        /// <summary>
        /// 注册抽象类型需要使用的实体类型
        /// 实际的配置信息可以从外层机制获得，例如通过配置定义
        /// </summary>
        static Assembler()
        {
           dictionary.Add(typeof(ITimeProvider), typeof(SystemTimeProvider));
        }

        /// <summary>
        /// 根据客户程序需要的抽象类型选择相应的实体类型，并返回类型实例
        /// 主要用于非泛型方式的调用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Create(Type type)
        {
            if ((type == null) || !dictionary.ContainsKey(type)) throw new NullReferenceException();
            Type targetType = dictionary[type];
            //此处使用反射的特性
            return Activator.CreateInstance(targetType);
        }
        /// <summary>
        /// 主要用于非泛型方式的调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

    }

    class Client
    {
        private ITimeProvider timeProvider;
        /// <summary>
        /// 在构造函数中注入
        /// </summary>
        /// <param name="timeProvider"></param>
        public Client(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }
        public int GetYear()
        {
            return timeProvider.CurrentDate.Year;
        }
    }

    class SystemTimeProvider:ITimeProvider
    {

        public DateTime CurrentDate
        {
            get { return DateTime.Now; }
        }
    }
}
