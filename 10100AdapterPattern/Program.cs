using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10100AdapterPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第八章 适配器模式

    //Convert the interface of a class int another interface clients expect.Adapter lets classes work together that couldnt otherwise because of incompatibel interfaces

    //adapter改造接口

    //适配器适主要有三个作用
    //1，完成旧接口到新街口的转换
    //2，将既有系统进行封装，逻辑上客户程序不知道既有系统的存在，将变化隔离在Adapter部分
    //3，如果客户程序需要迁移，仅需要在Adapter部分做修改


    //适配器模式的意图就是通过接口转换，是本来不兼容的接口可以协作。
    //一些类在项目中可以被反复使用，但常常碰到因为接口参数，返回值类型不匹配需要做小幅修改的情况。适配器模式给我们另一个思路，不是对现有类和接口进行改造，而是通过改造增加新对象完成转换工作。这个模式主要用于下述环境：
    //1，我们需要复用一些已经存在的类，但这些类提供的接口不满足我们新的需要，尤其之前这些类使我们花了很大成本完成的，他们经过项目考察表现得确实一直很稳定。
    //2，我们封装一些公共的类，他们会经常被重用，但我们不确定目标环境需要何种接口。
    //3，新的接口可能需要的接口功能是之前几个类所能提供的功能，而且新接口与他们不兼容。

    
    //************************************
    //类适配器 和 对象适配器  区别

    //类适配器：适配器继承被适配的类并实现了客户所依赖的接口，客户端通过所期待的接口调用被适配类

    //对象适配器：适配器调用被适配类的方法并实现了客户所依赖得接口，客户端通过所期待的接口调用被适配类


    //定义ITarget（客户所期待的接口）和Adaptee（被适配的类）
    public interface ITarget
    {
        void Request();
    }

    public class Adaptee
    {
        public void SpecifiedRequest()
        {

        }
    }

    //类适配器
    public class Adapter:Adaptee,ITarget
    {
        public void Request()
        {
            base.SpecifiedRequest();
        }
    }

    //对象适配器
    public class AdapterObject:ITarget
    {
        private Adaptee adaptee=null;//Adaptee 私有对象

        public void Request()
        {
            adaptee.SpecifiedRequest();
        }
    }


    /*
     * Class Adapter
     * 类适配器
     * 基于继承的概念inherit
     * 适配器之前不能继承自其他类，Target只能是接口形式ITarget
     * 可以覆盖Adaptee的某些方法
     * 虽然不可以适配子类，但是可以通过覆盖修改某些方法，部分情况下可以达到适配子类同样的效果
     * 
     * 
     * Object Adapter
     * 对象适配器
     * 基于对象组合的思路composite
     * Target可能是ITarget(接口)，TargetBase（抽象类），甚至是实体，只要Adapter满足不继承两个或两个以上类的限制
     * 无法覆盖Adaptee的方法
     * 不仅可以适配Adaptee，还可以适配Adaptee的任何子类
     * 
     */

    //8.3进一步扩展适配范围的组适配器    组适配器
    //组适配器 只能通过对象适配器的方式，限于单继承的要求，类适配器不能用


    public class OracleDatabase
    {
        public string GetDataBaseName()
        {
            return "oracle";
        }
    }

    public class SqlServerDatabase
    {
        public string DbName()
        {
            return "sqlserver";
        }
    }


    //定义抽象Adapter和两个具体Adapter
    
    public interface IDatabaseAdapter
    {
        string ProviderName { get; }

        string GetDetail();
    }

    public class OracleAdapter:IDatabaseAdapter
    {
        private OracleDatabase adaptee = new OracleDatabase();
        public string ProviderName { get { return adaptee.GetDataBaseName(); } }

        public string GetDetail()
        {
            return "this is oracle";
        }
    }

    public class SqlServerAdapter: IDatabaseAdapter
    {
        private SqlServerDatabase adaptee = new SqlServerDatabase();
        public string ProviderName { get { return adaptee.DbName(); } }

        public string GetDetail()
        {
            return "this is sqlserver";
        }
    }

    //工厂类型
    public sealed class DatabaseAdapterFactory
    {
        class DatabaseAdapterMapper
        {
            //用于注册要生成适配器的信息
            private static IDictionary<string, Type> dictionary = new Dictionary<string, Type>();

            //静态构造函数,在类调用后只创建一次，意思是只读取一次构造函数
            //静态构造函数用于初始化任何 静态 数据，或用于执行仅需执行一次的特定操作。 在创建第一个实例或引用任何静态成员之前，将自动调用静态构造函数。
            static DatabaseAdapterMapper()
            {
                //每次调用时后刷新
                dictionary.Clear();

                //数据库注册列表信息可以用构造函数依赖注入的方式从配置文件读取信息
                dictionary.Add("Oracle", typeof(OracleAdapter));
                dictionary.Add("SqlServer", typeof(SqlServerAdapter));
            }
            ///根据数据库类型，获得指定的Adapter类型名称
            public Type GetDateBaseType(string name)
            {
                if(!dictionary.ContainsKey(name))
                    throw new NotSupportedException(name);
                return dictionary[name];
            }

        }


        private DatabaseAdapterMapper mapper = new DatabaseAdapterMapper();
        //根据数据库类型返回DatabaseAdapter
        public IDatabaseAdapter Create(string name)
        {
            return (IDatabaseAdapter)(Activator.CreateInstance(mapper.GetDateBaseType(name)));
        }

    }

    public class TestClass
    {
        public string Test()
        {
            DatabaseAdapterFactory factory = new DatabaseAdapterFactory();
            IDatabaseAdapter adapter = factory.Create("oracle");

            return adapter.ProviderName;
        }
    }

    



    
    
}
