using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20601MediatorPatternDelegateEventBasedLightCoupling
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //20.3基于Delegate和事件的松耦合Mediator

    //经典中介者模式的一个弊端--必须引入一个IMediator的接口，这个问题其实与命令模式遇到的情况一样。
    //在.NET平台上，可以考虑使用delegate或事件作为更通用的中介对象

    /*
     * 1,IMediator有事件委托类型扮演，它通过.NETCLR的事件通知机制来实现
     * 2，通知的消息可以用Sender对象保存，也可以被保存在用户订制的EventArgs类型中，
     * 3，实际发出通知的对象就是抛出事件的那个对象sender=this
     * 4，上面示例的那个协作关系的Introduce方法由事件委托重载的+= 和-=运算符完成
     * 5，由于事件和委托机制是.NET语言的特性，所以一般应用的时候并不需要独立的IColleague接口。
     * 
     */

    public class DataEventArgs<T>:EventArgs
    {
        public T Data;
        public DataEventArgs(T data)
        {
            this.Data = data;
        }
    }

    public abstract class ColleagueBase<T>
    {
        private T data;

        protected T Data
        {
            get { return data; }
            set { data = value; }
        }
        public virtual void OnChanged(object sender,DataEventArgs<T> args)
        {
            Data = args.Data;
        }
    }

    public class A:ColleagueBase<int>
    {
        public event EventHandler<DataEventArgs<int>> Changed;
        public override int Data
        {
            get { return base.Data; }
            set
            {
                base.Data = value;
                Changed(this,new DataEventArgs<int>(value));
            }
        }
    }
    public class B:ColleagueBase<int>
    {

    }
    public class C:ColleagueBase<int>
    {

    }

    //20.4基于配置动态协调通知关系
    /*
     * 比如数据分发过程：
     * 
     * 1，数据库编程数据库连接串
     * 2，Blog编程URL表示
     * 3，FTP变成它的文件路径
     * 
     */
    //20.5 SOAP Mediator page367 404
}
