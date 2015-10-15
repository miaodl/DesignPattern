using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00101BatchFactory
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //******************************
    //批量工厂

    //定义产品类型容器

    //C#抽象产品类型与具体产品类型
    //抽象产品类型
    public interface IProduct
    {
        string Name { get; }//约定的抽象产品所必须具有的特征
    }


    /// <summary>
    /// 产品A
    /// </summary>
    public class ProductA : IProduct
    {
        public string Name
        {
            get { return "A"; }
        }
    }
    /// <summary>
    /// 产品B
    /// </summary>
    public class ProductB : IProduct
    {
        public string Name
        {
            get { return "B"; }
        }
    }

    //C#装载IProduct的容器类型
    public class ProductCollection
    {
        private IList<IProduct> data = new List<IProduct>();

        ///对外的集合操作方法
        public void Insert(IProduct item) 
        {
            data.Add(item);
        }

        public void Insert(IProduct[] items)
        {
            if ((items == null) || (items.Length == 0))
                return;
            foreach (IProduct item in items)
                data.Add(item);
        }
        public void Remove(IProduct item)
        {
            data.Remove(item);
        }
        public void Clear()
        {
            data.Clear();
        }
        ///获取所有IProduct内容的属性
        public IProduct[] Data
        {
            get
            {
                if ((data == null) || (data.Count == 0)) return null;
                IProduct[] result = new IProduct[data.Count];
                data.CopyTo(result, 0);
                return result;
            }
        }
        ///当前集合内元素数量
        public int Count { get { return data.Count; } }

        ///为了便于操作，重载的运算符
        public static ProductCollection operator +(ProductCollection collection,IProduct[] items)
        {
            ProductCollection result = new ProductCollection();
            if (!((collection == null) || (collection.Count == 0)))
                result.Insert(collection.Data);
            if (!((items == null) || (items.Length == 0)))
                result.Insert(items);
            return result;
        }
        public static ProductCollection operator +(ProductCollection source,ProductCollection target)
        {
            ProductCollection result = new ProductCollection();
            if (!((source == null) || (source.Count == 0)))
                result.Insert(source.Data);
            if (!((target == null) || (target.Count == 0)))
                result.Insert(target.Data);
            return result;
        }

    }

    ///定义批量工厂和产品类型容器
    ///C#定义并实现批量产品加工工厂  
    public interface IBatchFactory
    {
        /// <summary>
        /// 批量工厂的共同方法
        /// </summary>
        /// <param name="quantity">待加工的产品数量</param>
        /// <returns></returns>
        ProductCollection Create(int quantity);
    }

    /// <summary>
    /// 为方便批量工厂类扩展提供的抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchProductFactoryBase<T>:IBatchFactory where T:IProduct,new()
    {
        public virtual ProductCollection Create(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException();
            ProductCollection collection = new ProductCollection();
            for(int i=0;i<quantity;i++)
            {
                collection.Insert(new T());
            }
            return collection;
        }
    }

    


    ///两个实体批量生产工厂类型
    public class BatchProductAFactory:BatchProductFactoryBase<ProductA>
    {

    }
    public class BatchProductBFactory:BatchProductFactoryBase<ProductB>
    {

    }


    ///增设生产指导顾问Director
    ///Director本身组织了一系列的信息，他相当与指导客户程序跳读不同实体工厂生产产品的指挥者，为了让客户程序的调用成为一个连续的过程，Director可以采用迭代器组织每个Decision，这样客户程序就可以用一种连续的线性方式完成每个Dicision所记录的生产任务
    ///C#定义Director和Dicision
    
    public abstract class DecisionBase
    {
        protected IBatchFactory factory;
        protected int quantity;
        public DecisionBase(IBatchFactory factory,int quantity)
        {
            this.factory = factory;
            this.quantity = quantity;
        }

        public virtual IBatchFactory Factory
        {
            get
            {
                return factory;
            }
        }
        public virtual int Quantity
        {
            get
            {
                return quantity;
            }
        }

    }

    public abstract class DirectorBase
    {
        protected IList<DecisionBase> decisions = new List<DecisionBase>();

        ///实际项目中，最好将每个Director需要添加的Decision也定义在配置文件中，
        ///这样更增加新的Decision项都在后台完成，而不需要Assembler显示调用该方法补充
        protected virtual void Insert(DecisionBase decision)
        {
            if((decision==null)||(decision.Factory==null))
            {
                throw new ArgumentException("decision");
            }
            decisions.Add(decision);
        }

        ///便于客户程序使用增加的迭代器
        public virtual IEnumerable<DecisionBase> Decisions
        {
            get
            {
                return decisions;
            }
        }
    }


    ///由Director指导的客户程序
    ///完成三个辅助类型(ProductColleciton,Director,Decison）的设计后，就可以有Director指导生产的新客户程序
    ///C#采用硬编码，没用通过Assembler获取Director
    
    ///生产商品A的Decision
    class ProductADecision:DecisionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ProductADecision()
            : base(new BatchProductAFactory(), 2)
        {

        }
    }
    ///生产商品B的Decision
    class ProductBDecision : DecisionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ProductBDecision()
            : base(new BatchProductBFactory(), 2)
        {

        }
    }

    class ProductDirector:DirectorBase
    {
        public ProductDirector()
        {
            base.Insert(new ProductADecision());
            base.Insert(new ProductBDecision());
        }
    }


    /// <summary>
    /// 到入指导者后
    /// 客户程序直接调用指导者(director),就可以直接生产Product
    /// 只需关心指导者中生产决定的安排，其他的都不用关心
    /// 
    /// 外部注入Director需要提前明确生成的产品的类型，和构造产品对应工厂的类型
    /// 产品批量工厂（带产品类型和数量，功能可扩展）的调用根据具体生产决定中的产品安排来
    /// </summary>
    class Client
    {
        ///实际项目中，可以通过Assembler从外部把Director注入
        ///从外部导入就不需要这里进行自定义一个建好的有生产决定(ProductDecision)的指导者
        private DirectorBase director = new ProductDirector();
        public IProduct[] Produce()
        {
            ProductCollection collection = new ProductCollection();
            foreach(DecisionBase decision in director.Decisions)
            {
                collection += decision.Factory.Create(decision.Quantity);
            }
            return collection.Data;
        }
    }
    

    //Decision就是一个Stragedy，而Director本身可以作为客户程序的外部机制(Obserber)，亦不知道客户程序的执行



}
