using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _00200SingletonClassic
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //Singleton单例模式
    //Ensure a class only has one instance,and provide a global point of access to.

    //实现单件的方式有很多，但大体上有两种。
    //外部方式：客户程序在使用某些全局性的对象时，做些Try Create Store Use的工作，如果没有，就自己创建一个，但是任然把他搁在全局的位置上，如果原本有，就直接哪一个现成的

    //C# 客户端程序保证的Singleton方式
    class Target
    {

    }

    class Client
    {
        private static IList<Target> list = new List<Target>();
        private Target target;

        public void SomeMethod()
        {
            if (list.Count == 0)
            {
                target = new Target();
                list.Add(target);
            }
            else
                target = list[0];
        }

    }

    //内部方式：类型自己控制生成市里的数量，无论客户程序是否Try过了，类型自己就是一个实例，客户程序使用的都是这个线程的唯一实例。

    //除了经典Singleton，还将更具实际技术趋势介绍如何根据不同类型生产环境实现某种语义范围的Singleton.

    //实际上，Singleton模式要做的就是通过控制类型实例的创建，确保后续使用的是之前创建好的一个实例，通过这样的一个封装，客户程序就无须知道该类型实现的内部细节。

    //C++   Design Patterns:Elements of Reusable Object-Oriented Software
    ////Declaration
    class Singleton
    {
        public: 
            static Singleton* Instance();
        protected:
            Singleton();
        private:
            static Singleton* _instance;
    }
    //Implementation
    Singleton* Singleton::_instance=0;
    Singleton* Singleton::Instance()
    {
        if(_instance==0)
        {
            _instance=new Singleton;
        }
        return _instance;
    }

    //C# 把静态方法Instance()转换为静态属性后C#描述,非线程安全

    public class Singleton
    {
        private static Singleton instance;//唯一实例
        protected Singleton()//封闭客户程序的直接实例化
        {

        }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new Singleton();
                return Instance;
            }
        }
    }

    //1.这个Singleton类仅有一个公共类属性-Instance，区别于普通类，Singleton类仅有一个Protected的构造函数，客户程序不可以直接实例化Singleton，即类属性Instance就是客户端程序访问“private static Singleton instance”的唯一入口。
    //2.其中的那个if完成的控制部分则是控制实例数量额部分，只要instance被实例化就不会生成新的实例，直接把一个既有的实例反馈给客户程序。

    //3.上面代码满足最初的Singleton模式的设计要求，在大多是情况下代码也可以很好的工作。但是在多线程环境下，这种方式存在很多潜在的缺陷。一个最直接的问题在于if部分，当多个线程几乎同时调用Singleton类的Instance静态属性的时候，instance成员可能还没有被实例化，因此他被创建了多次，而且最终Singleton类中保存的是最后创建的那个实例，各个线程引用的对象不同，这违背了我们"唯一实例"的初衷

    //综合执行效率和线程同步后考虑
    //C#增加Double Check后的Singleton

    public class Singleton
    {
        protected Singleton()
        {
 
        }

        /// <summary>
        /// volatile多用于多线程的环境，当一个变量定义为volatile时，读取这个变量的值时候每次都是从momery里面读取而不是从cache读。这样做是为了保证读取该变量的信息都是最新的，而无论其他线程如何更新这个变量。
        /// </summary>
        private static volatile Singleton instance=null;//lazy方式创建唯一实例的过程

        public static Singleton Instance()
        {
            if (instance == null)//外层if
                lock (typeof(Singleton))//多线程中共享同步资源
                    if (instance == null)//内层if
                        instance = new Singleton();
            return instance;
        }
    }

    //1.虽然是多线程环境，但如果没有外城那个if,客户端每次执行的时候都需要被lock住Singleton类型，但在实际绝大多数情况下，运行起来的时候这个instance并不为空，每次都锁定Singleton类型，效率太差，这个lock很可能就成为整个引用的瓶颈
    //2.lock加外层的if部分等于组成了一个相对线程安全的实例构造小环境
    //3.一旦唯一的实例在那个线程安全的小环境被创建之后，后续新发起的调用都无须经过那个lock部分，直接在外层if判断之后就可以获得既有的唯一实例引用

    //4.volatile关键字也是这个要点，他表示字段可能被多个并发执行线程修改。声明为volatile的字段不受编译器优化(一般情况下默认的编译器优化假定由单个线程访问)的限制，这样可以确保该字段在任何时间呈现的都是最新的值，也就是在被lock之后，如果还没有完成new Singleton(),新计入的线程看到的instance都是null。






    //*************************************************
    //4.2线程安全的Singleton   Page 139  102

    ////线程安全
    class Singleton
    {
        private Singleton() { }
        public static readonly Singleton Instance = new Singleton();
    }

    //在多线程环境下，C#实现Singleton一个非常棒的实现方式
    //1.他省去了上面示例中那个laze构造过程，由于Instance是类的公共静态成员，因此相当于它会在类的第一次被用到的时候被构造，同样的原因也就可以省去把它放在静态构造函数构造额过程
    //2.这里示例构造函数被彻底的sing一为私有的，所有客户端程序和子类无法额外构造新的实例，所有的访问通过公共静态成员Instance获得唯一实例的引用。


    //4.3细节决定成败
    //下面最常见的两个导致“Singleton”变质的情景
    //1.不要实现ICloneable接口或继承自其相关的子类，否则客户端程序可以跳过已经隐藏起来得类构造函数。
    //C#会导致变质的情景
    public class BaseEntity:System.ICloneable
    {
        public object Clone()//对当前实例进行克隆
        {
            return this.MemberwiseClone();//例如采用这种方式克隆
        }
    }

    public class Singleton:BaseEntity
    {

    }

    //2.严防序列化。对于远程访问，往往需要把复杂的对象序列化后进行传递，但是序列化本身会导致Singleton的特性被破坏，因为序列化事实上完成了Singleton对象的拷贝。所以不能对期望有Singleton特性的类型声明SerializableAttribute属性。
    //C#导致变质额情景
    [Serializable]
    public class Singleton
    {
        //把Singleton实例通过二进制串行化为字符串
        public static string SerializeToString(Singleton graph)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, graph);
            Byte[] arrGraph = memoryStream.ToArray();
            return Convert.ToBase64String(arrGraph);
        }
        //通过二进制反攒新华从字符串回复出Singleton实例
        public static Singleton DeserializeFromString(string serializedGraph)
        {
            Byte[] arrGraph = Convert.FromBase64String(serializedGraph);
            MemoryStream memoryStream = new MemoryStream(arrGraph);
            return (Singleton)formatter.Deseralize(memoryStream);
        }

    }



    //4.4细颗粒度Singleton
    //4.2讨论的是线程安全的Singleton实现，但项目中我们往往需要更粗或者更细颗粒等的Singleton，比如某个线程是长时间运行的后台任务，它本身存在很多模块和中间处理，但是每个线程都希望有自己的线程内单独Singleton对象，其他线程也独立操纵自己线程内的Singleton，这样县城及Singleton的实例总数=1(每个线程内部唯一的一个)*N（线程总数）=N
    //.NET程序可以通过把静态成员标示为System.ThreadStaticAttribute,以确保它只是静态字段的值对每个线程都是唯一的，但这对于Windows Form程序很有效，对于Web Form和ASP.NET Web Service等Web类应用则不适用，因为他们是在同一个IIS线程下分割的执行区域，客户端调用时传递的对象是在HttpContext中共享的，也就是说，它本身不可简单的通过System.ThreadStaticAttribute实现。
    //[ThreadStatic]
    //public static readonly Singleton Instance = new Singleton();
    //按照.NET的设计要求，不要为标记该属性的字段指定初始值，因为这样的舒适化只会发生一次，因此在类构造函数执行时只会影响一个线程。在不指定初始值的情况下，如果它是值类型，可依赖初始化为其初始值的字段，如果它是引用类型，则可依赖初始化为null。也就是说，在多线程情况下，出来第一个是例外，其他线程虽然也期望通过这个方式获得唯一实例，但其实获得的是一个null，不能用。
    
    
    
    //4.4.2桌面引用中的细粒度Singleton问题
    public class Singleton
    {
        [ThreadStatic]//说明每个Instance仅在当前线程内静态
        private static Singleton instance;
        private Singleton()//封闭客户程序的直接实例化
        {

        }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new Singleton();
                return Instance;
            }
        }
    }


    //Unit Test
    //每次线程需要执行的目标对象定义
    //同时在它内部完成线程内部是否Singleton的情况

    class Work
    {
        public static IList<int> Log = new List<int>();
        ///每个线程的执行部分定义
        public void Procedure()
        {
            Singleton s1=Singleton.Instance;
            Singleton s2=Singleton.Instance;
            //证明可以正常构造实例
            Assert.IsNotNull(s1);
            Assert.IsNotNull(s2);
            //验证当期那线程执行内部两次引用的是否为同一个实例
            Assert.AreEqual<int>(s1.GetHashCode(),s2.GetHashCode());
            //登记当前线程所使用的Singleton对象标示
            Log.Add(s1.GetHashCode());
        }
    }

    [TestClass]
    public class TestSingleton
    {
        private const int ThreadCount=3;
        [TestMethod]
        public void Test()
        {
            //创建一定数量的线程执行体
            Thread[] threads=new Thread[ThreadCount];
            for(int i=0;i<ThreadCount;i++)
            {
                ThreadStart work=new ThreadStart((new Work()).Procedure);
                threads[i]=new Thread(work);
            }
            //执行线程
            foreach(Thread thread in threads) thread.Start();
            //终止线程并作其他清理工作
            //判断是否不同线程内部的Singleton实例是不同的
            for(int i=0;i<ThreadCount-1;i++)
                for(int j=i+1lj<ThreadCount;j++)
                    Assert.AreNotEqual<int>(Work.Log[i],Work.Log[j]);    
        }
    }


    //4.4.3解决Web应用中的细颗粒度Singleton问题
    //Page 146



    //4.4.4更通用的细颗粒度Singleton
    //采用WinForm和Web二选一的方式


    //4.5自动更新的Singleton

    //4.6参数化Singleton
    //C#硬编码方式实现僵化的参数化Singleton
    public class Singleton
    {
        ///参数化构造函数
        private string message;
        private Singleton(string message) { this.message = message; }
        public string Message { get { return this.message; } }

        //硬编码方式实现参数化构造函数
        private static Singleton instance;
        public static Singleton Instance
        {
            get
            {
                if(instance==null)
                lock (typeof(Singleton))
                    if (instance == null)
                        if ((DateTime.Now.DayOfWeek == DayOfWeek.Sunday) || (DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
                            instance = new Singleton("weekend");
                        else
                            instance = new Singleton("work day");
                return instance;
            }
        }
    }

    //直接硬编码太僵化，很多时候考虑“依赖注入”模式，把需要的参数注入到唯一的实例中

    //1,构造函数方式：这种方式在Singleton行不通，因为不允许客户端程序控制Singleton类型的实例化过程
    //2，Setter方式：通过暴露出公共实例属性，客户程序就可以修改唯一实例的内容，实现参数化不成问题，但同时这种方式泰国自由了，无论哪里的客户程序只要可以看到相关属性救恩能够修改，这很容易失控。
    //3.接口方式：侵入性太强，而且接口方法的而实现全部都在类内部，本身就是硬编码过程，解决不了问题。
    //4.Attribute方式：代价太大，备用

    //一个不错的选择是借助访问配置系统（配置文件，ini文件或数据库等）。可以把message的内容配置在App.Config文件来解决

    //***************App.Config********************
    <configuration>
        <appSettings>
            <add key="parametersizedSingletonMessage" value="Hello world">
        </appSettings>
    </configuration>

    //*********************C#***********************
    public class Singleton
    {
        //其他处理

        //通过访问配置实现参数化Singleton实例构造
        private static Singleton instance;
        public static Singleton Instance
        {
            get
            {
                if(instance==null)
                    lock(typeof(Singleton))
                        if(instance==null)
                        {
                            //读取配置并进行实例化
                            string key-"parameterizedSingletonMessage";
                            string message=ConfigurationManager.AppSettins[key];
                            instance=new Singleton(message);
                        }
                return instance;
            }
        }
    }





}