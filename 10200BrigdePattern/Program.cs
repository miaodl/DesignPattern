using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10200BrigdePattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第九章 桥模式

    //Decouple an abstraction from its implementation so that two can vary independently

    //将抽象部分与它的实现部分分离，使他们都可以独立地变化


    //桥模式是否可以理解为接口的嵌套？？？？？

    /*
     * 桥接模式主要应用的情景
     * 
     * 一个类型需要在它某个因素的抽象特征与具体特征间提供更高的灵活性
     * 
     * 一个类型收到多种因素变化的影响，每个因素具体采用何种表征需要在运行过程中动态匹配
     * 
     * 对其中某一个具体实现部分的修改不需要影响客户程序
     * 
     * 有些时候，希望在多个对象间共享某些因素的具体实现
     * 
     * 还有就是分工的需要，对于存在多个因素变化的对象，将每个变换因素变化情景的具体实现分工给其他开发人员，而对象本身的设计仅依赖于每个因素的抽象特征
     * 
     */


    public interface IImpl
    {
        void OperationImpl();
    }

    public interface IAbstraction
    {
        IImpl Implementor
        {
            get;
            set;
        }
        void Operation();
    }


    public class ConcreteImplementatorA:IImpl
    {
        public void OperationImpl()
        {

        }
    }
    public class ConcreteImplementatorB : IImpl
    {
        public void OperationImpl()
        {

        }
    }

    public class RefinedAbstraction:IAbstraction
    {
        private IImpl implementor;

        public IImpl Implementor
        {
            get { return implementor; }
            set { implementor = value; }
        }

        public void Operation()
        {
            implementor.OperationImpl();
        }

    }

    /*
     * 桥模式更多的是提示我们如何组织面向对象的设计分解方式
     * 
     * 1，把依赖具体变成依赖抽象
     * 
     * 2，如果同时沿用多个维度变化，那就顺次展开抽象因素
     * 
     * 3，为每个抽象因素提供具体实现
     */



    //9.3将复杂性进一步分解后的多级桥关系


    /*
     * 
     * 
     * 
     *                 (some implement here)    (some implement here)
     *                          |                    |
     *                          |                    |
     *                      interface            interface
     *                      IDataFormatter       ISqlBuilder
     *                        /|                   /|
     *                       /                    /
     *                      /                    /  
     *                     <>                   <>
     * client--------->interface<>--------->interface<>---------->interface
     *              IDataControl           IDataCommand           IDataConnection 
     *                    |                       |                       \
     *                    |                       |                        \
     *              (some implement here)     (some implement here)      (some implement here)
     * 
     * 
     */

    public interface IDataConnection
    {
        //数据库打开连接
        void Open();

        //返回数据，需要调用查询语句ISqlBuilder
        DataSet Query(string sql);
    }

    public interface ISqlBuilder
    {
        //将字符串拼成sql语句
        string GetWhereClause(object data);
    }

    public interface IDataCommand
    {
        //完成执行数据库命令需要  数据库连接方式和查询语句
        IDataConnection Connection { get; set; }
        ISqlBuilder Builder { get; set; }

        DataSet Execute(string databaseName,string tableName,object data);
    }

    public interface IDataFormatter
    {
        //将返回的数据集格式化为HTML等能在控件中显示的格式
        //具体的实现在调用该接口的类中实现
        string Format(DataSet dataSet);
    }

    public interface IDataControl
    {
        IDataCommand Command { get; set; }
        //将返回的数据集格式化为HTML等能在控件中显示和阅读的格式
        IDataFormatter Formatter { get; set; }

        //用户的选择或输入信息
        object Data { get; }

        string TableName { get; set; }

        string DatabaseName { get; set; }


        //负责Data提交给IDataCommand
        void Bind();
    }




    public class UserController:IDataControl
    {
        private string data;
        private string tableName;
        private string dataBase;
        private IDataCommand command;
        private IDataFormatter formatter;

        private string content;
        private DataSet ds;

        public string Content
        {
            get { return (ds==null)?null:formatter.Format(ds); }

        }

        public IDataCommand Command
        {
            get { return command; }
            set { command = value; }
        }

        public IDataFormatter Formatter
        {
            get { return formatter; }
            set { formatter = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public string DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

       

        public void Bind()
        {
            ds = command.Execute(dataBase, tableName, data);
        }
    }



    //********************************

    //个人总接，将整个流程抽象描述，使用借口来抽象个整个业务流程的顺序，然后抽象成接口嵌套接口调用


    //使用接口描述行为的方式，层次化了业务模型，每个接口作为一个结点，并提供多方面的具体实现


    //以上详细见Page 207 244



    //***************************************************************

    //9.4看着图造桥


    //9.5具有约束关系的桥

    /*
     * 
     * 
     * 
     *                 (some implement here)    (some implement here)
     *                          |                    |
     *                          |                    |
     *                      interface            interface
     *                      IDataFormatter       ISqlBuilder
     *                        /|                   /|
     *                       /                    /
     *                      /                    /  
     *                     <>                   <>
     * client--------->interface<>--------->interface<>---------->interface
     *              IDataControl           IDataCommand           IDataConnection 
     *                    | \                   | |                    /  \
     *                    |  \                  | |                   /    \
     *              (some implement here)     (some implement here)  /   (some implement here)
     *                         \                |                   /
     *                          \               |                  /
     *                           \___________Context______________/  
     * 
     * 
     * 
     */

    //设计一个上下文对象Context供各个接口进行数据共享
    //从使用方式看，Context相当于一个OutBound机制，它可以用于桥模式中一组对象抽象行为不变的情况，再具体实现部分增加这个Context交互，锲入更多控制信息的机制。



    //9.6小结

    //桥模式的功能是把多个维度变化情况通过关联一组抽象对象，把变化的影响控制在每个局部，进而在运行过程中动态加载，把客户程序与复合这一组抽象定义的具体类型进行协作，它关注的是结构性的布局问题。
    /*
     * 
     * 对于更多因素，尤其是具有分支情况的对象依赖关系做了介绍，考虑到动态加载的需要，把每个接口所以来的另一个接口都设计为Setter方式注入的形式
     * 
     * 
     * 多个依赖对象见需要共享一些公共控制的情况，基于Context的解决思路。
     * 
     */

}
