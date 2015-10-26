using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _00401BuiderPatternAsynchronousCallBuildUp
{
    class Program
    {
        static void Main(string[] args)
        {
            

            TestBuilder ts = new TestBuilder();

            ts.Test();


            Console.ReadLine();

        }

        



       
    }

    public class Car
    {
        public void AddWheel()
        {

        }
        public void AddEngine()
        {

        }
        public void AddBody()
        {

        }

    }

     //6.2异步调用的BuildUp
    //前面的Director的实现把整个步骤进行的硬编码，不灵活

    //构造一个汽车类型时，每个BuildPart步骤的委托定义，抽象了前面的步骤方法
    public delegate void BuildStepHandler();


    //考虑使用个通用的Director，它负责保存一个IList<BuildStepHandler>，Director在执行Constructor的时候一次便利每个BuildStepHandler()即可，装配列表的任务可以交个配置机制，或者是外部注入。这样Director直接就是一个BuildStepHandler列表



    public interface IBuilder
    {
        Car BuildUp();
    }

    public abstract class BuilderBase:IBuilder
    {
        protected IList<BuildStepHandler> steps = new List<BuildStepHandler>();
        protected Car car = new Car();

        public virtual Car BuildUp()
        {
            foreach(BuildStepHandler step in steps)
            {
                step();
            }
            return car;
        }
    }

    public class ConcreteBuilder:BuilderBase
    {
        //由于BuildStepHandler描述对象的BuildPart方法，因此实际项目中，
        //ConcreteBuilder需要配置的委托可以统一通过访问配置文件的方式获得

        public ConcreteBuilder():base()
        {
            steps.Add(car.AddEngine);
            steps.Add(car.AddWheel);
            steps.Add(car.AddBody);
        }
    }


    public class Client
    {
        //test method
        public void Test()
        {
            IBuilder builder = new ConcreteBuilder();
            Car car = builder.BuildUp();

            //Assert.IsNotNULL(car);
        }
    }

    /*BuilderStepHandler的分类

     * Pre Process     构造过程执行前的处理步骤
     * Construct        复杂对象的构造
     * Initialization       复杂对象子对象的初始化和装配过程（一般通过注入方式实现）
     * Post Process         构造过程执行后的扫尾步骤
    */


    //6.3为Builder打个标签
    //.NET平台提供Attribute标签机制，用Attribute来表示扩展目标产品类型的创建信息，完成一个通用的Builder类

    //工具类型设计
    //获取某个类型包括制定属性的集合
    //public static IList<T>


    //帮助客户类型和客户程序获取其Attribute定义中需要的抽象类型实例的工具类
    //static class AttributeHelper
    //{
    //    public static T Injector<T>(object target)where T:class
    //    {
    //        if (target == null) throw new ArgumentNullException("target");
    //        Type targetType = target.GetType();
    //        object[] attributes =targetType.GetCustomAttributes(typeof(),false);

    //        if((attributes==null)||(attributes.Length<=0))
    //            return null;
    //        foreach(DecoratorAttribute attribute in (DecoratorAttribute[])attributes)
    //            if(attribute.Type==typeof(T))
    //                return (T)attribute.Injector;
    //        return null;
    //    }
    //}


    //特性助手，帮助获取类的特性信息，标签信息
    public class AttributeHelper
    {
        /// <summary>
        /// 获取某个类型包括指定属性的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<T> GetCustomAttributes<T> (Type type) where T:Attribute
        {
            if (type == null) throw new ArgumentNullException("type");
            T[] attributes = (T[])(type.GetCustomAttributes(typeof(T), false));
            return (attributes.Length == 0) ? null : new List<T>(attributes);
        }


        /// <summary>
        /// 获得某个类型中包括指定属性的所有方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<MethodInfo> GetMethodsWithCustomAttributes<T>(Type type) where T : Attribute
        {
            if (type == null) throw new ArgumentNullException("type");
            MethodInfo[] methods = type.GetMethods();
            if((methods==null)||(methods.Length==0))return null;
            IList<MethodInfo> result =new List<MethodInfo>();
            foreach (MethodInfo method in methods)
                if (method.IsDefined(typeof(T), false))
                    result.Add(method);
            return result.Count == 0 ? null : result;
        }


        /// <summary>
        /// 获取某个方法指定类型的属性的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IList<T> GetMethodCustomAttributes<T>(MethodInfo method)where T:Attribute
        {
            if (method == null) throw new ArgumentNullException("method");
            T[] attributes = (T[])(method.GetCustomAttributes(typeof(T), false));
            return (attributes.Length == 0) ? null : new List<T>(attributes);
        }


        /// <summary>
        /// 获取某个方法的指定的类型(T:Attribute)的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T GetMethodCustomAttribute<T>(MethodInfo method) where T : Attribute
        {
            IList<T> attributes = GetMethodCustomAttributes<T>(method);
            return (attributes == null) ? null : attributes[0];
        }

    }

    //定义用于指导BuildPart过程的属性

    
    /// <summary>
    ///指导某个具体类型BuildPart过程目标方法和执行情况的属性
    ///
    /// sequence为某个方法在BuildUp过程中的次序，Times表示执行次数，Handler表示需要通过反射机制实际执行的目标方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false)]
    public sealed class BuildStepAttribute:Attribute,IComparable
    {
        private int sequence;
        private int times;
        private MethodInfo handler;

        public BuildStepAttribute(int sequence,int times)
        {
            this.sequence=sequence;
            this.times=times;
        }

        public BuildStepAttribute(int sequence):this(sequence,1)
        {

        }

        /// <summary>
        /// 该Attribute需要执行的目标方法
        /// </summary>
        public MethodInfo Handler
        {
            get{return handler;}
            set{this.handler=value;}
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
        }
        public int Times
        {
            get
            {
                return this.times;
            }
        }

        /// <summary>
        /// 确保每个BuildStepAttribute可以更具sequence比较执行次序
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int CompareTo(object target)
        {
            if((target==null)||(target.GetType()!=typeof(BuildStepAttribute)))
                throw new ArgumentException("target");
            return this.sequence-((BuildStepAttribute)target).sequence;
        }

        //借助这个属性，Builder可以获得执行某个BuildPart步骤的知道信息，包括该步骤的执行次序，需要执行的次数，以及通过反射获得方法的信息。

        //定义具有BuildPart自动发现机制的动态Builder


    }
    public interface IBuilder<T> where T : class,new()
    {
        T BuildUp();
    }

    public class Builder<T> : IBuilder<T> where T : class,new()
    {
        public virtual T BuildUp()
        {
            //获取类型的BuildPart()的属性信息（包括次数，次序和关联方法），并将属性实例排序
            IList<BuildStepAttribute> attributes = DiscoveryBuildSteps();

            //简单对象 如果没有BuildPart()步骤，退化为Factory模式
            if (attributes == null) return new T();

            //BuilderStepAttribute指导构造复杂对象
            T target = new T();
            foreach (BuildStepAttribute attribute in attributes)
                for (int i = 0; i < attribute.Times; i++)
                    attribute.Handler.Invoke(target, null);
            return target;
        }

        /// <summary>
        /// 借助反射获得类型T所须执行BuildPart()的自发现机制
        /// </summary>
        /// <returns></returns>
        protected virtual IList<BuildStepAttribute> DiscoveryBuildSteps()
        {
            //获得某个类型中包括指定属性的所有方法
            IList<MethodInfo> methods = AttributeHelper.GetMethodsWithCustomAttributes<BuildStepAttribute>(typeof(T));
            if ((methods == null) || (methods.Count == 0)) return null;
            
            //属性暂存表
            BuildStepAttribute[] attributes = new BuildStepAttribute[methods.Count];

            for (int i = 0; i < methods.Count; i++)
            {
                //获取每个方法对应的属性实例（属性中包含必要的信息）
                BuildStepAttribute attribute = AttributeHelper.GetMethodCustomAttribute<BuildStepAttribute>(methods[i]);

                //将方法本身关联到属性的成员当中
                attribute.Handler = methods[i];
                //将关联了方法的属性实例存到暂存表中
                attributes[i] = attribute;
            }

            //因为BuildStepAttribute属性继承了IComparable属性，所以可以对属性进行排列
            Array.Sort<BuildStepAttribute>(attributes);

            //最后返回排序后的属性表（装载了各种信息：次数，关联方法）
            return new List<BuildStepAttribute>(attributes);
        }



    }



    public class TestBuilder
    {
        public class MyCar
        {
            //用log来记录和表示mycar实例的内部构造
            public IList<string> log = new List<string>();


            //这里吧BuildStepAttribute封上了，实际项目中完全可以突破这个限制，把Times、Sequence通过配置文件告诉BuildStepAttribue
            [BuildStep(2,4)]
            public void AddWheel()
            {
                log.Add("Wheel");
            }
            public void AddEngine()
            {
                log.Add("Engine");
            }
            [BuildStep(1)]
            public void AddBody()
            {
                log.Add("Body");
            }

            [BuildStep(3,6)]
            public void AddWindow()
            {
                log.Add("window");
            }
        }

        public void Test()
        {
            Builder<MyCar> builder = new Builder<MyCar>();
            MyCar mycar = builder.BuildUp();

            foreach (string ss in mycar.log)
            {
                Console.WriteLine(ss);
            }


        }


        

    }


    //从性能上来看，通过反射调用每个BuildPart()步骤视乎有些慢，而且每次BuildUp()的时候都需要通过反射动态获取IList<BuildStepAttribute>,可以参考Enterprise Library中自动发现存储过程参数列表的办法，增加一个缓冲，确保获取IList<BuildStepAttribute>的步骤警备执行一次

   //private static IDictionary<Type, IList<BuildStepAttribute>> cache = new Dictionary<Type, IList<BuildStepAttribute>>();

   //     protected virtual IList<BuildStepAttribute> DiscoveryBuildSteps()
   //     {
   //         if(!cache.ContainsKey(typeof(T)))
   //         {

   //         }
   //         return cache[typeof(T)];
   //     }


    //很显然上面的设计并不是线程安全的，实际项目中可以把Cache设计成一个独立的Singleton对象，有这个Singleton实例维护所有的Type/IList<BuildStepAttribute>的对应关系，同时借助Singleton模式中介绍的线程安全Singleton实现即可







}
