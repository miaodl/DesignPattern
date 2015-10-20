using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00300AbstractFactoryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第五章
    //page 158 121    设计模式：基于C#的工程化实现及扩展


    //provide an interface for creating families of related or dependent objects
    //设计模式-可复用面对对象软件的基础  对抽象工厂的意图的描述----提供一个创建一系列相关或相互依赖对象的接口，而无须指定他们的具体类型

    //简单工厂模式和工厂方法模式，他们的核心目标都是一致的：直接有客户程序创建对象的时候，由于目标对象本身可能经常会变化，但他们的抽象能力却相对固定，因此我们需要通过工厂吧这个创建过程封装出来，让客户程序不需自己new()目标类型

    //简单工厂方法的职责非常简单：构造某个实体类型，然后把实例作为抽象类型返回；工厂方法则进一步抽象出一个抽象的创建者和抽象的产品类型，不过因为具体工厂和具体产品类型都可以被抽象为之前定义的抽象创建者和抽象类型，因此即便面对一个很庞大的具有家族关系的类型系统，客户程序在操作的过程中也可以基于抽象创建者获得满足某种抽象类型的产品实例。

    //设计模式-可复用面对对象软件的基础  对抽象工厂的意图的描述----提供一个创建一系列相关或相互依赖对象的接口，而无须指定他们的具体类型
    //1，可以反回一系列相关或相互以来对象的接口
    //2，需要指定一个接口，也就是抽象工厂，他的接口定义中包含返回那些相关对象接口的方法定义


    //C#
    //定义不同类型的抽象产品
    public interface IProductA
    {

    }
    public interface IProductB
    {

    }

    //定义抽象工厂
    public interface IAbstractFactory
    {
        IProductA CreateProductA();
        IProductB CreateProductB();
    }

    //定义实体产品
    public class ProductA1:IProductA
    {

    }
    public class ProductA2X:IProductA
    {

    }

    public class ProductA2Y:IProductA
    {

    }

    public class ProductB1:IProductB
    {

    }
    public class ProductB2:IProductB
    {

    }

    //实体工厂
    public class ConcreteFactory1:IAbstractFactory
    {

        public virtual IProductA CreateProductA()
        {
            return new ProductA1();
        }

        public virtual IProductB CreateProductB()
        {
            return new ProductB1();
        }
    }

    public class ConcreteFactory2 : IAbstractFactory
    {

        public virtual IProductA CreateProductA()
        {
            return new ProductA2Y();
        }

        public virtual IProductB CreateProductB()
        {
            return new ProductB2();
        }
    }

        
    public class Client
    {
        //UNIT TEST
        public void Testc()
        {
            IAbstractFactory factory =new ConcreteFactory2();
            IProductA productA=factory.CreateProductA();
            IProductB productB=factory.CreateProductB();
            //。。。。。。。。。。。。
            //虽然客户端依赖抽象的IAbstractFactory，但是为了获得IAbstractFactory，需要new ConcreteFactory2(),逻辑上和具体的工厂类型耦合在一起，“依赖注入”非常适用于结局这个问题，比如可以把ConcreteFactory2写在配置文件里，通过读取配置文件动态构造IAbstractFactory。
        }
    }
    
    //5.2按计划实施生产 PAGE 125 162

    //5.2.1为抽象工厂补充类型映射器
    //上面实例中每个Concrete Factory执行的内容都是固定的，即一次new()出每类对象，这几乎就是foreach过程，如果我们把之前把一批需要做好的任务一次性的告诉某个实体IAbstractFactory类型，是不是就可以吧这个过程自动化呢？

    public class ConcreteFactory:IAbstractFactory
    {
        private Type typeA;
        private Type typeB;
        public ConcreteFactory(Type typeA, Type typeB)
        {
            this.typeA = typeA;
            this.typeB = typeB;
        }

        public virtual IProductA CreateProductA()
        {
            return (IProductA)Activator.CreateInstance(typeA);
        }

        public virtual IProductB CreateProductB()
        {
            return (IProductB)Activator.CreateInstance(typeB);
        }


        //typeA和typeB通过配置文件获得，ConfigurationManager

    }

    //泛化CreateProduct()方法，改进
    //C#
    public interface IAbstractFactoryGai
    {
        T Create<T>() where T : class;
    }
    public abstract class AbstarctFactoryBase:IAbstractFactoryGai
    {
        protected IDictionary<Type, Type> mapper;
        public AbstarctFactoryBase(IDictionary<Type,Type> mapper)//构造函数注入
        {
            this.mapper = mapper;
        }

        public virtual T Create<T>() where T : class
        {
            if((mapper==null)||(mapper.Count==0)||(!mapper.ContainsKey(typeof(T))))
            {
                throw new ArgumentException("T");
            }
            Type targetType = mapper[typeof(T)];
            return (T)Activator.CreateInstance(targetType);
        }
    }
    public class ConcreteFactoryGai:AbstarctFactoryBase
    {
        public ConcreteFactoryGai(IDictionary<Type, Type> mapper) : base(mapper) { }
    }


    //测试客户端

    public class Assembler
    {
        public IAbstractFactoryGai AssembleyFactory()
        {
            //生成映射关系字典，将字典注入到IAbstractFactory中，实际项目中通过读取配置文件完成
            IDictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
            dictionary.Add(typeof(IProductA),typeof(ProductA1));
            dictionary.Add(typeof(IProductB),typeof(ProductB1));
            return new ConcreteFactoryGai(dictionary);
        }
    }

    public class TestClient
    {


        public void Test()
        {
            IAbstractFactoryGai factory = (new Assembler()).AssembleyFactory();
            IProductA productA = factory.Create<IProductA>();
            IProductB productB = factory.Create<IProductB>();
        }
    }


    //5.2.2工厂模式的硬伤，处理模式的硬伤
    //新增加商品类型时，对全部的接口和抽象工厂类都要进行改动

    //5.3定义计划与生产之间的映射关系
    //5.3.1


}
