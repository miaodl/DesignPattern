using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10601FlyweightPatternObjectPool
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    //***************************************************************************
    //13.3指定共享计划


    /*
     * 享元模式的主要目的是面向性能的，而生产环境可以直接调整性能是系统管理员而不是开发人员，为了链接开发人员实现中的“享元”和系统管理员系统的调整工作，很多时候需要把可调整的内容写入配置部分。
     * 
     * 目标类型保存多少个实例
     * 
     * 这些实例是否需要在计算量稀疏的时候自动回收
     * 
     * 每个实例最长的生命期是多少
     * 
     * 为了初次调用的响应时间，是否需要预先准备几个热的实例
     * 
     * 通过配置处理掉上面实例中那个很丑陋的switch
     * 
     */

    //*****************************************************************************************
    //13.4综合性的共享体系--object pool对象池

    //区别于单纯的FlayweightFactory对象池在享元的基础上提供对各类对象生命期的管理，提供通过AOP机制增加各种横切控制的办法


    /*
     * 应用运行过程中常常会根据调用要求，生成指定类型的对象实例，而对于复杂类型的频繁构造会消耗较多的系统资源，影响应用运行效率。
     * 
     * 另外，对于直接和用户交互的业务逻辑对象，由于客户端调用的随机性，为每个会话单独保存对象实例会增加应用逻辑层系统开销。
     * 
     * 针对上面的两种情况，可以设计一个专门管理对象实例的对象池，由它简洁控制对象市里的构造、激活、失效和释放过程
     * 
     * 对象池具有如下运行特性
     * 
     * 1，根据调用动态生成实例对象
     * 
     * 2，根据已经池化实例的使用情况，将空闲的实体提供给新的调用，减少重复创建过程。
     * 
     * 3，此外，随着.NET和JAVA等具有自动回收机制平台的出现，多数情况下无用实例的回收都由平台自动完成，但由于回收机制运行优先级一般很低，在密集业务调用过程中常常会因为无法获得执行机会导致资源快速膨胀，因此借助对象池的控制和实例重用可以有效的防止上诉情况发生。
     * 
     * 
     *                 Configuration
     *                 Management
     *                     |
     *                     |
     *                  Object----------------|
     *                  Builder               |
     *                     |                  |
     *                     |                  |
     * Lease ------------Object-------------Diapatch------------Listener---------Client
     * Management        Cache
     * 
     * 
     * 
     * 
     * Listener:用于和客户程序交互，一方面将客户程序需要使用的类型信息通知给Dispatch，另一方面把加工好的对象实例反馈回客户程序，实现了对象池对客户程序的透明。
     * 
     * Dispatch:主要工作室调度，跟据Listener发送的类型要求，在Object Cache中检查是否有已经构造王城，但没有激活的实例，如果有直接把实例反馈给Listener；否则同志Object Builder创建一个实例，在其注册到Bbject Cache之后，再反馈给Listener
     * 
     * Object Builder：跟据Dispatch输入的类型，参考其配置参数构造类型实例，它相当于一个支持各种享元帝乡的复合型FlyweightFactory。
     * 
     * Configuration Management:将外部配置信息翻译成配置对象，供Object Builder参考
     * 
     * Lease Management：更具池化类型的控制参数，参考Object Cache中缓冲实例的使用情况，回收和清理超期对象
     * 
     */


    //13.4.3抽象类型实体设计

    /*
     * 
     * PoolableBase             interface
     * #PreProcess()            IPoolableConfiguration
     *      |                   -Max:integer
     *      |                   Timeout:integer
     *      |
     *     \|/
     * interface
     * IPoolable
     * -Guid:String                     interface
     * -Type:Type                       IState
     * -CreateTime:DataTime             -Executable:Boolean
     * -AccessedTime:DataTime<>-------->-Uncoupled:Boolean
     * -Execurable:Boolean
     * -Uncoupled:Boolean               +Activate(item:IPoolable)
     *                                  +Deactivate(item:IPoolable)
     * +Activate()                      +Dispose(item:IPoolable)
     * +Deactivate()                                /|\
     * +ChangeState()                                |
     *                                               |
     *                                            StateBase   
     * 
     * 
     * 
     * 
     *      SSSSSSSS                EEEEEEE
     *         |                       |
     *         |                    Destory 
     *         |                      / \
     *    Constructed         Timeout/   \
     *         |                    /     \
     *         |invoke              \     /
     *         |      ____________   \   /Destory
     *         |     /after invoke\   \ /
     *      Activated               Deactivate 
     *               \_____________/
     *                   Re-invoke
     * 
     * 
     */



    //13.4.4享元模式的典型应用——缓冲  Page 266 303

    //对象池的主要性能优势有两个:
    //一个是可以通过重用已有的实例，减轻平凡构造的负载
    //二是通过共享实例，减少总体内存占用，因此缓冲机制是对象池的核心组成。
    //此外业务上经常存在峰值调用，峰值过后如果缓冲的对象过多或过长时间不用，也会额外占用很多系统资源，因此对象池需要一个缓冲对象的销毁机制

    public interface IState
    {
        bool Executable
        {
            get;
        }
        bool Uncoupled
        {
            get;
        }

        void Activate(IPoolable item);

        void Deactive(IPoolable item);
        void Dispose(IPoolable item);
    }


    public interface IPoolable
    {
        string Guid{get;}
        string Type{get;}
        
        DateTime CreateTime
        {
            get;
        }

        DateTime AccessTime
        {
            get;
        }
        bool Executable
        {
            get;
        }
        bool Uncoupled
        {
            get;
        }
        void Activate();
        void Deactivate();
        void ChangeState();
       
    }

    public interface IPoolableConfiguration
    {
        int Max
        {
            get;
        }
        int Timeout
        {
            get;
        }
    }

    public abstract class PoolableBase:IPoolable
    {

    }

    public class ObjectBuilder<T> where T:class,IPoolable,new()
    {
        public T BuildUp() 
        { 
            return new T(); 
        }
    }

    public class SizeRestrictedList<T>:List<T>
    {
       
    }


    //定义实际对象缓冲
    public class ObjectCache
    {
        //相关类型缓冲列表注册器
        private static IDictionary<Type, object> registry;

        //静态构造类型列表注册表

        static ObjectCache()
        {
            registry = new Dictionary<Type, object>();
        }

        //获取一个可用的缓冲对象
        public bool TryToAcquire<T>(out T item,out bool increasable)where T:PoolableBase,new()
        {
            PrepareTypeList<T>();
            return ((registry[typeof(T)]) as SizeRestrictedList<T>).Acquire(out item,out increasable);
        }
        //缓冲新类型实例
        public void Cache<T>(T item)where T:PoolableBase,new()
        {
            PrepareTypeList<T>();
            ((registry[typeof(T)]) as SizeRestrictedList<T>).Add(item);
        }

        //准备特定类型缓冲列表
        private void PrepareTypeList<T>()where T:PoolableBase,new()
        {
            if (!registry.ContainsKey(typeof(T)))
                registry.Add(typeof(T), new SizeRestrictedList<T>());
        }


    }



    //13.5用于并行处理的线程级享元 Page 270 307

    /*
     * 有关线程使用的享元对象有以下几种内存位置：
     * 
     * 线程内部申请，位于线程自己内存过程中生成的享元，如果是托管资源，那么这些资源一般在线程执行后别GC自动回收，故暂时不作考虑
     * 
     * 多个线程在宿主进程中申请的享元资源，但依据线程ID相互隔离使用，他们不会因线程执行完毕而被自动回收，而是随着线程生生灭灭，如果不对其进行处理，最后会形成很多持续的垃圾
     * 
     * 多个线程争用的宿主进程资源或宿主进程外资源，这部分资源在某个线程退出后也可以被其他线程使用，因此使用上除了考虑病啊处理外，与一般的享元关系不大。可暂不考虑
     */

    //13.6通过Delegate和队列实现异步Flyweight

    //关于资源共享，，可以采用等待队列和异步通知机制


    //小结

    /*
     * 享元模式住址在于通过共享节省总体资源占用，不过共享将导致资源访问上的一系列问题，一个有效的使用准则就是“尽量晚得地开始使用，尽量早地释放”。要求客户程序完成这些工作似乎不经合理，因此项目中往往需要在享元对象外增加必要的管理机制，比如本章介绍的对象池。
     */

}
