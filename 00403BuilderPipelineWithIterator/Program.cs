using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00403BuilderPipelineWithIterator
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass tc = new TestClass();
            tc.TestMethod();


            Console.ReadLine();
        }
    }

    //6.6用迭代器控制流水线

    //很多时候采用基于Attribute的Builder过于麻烦，但根据上下文条件，BuildUp和TearDown在执行每个BuidPart或TearDownPart的时候，需要有不同的次序，而不是将次序彻底固定下来。


    //与之前设计不同的是，这里迭代器迭代的是一个个执行方法，而不是具体的数据。至于谁来管理这个迭代器，还是回归到经典的创建者模式，由Director来处理，因为在大多数强框架Director更多的作为一个流程的组织者。



    //抽象定义部分

    //迭代的结果是这个委托所定义的抽象方法,对应下面的BuildPart
    public delegate string StudyHandler();

    public interface IBuilder
    {
        string StudyA();//buildpart
        string StudyB();//buildpart
        string StudyC();//buildpart
    }

    public interface IDirector
    {
        //返回一个类型的集合的循环枚举器
        IEnumerable<StudyHandler> PlanSchedule(IBuilder builder);

        //生成计划
        IList<string> Construct(IBuilder builder);
    }

    public abstract class DirectorBase:IDirector
    {
        public abstract IEnumerable<StudyHandler> PlanSchedule(IBuilder builder);
        public IList<string> Construct(IBuilder builder)
        {
            //这里将使用处理过后的 buidlpart委托(StudyHandler)列表
            IList<string> schedule = new List<string>();
            foreach(StudyHandler handler in PlanSchedule(builder))
            {
                schedule.Add(handler());
            }
            return schedule;
        }
    }


    //两个不同的Director类和其指导方法
    public class DirectorA:DirectorBase
    {
        public override IEnumerable<StudyHandler> PlanSchedule(IBuilder builder)
        {
            //迭代器返回方式
            yield return new StudyHandler(builder.StudyA);
            yield return new StudyHandler(builder.StudyB);
            yield return new StudyHandler(builder.StudyC);
        }
    }
    public class DirectorB : DirectorBase
    {
        public override IEnumerable<StudyHandler> PlanSchedule(IBuilder builder)
        {
            yield return new StudyHandler(builder.StudyB);
            yield return new StudyHandler(builder.StudyB);
            yield return new StudyHandler(builder.StudyA);
        }
    }


    public class ConcreteBuilder:IBuilder
    {

        public string StudyA()
        {
            return "this is lesson A";
        }

        public string StudyB()
        {
            return "this is lesson B";
        }

        public string StudyC()
        {
            return "this is lesson C";
        }
    }


    //委托+迭代器可以很好地把BuildPart()的步骤基于了子类完成，而Director仅仅负责最高层抽象部分、

    public class TestClass
    {
        public void TestMethod()
        {
            IBuilder builder = new ConcreteBuilder();
            IDirector director = new DirectorA();
            IList<string> schedule = director.Construct(builder);
            foreach(string str in schedule)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("****************************");

            director = new DirectorB();
            schedule = director.Construct(builder);
            foreach (string str in schedule)
            {
                Console.WriteLine(str);
            }

        }

    }





    //小结

    //在整个创建型模式中，创建对象之外还要覆辙把复杂的对象及其他的每个部分组装起来。

    //工程中的扩展

    //1，为了动态组织构造工艺，我们把每个加工步骤做了抽象，以异步动态组装的放啊是实现构造过程

    //2，为了不一遍一遍的实现IBuider，我们给特定构造目标的某些方法“贴上标签”，借助反射和Attribute，是吸纳了一个具有动态发现机制的Builder

    //3，考虑到构造出来的产品在处理青可能同样复杂，甚至比构造过程更复杂，我们实现了具有闭合操作BuildUp TearDown的builder

    //4，考虑到不是和后续升级的问题，为了不打破应用框架，又可以把平凡的更新部署工作推给系统管理员，可以把变化频繁的对象定义到配置文件里。

    //5，我们把构造次序本身对象抽象为一个迭代器，不同Director只要拿到这个迭代器就可以按照迭代器说明的要求，依次指向每个加工步骤。IEumerable<T>



}
