using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20700MementoPattern
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

    //第21章 备忘录模式

    //without violating encapsulation,capture and externalize an object's internal state so that object can be restroed to this state later.
    /*
     * 备忘录模式其实是给我们的应用一个Undo的机会，很多时候我们操作某些软件的时候，常常需要回退自己刚才执行的操作。
     * 
     * 
     * 要使用应用可以恢复到之前的某个状态，我们必须提前将该对象之前的某些状态保存起来：从面向对象的角度看，一般情况下状态是保存在对象的属性中的。但随着实际应用越来越复杂，很多表面的单个对象实际是一个具有复杂内部结构的对象系统，这时候如何恢复整个对象层次每个对象的属性就成了比较有挑战的事情。
     * 
     * 
     * 实现方式的选择有以下几种：
     * 1，客户程序记录目标对象某个时刻的状态，在需要恢复的时候修改目标对象。
     * 2，客户程序通过序列化或持久化等方式，把某个时刻的对象保存下来，在需要恢复的时候通过反操作把对象实例重建回来，同时修改自己对该对象的引用
     * 3，目标对象提供了获得当前状态的接口，客户程序找到一个专门的第三方对象来保存目标对象的目标状态，适当的时候再把状态恢复到某个环境。（主要采用这种）
     * 4，如果目标对象内部已经封装了类似的Undo机制，客户程序可以在不知道目标对象内部结构，不打破现有封装的前提下，通过简单调用目标对象的Undo（）方法实现。
     * 
     * 方法4的实现机制：
     * 1，内部类
     * 2，集合类型和泛型集合类型
     * 3，Context对象
     */


    //21.2 经典回顾
    /*
     * 备忘录模式的意图是在保证对象封装的前提下，获取对象内部的状态，并交给第三方对象保存起来，后续运行中根据需要把对象恢复带之前某个状态。
     * 
     * 什么时候需要用到备忘录模式？
     * 1，必须保存对象某个时刻的状态，已被后续恢复
     * 2，出于对象本身的封装性（结构，中间状态，属性私密性）方面的考虑，不方便直接把对象内部属性直接暴露给外部对象。
     * 
     * 3，对象本身位于一系列连续但往复状态之中，为了减少反复建立对象的过程，需要让对象本身可以在往复运动中，通过额外对象协助记录状态的方法，执行反复包装/拆包的工作
     * 
     * 
     *                                                                  
     *                                                                  
     *     Originator                           interface                                  
     *     -state            ------------------>IMenmoto---------------<>Caretaker              
     *      +Memento:IMemento                   +State                                   
     *              \                                                    
     *               \                                                   
     *                \                                                  
     *                 \                                                 
     *                  \                                                
     *    SetMemento(IMemento m):state=m.state                                      
     *    CreateMemento:return new Memento(state)                                             
     *                                                                  
     *   Originator：原发起，负责创建备忘录记录当前的内部状态，并可使用备忘录恢复内部状态。原发器可根据需要决定备忘录存储原发器的哪些内部状态，甚至可以把自己做一次Clone(),直接传递给备忘录。原发器是业务程序需要使用的业务对象。
     *  IMemento：备忘录，负责储存原发器对象的内部状态
     *  CareTake：负责保存好备忘录
     */
    //为了便于定义抽象状态类型所定义的接口
    public interface IState
    {

    }

    //抽象备忘录对象接口,保存具有状态的类型对象
    public interface IMemento<T>where T:IState
    {
        T State
        {
            get;
            set;
        }
    }
    public abstract class MementoBase<T>:IMemento<T>where T:IState
    {
        protected T state;

        public virtual T State
        {
            get { return state; }
            set { state = value; }
        }
    }


    //抽象原发器对象接口

    public interface IOriginator<T,M>where T:IState where M:IMemento<T>,new()
    {
        IMemento<T> Memento
        {
            get;
            set;
        }
    }

    public abstract class OriginatorBase<T,M>:IOriginator<T,M>where T:IState where M:IMemento<T>,new()
    {
        //原发器中对象的状态
        protected T state;

        //把状态保存到备忘录，或者从备忘录恢复之前的状态
        public virtual IMemento<T> Memento
        {
            get
            {
                M m = new M();
                m.State = this.state;
                return m;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                this.state = value.State;
            }
        }
    }




    //具体状态类型
    public struct Position:IState
    {
        public int X;
        public int Y;
    }

    //具体备忘录类型
    public class Memento:MementoBase<Position>
    { }

    //具体原发器类型
    public class Originator:OriginatorBase<Position,Memento>
    {
        //共客户程序使用的非备忘录相关操作
        public void UpdateX(int x)
        {
            base.state.X = x;
        }
        public void DecreaseX()
        {
            base.state.X--;
        }
        public void IncreaseY()
        {
            base.state.Y++;
        }
        public Position Current
        {
            get
            {
                return base.state;
            }
        }
    }

    public class CareTaker<T> where T:IState
    {
        private Stack<IMemento<T>> memento;

        public CareTaker()
        {
            memento = new Stack<IMemento<T>>();
        }
        public void Push(IMemento<T> m)
        {
            if(m==null)throw new ArgumentNullException();
            memento.Push(m);
        }
        public IMemento<T> Pop()
        {
            if (memento.Count <= 0)
                throw new ArgumentOutOfRangeException();
            return memento.Pop();
        }
    }


    public class Client
    {
        public void TestMethod()
        {

            Originator ori = new Originator();

            Console.WriteLine(ori.Current.X+"   "+ori.Current.Y);

            IMemento<Position> m1 = ori.Memento;

            ori.DecreaseX();
            ori.IncreaseY();

            Console.WriteLine(ori.Current.X + "   " + ori.Current.Y);

            Console.WriteLine(m1.State.X+"  "+m1.State.Y);

            IMemento<Position> m2 = ori.Memento;

            Console.WriteLine(m2.State.X + "  " + m2.State.Y);

            ori.Memento = m1;

            Console.WriteLine(ori.Current.X + "   " + ori.Current.Y);



            CareTaker<Position> care = new CareTaker<Position>();
            care.Push(ori.Memento);


            Console.ReadLine();
        }
    }



    //*******************************************
    //为了获得原发器对象更好的封装性，可以吧备忘录类型直接作为某个原发器基类所具有的内部类型来设计，这样有关保持状态的操作全部通过原发器与其内部类型操作即可，而原发器可以设计的只有Undo或SaveCheckPoint这样的接口方法，客户程序使用上相对得到简化。
    //改进的原发器类

    public abstract class OriginatorPromotedBase<T>where T:IState
    {
        //原发器中需要保存对象的状态
        protected T state;

        //把备忘录定义成原发器的内部类型
        protected class InternalMemento<T>:IMemento<T> where T:IState
        {
            private T state;

            public T State
            {
                get { return state; }
                set { state = value; }
            }
        }

        protected virtual IMemento<T> CreateMemeto()
        {
            IMemento<T> me = new InternalMemento<T>();
            me.State = this.state;
            return me;
        }

        //栈保存备忘录
        private Stack<IMemento<T>> mStack = new Stack<IMemento<T>>();

        //把状态保存到备忘录
        public virtual void SaveCheckPoint()
        {
            mStack.Push(CreateMemeto());
        }


        //从备忘录恢复之前状态
        public virtual void Undo()
        {
            if(mStack.Count>0)
            {
                state = mStack.Pop().State;
            }
            else
            {

            }
        }



    }

    //*******************
    /*
     * 与前面的经典实现而言，后面这种是C#的内部内实现，C++只能采用前面那种实现
     * C#实现的区别
     * 1，封装性更严密
     * 2，使用不同原发器类型嵌套使用，所有SaveCheckpoint()和Undo()方法均对外层原发器类型直至客户程序起封装作用
     * 3，需要原发器本身增加额外的接口，相对破坏原发器类型的单一职责特性
     */

    //虽然备忘录模式实现的目标相对简单，但实现上并不容易，而且随着.NET及相关WebService技术的更新，备忘录模式所要求的对于状态的访问本身被弱化。因为调用的是分布式的，所以越来越多的时候我们直接采用序列化实现深拷贝的方式，将业务对象某个时刻的状态保存到I/O，数据库等辞旧花机制上，而不是全部保存在内存里，因此实际项目中专门的备忘录模式采用并不广泛


    //**************************************
    //21.3把备忘压栈
    //前面已经实现

}
