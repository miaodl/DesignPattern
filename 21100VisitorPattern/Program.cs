using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21100VisitorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第二十五章 访问者模式
    //Represent an operation to be performed on the elements of an object structure.Visitor lets you define a new operation without changing the classes of the elements on which it operates

    /*
     * 由于应用开发过程中先前完成的类型会因为需求变化（无论是业务功能，还是技术实现火石处于集成的需要）增加新的方法，如果直接在基类中增加新的方法，其派生类可能需要相应的比较繁琐的处理。因此，如何在不改变既有类型层次的前提下，运行时动态为类型的每个类增加新的操作就成了访问者模式视图解决的问题。
     * 
     * Page 424 461
     */

    //25.2经典回顾
    /*
     *  访问者模式的使用情况：
     *  
     * 1，一个类型需要依赖很多不同接口的类型，在结构尽量松散的前提下，希望可以用到这些类型不同的接口方法
     * 
     * 2，经常需要为一个结构相对固定的对象结构添加一些新的操作
     * 
     * 3，需要用一个独立的类型来组织一批不相干的操作，使用他的类型可以根据应用需要进行定制 
     * 
     *                                                          
     *                            ____________Client_______                     
     *                           /                         \     
     *                          /                           \   
     *                         /                             \    
     *                        /                               \   
     *                       /                                 \  
     *   objectStrure<------     interface                       interface              
     *         |---------------->IElement                        IVisitor                         
     *                          +Accept(IVistor):void            +VisitElementA():void             
     *                           /\          /\                  +VisitElementB():void
     *                          /              \                         /|\
     *                         /                \                         |
     *                        /                  \                        |
     *          ConcreteElementA             ConcreteElementB       ConcreteVisitor           
     *          
     * 
     * 
     * //IVisitor：声明以一个或多个需要添加的操作
     * ConcreteVisitor：增加具体操作的类型
     * IElement:接受IVisitor并使用其操作的抽象类型
     * ObjectStructure:一个高层接口，用来做组织IElement，一般需要提供遍历每个IElement的能力
     */

    //visitor 需要影响到的Element,Visitable
    public interface IEmployee
    {
        //相关属性
        string Name
        {
            get;
            set;
        }
        double Income
        {
            get;
            set;
        }
        int VacationDays
        {
            get;
            set;
        }

        //接受IVisitor的方法
        void Accept(IVisitor visitor);
    }

    //抽象Visitor接口
    public interface IVisitor
    {
        //以下方法为访问Visiable类提供了具体操作，如何修改IVisiable类在方法中进行
        void VisitEmployee(IEmployee employee);//这里修改就只限定于IEmployee的具体成员
        void VisitManager(Manager manager);//这里的修改限定于Manager的具体成员
    }

    //一个具体Element
    public class Employee:IEmployee
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private double income;

        public double Income
        {
            get { return income; }
            set { income = value; }
        }
        private int vacationDays;

        public int VacationDays
        {
            get { return vacationDays; }
            set { vacationDays = value; }
        }

        public Employee(string name,double income,int vacationdays)
        {
            this.name = name;
            this.income = income;
            this.vacationDays = vacationdays;
        }

        //引入Visitor对自身的操作
        public virtual void Accept(IVisitor visitor)
        {
            visitor.VisitEmployee(this);
        }


    }

    //另一具体的Element
    public class Manager:Employee//层次的Object Structure
    {
        private string department;

        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        public Manager(string name,int income,int vacationdays,string department):base(name,income,vacationdays)
        {
            this.department = department;
        }

        //引入Visitor对自身的操作
        public override void Accept(IVisitor visitor)
        {
            visitor.VisitManager(this);
        }

    }

    //为了便于对HR系统的对象进行批量处理增加的集合类型
    public class EmployeeCollection:List<IEmployee>
    {
        //组合起来的批量Accept操作,其中IVisitor为对象结构提供了统一的访问
        public virtual void Accept(IVisitor visitor)
        {
            foreach (IEmployee employee in this)
                employee.Accept(visitor);
        }
    }

    /*
     * 可尝试在Visitable类中增加一个委托列表或字典，用具体的Visitor来维护类中的委托列表，使类结构增加新的操作方法
     * 
     */

    //具体的Visitor类，增加休假天数
    class ExtraVacationVisitor : IVisitor
    {
        public void VisitorEmployee(IEmployee employee)
        {
            employee.VacationDays += 1;
        }
        public void VisitorManager(Manager manager)
        {
            manager.VacationDays += 2;
        }
    }

    //具体Visitor，加薪
    class RaiseSalaryVistor:IVisitor
    {
        public void VisitEmployee(IEmployee employee)
        {
            employee.Income *= 1.1;
        }
        public void VisitManager(Manager manager)
        {
            manager.Income *= 1.2;
        }
    }


    public class Client
    {
        public void TestMethod()
        {
            EmployeeCollection employees =new EmployeeCollection();
            employees.Add(new Employee("joe", 2500, 14));
            employees.Add(new Employee("frank", 4500, 24));
            employees.Add(new Employee("john", 3500, 22));
            employees.Add(new Manager("dick", 6500, 34,"Sales"));

            employees.Accept(new ExtraVacationVisitor());
            employees.Accept(new RaiseSalaryVistor());
        }
    }

    /*
     * 1，Employee类型并没有加薪和修改休假天数的方法，但从效果上看确实获得了相应的功能
     * 
     * 2，新增功能通过外部具体IVisitor类型完成的，他们虽然完全独立，但只要被Employee或它的某个子类型请进去就可以完成新的操作
     * 
     * 3，访问者模式将有关的行为集中到一个统一抽象的IVisitor上面，而不是每个具体操作的类型上。
     * 
     * 但是从静态结构图和示例中我们也会发现访问者模式的一些缺点：
     *1， 当需要增加新的ConcreteElement的时候，IVisitor需要修改
     *2，从面向对象的角度看，Employee的封装性被破坏了，因为新增的功能是在Employee外部完成的
     * 
     * 
     */



}
