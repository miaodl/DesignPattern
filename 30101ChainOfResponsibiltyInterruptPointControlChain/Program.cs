using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20101ChainOfResponsibiltyInterruptPointControlChain
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //15.3用断点控制链式过程  Page 297 334

    /*           Successor                                                           
     *          |--------|                                                   
     *          |        |                                                    
     *         \|/       |                                                   
     *          interface                                                             
     *          IHandler                                                 EventArgs            
     *    event:                                                         CallHandlerEventArgs    
     *         +Break EvenHandler<CallHandlerEventArgs>-----------------+Handler:IHandler
     *                                                                  +Request:Request             
     *    +HasBreakPoint:bool                                                                   
     *    +Successor:IHandler                                                                   
     *                                                                       
     *    +HandlerRequest(IHandler):void                                                                   
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
        public Request(double price, PurchaseType type)
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

        bool HasBreakPoint
        {
            get;
            set;
        }
        event EventHandler<CallHandlerEventArgs> Break;
    }
    
    public class CallHandlerEventArgs:EventArgs
    {
        private IHandler handler;

        public IHandler Handler
        {
            get { return handler; }
            set { handler = value; }
        }
        private Request request;

        public Request Request
        {
            get { return request; }
            set { request = value; }
        }
       
        public CallHandlerEventArgs(IHandler handler,Request request)
        {
            this.handler = handler;
            this.request = request;
        }
    }




    //为了简化具体操作对象实现而增加的抽象处理类型
    public abstract class HandlerBase : IHandler
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

        private bool hasBreakPoint;

        protected bool HasBreakPoint
        {
            get { return hasBreakPoint; }
            set { hasBreakPoint = value; }
        }
        public HandlerBase(PurchaseType type, IHandler successor)
        {
            this.type = type;
            this.successor = successor;
        }

        public HandlerBase(PurchaseType type)
            : this(type, null)
        {

        }

        //需要具体IHandler类型处理的内容
        public abstract void Process(Request request);

        //按照链式方式依次把调用继续下去
        public virtual void HandleRequest(Request request)
        {

            if (HasBreakPoint && Break != null)
                Break(this, new CallHandlerEventArgs(this, request));

            if (request == null) return;
            if (request.Type == Type)
                Process(request);
            else
                if (Successor != null)
                    successor.HandleRequest(request);

        }

        public event EventHandler<CallHandlerEventArgs> Break;
        
        //可以定义在外部的事件函数
        public virtual void OnBreak(CallHandlerEventArgs args)
        {
            if (Break != null)
                Break(this, args);
        }

    }


    //具体操作对象
    public class InternalHandler : HandlerBase
    {
        public InternalHandler()
            : base(PurchaseType.Internal)
        {

        }
        public override void Process(Request request)
        {
            request.Price *= 0.6;
        }
    }

    public class MailHandler : HandlerBase
    {
        public MailHandler()
            : base(PurchaseType.Mail)
        {

        }
        public override void Process(Request request)
        {
            request.Price *= 1.3;
        }
    }

    public class DiscountHandler : HandlerBase
    {
        public DiscountHandler()
            : base(PurchaseType.Discount)
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
        private PurchaseType currentType;
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
            //IHandler head = handler1;

            //Request request = new Request(20, PurchaseType.Mail);
            //head.HandleRequest(request);

            handler1.HasBreakPoint = true;
            handler1.Break += this._Break;
            handler3.HasBreakPoint = true;
            handler3.Break += this._Break;


            Console.ReadLine();



        }

        void _Break(object sender, CallHandlerEventArgs e)
        {
            IHandler handler = e.Handler;

            //为第二个调用做修改

            currentType = PurchaseType.Mail;
        }


    }



    //15.4链式反应 page 298 335

    //上面的链式操作几乎是单纯的一根筋到底的调用，但在项目中我们会遇到需要处理分支的情况，即便是项目刚开始只有一根链，随着业务逻辑和操作规则的复杂化，这种分支情况也会越来越多，另一方面，随着多芯多核技术的应用，如何让这些含有分支的链式调用尽快完成，也需要专门的设计。


    //对于并行处理系统而言一方面需要增加后继结点的数量，另一方面也可以通过并行触发每个后继结点的HandleRequest()方法实现更高效的多分支COR处理，下面是一个非并行接口版本的多分支链式调用

    public interface IHandlerParallel
    {
        //处理客户程序请求
        void HandleRequest(Request request);
        //后续结点
        IList<IHandler> Successors { get; }
        //增加新的后继分支结点
        void AddSuccessor(IHandler successor);
        //当前Handler
        PurchaseType Type
        {
            get;
            set;
        }
    }

    //HanderBase的修改

    //按照链式方式一次把调用继续下去
    public abstract class HandlerBaseParallel : IHandlerParallel
    {
        private IList<IHandler> successors;
        private PurchaseType type;


        public void HandleRequest(Request request)
        {
            if (request == null) return;
            if (request.Type == Type)           
            {
                //process(Request);
            }
            else
            {
                if (Successors.Count > 0)
                    foreach (IHandler successor in Successors)
                        successor.HandleRequest(request);
            }
                
        }

        //当前分支的后继结点
        public virtual IList<IHandler> Successors
        {
            get
            {
                return successors;
            }
        }

        public void AddSuccessor(IHandler successor)
        {
            if (successor == null)
                throw new ArgumentNullException("successor");
            Successors.Add(successor);
        }

        public PurchaseType Type
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //如果后继结点本身结构更加复杂，我们可以借助前面介绍的组合模式，把复杂的后继结构组织起来
    }




    
}
