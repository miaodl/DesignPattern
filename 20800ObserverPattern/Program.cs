using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20800ObserverPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.TestMethod();

            Console.ReadLine();
        }
    }

    //第二十二章 观察者模式
    //Define a one-to-many dependency between objects so that when one object changes state,all ites dependents are notified and updated automatically

    //观察者模式中主要用于1：N的通知发布机制，他希望解决一个对象状态变化时可以及时告知相关以来对象的问题，令他们也可以做出响应。

    //22.2经典回顾

    /*
     *       interface                              interface         
     *       ISubject                               IObserver       
     *  +Attach(Observer):void -------------------->update():void            
     *  +Detach(Observer):void                           /|\               
     *  +Notify():void                                    |       
     *          /|\                                       |
     *           |                                        |
     *           |                                        |
     *           |              subject                   |
     *   ConcreteSubject <--------------------------ConcreteObserver                  
     *       +State                                 Upadate():void
     * 
     * 
     * 
     * 该模式不仅对目标对象进行了抽象，同时对它保持关注的对象也进行了抽象，通过在目标对象维护一个观察者对象列表，当状态变更的时候进行逐个通知。他的主要应用情景如下：
     * 
     * 1，当存在一类对象通知关系依赖于另一类对象的时候，把他们进行抽象，确保两类兑现具体实现都可以相对独立变化，但他们交互的接口保持稳定。
     * 2，一个类型变化时，需要通知位置数量的其他对象
     * 3，最重要的是提供了目标对象与希望通知获得的对象间松散耦合。
     * 
     */
    //观察者类型接口
    public interface IObserver<T>
    {
        void Update(SubjectBase<T> subject);
    }

    public interface ISubject<T>
    {

        void Attach(IObserver<T> observer);

        void Detach(IObserver<T> observer);

        void Notify();
    }


    //目标对象抽象类型
    public abstract class SubjectBase<T>:ISubject<T>
    {
        //登记所有需要通知的观察者
        protected IList<IObserver<T>> observers = new List<IObserver<T>>();

        protected T state;
        public virtual T State
        {
            get
            {
                return state;
            }
        }

        public static SubjectBase<T> operator +(SubjectBase<T> subject,IObserver<T> observer)
        {
            subject.observers.Add(observer);
            return subject;
        }

        public static SubjectBase<T> operator -(SubjectBase<T> subject, IObserver<T> observer)
        {
            subject.observers.Remove(observer);
            return subject;
        }
        public void Attach(IObserver<T> observer)
        {
            this.observers.Add(observer);
        }
        public void Detach(IObserver<T> observer)
        {
            this.observers.Remove(observer);
        }



        //更新各观察者
        public virtual void Notify()
        {
            foreach(IObserver<T> observer in observers)
            {
                observer.Update(this);
            }
        }


        //共客户程序对目标对象进行操作的方法
        public virtual void Update(T state)
        {
            this.state = state;
            Notify();
        }

    }

    //具体目标类型部分
    public class Subject<T> : SubjectBase<T> { }

    //具体观察者类型
    public class Observer<T>:IObserver<T>
    {
        public T state;
        public void Update(SubjectBase<T> subject)
        {
            this.state = subject.State;
        }
    }


    public class Client
    {
        public void TestMethod()
        {
            Subject<int> subject = new Subject<int>();
            Observer<int> observer1 = new Observer<int>();
            observer1.state = 1;
            Observer<int> observer2 = new Observer<int>();
            observer2.state = 2;

            subject.Attach(observer1);
            subject.Attach(observer2);

            subject.Update(55);

            Console.WriteLine(observer1.state);
            Console.WriteLine(observer2.state);
            
        }
    }



    //*******************************************
    /*
     * 讨论观察者如何更新自己的数据，当目标对象状态更新的时候，观察者可以通过以下两种方式更新信息：
     * 1，目标对象通知里把需要更新的信息作为参数提供给IOberser的Update（）方法
     * 2，目标对象仅仅告诉观察则有新的状态，至于该状态是什么，观察者自己访问目标对象来获取
     * 
     * 前者我们称之为 推 方式，更新的数据是目标对新硬塞给观察者的，后者被称为 拉 模式，是观察者主动从目标对象拽下来的，从面向对象来看，他们的优劣如下：
     * 
     * 1，推 方式每次都会吧通知以广播的方式发送给所有观察者，所有观察者只能被动接受，如果通知的内容比较多时，多个观察者同时接受可能会对网络和内存有比较大的影响
     * 
     * 2，拉 方式更加自由，他只要知道有情况就可以了，至于什么时候获取，获取那些内容甚至是否要获取都可以自主决定，但这也带来两个问题，如果某个观察者不紧不慢，它可能会漏掉之前通知的内容，另外，他必须保存一个对目标对象的应用，而且需要了解目标对象的结构，即产生了一定依赖
     * 
     * 项目中将两种方式区分开来，主要原因来自安全性要求，原则上我们都希望高信任区域可以读写低信任区域，而低信任区域不能写高信任区域，很多时候连读都不允许，这个时候 推 模式比较适合高信任区域不信任低信任区域的写方式，而 拉 一般要求高信任区域信任某个低信任区域的访问，或者就是高信任区域访问低信任区域。
     */


    //22.3.NET内置的Observer机制---事件


    //*****************************
    //22.4 具有Observer的集合类型

    //工程中我们使用集合类型来缓存一系列信息，这些信息往往会被多个模块共享，如果缓存信息发生变更，有可能需要通知到各方，我们可以采用为集合类型增加事件的方式使他们具有Observer机制。

    //22.5面向服务接口的Observer Page 394,431  没看

    //*****************************
    /*
     * 小结
     * 
     * 观察者模式也会产生一些不太好的后果，例如：
     * 1，降低了性能：不使用该模式的时候对象间是直接引用的调用过程，而且引用关系咋在编译阶段就决定了
     * 
     * 2，内存泄漏：即便每个主体对象上的所有观察者都已经失效了，但他们没有调用Detach方法，因此无论是观察者本身还是主体对象间都可能因为相互的引用关系无法被GC回收。
     * 
     * 3，测试和调试更加困难：采用观察者模式会令调用关系的以来不如之前的直接调用明显，因此猜测是或调试的时候我们需要时刻注意每一个Attach和Detach过程，而且还要注意Notify当前遍历的位置，否则难于跟踪并找到当前实际执行的类型。
     * 
     * 通常情况下，可以采用.NET的事件完成观察者模式
     * 
     */
}
