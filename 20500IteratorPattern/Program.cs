using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20500IteratorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass tc = new TestClass();

            tc.Test();
        }
    }

    //第十九章 迭代器模式
    //provide a way to access the elements of an aggregate object squentially without exposing its underlying representation.

    //迭代器模式要求我们从使用者的角度考虑如何设计和对外访问内部对象接口。即便我们组织的对象系统内部结构很复杂，但对于客户程序而言最简单的方式莫过于通过for foreach循环依次遍历，至于遍历过程中的次序，分类筛选等，则有目标类型自己封装。

    /*
     * 迭代器的组织看,主要有三个问题:
     * 
     * 1,面向什么主题？
     * 2，依据哪种次序？
     * 3，根据上下文需要做哪种筛选？
     *
     * 我们可以视搜索引擎的结果为某个庞大系统提供的迭代结果，该结果如果进行并行的爬出来，并结合商业利益组织相关信息可能是我们应用迭代模式时需要考虑的问题。
     * 
     * 
     * 
     *                              /|\Subject
     *                               |
     *                               |
     *                 IEnumerator   |
     *            Target-------------O---------------------->Sort
     *                              /
     *                             /
     *                            /
     *                         filter 
     *                         
     * 
     *  什么时候会用到迭代器？
     *  1，对象内部结构比较复杂，不希望客户程序看到这些复杂性
     *  2，希望按照既定的方式，更具客户程序的调用要求，组织对象的输出次序
     *  3，不确定目标类型的规模，无法直接通过一个IList<T>或IDictionary<K,T>,T[]方式返回，比如：访问某个网络或某个活动目录域中的所有节点。
     *  
     * 
     */


    /*
     * 
     * interface                                           interface
     * IAggregate                                           IIterator
     * +CreateIterator():IIterator                          +Current:T      
     *      |                                              +IsDone:bool       
     *      |                                               +MoveFirst():void      
     *      |                                               +MoveNext():void
     *      |                                                  |   
     *      |                                                  |   
     *      |                                                  |   
     *      |                                                  |   
     *  ConcreteAggregate           —————————>ConcreteIterator                              
     *  +ConcreteIterator():Iterator <-----------------+MoveFirst():void                             
     *                                                  MoveNext():void         
     *                                                           
     * IAggregate：一般的局和对象，即对外提供一个迭代器，让客户程序访问自己内部结构的对象
     * 
     * Iterator:所有迭代器对象的抽象结构，具有First,Next,是否便利结束，以及获得当前结点的方法和属性
     */


    //抽象迭代器接口
    public interface IIterator<T>
    {
        T Next();
    }

    //抽象聚合对象接口
    public interface IAggregate<T>
    {
        int Capacity { get; }
        T[] Items { get; }
        void Add(T item);

        IIterator<T> CreateIterator();
    }



    //聚合对象
    public class Aggregate<T>:IAggregate<T>
    {
        public const int Max = 5;
        private T[] items = new T[Max];
        public T[] Items
        {
            get
            {
                return items;
            }
        }

        private int capacity = 0;

        public int Capacity
        {
            get { return capacity; }
        }

        public void Add(T item)
        {
            if (capacity == Max) throw new IndexOutOfRangeException();
            Items[capacity++] = item;
        }

        public IIterator<T> CreateIterator()
        {
            return new Iterator<T>(this);
        }
    }


    public class Iterator<T>: IIterator<T>
    {
        private IAggregate<T> aggregate;
        private int index;
        public Iterator(IAggregate<T> aggregate)
        {
            this.aggregate=aggregate;
            index=0;
        }
        public T Next()
        {
            if (index == aggregate.Capacity) throw new IndexOutOfRangeException();
            return aggregate.Items[index++];
        }
        
    }


    public class TestClass
    {
        public void Test()
        {
            IAggregate<string> target = new Aggregate<string>();

            target.Add("A");
            target.Add("B");
            target.Add("C");
            target.Add("D");

            IIterator<string> Iterator = target.CreateIterator();

            Console.WriteLine(Iterator.Next());

            Console.ReadLine();

        }

    }


    /*//实际项目中，绝大多数情况下，我们通过foreach逐个遍历内部对象，因此将使用下面四个.NET Framework提供的类型
     * 
     * IEnumerable
     * IEnumerator
     * 
     * IEumerable<T>
     * IEumerator<T>
     * 
     */

    //C#改进版的Iterator
    public interface IAggregateNet<T>
    {
        void Add(T item);
        IEnumerator<T> GetEnumerator();
    }

    public class AggregateNet<T>:IAggregateNet<T>
    {
        public const int Max = 5;
        private T[] items = new T[Max];
        public T[] Items
        {
            get
            {
                return items;
            }
        }

        private int capacity = 0;

        public int Capacity
        {
            get { return capacity; }
        }

        public void Add(T item)
        {
            if (capacity == Max) throw new IndexOutOfRangeException();
            Items[capacity++] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i=0;i<capacity;i++)
            {
                yield return items[i];
            }
        }
    }



}
