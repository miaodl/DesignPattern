using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20400CommandPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第十八章 命令模式
    //Encapsulate a request as an object,thereby letting you parameterize clients with different request,queue or log requests,and support undoable operations

    //命令模式是对一类对象公共操作的抽象，他们具有相同的方法签名，所以具有类操作，可以被抽象处来，称为一个抽象的“命令”对象。这样实际操作的调用者就不是和一组对象打交道，它只需要依赖这个命令对象的方法签名，并依据这个操作签名调用相关的方法。


    /*
     * 什么时候用到命令模式？
     * 
     * 1，调用者同时与多个执行对象交互，而且每个操作可以抽象为近似的形式。
     * 
     * 2，我们需要控制调用本身的生命期，而不是调用者直截了当地进行一个调用，有可能根据需要合并，分配，疏导相关的调用
     * 
     * 3，一系列类似的调用可能需要辅以Redo（）或Undo（）之类的特性
     * 
     * 4，类似以往函数指针，我们需要在执行一个调用的同时告诉它需要回调那些操作。
     * 
     * 5，方法本身太过复杂，从整个项目重用的角度考虑，需要把方法的实现抽象为一组可以协作的对象
     * 
     */


    /*                                                            
     *         |———————————————————————————|                             
     *         |                                                      | 
     *         |                                                     \|/  
     *      Client--------------->Receiver    <---------------- ConcreteCommand                   
     *                           +Action():void                 -state:object           
     *                                                         +Receiver:Receiver
     *                                                        / +Execute():void
     *                                                       /          |
     *                                                      /           | 
     *                                               -------         receiver.Action()
     *                                              /         
     *      Invoker<>---------------interface      /                                           
     *                              ICommand      /          
     *                              +Execute():void                 
     *                                                       
     * Invoker:请求操作的对象    
     * Receiver:接受请求并执行某操作的对象
     * ICommand：让Invoker对Receiver一无所知。Receiver也不知道Invoker
     */

    //实际动作的执行者
    public class Receiver
    {
        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }
        }
        private string address = string.Empty;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        //Action
        public void SetName()
        {
            name="name";
        }
        //Action
        public void SetAddress()
        {
            address="address";
        }


        //抽象命令对象
        public interface ICommand
        {
            //提供给调用者的统一操作方法
            void Execute();
            Receiver Receiver
            {
                set;
            }
        }

        public abstract class CommandBase:ICommand
        {
            protected Receiver receiver;

            public Receiver Receiver
            {
                set { receiver = value; }
            }
            public abstract void Execute();

        }


        //具体命令对象
        public class SetNameCommand:CommandBase
        {
            public override void Execute()
            {
                receiver.SetName();
            }
        }

        public class SetAddressCommand:CommandBase
        {
            public override void Execute()
            {
                receiver.SetAddress();
            }
        }

        //调用者
        public class Invoker
        {
            private IList<ICommand> commands = new List<ICommand>();
            public void AddCommand(ICommand command)
            {
                commands.Add(command);
            }

            //经过调用者组织后，供客户程序操作命令对象的方法
            public void Run()
            {
                foreach (ICommand command in commands)
                    command.Execute();
            }
        }


        public class TestMethod
        {
            public void Test()
            {
                //构造Receiver对象
                Receiver receriver = new Receiver();

                //组织Command对象
                ICommand command1 = new SetNameCommand();
                ICommand command2 = new SetAddressCommand();
                command1.Receiver = receriver;
                command2.Receiver = receriver;

                Invoker invoker = new Invoker();
                invoker.AddCommand(command1);
                invoker.AddCommand(command2);

                invoker.Run();

                Console.ReadLine();

            }
        }

        //18.3轻量级的Command--委托

        public delegate void VoidHandler();

        public class Receiver1
        {
            public void A()
            {

            }
        }
        public class Receiver2
        {
            public void B()
            {

            }
        }
        public class Receiver3
        {
            public void C()
            {

            }
        }


        public class InvokerDelegate
        {
            IList<VoidHandler> handlers = new List<VoidHandler>();

            public void AddHandler(VoidHandler handler)
            {
                handlers.Add(handler);
                
            }

            public void Run()
            {
                foreach(VoidHandler handler in handlers)
                {
                    handler();
                }

            }
        }

        //虽然委托被很多人认为是对面向对象概念的破坏，但从IL看，.NET还是把它编译成一个类，从这个意义上来说委托是表象上的破坏，从工程角度看，委托可以简化很多二次封装的代码工作，使用得当的话代码会变得非常灵巧。



        //18.4异步Command

        /*
         *  用到异步命令的对象
         *  
         * 1，操作时间过长，如果同步方式，客户程序延迟过久，用户体验不好
         * 2，操纵需要进行协议转换或操作的协议本身就不是持续连接的
         * 3，前后台系统并发能力不同
         * 4，需要的触发一次调用，但其执行结果同时通知多个预定对象
         * 5，需要协调高速，低速进程或设备
         * 6，不要求可靠提交的，类似UDP协议相对于TCP协议
         */
        //page 339 376

    }


}
