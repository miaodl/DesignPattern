using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00102GenericFactoryParam
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

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
        public static ProductCollection operator +(ProductCollection collection, IProduct[] items)
        {
            ProductCollection result = new ProductCollection();
            if (!((collection == null) || (collection.Count == 0)))
                result.Insert(collection.Data);
            if (!((items == null) || (items.Length == 0)))
                result.Insert(items);
            return result;
        }
        public static ProductCollection operator +(ProductCollection source, ProductCollection target)
        {
            ProductCollection result = new ProductCollection();
            if (!((source == null) || (source.Count == 0)))
                result.Insert(source.Data);
            if (!((target == null) || (target.Count == 0)))
                result.Insert(target.Data);
            return result;
        }

    }


    ///基于类型参数的Generic Factory
    ///随着泛型的使用，IProduct的含义也被扩展了，项目中经常需要提供IProduct<T>,甚至IProduct<T1,T2,...>之类的泛型抽象产品类型。这些类型在类库的最外层往往被赋予了具体的类型参数，但是内层类库部分，或者在高度抽象的通用算法部分，往往会继续保持泛型的抽象类型。为了保证工厂类型的加工过程的通用性，也需要设计具有泛型的Generic Factory
    
    ///C#抽象的泛型工厂类型
    public interface IFactory<T>
    {
        T Create();
    }

    public abstract class FactoryBase<T>:IFactory<T> where T:new()
    {
        ///由于批量工厂的可能应用概率比较小，因此默认为实现单个产品的工厂
        public virtual T Create()
        {
            return new T();
        }
    }

    ///生产单个产品的实体工厂
    public class ProductAFactory:FactoryBase<ProductA>
    {

    }

    public class ProductBFactory:FactoryBase<ProductB>
    {

    }


    ///生产批量产品工厂的抽象定义
    public abstract class BatchFactoryBase<TCollection,TItem>:FactoryBase<TCollection> where TCollection:ProductCollection,new() where TItem:IProduct,new()
    {
        protected int quantity;
        public virtual int Quantity
        {
            set
            {
                this.quantity = value;
            }
        }
        public override TCollection Create()
        {
            if (quantity <= 0) throw new ArgumentException("quantity");
            TCollection collection = new TCollection();
            for(int i=0;i<quantity;i++)
            {
                collection.Insert(new TItem());
            }
            return collection;
        }
    }

    ///生产批量产品的实体工厂
    public class BatchProductAFactory:BatchFactoryBase<ProductCollection,ProductA>
    {

    }

    public class BatchProductBFactory : BatchFactoryBase<ProductCollection, ProductB>
    {

    }



}
