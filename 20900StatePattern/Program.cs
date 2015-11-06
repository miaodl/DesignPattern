using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20900StatePattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //第二十三章 状态模式 (不太明白，需要再看)

    //Allow an object to alter its behavior when its internal state changes.The object appear to change its

    //23.2经典回顾
    /*
     * 状态模式的意图就是引入独立的状态管理类型，由后者负责在对象类型状态变化的时候相应的改变对象的行为，这样从外部看上去对象的执行逻辑就好像被修改了一样。
     * 
     * 什么时候需要用到状态模式呢？除了上面提到的，还有如下情景：
     * 1，对象的行为决定于对象当前的状态，而且实际的执行欧过程只能在运行状态决定
     * 2，业务对象或处理逻辑中包含太多的分支，而且这些枝杈可能会随着业务逻辑的深入愈发复杂
     * 
     * 随着工作流的流行，我们发现很多时候需要用状态决定数据和业务管辖关系的转换，这时候状态模式是很好的选择，只不过需要对整个编排的流程增加一个前面提到的解释器，然后按照依赖倒置的原则在执行流程中某个具体操作点的时候，调用当前状态对象的处理过程。
     * 
     * 
     *    interface                                                        
     *    IContext                                 interface                     
     *    +state         <>------------------------ IState                                      
     *    +Request():void                         +Handle():void                             
     *         |                                        /|\         
     *         |                                         |        
     *         |                                         |        
     *         |                                    ConcreteState             
     *     State Handle                                                     
     *    
     * 1,IContext：是客户程序实际会使用的一个逻辑堆对象，它的执行情况会随着注入的状态（IState的某个实体类型）不同而改变
     * 
     * 2，IState封装了特性状态与IContext特性行为间的内容，它往往与一组IState对象交互，代表其不同的运行情况
     * 
     * 3，对于采用工作流的系统而言，每个IState有一组EndPoint作为入口和出口（不过这时候一般被称为IActivity），用来说明当前状态之后的转换情况（比如正常退出，发现某类异常，异常退出等）
     */

    public interface IState
    {
        void Open();
        void Close();
        void Query();
    }

    public abstract class ContextBase
    {
        private IState state;

        public IState State
        {
            get { return state; }
            set { state = value; }
        }

        public virtual void Open()
        {
            state.Open();
        }
        public virtual void Close()
        {
            state.Close();
        }
        public virtual void Query()
        {
            state.Query();
        }

    }

    //具体实现类

    class OpenState:IState
    {
        public void Open()
        {
            throw new NotSupportedException();
        }
        public void Close()
        {

        }
        public void Query()
        {

        }
    }
    class CloseState : IState
    {
        public void Open()
        {
            
        }
        public void Close()
        {
            throw new NotSupportedException();
        }
        public void Query()
        {
            throw new NotSupportedException();
        }
    }


    class Connection:ContextBase
    {

    }


    class TestClass
    {
        public void TestMethod()
        {
            Connection conn = new Connection();
            conn.State = new OpenState();
            try
            {
                conn.Open();

            }
            catch
            {

            }
        }
    }


}
