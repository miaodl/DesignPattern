using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20100ChainOfResponsibility
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }


    //第四篇 行为型模式 算法 控制流和同性关系的对象化处理

    //第十五章 职责链模式

    //Avoid coupling the sender of a request to its receiver by giving more than one object a chance to handle the request.Chain the receivint objects and pass the request along the chain until an object handles it

    //职责链模式，用于对目标对象施与一系列操作的情况，为了避免调用双方和操作之间的耦合关系，介意把这些操作组成一个链表，通过遍历链表找到合适处理的操作。

    /*
     * 什么时候可以考虑启用这种链式处理
     * 
     * 1，输入对象需要经过一系列处理，而每个处理环节也只针对这个对象进行修改，但产出的是同一个对象。
     * 
     * 2，对象本身要经过哪些处理需要在运行态动态决定，决定的因素可能取决于对象当前的某些属性和外部策略，但为了把输入方和输出方从每个具体的处理环节的耦合关系中解脱出来，可以考虑把他们做成一个链条，按照每个节点的后继依次遍历，酌情处理。
     * 
     * 3，需要多个操作发送处理请求，以链表形式组织它们
     * 
     * 4，根据链表的动态特性,在对象处理经常发生动态变化的情况下，借助链表动态维护处理对象。
     * 
     * 
     * 
     * 
     * 
     * Client----------------------->interface<-----------------|
     *                               IHandler                   |
     *                                                          |Successor
     *                              +Successor:IHandler---------|
     *                              
     *                              +HandleRequest:void
     *                                       《》                      
     *                                        |                      
     *                                        |                      
     *                                        |                      
     *                                  CreateHandler                            
     *                                 +HandleRequest：void                             
     *                                                              
     *                                                              
     */

    //C#抽象的Request对象
    public enum PurchaseType
    {
        Internal,//内部认购价格
        Discount,//折扣
        Regular,//平价
        Mail//邮购价
    }

    public class Request
    {
        private double price;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        private PurchaseType type;

        public PurchaseType Type
        {
            get { return type; }
            set { type = value; }
        }
        public Request(double price ,PurchaseType type)
        {
            this.price = price;
            this.type = type;
        }
    }

    //抽象的处理对象
    public interface IHandler
    {
        void HandleRequest(Request request);
        //后继结点
        IHandler Successor
        {
            get;
            set;
        }
        //当前Handler处理的请求
        PurchaseType Type
        {
            get;
            set;
        }
    }

    //为了简化具体操作对象实现而增加的抽象处理类型
    public abstract class HandlerBase:IHandler
    {
        private IHandler successor;

        protected IHandler Successor
        {
            get { return successor; }
            set { successor = value; }
        }
        private PurchaseType type;

        protected PurchaseType Type
        {
            get { return type; }
            set { type = value; }
        }

        public HandlerBase(PurchaseType type,IHandler successor)
        {
            this.type = type;
            this.successor = successor;
        }

        public HandlerBase(PurchaseType type):this(type,null)
        {

        }

        //需要具体IHandler类型处理的内容
        public abstract void Process(Request request);

        //按照链式方式依次把调用继续下去
        public virtual void HandleRequest(Request request)
        {
            if (request == null) return;
            if (request.Type == Type)
                Process(request);
            else
                if (Successor != null)
                    successor.HandleRequest(request);

        }


    }


    //具体操作对象
    public class InternalHandler:HandlerBase
    {
        public InternalHandler():base(PurchaseType.Internal)
        {

        }
        public override void Process(Request request)
        {
            request.Price *= 0.6;
        }
    }

    public class MailHandler:HandlerBase
    {
        public MailHandler():base(PurchaseType.Mail)
        {

        }
        public override void Process(Request request)
        {
            request.Price *= 1.3;
        }
    }

    public class DiscountHandler:HandlerBase
    {
        public DiscountHandler():base(PurchaseType.Discount)
        {

        }
        public override void Process(Request request)
        {
            request.Price *= 0.9;
        }
    }
    public class RegularHandler : HandlerBase
    {
        public RegularHandler()
            : base(PurchaseType.Regular)
        {

        }
        public override void Process(Request request)
        {
           
        }
    }


    public class TestClass
    {
        public void TestMethod()
        {
            IHandler handler1 = new InternalHandler();
            IHandler handler2 = new DiscountHandler();
            IHandler handler3 = new MailHandler();
            IHandler handler4 = new RegularHandler();

            //组合链式结构
            //internal->mail->discount->retular->null
            handler1.Successor = handler3;
            handler3.Successor = handler2;
            handler2.Successor = handler4;
            IHandler head = handler1;

            Request request = new Request(20, PurchaseType.Mail);
            head.HandleRequest(request);

            Console.ReadLine();



        }
    }




}
