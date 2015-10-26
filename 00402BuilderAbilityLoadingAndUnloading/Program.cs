using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00402BuilderAbilityLoadingAndUnloading
{
    class Program
    {
        static void Main(string[] args)
        {
        }


    }

    //*********************************************
    //6.4具有装配和卸载能力的Builder

    //上面介绍了一个创建者的工作BuidUp一个产品类型的实例，但实际应用中不仅仅是BuildUp(),或者说当我们无需某个创建出的实例后，不一定单纯的product=null;就可以结局问题。比如：一个Database对象在被置为null之前最后关闭已经打开的链接，如果引用了Sicket端口，最后把它释放掉。

    //下面针对这样一个复杂的类型设计具有闭合操作的创建者。


    //产品类型
    public class Product
    {
        public int Count;
        public IList<int> Items;
    }


    //描述具有闭合操作的抽象创建者
    public interface IBuilder<T>
    {
        T BuildUp();
        T TearDown();
    }


    public class ProductBuilder:IBuilder<Product>
    {
        private Product product = new Product();
        private Random random = new Random();

        public Product BuildUp()
        {
            product.Count = 0;
            product.Items = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                product.Items.Add(random.Next());
                product.Count++;
            }
            return product;
        }

        public Product TearDown()
        {
            while(product.Count>0)
            {
                int val = product.Items[0];
                product.Items.Remove(val);
                product.Count--;
            }
            return product;
        }

    }


    //6.5登记配置文件
    //6.5.1把UML对象变成XSD    Page157   194

    //这部分包含从配置文件创建对象的内容


}
