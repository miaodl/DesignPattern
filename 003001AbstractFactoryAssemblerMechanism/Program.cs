using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _00301AbstractFactoryAssemblerMechanism
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    //5.2.2工厂模式的硬伤，处理模式的硬伤
    //新增加商品类型时，对全部的接口和抽象工厂类都要进行改动

    //5.3定义计划与生产之间的映射关系
    //5.3.1

    //前面的抽象工厂模式对某一个IAbstractFactory的处理提供了解决办法，可是项目中IAbstractFatory往往不只一个，毕竟几百个甚至上万个类的应用经常可以碰到，它们中发现“相关或相互依赖”的往往不只是个位数，因此IAbstractFactory也不只有单独的一个

    //解决方案，将Assembler提升为集中化的AssemblerMechanism，变每个Assembler支持一个IAbstractFactory为AssemblerMechanism支持一套TypeMapper/IAstractFactory字典即可


    //AssemblerMechanism
    //优势：大大降低类库的编码量，应用中几个主要领域的多组“一系列相关或相互依赖”的类型都可以套用版样快速完成。工程中推荐TypeMapperDictionary通过配置解决，通过访问配置文件一次性（或lazy方式）将需要使用的TypeManager/IAbstractFactory读取到TypeMapperDictionary里

    //局限性：通用性太强的问题容易导致对个性问题的处理不够理想。退回到经典抽象工厂模式

    //***********************************************
    //物品集合
    public interface IProductXA 
    { 

    }
    public interface IProductXB
    {

    }
    public interface IProductYA
    {

    }
    public interface IProductYB
    {

    }
    public interface IProductYC
    {

    }

    public class ProductXA1 : IProductXA { }
    public class ProductXA2 : IProductXA { }
    public class ProductXA3 : IProductXA { }

    public class ProductXB1 : IProductXB { }

    public class ProductYA1 : IProductYA { }

    public class ProductYB1 : IProductYB { }
    public class ProductYB2 : IProductYB { }

    public class ProductYC1 : IProductYC { }



    //5.3.2登记映射关系
    //定义一个类型对于类型映射的字典
    public abstract class TypeMapperBase:Dictionary<Type,Type>
    {

    }

    //定义一个类型对于保存类型映射字典的映射的字典
    public class TypeMapperDictionary:Dictionary<Type,TypeMapperBase>
    {

    }

    //5.3.3用TypeMapper协助协助工厂生产

    public interface IAbstractFactory
    {
        T Create<T>();
    }

    //增加了TypeMapper的IAbstractFactory
    public interface IAbstractFactoryWithTypeMapper:IAbstractFactory
    {
        TypeMapperBase Mapper { get; set; }
    }

    public abstract class AbstractFactoryBase:IAbstractFactoryWithTypeMapper
    {
        protected TypeMapperBase mapper;
        public virtual TypeMapperBase Mapper
        {
            get 
            {
                return mapper; 
            }
            set
            {
                mapper = value;
            }
        }
        public virtual T Create<T>()
        {
            Type targetType = mapper[typeof(T)];
            return (T)Activator.CreateInstance(targetType);
        }
    }


    //5.3.4定义实体TypeMapper和实体工厂

    //X系列和Y系列产品的具体TypeManager类
    public class ConcreteXTypeMapper:TypeMapperBase
    {
        public ConcreteXTypeMapper()
        {
            base.Add(typeof(IProductXA), typeof(ProductXA2));
            base.Add(typeof(IProductXB), typeof(ProductXB1));
        }
    }

    public class ConcreteYTypeMapper : TypeMapperBase
    {
        public ConcreteYTypeMapper()
        {
            base.Add(typeof(IProductYA), typeof(ProductYA1));
            base.Add(typeof(IProductYB), typeof(ProductYB1));
            base.Add(typeof(IProductYC), typeof(ProductYC1));
        }
    }

    public class ConcreteFactoryX:AbstractFactoryBase
    {

    }
    public class ConcreteFactoruY:AbstractFactoryBase
    {

    }
    
    //5.3.5实现装配机制
    //把分散的Assembler集中到一个统一的AssemblerMechanism之后，由于它自己维护一个TypeMapperDictionary字典，因此他知道（通过配置或外部的注册机制）该把那个TypeMapper实体植入到相应的IAbstractFactory中

    public static class AssemblerMechanism
    {
        //类似于单例模式？？？静态类在最初的时候实例化，所有静态对象都被调用
        private static TypeMapperDictionary dictionary = new TypeMapperDictionary();
        //加载相关TypeManager/IAbstractFactory的对应信息，实际项目中可以通过访问配置完成
        static AssemblerMechanism()
        {
            dictionary.Add(typeof(ConcreteFactoryX), new ConcreteXTypeMapper());
            dictionary.Add(typeof(ConcreteFactoruY), new ConcreteYTypeMapper());
        }

        //为AbstractFactory找到它的TypeMapper，并注入
        public static void Assembly(IAbstractFactoryWithTypeMapper factory)
        {
            TypeMapperBase mapper = dictionary[factory.GetType()];
            factory.Mapper = mapper;
        }

    }

    public class Client
    {
        public void Test()
        {
            //可以通过配置文件或者其他外部文件对客户程序进行注册，依赖注入
            IAbstractFactoryWithTypeMapper factory = new ConcreteFactoryX();
            AssemblerMechanism.Assembly(factory);//绑定TypeMapper
            IProductXB productXB = factory.Create<IProductXB>();

            //********
            
        }
    }



    //5.4配置生产计划
    //抽象工厂可以隔绝客户程序和一组“相关或有依赖关系”对象的创建关系，它的工作其实也是解决某和问题。


    //。。。。。。。。。。。。。。。。。。

    //5.6小结

    //抽象工厂模式时简单工厂的升华，它除了解决对象构造过程后置之外，更主要的是提供了一组“相关或具有依赖关系”的加载过程，也就是具有了一组类型构造过程组合和调度的能力；相对于工厂方法而言，它更像一个“封装”的工厂，而不是简单地提供创建某个类型产品的“方法”。
    //使用中可以把工厂方法、静态工厂、简单工厂、抽象工厂混合起来，一个建议的布局：
    //PAGE 136，173

    //静态工厂 作为最底层，主要是因为它不能被继承和进一步扩展，所以让它做最基本但是最通用的工作，实际工程中用Activator充当这个角色。
    //简单工厂方法的适用范围很广，根据应用的需要在某些需要隔绝抽象与具体的位置上适用。
    //工厂方法引用在有一套较纵深层次的对象上，可以通过实体工厂类型选择继承层次上一个合适的具体产品来构造，而客户程序则把握抽象的工厂类型和抽象的产品类型即可。
    //抽象工厂方法则用在某些应用领域上，面向某些应用子系统或专业领域的对象创建工作，相对而言我们可以更多地把它应用于项目的中高层设计中。



    
}
