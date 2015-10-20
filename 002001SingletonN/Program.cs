using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00201SingletonN
{
    class Program
    {
        static void Main(string[] args)
        {



            Console.ReadKey();
        }

        //[TestMethod]
        public void Test()
        {
            SingletonN s1 = SingletonN.Instance;
            SingletonN s2 = SingletonN.Instance;
            SingletonN s3 = SingletonN.Instance;

            /*

            Assert.IsNull(s3);//超出容量，所以不能获得实例引用
            Assert.AreNotEqual<int>(s1.GetHashCode, s2.GetHashCode);//两个不同实例

            s1.DeActivate();//释放空间
            s3 = SingletonN.Instance;
            Assert.IsNotNull(s3);//有了空间，所以可以获得引用
            s2.DeActivate();
            Assert.IsNotNull(s3);//有了空间，所以可以获得引用

            //s3虽然获得了新的引用，但其其实之前创建的某个现成的
            Assert.IsTrue((s3.GetHashCode() == s1.GetHashCode()) || (s3.GetHashCode() == s2.GetHashCode()));
              
             */
            
        }



    }


    //4.7跨进程的Singleton

    //没看完 企业级大型系统用 不太明白  Page 150

    //4.8Singleton的扩展-----Singleton-N
    //从某种层度上来说，Singleton-N是ObjectPool（对象池）模式的一个预备模式，它除了完成创建型模式的一般new()之外，还要负责检查和数量控制，而且往往在一个多线程环境下进行，因此，在设计上要为它增加一些助手。

    //DoubleCheck方式暂时还有改造的空间。为了不打破Double Check框架，并且依据单一职责原则，我们要增加一个WorkItemCollection，并且满足以下条件：
    //1.本身是个集合类型
    //2.最多存放某种类型的N的实例
    //3.集合中的而每个实例都可以通过状态标识自己处于忙碌状态还是处于闲暇状态，同时可以根据外部反馈，修正自己的状态
    //4.可以告诉外界，是否还有空位，或则是否可以找出一个“热心”的闲着的对象。
    //5.至于New()么，WorkItemCollection还是别管了，否则容易让SingletonN类型的构造过程失控


    //4.8.1定义具有执行状态的抽象对象

    public enum Status
    {
        Busy,//被客户程序占用
        Free//闲置当中
    }
    interface IWorkItem
    {
        Status Status
        {
            get;
            set;
        }
        void DeActivate();//放弃使用
    }

    //SingletonM实例集合的示例
    class WorkItemCollection<T>where T:class,IWorkItem
    {
        //定义最多允许保存的实例数量N
        protected int max;
        protected IList<T> items = new List<T>();
        public WorkItemCollection(int max)
        {
            this.max=max;
        }

        //外部会的T类型实例的入口
        public virtual T GetWorkItem()
        {
            if((items==null)||(items.Count==0))
                return null;
            //如果可能的话,对外反馈一个现成实例
            foreach(T item in items)
                if(item.Status==Status.Free)
                {
                    item.Status=Status.Busy;
                    return item;
                }
            return null;//虽然有现成实例，但都处于忙碌状态，所以返回null

        }

        //新曾一个实例
        public virtual void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (!CouldAddNewInstance) throw new OverflowException();
            item.Status = Status.Free;
            items.Add(item);
        }

        //判断是否可以增加新实例
        public virtual bool CouldAddNewInstance
        {
            get
            {
                return (items.Count < max);
            }
        }

    }


    //4.8.3在基本Singleton模式实现的框架下引入实例合集

    public class SingletonN:IWorkItem
    {
        private const int MaxInstance = 2;//定义Singleton-N的这个N
        private Status status = Status.Free;//初始状态；
        public void DeActivate()
        {
            this.status = Status.Free;
        }
        public Status Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }
        private static WorkItemCollection<SingletonN> collection = new WorkItemCollection<SingletonN>(MaxInstance);
        private SingletonN() { }
        public static SingletonN Instance
        {
            get
            {
                //在实现基本框架不变的情况下，引入集合实现Singleton-N的锁哥实例对象管理
                SingletonN instance = collection.GetWorkItem();
                if (instance == null)
                    if (!collection.CouldAddNewInstance)
                        return null;
                    else
                    {
                        instance = new SingletonN();
                        collection.Add(instance);
                    }
                instance.Status = Status.Busy;//激活使用
                return instance;
            }
        }


    }

    //unit test//见program

    
    
    //从上面的实例不难看出来，SingleN相对传统Singleton模式而言，增建了多个实例的管理和执行调度，但与Object Pool不同的是它没有实现对象销毁机制，算是一个半成品。


    //4.9 引用配置文件管理Singleton
    //参数化Singleton部分本身也可能把很多初始化参数写在某个配直接或配置元素集合里
    //ConfigurationManager获取.NETFrameWork提供的“4件套”配置对象
    //ConfigurationSectionGroup,ConfigurationSection,ConfigurationElement,ConfigurationElemtnColletion



}
