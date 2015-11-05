using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20600MediatorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第20章
    //Define an object that encapsualtes how a set of object interact.Mediator promotes loose coupling by keeping objects from referring to reach other explicitily,and it lets you vary their interation independently.

    //中介者模式是在依赖倒置原则基础上进一步扩展出来的，它主要解决M:N依赖关系 PAGE357 394

    //把调用抽象出来，并为它们增加一个中介角色，由中介角色管理调用关系的路由及转发控制，这样抽象对象间M:N的关系就特化为M：1


    /*
     *  中介者解决的问题：
     *  1，解决不同类型系统间的M:N依赖关系
     *  2，某个类型需要依赖很多其他类型，而且依赖关系比较复杂，影响到该类型本身的重用性，比如，其他类型不愿意因为采用其他一两个方法，就因此惹上一系列其他类型。
     *  3，希望在一个较为分布的环境中，依据标准同时与未知数量的多个系统，平台，网络进行交互，面对提供服务的对象本上，它不希望关心到底有多少服务的消费者或服务协作者
     * 
     * 
     *                                                       
     *   interface   IMediator                                    interface       
     *      +Interact()//交互方法                                IColleague        
     * +introduce(IColleague,IList<IColleague>):void<-------- +Mediator:IMediator         
     * +introduce(IColleague,IColleague):void                /            \                        
     *            /|\                                       /              \
     *             |                                       /                \
     *             |                                      /                  \
     *             |                                     /                    \
     *             |                                    /                      \
     *             |                                   /                        \
     *     ConcreteMediator------------------>ConcreateColleagueA           ConcreteColleagueB 
     *             |                                                             /|\
     *             |                                                              |
     *             |--------------------------------------------------------------|
     *             
     * 经典模式中只提及了IMediator对IColleague的通知能力，并没有明确其方法，所以这里命名了两个重载方法Introduce(),用于建立某个IColleague与其他某个或一组IColleague的同时关系。在每个IColleague中增加一个对IMediator市里的引用，借助这个引用把通知发送给具体的中介对象，并经由它传达给同时对象
     * 
     */


    public interface IMediator<T>
    {
        //提供给IColleague的触发方法
        void Change();//定义各个provier和consumer之间的交互方法，通知或广播协议

        void Introduce(IColleague<T> provider, IList<IColleague<T>> consumers);
        void Introduce(IColleague<T> provider, IColleague<T> consumer);
        void Introduce(IColleague<T> provider, params IColleague<T>[] consumers);
    }

    //协同对象接口
    public interface IColleague<T>
    {
        T Data{get;set;}
        IMediator<T> Mediator{get;set;}
    }

    public abstract class ColleagueBase<T>:IColleague<T>
    {
        protected T data;

        public virtual T Data
        {
            get { return data; }
            set { data = value; }
        }
        protected IMediator<T> meidator;

        public virtual IMediator<T> Meidator
        {
            get { return meidator; }
            set { meidator = value; }
        }
    }

    //具体中介者类型
    public class Mediator<T>:IMediator<T>
    {
        //关于消费者的数量都可以非常巨大，在具体的交互方法 如change中进行，使用多线程的方式可提高广播或交互速度
        //是否可以采用中介模式提供一个游戏的交互？？？

        //保存主从列表
        protected IColleague<T> provider = null;
        protected IList<IColleague<T>> consumers = null;

        public virtual void Change()
        {
            if ((provider != null) && (consumers != null))
                foreach (IColleague<T> colleague in consumers)
                    colleague.Data = provider.Data;
        }

        public virtual void Introduce(IColleague<T> provider, IList<IColleague<T>> consumers)
        {
            this.provider = provider;
            this.consumers = consumers;
        }

        public virtual void Introduce(IColleague<T> provider, IColleague<T> consumer)
        {
            IList<IColleague<T>> consumers = new List<IColleague<T>>();
            this.provider = provider;
            this.consumers = consumers;
        }

        public virtual void Introduce(IColleague<T> provider, params IColleague<T>[] consumers)
        {
            if(consumers.Length>0)
            {
                IList<IColleague<T>> array = new List<IColleague<T>>(consumers);
                this.consumers = array;
            }
            this.provider = provider;
        }
        
    }

    //测试用IColleague

    //provider
    class A:ColleagueBase<int>
    {
        public override int Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                base.Data = value;
                meidator.Change();
                //在对a的实例赋值的时候，便调用中介者定义的交互方法，但实际上这样并不好
                //在中介者定
            }
        }
    }

    //consumer
    class B:ColleagueBase<int>
    {

    }
    class C:ColleagueBase<int>
    {

    }


    public class TestClass
    {
        public void Test()
        {
            Mediator<int> mA2BC = new Mediator<int>();
            A a = new A();
            B b = new B();
            C c = new C();


            a.Meidator = mA2BC;
            b.Meidator = mA2BC;
            c.Meidator = mA2BC;
            mA2BC.Introduce(a, b, c);//introduce方法相当于直接定义了主从协议，可以cache储存多个Mediator定义多个provider或consumer之间的交互协议，每次需要哪个协议就调用哪个，不必重复的创建新的主从交互协议。
            a.Data = 20;
            //a.Meidator.Change();实际上等于mA2BC.Change();
        }



        //其实中介者模式本身可以作为一个中继器，结合之前在代理模式部分介绍的远程代理（WCF,REMOTING,Socket，COM+）,可以很容易地把调用迅速广播到更大的环境中，无论借鉴之前的职责链模式思想，还是结点命令模式的复合命令方式。

        //Page 362 399
    }


}
