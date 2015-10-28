using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _00400BuilderPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    ////创建者模式
    ////page 136 176

    ////separate the construction of a complex object from its presentation so that the same construction pricess can create different representation.

    ////创建者模式Builer模式一般用于创建复杂对象，从独立创建每个部分到最后的组装，它承担每个步骤的工作。


    ////builer包括三个角色
    ////builder（IBuilder）：负责描述创建一个产品各个组成的抽象接口
    ////Concrete Builder：实现Builder要求的内容，并且提供一个获得产品的方法。
    ////Director：虽然Builer定义了构造产品的每个步骤，但Director只是告诉借助Builder生产产品的过程，它对Builder的操作完全基于Builder的抽象方法。

    ////生产非同源产品的经典Builder模式实现

    //public class House
    //{
    //    public void AddWindowAndDoor()
    //    {

    //    }
    //    public void AddWallAndFloor()
    //    {

    //    }
    //    public void AddCeiling()
    //    {

    //    }
    //}

    //public class Car
    //{
    //    public void AddWheel() 
    //    {

    //    }
    //    public void AddEngine()
    //    {

    //    }
    //    public void AddBody()
    //    {

    //    }

    //}

    //public interface IBuilder
    //{
    //    void BuildPart1();
    //    void BuildPart2();
    //    void BuildPart3();
    //}

    //public class CarBuilder:IBuilder
    //{
    //    private Car car;
    //    public void BuildPart1() 
    //    {
    //        car.AddEngine();
    //    }
    //    public void BuildPart2()
    //    {
    //        car.AddWheel();
    //    }
    //    public void BuildPart3()
    //    {
    //        car.AddBody() ;
    //    }
    //}

    //public class HouseBuilder : IBuilder
    //{
    //    private House house;
    //    public void BuildPart1()
    //    {
    //        house.AddWallAndFloor();
    //    }
    //    public void BuildPart2()
    //    {
    //        house.AddWindowAndDoor();
    //    }
    //    public void BuildPart3()
    //    {
    //        house.AddCeiling();
    //    }
    //}

    //public class Director
    //{
    //    public void Construct(IBuilder builder)//知道IBuilder的创建过程
    //    {
    //        builder.BuildPart1();
    //        builder.BuildPart2();
    //        builder.BuildPart3();
    //    }
    //}


    ////生产同源产品的经典Builder模式实现
    //public interface IProduct
    //{
    //    string Name { get; set; }
    //}
    //public class ConcreteProduct:IProduct
    //{
    //    protected string name;

    //    public string Name
    //    {
    //        get
    //        {
    //            return this.name;
    //        }
    //        set
    //        {
    //            this.name = value;
    //        }
    //    }
    //}

    //public interface IBuilder
    //{
    //    void BuildPart();
    //    //相当于GetResult()方法，由于构造的产品属于同一种类型
    //    //因此这类Builder模式视线中，该方法直接定义在抽象对象上
    //    IProduct BuilderUp();
    //}

    //public class ConcreteBuilderA:IBuilder
    //{
    //    private IProduct product = new ConcreteProduct();
    //    public void BuildPart()
    //    {
    //        product.Name = "A";
    //    }
    //    public IProduct BuildUp()
    //    {
    //        return product;
    //    }
    //}

    //public class Director
    //{
    //    public void Construct(IBuilder builder)
    //    {
    //        builder.BuilderUp();
    //    }
    //}

    ////创建者模式将复杂对象的每个组成创建步骤暴露出来，借助Director或客户端自己既可以选择其执行次序，也可以选择要执行哪些步骤。上述过程可以在应用中动态完成，相比较工厂方法和抽象工厂模式的一次性创建过程而言，创建者模式适合创建更为复杂且每个组成变化较多的类型

    ////向客户端屏蔽了对象创建过程的多边性

    ////正如构造过程的最终成果可以更具实际变化的情况，选择使用一个统一的接口，或者不同类的对象，增加客户类型更大的灵活度。

    ////劣势：相对而言，创建者模式会暴露更多的执行步骤，需要Director具有更多的领域知识，使用不慎很容易造成相对更为紧密的耦合。


}
