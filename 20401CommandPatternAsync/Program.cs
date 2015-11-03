using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20401CommandPatternAsync
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

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



    //在经典模式的基础上定义一个具有异步事件响应能力的命令模式系统

    public interface ICommand
    {
        void Execute();
    }

    //扩展后具有异步调用的Command对象
    public interface IAsyncCommand:ICommand
    {
        //用于异步调用事件
        event AsyncCallback AsyncCompleted;
        //用于同步调用事件
        event EventHandler Completed;
        //用于异步调用的Command对象方法
        void AsyncExecute();
    }


    public abstract class CommandBase:IAsyncCommand
    {
        public event AsyncCallback AsyncCompleted;
        public event EventHandler Completed;
        protected bool isAsync = false;
        public virtual void Execute()
        {
            if((Completed!=null)&&(!isAsync))
            {
                Completed(this, EventArgs.Empty);
            }
        }

        public virtual void AsyncExecute()
        {
            if((AsyncCompleted!=null)&&(Completed!=null))
            {
                isAsync = true;
                Completed.BeginInvoke(this, EventArgs.Empty, AsyncCompleted, null);
            }
            Execute();
            isAsync = false;
        }
    }

    //测试用的实体命令类
    class DemoCommand:CommandBase
    {
        public DemoCommand()
        {
            Completed += this.OnCompleted;
            AsyncCompleted += new AsyncCallback(this.OnAsyncCompleted);
        }
        public void OnCompleted(object sender,EventArgs args)
        {
            Log.Add("OnCompleted");
        }
        public void OnAsyncCompleted(IAsyncResult result)
        {
            Log.Add("OnAsyncCompleted");
        }
        public List<string> Log = new List<string>();
    }

    //。。。。。。。。。。。。。。。。。。。。。
    //没看完


    //18.5把Command打包

    //18.5.1外观模式方式


    //18.5.2组合模式方式

    //18.6把Command排队queue

}
