using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _21101VisitorPatternReflectionImplement
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    //25.3借助反射实现Visitor

    /*
     * 前面的例子虽然实现了经典的访问者模式，不过还是有些别扭，因为Employee和Manager类型都需要知道自己应该调用VisitEmployee和VisitManager方法。这样一方面IVisitor的这两个方法被外部类型显示依赖，另一方面也翻盖IVisitor通过其他模式（策略模式，状态模式，解释器模式）动态加载合适各个IEmployee类型曹组的能力。
     * 
     * 下面采用反射实现：
     * 
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
        void Visit(IEmployee employee);
    }

    //一个具体Element
    public class Employee : IEmployee
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

        public Employee(string name, double income, int vacationdays)
        {
            this.name = name;
            this.income = income;
            this.vacationDays = vacationdays;
        }

        //引入Visitor对自身的操作
        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }


    }

    //另一具体的Element
    public class Manager : Employee//层次的Object Structure
    {
        private string department;

        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        public Manager(string name, int income, int vacationdays, string department)
            : base(name, income, vacationdays)
        {
            this.department = department;
        }

        //引入Visitor对自身的操作，所有的子类都依赖于IVisitor，这里的Accept方法可以不用重写了
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

    }


    //*******基于反射的Visitor*******

    public class ReflectionVisitor:IVisitor
    {
        //通过反射和预定好饿方法命令规则动态执行
        public void Visit(IEmployee employee)
        {
            string typeName = employee.GetType().Name;//获取employee的类型名
            string methodName = "Visit" + typeName;//使用Visit+类型名拼接关联的方法名
            MethodInfo method = this.GetType().GetMethod(methodName);//获取反射Visitor类的关联employee的方法
            method.Invoke(this, new object[] { employee });//调用这个关联的方法，并把employee作为参数传入，这里实现了一个动态的调用
        }

        //这样对相关类的所有Visit访问操作，都写在这两个方法里,并且只依赖于IEmployee接口
        public void VisitEmployee(IEmployee employee)
        {
            employee.Income *= 1.1;
            employee.VacationDays += 1;
        }
        public void VisitManager(IEmployee employee)
        {
            Manager manager = (Manager)employee;
            manager.Income *= 1.2;
            manager.VacationDays += 2;
        }

    }
    //**********************
    //这里还可以经一部调整把具体类型与应该完成Visit的方法卸载配置文件里，这样可以大大减少经典访问者模式
    /*
     * <configSetions>
     *      <section name="marvellousWorks.practicalPattern.visitorPatten" type="System.Configuration.NameValueSectionHandler"/>
     * </configSections> 
     * <marvellousWorks.practicalPattern.visitorPatten>
     *      <add key="Employee" value="VisitCommonEmployee"/>
     *      <add key="Manager" value="VisitDepartmentManager"/>
     * </marvellousWorks.practicalPattern.visitorPatten>
     * 
     * 
     * System.Configuration.NameValueSectionHandler是系统默认的Section操作类
     */

    //Page 430 467


    //***************************************
    //25.4用委托是引用关系更加松散
    //用委托把上面IVisitor中对IEmployee的匹配过程抽象出来，，，或者采用委托或事件机制注入具体的访问操作



    //***************************************
    //小结
    /*
     * 访问者模式是最复杂的一个模式，访问者模式被限制在一个相对特定的类型体系环境下，实现上非常有技巧性，项目实施中我们应该考虑这种复杂性是否有必要：
     * 
     * 1，双因素依赖倒置调试困难
     * 2，相对定位更加困难。
     * 3，打破面向对象的封装性。
     * 4，执行效率问题。
     * 5，通过反射+配置文件的方式可以解决因为类型结构扩大带来访问者的脆弱性。
     * 
     * 项目中是否使用访问者模式，还需要多权衡。
     * 
     * 
     */
}
