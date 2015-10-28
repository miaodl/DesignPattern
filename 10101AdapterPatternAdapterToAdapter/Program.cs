using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _10101AdapterPatternAdapterToAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //8.4 Adapter ---- Adapter互联模式 Adapter To Adapter

    /*
     *                    客户程序
     *                        |
     *                        |
     *                        |
     *                    EndPoint
     *                    /       \
     *                   /         \
     *                  /           \
     *              AdapterA     AdapterB
     *              
     * 客户程序需要同时与两个对象频繁地打交道，而这个教导的过程非常固定，都是基于ITarget接口操作的，其中有可能需要对中间结果进行临时缓存，抽象一个EndPoint对象，负责两个或者更多的Adapter打交道
     * 
     * 
     * 客户程序与EndPoint之间的交互可能是同步的，也可能是一步的，至于调用的描述也相对简单，对于同步调用，需要客户程序从EndPoint获得两个Adapter的引用，然后调用即可；如果是异步方式，可以把客户程序交给EndPoint的任务
     * 
     * 表示为以下方式：
     * 
     * Direction：谁调用谁？A-B OR B-A大家都是一样的接口，所以称呼为Adapter Invoke Adapter
     * 
     * Requset Method:发起方执行的方法
     * 
     * Response Method：响应方执行的方法
     * 
     * Need Relay:是否需要传递Request Method计算的中间结果
     * 
     */

    public class OracleDatabase
    {
        public string GetDatabaseName()
        {
            return "Oracle";
        }
        public int Select()
        {
            return (new Random()).Next();
        }
        public void Add(int data)
        {

        }
    }

    public class SqlServerDatabase
    {
        public string GetDbName()
        {
            return "SqlServer";
        }
        public int Select()
        {
            return (new Random()).Next();
        }
        public void Add(int data)
        {

        }
    }

    public interface IDatabaseAdapter
    {
        string ProviderName
        {
            get;
        }
        int GetData();
        void SetData(int data);
    }


    public class OracleAdapter:IDatabaseAdapter
    {
        private OracleDatabase adaptee = new OracleDatabase();
        public string ProviderName
        {
            get
            {
                return adaptee.GetDatabaseName();
            }
        }
        public int GetData()
        {
            return adaptee.Select();
        }
        public void SetData(int data)
        {
            adaptee.Add(data);
        }

    }

    public class SqlServerAdapter : IDatabaseAdapter
    {
        private SqlServerDatabase adaptee = new SqlServerDatabase();
        public string ProviderName
        {
            get
            {
                return adaptee.GetDbName();
            }
        }
        public int GetData()
        {
            return adaptee.Select();
        }
        public void SetData(int data)
        {
            adaptee.Add(data);
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
                if (!dictionary.ContainsKey(name))
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



    //***********************************
    //客户端调用方式1：客户端直接调度Adapter
    public class TestClient1
    {
        public void Test()
        {
            DatabaseAdapterFactory factory = new DatabaseAdapterFactory();
            IDatabaseAdapter adapterA = factory.Create("Oracle");
            IDatabaseAdapter adapterB = factory.Create("SqlServer");

            adapterB.SetData(adapterA.GetData());
        }
    }
    
    //*****************************************
    //基于标准约定调度Adapter

    public class EndPoint
    {
        private IList<IDatabaseAdapter> adapters = new List<IDatabaseAdapter>();
        public EndPoint()
        {
            DatabaseAdapterFactory factory = new DatabaseAdapterFactory();
            adapters.Add(factory.Create("Oracle"));
            adapters.Add(factory.Create("SqlServer"));

            //构造函数部分执行其他Adapter的构造，准备工作的相关人物
        }
        public IDatabaseAdapter GetAdapter(int index)
        {
            return adapters[index];
        }

        public IDatabaseAdapter this[int index]
        {
            get
            {
                return adapters[index];
            }

        }
    }

    public class TestClient2
    {
        public void Test()
        {
            EndPoint endPoint = new EndPoint();
            endPoint[0].SetData(endPoint[1].GetData());
        }
    }


    //**************************************
    //8.4.4 方式3借助仿射和约定完成异步调用

    //项目如果就是处理各种数据交换迁移的，为每一种操作都定义EndPoint成本比较高，那么我们可以考虑把操作借助反射异步化，让EndPoint变得更通用，而各种客户程序可以使用同一个EndPoint


    //异步调用方法的委托
    delegate void AfterInvokeHandler(int requestIndex,int responseIndex,string requestMethod,string responseMethod);


    class EndPointGeneric
    {
        private IList<IDatabaseAdapter> adapters = new List<IDatabaseAdapter>();
        public EndPointGeneric()
        {
            DatabaseAdapterFactory factory = new DatabaseAdapterFactory();
            adapters.Add(factory.Create("Oracle"));
            adapters.Add(factory.Create("SqlServer"));

            //构造函数部分执行其他Adapter的构造，准备工作的相关人物
        }
        public IDatabaseAdapter GetAdapter(int index)
        {
            return adapters[index];
        }

        public IDatabaseAdapter this[int index]
        {
            get
            {
                return adapters[index];
            }

        }

        /// <summary>
        /// 通过反射，执行一项具体任务
        /// </summary>
        /// <param name="requestIndex"></param>
        /// <param name="requestMethod"></param>
        /// <param name="responseIndex"></param>
        /// <param name="responseMethod"></param>
        /// <param name="needRelay"></param>
        /// <param name="callback"></param>
        public void Invoke(int requestIndex,string requestMethod,int responseIndex,string responseMethod,bool needRelay,AfterInvokeHandler callback)
        {
            object requester = adapters[requestIndex];
            object responser = adapters[responseIndex];
            MethodInfo request = requester.GetType().GetMethod(requestMethod);
            MethodInfo response = responser.GetType().GetMethod(responseMethod);

            object result = response.Invoke(responser, null);
            if (needRelay)
                request.Invoke(requester, new object[] { result });
            else
                request.Invoke(requester, null);

            //异步回调，通知执行完毕
            callback(requestIndex, responseIndex, requestMethod, responseMethod);
            
        }


    }



    public class TestClient3AsyncEndPoint
    {
        public void Test()
        {
            EndPointGeneric endPointGeneric = new EndPointGeneric();
            //endPoint[0].SetData(endPoint[1].GetData());
            endPointGeneric.Invoke(0, "SetData", 1, "GetData", true, Log);
        }
        private void Log(int requestIndex,int responseIndex,string requestMethod,string responseMethod )
        {

        }

    }



    //8.5用配置约定适配过程

    public class DataBaseConnectionTest
    {
        public void Test()
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["sales"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(setting.ProviderName);
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = setting.ConnectionString;

        }
    }

    //ADO.NET2.0配置给我们一个不错的适配器类型配置思路，
    /*
     * 总结如下
     * 
     * 实现客户程序明确自己需要的接口ITarget
     * 
     * 设计App.Config(Web.Config)的结构，考虑扩展和需要，可以把这个配置部分设计为一个ConfigurationElementCollection,可以考虑动态增加
     * 
     * 
     * 考虑Adapter还需要调整那些特性，比如访问中的Package Size。。。。
     * 
     * 完成或实现第三方实现ITarget类库
     * 
     * 为这些类库里面实现了ITarget的类型起一个逻辑名称，并把他们的Quanlified名称登记在每个ITarget的ConfigurationElement中，相关的参数顺次登记在子节点中
     * 
     * 按增加一个工厂类，按照逻辑穿件ITarget
     */


}
