using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _00500PrototypePattern
{
    class Program
    {
        static void Main(string[] args)
        {

            TestClass tc = new TestClass();

            tc.TestMethod1();
            tc.TestMethod2();

            Console.ReadLine();
        }
    }




    //第七章 原型模式

    //page 164 200

    //Specify the kinds of ojects to create using a prototypical instance,and create new objects by copying this prototype

    //与之前借助第三方对象从无到有构造出对象实例不同，原型模式是一个从既有的对象克隆出一个新的实例。


    //原型模式时NET framework内置的一个模式，它提供了System.ICloneable接口
    public interface IPrototype
    {
        IPrototype Clone();
        string Name { get; set; }//为了演示，增加额外属性
    }

    public class ConcretePrototype:IPrototype
    {
        private string name;

        public IPrototype Clone()
        {
            return (IPrototype)this.MemberwiseClone();
        }


        
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }

    //MemberwiseClone方法创建一个浅表副本，该方法创建一个新的对象，然后将当前对象的非静态字段复制到该对象。如果字段是值类型的，则对该字段执行逐位复制，如果字段是引用类型的，则复制引用但不复制引用的对象（也就是一个指针）。因此样本对象及其副本引用同一对象。


    //7.2表面模仿还是深入模仿     深拷贝 浅拷贝

    //一个对象内部的某个成员本身也是另一个应用对象情况，而且可能会很多层嵌套下去。。为了腾出一个扩展的“道”，NET定义了一个名为System.ICloneable的接口，有一个Clone()方法，等于告诉开发人员“按照需要自己定义好了，至于复制的深度自己拿捏”。实际项目中，虽然图省事可以直接使用自带的MemberWiseClone,但是从项目角度看最好还是忘了它，因为一旦您或某位同时为某个类型增加了引用型成员，那么克隆过程就可能落下点什么而且MemberwiseClone返回的结果是object类型，估计最初编码的时候会对它进行强制类型转换，这样如果进行了修改，即使在编译态都不容易发现这个隐藏很深的问题，到了生产环境就不容易定位一些问题。此时不妨考虑在做设计的时候，就定义某些类型必须实现ICloneable接口，相对MemberwiseClone那种太容易获得的方式，也许这种反噬更容易引发开发维护人员的注意。


    //看类图或E-R图中是否存在类型层次关系
    //1，如果设计足够详细，那么Association，Dependency，Composition和Aggregation都需要引起注意，因为他们一般都是类这种引用对象的关系，如果进行深层次复制就需要定制。

    //2，很多时候我们的业务对象也是来自于数据库的ORM结果，如果E-R关系中纯在外检，那么一般映射为对象也具有引用关系，大部分情况下深层复制层需要定制。

    //3，还有一种情况，就是基于纯XML环境的开发，由于使用中XML本身就是层次结构的，所以当选用XML数据来表示具有引用关系的对象时，很多时候深层复制反而不需要定制了，因为它本身就是个文本，可以直接用一个字符串复制（用NET Framework或自定义的XML对象传递除外） 


    //**************************************
    //实现深层拷贝

    //一种常用方法就是序列化，无论是二进制序列化还是XML序列化。。都是可行的，依据调用环境
    //因为一般情况下，使用System.SerializableAttribute给目标类型贴个标签即可，编译器会帮组检查，而且会沿着引用和继承关系一查到底，二则需要紫荆店址ISerializable序列化过程。



    //一个贴标签的实例，并用它完成一个服务于Clone()工具类，放在公共工具箱Common项目里

    //*********************************************************
    //7.2.2制作实现克隆的工具类型

    //为了实现深层复制，先完成一个用于序列化的静态类，它根据客户程序要求，通过二进制或SOAP方式序列化可序列化的对象
    public enum FormatterType
    {
        Soap,
        Binary
    }

    //public interface IRemotingFormatter
    //{
    //    void Serialize(Stream mm,object obj);
    //    object Deserialize(Stream mm);
    //}

    //public class BinaryFormatter:IRemotingFormatter
    //{

    //}

    //public class SoapFormatter : IRemotingFormatter
    //{

    //}



    public static class SerializationHelper
    {
        private const FormatterType DefaultFormatterType = FormatterType.Binary;
        //按照串行化的编码要求，生成对应的编码器
        private static IRemotingFormatter GetFormatter(FormatterType formatterType)
        {
            switch(formatterType)
            {
                case FormatterType.Binary: return new BinaryFormatter();
                case FormatterType.Soap: return new SoapFormatter();
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 把对象序列化转换成字符串
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="formatterType"></param>
        /// <returns></returns>

        public static string SerializeObjectToString(object graph,FormatterType formatterType)
        {
            using(MemoryStream memoryStream =new MemoryStream())
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                formatter.Serialize(memoryStream, graph);
                Byte[] arrGraph = memoryStream.ToArray();
                return Convert.ToBase64String(arrGraph);
            }
        }

        public static string SerializeObjectToString(object graph)
        {
            return SerializeObjectToString(graph, DefaultFormatterType);
        }

        
        public static T DeserializeStringToObject<T>(string graph,FormatterType formatterType)
        {
            Byte[] arrGraph = Convert.FromBase64String(graph);
            using(MemoryStream memoryStream=new MemoryStream(arrGraph))
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        public static T DeserializeStringToObject<T>(string graph)
        {
            return DeserializeStringToObject<T>(graph, DefaultFormatterType);
        }

    }

    [Serializable]
    public class UserInfo
    {
        //[NonSerialized]
        public string Name;
        public IList<string> Education=new List<string>();
        public UserInfo GetShallowCopy()
        {
            return (UserInfo)this.MemberwiseClone();
        }
        public UserInfo GetDeepCopy()
        {
            string graph = SerializationHelper.SerializeObjectToString(this);
            return SerializationHelper.DeserializeStringToObject<UserInfo>(graph);
        }

    }

    public class TestClass
    {
        //验证浅拷贝
        public void TestMethod1()
        {
            UserInfo user1 = new UserInfo();
            user1.Name = "joe";
            user1.Education.Add("A");
            UserInfo user2 = user1.GetShallowCopy();

            user2.Education[0] = "B";

            Console.WriteLine("user1:" + user1.Education[0]);
            Console.WriteLine("user2:" + user2.Education[0]);

            user2.Education.Add("C");

            Console.WriteLine("******user1*******");
            foreach(var s in user1.Education)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("******user2*******");
            foreach (var s in user2.Education)
            {
                Console.WriteLine(s);
            }
        }

        public void TestMethod2()
        {
            UserInfo user3 = new UserInfo();
            user3.Name = "joe";
            user3.Education.Add("A");
            UserInfo user4 = user3.GetDeepCopy();

            user4.Education[0] = "B";

            Console.WriteLine("user3:" + user3.Education[0]);
            Console.WriteLine("user4:" + user4.Education[0]);


            user3.Education.Add("asd");
            user4.Education.Add("qqq");


            Console.WriteLine("******user3*******");
            foreach (var s in user3.Education)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("******user4*******");
            foreach (var s in user4.Education)
            {
                Console.WriteLine(s);
            }

        }
        
    }



    //7.2.3克隆也可以保留个性
    //不需要属性可以被[NonSerializedAttribute]标记，则不会被克隆
    



    //7.2.4定制并管理的克隆过程
    //虽然上面的示例中，配合使用[Serializable]和[NonSerialized]是吸纳了总体序列化情况下有选择地提出某些属性，但是颗粒度有点怵，个别情况下我们还需要更加定制化的才做，这时候可以选择配合使用System.Runtime.Serialization.ISerilaizable和ICloneable的方式。

    [Serializable]
    public class UserInfoMode:ISerializable//实现ISerializable接口以控制序列化过程
    {
        public string Name;
        public int Age;
        public IList<string> Education = new List<string>();
        public UserInfoMode()
        {

        }

        //还原过程
        protected UserInfoMode(SerializationInfo info,StreamingContext context)
        {
            this.Name = info.GetString("Name");
            this.Education = (IList<string>)info.GetValue("Education", typeof(IList<string>));
        }

        //定制序列化过程，仅序列化如下内容：Name和Education记录的前三项
        public void GetObjectDate(SerializationInfo info,StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            IList<string> education = new List<string>();
            if(this.Education.Count>0)
            {
                for(int i=0;i<(Education.Count>3?3:Education.Count);i++)
                {
                    education.Add(this.Education[i]);
                }
            }
        }
     /* 
      * 
      * 任何可以序列化的类都必须用 SerializableAttribute 进行标记。 如果某个类需要控制其序列化进程，它可以实现 ISerializable 接口。 Formatter 在序列化时调用 GetObjectData，并使用表示对象所需的全部数据来填充所提供的 SerializationInfo。 Formatter 使用图形中对象的类型来创建 SerializationInfo。 需要自己发送代理的对象可以使用 SerializationInfo 上的 FullTypeName 和 AssemblyName 方法来更改所传输的信息。
      * 
      * 在类继承的情况下，可以序列化从实现 ISerializable 的基类中派生的类。 这种情况下，派生的类应在 GetObjectData 的实现内调用 GetObjectData 的基类实现。 否则，不会序列化来自基类的数据。

     * ISerializable 接口表示带有 Constructor 签名（SerializationInfo 信息、StreamingContext 上下文）的构造函数。 在反序列化时，仅在格式化程序已反序列化 SerializationInfo 中的数据后才调用当前构造函数。 一般而言，如果该类未密封，则应保护此构造函数。

     * 无法保证对象被反序列化的顺序。 例如，如果一种类型引用尚未反序列化的类型，则会引发异常。 如果创建具有这种依赖关系的类型，可以通过实现 IDeserializationCallback 接口和 OnDeserialization 方法来解决该问题。

     * 序列化结构处理像处理扩展 Object 的类型一样，处理扩展 MarshalByRefObject 的对象类型。 这些类型都可以使用 SerializableAttribute 来标记，并且可以将 ISerializable 接口实现为其他任何对象类型。 它们的对象状态将被捕获并在流中持续。

     * 当通过 System.Runtime.Remoting 使用这些类型时，远程处理结构会提供一个代理项，它将取代常用的序列化，而将代理序列化为 MarshalByRefObject。 代理项是知道如何将特定类型的对象序列化和反序列化的帮助器。 代理在大多数情况下对于用户不可见，其类型将是 ObjRef。

     * 作为一种常规的设计模式，类很少会既使用可序列化特性来标记，又扩展 MarshalByRefObject。 当组合这两项特性时，开发人员应仔细考虑可能的序列化和远程处理方案。 MemoryStream 就是一个适用的示例。 当 MemoryStream ( Stream ) 的基类从 MarshalByRefObject 扩展时，可以捕获 MemoryStream 的状态并随时将其还原。 因此，这样做可能是有意义的：将该流的状态序列化到数据库中，并在稍后某一时间将其还原。 但是，当通过远程处理来使用时，这种类型的对象将设置代理。

     * 有关序列化 MarshalByRefObject 的派生类的更多信息，请参见 RemotingSurrogateSelector。 有关如何实现 ISerializable 的更多信息，请参见 自定义序列化。

     * 对实现者的说明
     * 实现此接口，以允许对象参与其自己的序列化和反序列化过程。
     */



        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }



    //7.3重新定义原型方法

    //序列化仅仅是一个手段，我们要做实现克隆的过程，并实现原型模式

    public interface IPrototypeNew
    {
        string Name { get; set; }
        IPrototypeNew Clone();
    }


    [Serializable]
    public abstract class PrototypeBase:IPrototypeNew
    {
        public virtual IPrototypeNew Clone()
        {
            string graph = SerializationHelper.SerializeObjectToString(this);
            return SerializationHelper.DeserializeStringToObject<IPrototypeNew>(graph);
        }

        protected string name;
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }


    //

    /*
     * 这样做的好处就是以后那么类型如果觉得工厂太麻烦，用原型也懒得一遍遍编写Clone()的时候，直接继承PrototypeBase就可以了：不过这种方法也有一个不足之处，即C#使单继承的，如果项目为了通盘考虑，首相定义了一个基类，那客户类型就不能用PrototypeBase了。如果如果确定没有什么Singleton应用，可以考虑把其中的Clone方法复制过来。
    
     */


}
