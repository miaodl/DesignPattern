using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00100AbstractFactory
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    /*简单工厂
    //简单工厂
    public interface IProduct
    {

    }

    public class ConcreteProductA : IProduct
    {

    }
    public class ConcreteProductB : IProduct
    {

    }

    public class Factory
    {
        public IProduct Create()
        {
            return new ConcreteProductA();
        }
    }

    ////Test
    ////隐藏了new()
    //Factory factory=new Factory();
    //IProduct product=factory.Create();

    /*
     * 1.通过IProduct隔离了客户程序与具体ConcreteProductX的依赖关系，在客户程序视野内没有ConcreteProductX
     * 2.即使ConcreteProductX增加，删除方法或属性，无房大局，只要按照要求实现IProduct就可以，Client无须关心ConcreteProductX的变化（确切的说它也关心不着）
     * 
     */


    //把工厂设计为Singleton方式，因为工厂的职责相对单一，所有客户端需要的加工过程使用的都是一个唯一的共享实例。



    //参数化工厂

    //C#抽象产品类型与具体产品类型
    //抽象产品类型
    public interface IProduct
    {
        string Name { get; }//约定的抽象产品所必须具有的特征
    }

    //产品的具体类型
    public class ProductA : IProduct
    {
        public string Name
        {
            get { return "A"; }
        }
    }

    public class ProductB : IProduct
    {
        public string Name
        {
            get { return "B"; }
        }
    }

    //C#抽象的工厂类型描述
    public interface IFactory
    {
        IProduct Create();//每个工厂需要具有的工厂方法创建产品
    }

    //C#两个实体的工厂类型
    public class FactoryA:IFactory
    {
        public IProduct Create()
        {
            return new ProductA();
        }
    }

    public class FactoryB : IFactory
    {
        public IProduct Create()
        {
            return new ProductB();
        }
    }

    //C#为Assembler增加有关IFactory的注册项
    //public class Assembler
    //{
    //    private static Dictionary<Type,Type> dictionary=new Dictionary<Type,Type>();
        
    //    static Assembler()
    //    {
    //        //注册抽象类型需要使用的实体类型
    //        //实际的配置信息可以从外层机制获得，例如配置文件
    //        dictionary.Add(typeof(IFactory),typeof(FactoryA));
    //        dictionary.Add(typeof(IFactory),typeof(FactoryB));
    //    }

    //    /// <summary>
    //    /// 根据客户程序需要的抽象类型选择相应的实体类型，并返回类型实例
    //    /// 主要用于非泛型方式的调用
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public object Create(Type type)
    //    {
    //        if ((type == null) || !dictionary.ContainsKey(type)) throw new NullReferenceException();
    //        Type targetType = dictionary[type];
    //        //此处使用反射的特性
    //        return Activator.CreateInstance(targetType);
    //    }
    //    /// <summary>
    //    /// 主要用于非泛型方式的调用
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public T Create<T>()
    //    {
    //        return (T)Create(typeof(T));
    //    }
    //}

    //C#相应客户程序就可以修改为依赖Assembler的方式
    class Client
    {
        private IFactory factory;
        public Client(IFactory factory)
        {
            //外部机制将IFactory借助Assembler以Setter方式注入
            if (factory == null) throw new ArgumentNullException("factory");
            this.factory = factory;
        }

        //..............
    }



    //基于配置文件的Factory
    //上面的方法虽然已经解决了很多问题，但是工程中我们往往要要求Assembler知道更多的IFactory/ConcreteFactory配置关系，而且考虑到应用的Plug和Play要求，新写的ConcreteFactory类型最好保存在一个额外的程序集李米娜，不涉及既有的已部署好的应用。
    //一个常见的选择就是把Assembler需要加载的IFactory/Concrete Factory列表保存到配置文件里。

    public class Assembler
    {
        //配置节名称
        private const string SectionName = "";
        //IFactory在配置文件中的键值
        private const string FactoryTypeName = "IFactory";

        //保存抽象类型和实体类型对应关系的字典
        private static Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();

        static Assembler()
        {
            //通过配置文件加载相关抽象产品类型实体产品类型的映射关系
            NameValueCollection collection = (NameValueCollection)ConfigurationManager.GetSection(SectionName);
            for (int i = 0; i < collection.Count; i++)
            {
                string target = collection.GetKey(i);
                string source = collection[i];
                dictionary.Add(Type.GetType(target), Type.GetType(source));
            }

        }
    }



    //小结
    //经典工厂方法给我们设计应用一个很好的模式化new()思维，通过引入新的对象，在很大程度上街坊了客户程序对具体类型的依赖，其方法就是延迟构造到子类，单依赖关系是始终存在的，解放一个对象的时候等于把麻烦转嫁到另一对象上。为此，工程中往往把最后推不掉的依赖关系推到配置文件上，也就是推到.NET Framework上，这样经典的工厂方法模式往往在项目中被演绎成“工厂方法+依赖注入+配置文件访问”的组合方式。
    //考虑到性能因素，或者干脆为了省区多次调用的麻烦，工程中往往会有生产一批对象的需要，比如生成具有某些配置选项的多个线程，这时候就会用到批量工厂对象工厂。但如前面章节所讨论，写Demo和完成一个项目是两码事，其中一个关键的因素就是人，为了让整个实施过程中工厂实现“中规中矩”，有时候要统一一些内容，泛型工厂能冲根上约束整个工厂体系的加工方法命名，也许看起来需要使用一段时间，单在使用上起码多了个选择。




}
