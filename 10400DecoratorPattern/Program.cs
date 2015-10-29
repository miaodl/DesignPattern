using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10400DecoratorPattern
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

    //第十一章 装饰者模式

    //Attach additional responsibilties to an object dynamically.Decorators provide a flexible alternative subclassing for extending functionality
    
    //装饰者模式是设计模式中实现技巧性非常明显的一个模式，它很多时候也会用来应对类型在继承过程中快速膨胀的情况，而导致膨胀的原因是我们往往需要为类型增加新的职责（功能），而这恰恰是软件在开发和维护阶段过程中最经常的变化。

    /*
     * 
     *            interface
     *            IComponent----------
     *            +operation()       |
     *             /          \      |
     *            /            \     |
     *           /              \    <>
     *          /                interface
     *         /                 IDecorator
     * ConcreteComponent         /        \
     *                          /          \
     *                         /            \
     *          ConcreteDecoratorA           ConcreteDecoratorA
     *          +NewOperation                -newProperty
     * 
    */

    //以上结构被装饰的实体类和需要的装饰类都继承统一接口，可以看做是主件和插件的组合？？

    /*
     * 装饰者模式适用的场景
     * 
     * 在不影响其他对象的情况下，以动态透明的方式给单个对象添加职责。毕竟客户程序依赖的仅仅是IComponent接口，至于这个接口被做过什么装饰只有实施装饰的对象才知道，而客户程序只负责根据IComponent的方法调用
     * 
     * 
     * 屏蔽某些职责，也就是在套用某个装饰类型的时候，并不增加新的特征，而只把既有方法屏蔽
     * 
     * 避免出现了适应变化而子类膨胀的情况
     */


    //抽象部分,这个接口用于外扩出目标实体的需要被附加操作的属性或方法
    public interface IText
    {
        string Content { get; }
    }

    //通过继承ITarget获取目标类的需要被操作的属性或方法，并规范装饰类型的方法
    public interface IDecorator:IText
    {

    }

    //继承并包含 目标实体类需要的接口！！！
    public abstract class DecoratorBase:IDecorator//is a IDecorate
    {
        //has a
        protected IText target;

        public DecoratorBase(IText target)
        {
            this.target = target;
        }
        public abstract string Content
        {
            get;
        }
    }
    //




    //具体装饰类实现

    public class BoldDecorator:DecoratorBase
    {
        public BoldDecorator(IText target):base(target)
        {

        }

        public override string Content
        {
            get { return ChangeToBoldFont(target.Content); }
        }

        public string ChangeToBoldFont(string content)
        {
            return "<b>" + content + "</b>";
        }

    }

    //具体装饰类
    public class ColorDecorator:DecoratorBase
    {
        public ColorDecorator(IText target):base(target)
        {

        }

        public override string Content
        {
            get { return AddColorTag(target.Content); }
        }

        public string AddColorTag(string content)
        {
            return "<color>" + content + "<color>";
        }
    }


    //具体装饰类

    public class BlockAllDecortor:DecoratorBase
    {
        public BlockAllDecortor(IText target):base(target)
        {

        }
        public override string Content
        {
            get { return string.Empty; }
        }
    }

    //实体对象类
    public class TextObject:IText
    {
        public string Content
        {
            get
            {
                return "hello";
            }
        }
    }



    public class TestClass
    {
        public void TestMethod()
        {
            IText text = new TextObject();

            text = new BoldDecorator(text);
            text = new ColorDecorator(text);

            //text = new BlockAllDecortor(text);

            Console.WriteLine(text.Content);
        }
    }




    //11.3具有自我更新特征的装饰模式

    //................................


    //11.4设计Decorator与Builder协作的产物


    //C#登记客户类型与相应装饰类型配置关系的Assembly类型

    public class DecoratorAssembly
    {
        private static IDictionary<Type, IList<Type>> dictionary = new Dictionary<Type, IList<Type>>();

        //项目中这个加载过程可以借助配置完成,通过构造函数依赖注入
        static DecoratorAssembly()
        {
            IList<Type> types = new List<Type>();
            types.Add(typeof(BoldDecorator));
            types.Add(typeof(ColorDecorator));
            dictionary.Add(typeof(TextObject), types);
        }

        //索引方式返回客户类型选择相应的Decorator列表
        public IList<Type> this[Type type]
        {
            get
            {
                if (type == null)
                    throw new ArgumentNullException("type");
                IList<Type> result;
                return dictionary.TryGetValue(type, out result) ? result : null;
            }
        }

        //完成装配过程的Builder类型
        public class DecoratorBuilder
        {
            private DecoratorAssembly assembly = new DecoratorAssembly();
            public IText BuildUp(IText target)
            {
                if (target == null)
                    throw new ArgumentNullException("null");
                IList<Type> types = assembly[target.GetType()];
                if((types!=null)&&(types.Count>0))
                {
                    foreach (Type type in types)
                        //相当于text=new ColorDecorator(text);
                        target = (IText)Activator.CreateInstance(type, target);
                }
                return target;
            }
        }


        //修改后的IText仅仅依赖于一个Builder类型
        //IText text=new TextObject();
        //TextObject=(new DecoratorBuilder()).BuildUp(text);



        //11.5使用Attribute注入

        //Page 236 273




        //还有很多类容没看。。。。。。。。
    }

}
