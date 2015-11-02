using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20200TemplateMethodPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第十六章 模板方法模式

    //Define the skeleton of an algorithm in an operation,deferring some steps to subclasses.Template Method lets subclasses redefine certaim steps of an algorithm without changing the algorithms structure.

    //模板方法模式是面向对象系统中非常朴实的一种模式，体现出面向对象设计中及继承和多态的基本特征。由于设计的需要，我们往网会在初期规划一些较为粗颗粒的算法，而且对参与计算的对象进行抽象，明确算法会使使用到哪些方法，每个方法可能提供哪些支持，但此时每个方法本身并没有细化，随着开发过程的展开，我们可能会具体实现每个该方法，或则对最初的一些方法进行替换，覆盖上新的内容，这样就在一个相对固定的算法框架下，通过子类的变化或其他方法的变化，实现了算法的差异性。


    /*
     * 什么时候用到模板方法模式？
     * 
     * 1，我们实现一个算法，但发现其中有些部分非常易变，或则很容易随着运行环境，后续开发的不同产生很多变化，所以把他们抽象出来，供子类完成。
     * 
     * 2，随着项目的展开，我们发现之前有些工作可以被独立出来形成公共的开发库
     * 3，对一系列子类进行约束，要求他们必须实现算法要求的某些方法，便与其他客户程序按照这些方法操作子类。
     * 
     * 通过委托，泛型，配直节或配置元素等手段
     */

    //定义抽象类型及抽象算法
    public interface IAbstract
    {
        int Quantity { get; }
        double Total { get; }
        double Average { get; }
    }

    //定义了算法梗概的抽象类型
    public abstract class AbstractBase:IAbstract
    {

        public abstract int Quantity
        {
            get;
        }

        public abstract double Total
        {
            get;
        }

        public abstract double Average
        {
            get
            {
                return Total / Quantity;
            }
        }
    }

    //具体类型
    public class ArrayData:AbstractBase
    {

    }


    //16.3满足多套模板要求Page 309 346

    //定义多个Template约束
    public interface ITransform
    {
        string Transform(string data);
        bool Parse(string data);
        string Replace(string data);
    }
    public interface ISetter
    {
        string Append(string data);
        bool CheckHeader(string data);
        bool CheckTailer(string data);
    }

    //C#定义初步每个Template的抽象类型

    public abstract class TranformBase:ITransform
    {
        //抽象算法
        public virtual string Transform(string data)
        {
            if (Parse(data))
                data = Replace(data);
            return data;
        }

        public abstract bool Parse(string data);
        public abstract string Replace(string data);
    }

    public abstract class SetterBase:ISetter
    {
        //抽象算法
        public virtual string Append(string data)
        {
            if (!CheckHeader(data)) data = "HH" + data;
            if (!CheckTailer(data)) data = data + "TT";
            return data;
        }

        public abstract bool CheckHeader(string data);

        public abstract bool CheckTailer(string data);
    }


    //C#实现了多个Template的具体类型,同时实现两个接口，通过内部类
    public class DataBroker:ITransform,ISetter
    {
        //对于多个比较复杂的Template类型，可以通过内部类
        //或则其他外部类来减轻主类型的实现负担
        class InternalTranform:TranformBase
        {

        }
        class InternalSetter:SetterBase
        {

        }

        private ITransform transform = new InternalTranform();
        public string Transform(string data)
        {
            return transform.Transform(data);
        }
        public bool Parse(string data)
        {
            return transform.Parse(data);
        }

        public string Replace(string data)
        {
            return transform.Replace(data);
        }


        private ISetter setter = new InternalSetter();

        public string Append(string data)
        {
            return setter.Append(data);
        }
        public bool CheckHeader(string data)
        {
            return setter.CheckHeader(data);
        }
        public bool CheckTailer(string data)
        {
            return setter.CheckTailer(data);
        }
    }

    //具体类型同时满足ITransform和ISetter的末班要求，可以采用两次继承的办法，但这样一方面会令系统类型较快的膨胀，另一方面也会形成类型关系上的复杂性。这里按照组合原则，让具体类型同时实现两个接口，从客户程序角度看，依然可以用不同的模板视角来看到最终的具体类型。






    //16.4方法的模板---Delegate  委托事件。。

    public class CounterEventArgs:EventArgs
    {
        private int value;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public CounterEventArgs(int value)
        {
            this.value = value;
        }
       
    }

    public class Counter
    {
        private int value = 0;
        public event EventHandler<CounterEventArgs> Changed;

        public void Add()
        {
            value++;
            Changed(this, new CounterEventArgs(value));
        }
    }

    public class TestClass
    {
        public void Test()
        {
            Counter counter = new Counter();
            counter.Changed += delegate(object sender, CounterEventArgs args) { };
            counter.Changed += (i, y) => { };
            
        }

    }


    //16.5类型的模板-Generic

    //C#模板类

    //16.6用配置勾勒模板
    //社区提供给各种开发框架，配置本身成了框架自身执行算法的骨架，很多时候一个应用是否可以在框架中运行，需要做的主要工作就是完成配置文件。配置文件对目标类型的加载一般是依据控制反转原则IOC完成的，虽然算法会更具需要以接口或抽象类的方式从配置加载目标类型，但具体目标类型是什么，他们如何按照算法要求完成计算则都是运行态决定的。
    //相对于经典的模板方法模式，把模板的要求置于配置本身更加开放，除了可以保证算法在开发过程中顺利执行，还可以在系统上线后继续维护并动态更新，所以从工程的角度看，模板那配置化是一个普遍的趋势。



}


