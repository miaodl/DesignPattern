using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10300CompositePattern
{
    class Program
    {
        static void Main(string[] args)
        {

            IFactory<TestClass> factory = new Factorybase<TestClass>();
            factory.Create().Test();

            Console.ReadLine();


        }
    }

    public interface IFactory<T>
    {
        T Create();
    }

    public class Factorybase<T>:IFactory<T>where T:class,new()
    {
        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }


    //第十章 组合模式

    //Compose objects into tree structures to represent part-whole hierarchies.Composite lets clients treat individial objects and compositions of objects uniformly.


    /*
     * 
     * client------------------------->Component<------------------
     *                                    /|\                     |
     *                                     |                      |
     *                                     |                      |  
     *                                     |                      |
     *                                     |                      |
     *                  leaf-----------------------------------Composite
     *                                                           add()
     *                                                           remove()
     *                                                           getchild()
     *                                                           other()
     *                                                           
     * 
     * leaf：叶子代表单个个体，没有子节点
     * composite：组合节点代表具有容器特征的类型
     * componnent：定义leaf和composite的公共特征
     */

    public abstract class Component
    {
        //保存子节点
        protected IList<Component> children;

        /// <summary>
        /// leaf和composite的共同特征
        /// setter方式注入
        /// </summary>
        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// 其实只有Composite类型才需要真正实现的功能
        /// </summary>
        /// <param name="child"></param>
        public virtual void Add(Component child)
        {
            children.Add(child);
        }
        public virtual void Remove(Component child)
        {
            children.Remove(child);
        }
        public virtual Component this[int index]
        {
            get
            {
                return children[index];
            }
        }


        /// <summary>
        /// 实现迭代器，并且对容器对象实现隐性递归
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetNameList()
        {
            //yield return name;

            //如果是leaf结点，名字做--标示
            if (children != null)
                yield return name;
            else
                yield return "---" + name;

            if ((children != null) && (children.Count > 0))
                foreach (Component child in children)
                    foreach (string item in child.GetNameList())
                        yield return item;
        }


    }

    public class Leaf:Component
    {
        public override void Add(Component child)
        {
            throw new NotSupportedException();
        }
        public override  void Remove(Component child)
        {
             throw new NotSupportedException();
        }
        public override  Component this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }


    public class Composite:Component
    {
        public Composite()
        {
            base.children = new List<Component>();
        }
    }
   


    //增加一个工厂类型

    public class ComponentFactory
    {
        public Component Create<T>(string name) where T : Component, new()
        {
            T instance = new T();
            instance.Name = name;
            return instance;
        }
        /// <summary>
        /// 直接向某个节点下增加新节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Component Create<T>(Component parent,string name)where T:Component,new()
        {
            if(parent==null)throw new ArgumentNullException("parent");
            if (!(parent is Composite)) throw new Exception("non-composite type");

            Component instance = Create<T>(name);
            parent.Add(instance);
            return instance;
        }

    }

    //测试类
    public class TestClass
    {
        public void Test()
        {
            ComponentFactory factory = new ComponentFactory();
            Component corprate = factory.Create<Composite>("corprate");
            factory.Create<Leaf>(corprate, "president");
            factory.Create<Leaf>(corprate, "vice president");
            Component sales = factory.Create<Composite>(corprate, "sales");
            Component market = factory.Create<Composite>(corprate, "market");
            factory.Create<Leaf>(sales, "joe");
            factory.Create<Leaf>(sales, "bob");
            factory.Create<Leaf>(market, "judi");
            Component branch = factory.Create<Composite>(corprate, "branch");
            factory.Create<Leaf>(sales, "manager");
            factory.Create<Leaf>(sales, "peter");

            IList<string> names = new List<string>(corprate.GetNameList());

            foreach(var n in corprate.GetNameList())
            {
                Console.WriteLine(n);
            }
            Console.WriteLine("******************************");
            foreach (var n in names)
            {
                Console.WriteLine(n);
            }

            //Console.WriteLine(corprate.GetNameList().Single((i) => { return (i == "---judi") ? true : false; }));

        }
    }
   

    //10.4适于XML信息的组合模式



}
