using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10600FlyweightPattern
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

    //第十三章 Flyweight享元模式

    //Some applications could benefit from using objects throughout their design,but a naive implemetation would be prohibitively expensive

    /*
     * 
     * 享元模式提醒我们注意一个问题“用面向对象思想设计的应用常常会面临对象实例过多的问题”，一个最直接的结果就是占用比较多的内存，日过这些对象是主动对象，那么处理器和其他外围设备将承受较大的计算压力
     * 
     * .NET语言中实例对象在调用中既可以用值方式也可以用引用方式传递，也就是常说的值类型和引用类型。
     * 
     * 值类型要么是堆栈分配的，要么是在结构中以内敛方式分配的，值类型包括类型，枚举类型和结构类型
     * 
     * 引用类型是堆分配的，操作他们的时候，我们是基于引用完成的，也就是说如果可以通过某种方式修改概念上的两个对象使用同一个引用，即操作的目标是同一个堆结构，内存上使用了相当于仅占用了一次。引用类型包括类类型，接口类型，委托类型和数组类型。
     * 
     * 
     * 整个.NET托管类型系统（引用类型和值类型）都是从基类Object派生出来的。当值类型需要充当对象是，就在堆上分配一个包装（该包装能使值类型看上去像应用对象一样），并且将该值类型的复制给它。该包装加上标记，以便系统知道它包含一个值类型。这个进程被称为Boxing，其反向进程被称为Unboxing
     */



    //13.2

    //享元模式的意图是通过共享有效支持大量细粒度对象。

    //细粒度对象通过共享节约资源
    //1，某个类型的对象实例众多,2，众多实例可以被有效地归为数量相对小很多的种类

    /*
     * 
     *                              
     *                                              interface   
     * FlyweightFactory<>--------------------------->IFlyweight
     *        |                                      +operation()
     *        |                                       /       \
     *        |                                      /         \
     *        |                                     /           \
     *        |                    UnsharedFlyweight             ConcreteFlyweight
     *        |                    get+IntrisicState             get+AllState
     *        |                            /|\                        /|\
     *        |                             |                          |
     *        |                             |                          |
     *        |                             |                          |
     *      Client-------------------------------------------------------
     *      
     */
    
     


    //实例中没有体现出经典模式中外部状态和内部状态的区别。
    public abstract class FoodBase
    {
        //食物可能兼有几种口味。。。。
        //同时也是相对比较占空间的部分

        //注意是私有函数。。其子类不具备
        private IList<string> tastes = new List<string>();

        private string name;
        public FoodBase(string name)
        {
            this.name = name;
        }


        //这个方法相当于对子类开放了，子类对该父类以上私有集合的访问权限
        public FoodBase AddTaste(string taste)
        {
            tastes.Add(taste);
            return this;
        }

        public virtual string Name
        {
            get
            {
                return this.name;
            }
        }

    }




    //定义具体类型和工厂
    public class Capsium:FoodBase
    {
        public Capsium():base("capsium")
        {
            base.AddTaste("Hot");
        }
        public string MostPopularInCuisine
        {
            get
            {
                return "chuancai";
            }
        }
    }

    public class Cheese:FoodBase
    {
        public Cheese():base("Cheese")
        {
            base.AddTaste("Salty");
        }

        public int MeltingPoint
        {
            get
            {
                return 280;
            }
        }

    }



    public class FoodFactory
    {
        private IDictionary<string, FoodBase> dictionary = new Dictionary<string, FoodBase>();
        public FoodBase Create(string name)
        {
            FoodBase result;
            if (dictionary.TryGetValue(name, out result))
                return result;
            switch(name)
            {
                case "Capsium": result = new Capsium(); break;
                case "Cheese": result = new Cheese(); break;
                default: throw new NotSupportedException();
            }
            dictionary.Add(result.Name, result);
            return result;

        }
    }


    public class TestClass
    {
        public void TestMethod()
        {
            FoodFactory factory = new FoodFactory();
            FoodBase f1 = factory.Create("Cheese");
            FoodBase f2 = factory.Create("Cheese");

            if(f1.GetHashCode()==f2.GetHashCode())
            {
                Console.WriteLine("Equal");
            }
            else
            {
                Console.WriteLine("NotEqual");
            }

            if (f1==f2)
            {
                Console.WriteLine("Equal");
            }
            else
            {
                Console.WriteLine("NotEqual");
            }

            Console.ReadLine();
        }
    }


    //应该认识到享元模式只是给出了一个本地内存资源节省的方案，不一定会对实际操作起到很好的支持作用，尤其在网络和互联网情况下：不过作为其他性资源的控制手段，享元模式不错




}
