using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10700ProxyPatten
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    //第14章 代理模式

    //Provide a surrogate or placeholder for anthoer object to control access to it

    //代理模式的意图非常简单——为其他对象提供一种代理以控制对这个对象的访问

    /*
     * 代理模型复杂性的关键主要来自于不同运行环境的复杂性
     * 
     * 远程访问
     * 
     * 数据库访问
     * 
     * 各种透明的安全控制
     * 
     * 负载均衡的设计
     * 
     * 随着Web Service的普及，如何适应整个互联网服务环境下的需要
     * 
     * 诸如面向大计算量的虚拟代理，类似智能指针的智能代理等
     */

    //代理本身的目的就是要把这些复杂性封装起来，让客户程序更容易使用藏在它背后的那些对象。

    //14.2 page274 311

    /*
     * 
     * 代理对象需要具备的特征：
     * 
     * 引入代理对象并不应该增加客户程序的复杂性，按照依赖倒置的原则，客户程序需要知道的也只是目标对象的抽象接口，因此相应的代理对象也就应该实现这个接口，否则等于变相向客户程序引入新的复杂性
     * 
     * 
     * 代理的目的是控制客户程序对目标对象的访问，因此代理必须可以直接或间接地知道目标类型在哪儿，以及如何访问
     * 
     * 代理类不必知道具体的目标类型，很多时候它只要能够按照与客户程序统一的约定，提供一个具有抽象特征的类型即可，至于具体目标类型，完全可以参考前面说的创建型模式来实现。
     * 
     * 
     * ***********************************
     * 代理模式静态结构：
     * 
     * 
     * Client-------------------->interface
     *                            ISubject
     *                            +Requset():void
     *                           /     《》           
     *                   _______/       |            
     *                  /               |            
     *                 /                |            
     *   RealSubject        realSubject |                   
     *   +Request():void <-----------Proxy          ----------realSubject.Request()             
     *                               +Request():void              
     *                                                                                                 
     */

    //定义客户程序需要的抽象
    public interface ISubject
    {
        string Request();
    }


    //具体实现客户程序需要的类型
    public class RealSubject:ISubject
    {
        public string Request()
        {
            return "from real Subject";

        }
        //这里使用Singleton的目的是模拟一个复杂性
        //比如：客户程序并不知道如何使用远端的具体类型
        private static ISubject singleton = new RealSubject();
        private RealSubject()
        {

        }

        public static ISubject Singleton
        {
            get
            {
                return singleton;
            }
        }
    }

    public class Proxy:ISubject
    {
        public string Request()
        {
            return RealSubject.Singleton.Request();
        }
    }
    


    class TestClass
    {
        public void TestMethod()
        {
            ISubject subject = new Proxy();
            //在Subject里面人为增加了一个Singleton访问模式的处理，不过对于客户程序而言，它看不到这个部分，因为复杂性被代理类型封装了
            subject.Request();

            Console.ReadLine();
        }
    }

    /*外观模式和代理模式的区别：
     * 
     * 外观模式和代理模式都是屏蔽复杂性的，只不过外观模式处理的是一个逻辑上的“子系统”，而且封装后的结果并没有具体抽象接口的要求，但在代理模式中客户程序需要的接口明确化了
     * 
     * 
     */



    //14.3远程访问代理 PAGE 277 314


    //14.4数据访问代理

    //14.5对象缓存代理Page 282 320 有关缓存的管理需时要可看 

    //项目中的大多数应用都有一个很有意思的规律，那些宏观的，几乎被所有人关注的信息，往往都是异步的，或者都是相对滞后一点的事件才发布的，因此我们往往把那些相对不太动态的信息保存在缓存里，而对于那些总是变化的数据，还是一次一次地进行访问最好。

    //C#定义具有淘汰机制的缓存的数据访问代理
    //private CacheQueue cache=new CacheQueue();



    //14.6为代理增加预处理和后续处理的支持Page 284 321 重要？？？



}


