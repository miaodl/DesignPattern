using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _40100MVCFrameworkPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Client cl = new Client();
            cl.TestMethod();

            ClientPositive clp = new ClientPositive();
            clp.TestMethod();

        }
    }

    //第二十八章 MVC模式



    //*******************************************************************
    //被动方式MVC
    //视图数据接口
    public interface IViewData
    {
        IList<IModel> ModelList { get; set; }
    }

    //实际数据类
    public class ViewData:IViewData
    {
        public ViewData(params IModel[] m)
        {
            modelList = new List<IModel>(m);
        }
        private IList<IModel> modelList;

        public virtual IList<IModel> ModelList
        {
            get { return modelList; }
            set { modelList = value; }
        }

    }


    //视图的功能接口
    public interface IView
    {
        void Display(IViewData data);
    }

    //具体视图类基类
    public class ViewBase:IView
    {

        public virtual void Display(IViewData data)
        {
            foreach(IModel model in data.ModelList)
            {
                Console.WriteLine(model.GetStringData());
            }
        }
    }


    //数据模型接口
    public interface IModel
    {
        string ModelName{get;set;}

        string GetStringData();
    }


    //具体数据模型类
    public class Model:IModel
    {
        public Model(string name)
        {
            this.modelName = name;
        }

        public Model(string name,params object[] obj)
        {
            this.modelName = name;
            modelDataList = new List<object>(obj);
        }

        private string modelName;

        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }

        private IList<object> modelDataList;

        public string GetStringData()
        {
            if (modelDataList != null && modelDataList.Count > 0)
            {
                string result=string.Join("---",Array.ConvertAll<object,string>(modelDataList.ToArray<object>(),delegate(object n){return Convert.ToString(n);}));
                return modelName + result;
            }
            return modelName;
        }

    }

    //控制器方法接口
    public interface IControl
    {
       IList<IModel> ModelList { get; set; }

        void Process();
    }

    //具体控制器
    public class DataController:IControl
    {

        public DataController()
        {
            viewList = new List<IView>();
            modelList = new List<IModel>();
        }

        private IList<IModel> modelList;

        public IList<IModel> ModelList
        {
            get { return modelList; }
            set { modelList = value; }
        }

        public void AddModelToList(IModel mo)
        {
            if (modelList != null && mo != null)
                modelList.Add(mo);
        }
        public void RemoveModelFromList(IModel mo)
        {
            if (modelList != null && mo != null)
                if (modelList.Contains(mo))
                    modelList.Remove(mo);
        }

        private IList<IView> viewList;

        public void AddViewToList(IView vi)
        {
            if (vi != null)
                viewList.Add(vi);
        }
        public void RemoveViewFromList(IView vi)
        {
            if (viewList.Count > 0 && vi != null)
                if (viewList.Contains(vi))
                    viewList.Remove(vi);
        }


        public void Process()
        {
            if(modelList.Count==0) return;
            if (viewList.Count == 0) return;

            IViewData vdata=new ViewData(modelList.ToArray<IModel>());

            foreach(IView view in viewList)
            {
                view.Display(vdata);
            }
        }
    }


    public class FirstView:ViewBase
    {

    }

    public class SecondView:ViewBase
    {
        public override void Display(IViewData data)
        {
            Console.WriteLine("this is second view");
            base.Display(data);
        }
    }



    //测试用的客户端类
    public class Client
    {
        public void TestMethod()
        {
            Console.WriteLine("******************Negative MVC*******************");
            Model mm1 = new Model("miaodl");
            Model mm2 = new Model("miao","nishiyigeguawazi","woyeshi zheyang jue de de");

            DataController control = new DataController();
            control.AddModelToList(mm1);
            control.AddModelToList(mm2);

            control.AddViewToList(new FirstView());
            control.AddViewToList(new SecondView());

            //被动方式MVC，由controller修改了model后，通知View显示
            control.Process();

            Console.ReadLine();
        }
    }



    //*****************************************************************
    //主动方式MVC
    
    //模型参数
    public class ModelEventArgs:EventArgs
    {
        private string content;
        public string Context
        {
            get
            {
                return this.content;
            }
        }
        public ModelEventArgs(int[] data)
        {
            content=string.Join(",",Array.ConvertAll<int,string>(
                data,delegate(int n)
                {
                    return Convert.ToString(n);
                }
            ));
        }
    }


    //抽象的Model
    public interface IModelPositive
    {
        //提供给View借助时间方式预定Model变化的入口
        event EventHandler<ModelEventArgs> DataChanged;
        //Model对信息自身操作的封装
        int this[int index]
        {
            get;
            set;
        }
    }


    public interface IViewPositive
    {
        //用于接受Model信息变更的预定，一个委托
        EventHandler<ModelEventArgs> Handler
        {
            get;
        }


        void Print(string data);
    }

    public class PositiveController
    {
        private IModelPositive model;

        public virtual IModelPositive Model
        {
            get { return model; }
            set { model = value; }
        }
        
        public static PositiveController operator +(PositiveController control,IViewPositive view)
        {
            if (view == null)
                throw new ArgumentNullException();
            //把view中的响应委托(一系列列表函数)通过委托加减方式注入到model中
            control.model.DataChanged += view.Handler;
            return control;
        }
        public static PositiveController operator -(PositiveController control, IViewPositive view)
        {
            if (view == null)
                throw new ArgumentNullException();
            control.model.DataChanged -= view.Handler;
            return control;
        }
        
    }


    class ModelPositive:IModelPositive
    {
        public event EventHandler<ModelEventArgs> DataChanged;

        private int[] data;
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                this.data[index] = value;
                if(DataChanged!=null)
                {
                    DataChanged(this, new ModelEventArgs(data));
                }
            }
        }

        public ModelPositive()
        {
            Random rnd = new Random();
            data = new int[10];
            for(int i=0;i<data.Length;i++)
                data[i]=rnd.Next();
        }
    }

    abstract class ViewBasePositive:IViewPositive
    {
        //这是一个委托
        protected  EventHandler<ModelEventArgs> handler;

        public abstract void Print(string data);

        //View当获得Model数据便计划后，重新打印最新的数据
        public virtual void OnDataChanged(object sender,ModelEventArgs args)
        {
            Print(args.Context);
        }

        public virtual void OnDataChanged1111(object sender, ModelEventArgs args)
        {
            Console.WriteLine("这是在基类的事件handler中注册的其他方法，在基类构造的时候注入");
        }

        //这个委托通过Controller传给Model后，当Model数据变化，相当于Model直接代替View，触发了这个委托中的所有方法
        public EventHandler<ModelEventArgs> Handler
        {
            get
            {
                return this.handler;
            }
        }
        public ViewBasePositive()
        {
            Console.WriteLine("调用父类无参构造函数");
            handler += OnDataChanged;
            handler += OnDataChanged1111;
        }

    }

    class TraceView:ViewBasePositive
    {
        //子类构造时会默认调用父类的无参构造函数
        public TraceView()
        {
            handler += OnDataChanged1111;
            handler += OnDataChanged2222;
        }

        public virtual void OnDataChanged2222(object sender, ModelEventArgs args)
        {
            Console.WriteLine("11111111111111111");
        }

        public override void Print(string data)
        {
            Console.WriteLine(data);
            Trace.WriteLine(data);
        }
    }

    class EventLogView:ViewBasePositive
    {
        public override void Print(string data)
        {
            Console.WriteLine("Demo{0}",data);
            //EventLog.WriteEntry("Demo", data);
        }
    }


    public class ClientPositive
    {
        public void TestMethod()
        {
            PositiveController control = new PositiveController();
            IModelPositive model = new ModelPositive();
            control.Model = model;

            //把view中的响应事件(一系列列表函数)通过委托加减方式注入到model中，model变化时，执行view的方法
            control += new TraceView();
            control += new EventLogView();

            Console.WriteLine("******************Positive MVC*******************");
            Console.WriteLine("please input Positive MVC model data");

            int n=20;

            while(n>0)
            {
                n--;
                string myinput = Console.ReadLine();
                int index = Convert.ToInt32(myinput)%10;

                myinput = Console.ReadLine();
                int num = Convert.ToInt32(myinput);

                model[index] = num;
            }

            Console.ReadLine();
        }
    }


    /*
     * .NET委托事件的方式实现主动方式MVC的优势有下：
     * 
     * 1，结合更加松散耦合，M/V之间没有直接的依赖关系，组装过程可以由C完成，M/V之间的观察者仅有.NET标准事件和委托进行交互
     * 2，不用设计独立的观察者对象。
     * 3，由于C不需要参与M数据变更后实际的交互过程，因此C也无需设计用来保存V的容器。
     * 4，如果EventArgs设计合理的话，可以更自由地与其他产品或第三方对象体系进行集成
     * 
     * 
     */



}
