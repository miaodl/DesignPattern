using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21000StragedyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第二十四章 策略模式
    //Defube a family of algorithms,encapsule each one,and make them interchangeable.Stragedy lets the algotithm vary independently from clients that use it.

    //在以往结构化软件时代，我们认为 软件=算法+数据结构，，面向对象时代  软件=对象+算法+数据结构

    //策略模式给我们一个新思路：算法+数据结构=封装的策略对象

    /*
     *  策略模式的几个特征描述：
     *  1，完成一件事情不止有一个可选择的方法，或者虽然现在是唯一的，但预期不远的将来会不止一个方法。
     *  2，每个方法视图解决的是同一件事情
     *  3，每个事情选择何种解决方法取决于当前的某些条件
     *  4，每个方法是独立于其他方法的
     */

    //24.2经典回顾
    /*
     * 策略模式应用情景：
     * 1，应用中包括了很多对象，他们之间虽然没有继承抽象关系的相关性，但在解决默写问题的时候却非常类似。比如：无论做前台展现的DataGridhi还是做后台多线程调度的管理类，他们都需要用到排序算法。
     * 
     * 2，根据运行环境的不同，需要因地制宜的算法。比如:同样是排序算法，考虑到CPU和内存容量的不同，在手机和在PC计算机上可能采取不同的排序算法
     * 
     * 3，封装复杂的数据结构，尤其是那些更多应用于局部的数据结构，让客户程序仅需按照其思路输入 数据，获得结果即可。比如：实现椭圆曲线算法加密，我们不需要知道它的面向门电路优化，需要什么样的结构体，只要把内存一段数据提交给它就可以做出一个没有秘钥几乎很难破解额结果。
     * 
     * 4，为了决定使用何种算法，应用主题可能需要很多的if else 或 switch，这时候不妨把没一个分支需要执行的内容抽象为独立的策略对象，然后由外部向客户程序注入一个算法，一面它犹豫不决。
     * 
     * 
     *    interface                               interface           
     *    IContext           <>-------------------IStrategy                     
     *  +SomeOperation():void                     +DoAlgorithm:void                       
     *                                                   /|\
     *                                                    |
     *                                                    |
     *                                            ConcreteStrategy
     *                                             
     */


    //抽象策略对象
    public interface IStrategy
    {
        //算法需要完成的功能
        int PickUp(int[] data);
    }
    
    //抽象Context对象
    public interface IContext
    {
        IStrategy Stragedy
        {
            get;
            set;
        }
        int GetData(int[] data);
    }


    //具体策略类型
    class DescentSortStrategy:IStrategy
    {
        public int PickUp(int[] data)
        {
            Array.Sort<int>(data);
            return data[data.Length - 1];
        }
    }

    class FirstDataStrategy:IStrategy
    {
        public int PickUp(int[] data)
        {
            return data[0];
        }
    }

    class AscentStrategy:IStrategy
    {
        public int PickUp(int[] data)
        {
            Array.Sort<int>(data);
            return data[0];
        }
    }

    //需要采用可代替策略的执行的对象
    public class Context:IContext
    {
        private IStrategy strategy;

        protected IStrategy Strategy
        {
            get { return strategy; }
            set { this.strategy = value; }
        }
        //执行对象依赖于策略对象的操作方法
        public int GetData(int[] data)
        {
            if (strategy == null)
                throw new NullReferenceException("strategy");
            if (data == null)
                throw new ArgumentNullException("data");
            return strategy.PickUp(data);
        }

    }

    public class Client
    {
        public void TestMethod()
        {
            int[] data = { 12, 42, 5, 17, 8 };
            IContext context = new Context();
            context.Stragedy = new DescentSortStrategy();

            Console.WriteLine(context.GetData(data));

            //切换算法策略
            context.Stragedy = new FirstDataStrategy();
            Console.WriteLine(context.GetData(data));

            //切换算法策略
            context.Stragedy = new AscentStrategy();
            Console.WriteLine(context.GetData(data));
        }
    }

    //***************************************
    //24.3Strategy与Interpreter协作
    /*
     * 上面的例子中虽然策略对象可以被替换，但它是从客户程序发起的，在一些规模相对比较大的应用情景中，我们需要增加一个因素使策略的选择更加智能化或自动化--提供每个策略的适用条件。
     * 
     * Page418 455
     * 
     * 
     * IContext---(operation)--->StrategyManager---(get rules)--->IStrategyRule---(return)--->StrageManager---(interpret)--->IInterpreter---(return)--->StrategyManager---(findMatchedstrategy)--->IStrategy---(return)--->StrategyManager---(SetStrategy)--->IContext---(execute strategy algorithm)--->IContext
     * 
     * 
     * 采用StrategyManager管理策略！！
     */


    //**************************************
    //24.4充分利用.NET Framework自带的Strategy接口  Page 421 458



    //24.5动态策略


    //*******************************
    //小结
    //从更高的层次看，策略木事进一步强调了“组合由于集成”的思路，之前我们已经在结构型模式上看到了一批如何尽量避免类型体系膨胀的模式，策略木事则从方法的角度出发，在保证类型外部框架不变的情况下，可以通过替换算法对象提高对对象应对各种变化的能力。


}
