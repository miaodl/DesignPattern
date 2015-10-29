using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace _10500FacadePattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第十二章外观模式

    //Provide a unified interface to a set of interfaces in a subsystem.Facade defines a higher-lecel interface that makes the subsystem easier to use

    //外观模式是设计模式中非常朴素地体现面向对象“封装”概念的模式，它的基本原理是将复杂的内部实现以统一接口的方式暴露出来，最大程度地减少客户程序对某些子系统内部众多对象的依赖关系。

    //从更高的角度看，SOA和Enterprise2.0藏到的软件及服务的概念，其实在一个新的层次上演绎着外观模式的理解--屏蔽语言，平台，开发工具和网络性的差异性，以统一的服务概念供外部还怂恿：从更低的角度看，为了便于开发人员使用我们包装的类型，很多时候也需要通过重载和恋官借口的方式，将最舒服的外观提供给别人，在满足多态差异性的同时，再干呢搞笑的子系统范围内--类内部实现更简明的外部接口。

    //外观模式的意图很明确，卫子系统的一组接口提供一个高层接口，该接口使子系统更易于使用。他的主要动机就是减少子系统内部与外部间对象通信的依赖复杂程度。


    /*
     * 外观模式的用于项目实施的具体优势
     * 
     * 子系统可能会随着开发过程的深入或客户需求的变化而越来越复杂，但从客户程序看，他需要的接口（也就是那个外观类型）规格相对固定，因此外观的隔离作用更利于客户程序的稳定
     * 
     * 应用中经常存在很多的子系统，为了避免客户程序使用中过多与子系统内部对象产生依赖，从高层逻辑角度，有必要每个子系统封装集中的外观对象
     * 
     * 在保证外观对象稳定的情况下，子系统自身可以根据运行环境需要做迁移。
     * 
     * 另外一个就是设计开发颗粒度的问题，本着2/8原则大部分情况子系统内部虽然有很多类型，能够提供非常细颗粒的调用支持，但客户程序往往只需要按照固定的流程操作这些对象，为此不妨把这些default的处理过程以外观对象的方式呈现给客户程序，同时，不封闭细颗粒度定制调用
     * 
     * 随着软件规模扩大，一些子系统往往分别由不同的团队独立开发，采用外观接口可大大简化将他们装配成引应用的过程，同时可提高子系统的重用性。
     * 
     */


    //在ADO.NET System.Data.Common命名空间的基础上实现一个简单的数据驱动无关的DataFacade
    public class DataFacade
    {
        private const string dbName = "AdventiureWorks";
        private static DbProviderFactory factory;
        private static string connectionString;

        /// <summary>
        /// 通过配置访问找到服务于具体数据库Provider的抽象工厂
        /// </summary>
        static DataFacade()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[dbName];
            factory = DbProviderFactories.GetFactory(settings.ProviderName);
            connectionString = settings.ConnectionString;
        }

        private static DbConnection CreateConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = DataFacade.connectionString;
            return connection;
        }

        public DataSet ExecuteQuery(string sql, params DbParameter[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            using(DbConnection connection=CreateConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                foreach(var p in parameters)
                {
                    command.Parameters.Add(p);
                }
                
                DbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;
                DataSet result = new DataSet();
                adapter.Fill(result);
                return result;
            }
        }

        public class TestClass
        {
            public void Test()
            {
                DataFacade facade = new DataFacade();
                DataSet result = facade.ExecuteQuery("ddd",new SqlParameter("name","yes"));
            }
        }



        /*
         * 
         * 外观接口对子系统的内部调用最好依从 依赖于抽象 的原则，使用抽象类型或接口，而不是面向具体类型，由于所依赖的常常是一组相关对象，有必要增加一个抽象工厂。
         * 
         * 由于外观类型经常被作为一个大包的API使用，而且具体执行过程中间调用子系统内部对象完成，因此很多时候使用只需要保留一个Singleton-N入口实例即可，或者索性定义一个静态类
         * 
         * 
         */




        //12.3 page 249 286 Facade接口


        /*
         * 
         *                    interface
         * client------------>IDataFacade<|——————————ConcreteDataFacade
         *   |                     |                                   |
         *   |                     |                                   |
         *   |                     |Create                           subSystem
         *   |                DataFacadeFactory                     System.Data
         *   |--------------->+Create():IDataFacade
         * 
         */





        //12.4RemoteFacade

        /*
         * 在外观类型需要的功能抽象为接口后，原则上外观类型的执行与客户程序就可以完全分离了，不仅从抽象对象角度看可以进行这个分离，而且跨进程，跨物理位置的调用也可称为可能，这是Facade变成RemoteFacade
         * 
         * Socket，Enterprise Service，WebService，.NET Remoting四种Remote Facade
         * 
         */


        /*演示,NET Remote之后基于外观接口在引用部署方面的优势，我们采用以下项目布局
         * 
         * 名称           类型              功能                      部署位置            依赖于       
         * 
         * Common       Class Library       定义Facade接口           客户程序 远程服务端
         * 
         * BussinessLogic Class Library     实现Facade接口定义的数据访问要求 远程服务端   Common
         * 
         * Host         Client Console      作为远程服务进程      远程服务端     Common BussinessLogic
         * 
         * Client       Windows Application     根据Common约定的Facade接口消费相关服务  客户程序 Common
        */

        //Page 251 288




        //12.5面向性能考虑升级的RemoteFacade    DataSet 变为 DataReader

    }



}
