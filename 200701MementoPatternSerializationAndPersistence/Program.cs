using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalPattern.CommonTools;


namespace _20701MementoPatternSerializationAndPersistence
{
    class Program
    {
        static void Main(string[] args)
        {
         
        }
    }



    //21.4Memento的序列化和持久化

    /*
     * 项目中备忘操作往往会涉及有关对象的持久化操作：
     * 1，应用采用集群技术，每次操作可能位于不同的服务器上，为了让前后两次调用可以共享对象的备忘信息，需要把信息暂时保存在数据库中
     * 
     * 2，对于Web调用而言，由于简单HTTP操作本身并没有状态，为此需要把备忘信息历史保存在Session中。
     * 3，对于很多桌面应用而言，用户操作信息可能就是本地的一个文档文件，为此可以考虑把备份信息以本地临时文件的方式保存到I/O上
     * 
     * 4，另一方面，考虑对象内部结构的复杂性及分布式调用的性能问题，还需要把对象序列化，调用时一次性把序列化后的复杂对象传递到操作方，待一系列操作结束后再序列化回来，并把修改后的结果在服务端进行一次性地提交
     */


    //**************************************************************
    //为了便于定义抽象状态类型所定义的接口
    public interface IState
    {

    }
    /// <summary>
    ///  定义保存备忘信息的持久化对象接口，由于持久对象往往会服务于多个原发器对象，因此
    ///  为了区分不同实例的备忘内容，保存的时候需要提供：
    ///  1，原发器对象标识
    ///  2，备忘信息版本
    ///  3，装填信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersistenceStore<T>where T:IState
    {
        void Store(string originatorID, int version, T target);
        T Find(string originatorID, int version);
    }

    /// <summary>
    /// 包括内部备忘录类型的原发器抽象定义
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OriginatorBase<T>where T:IState
    {
        //原发其中对象的状态
        protected T state;

        //用于标识原发器对象的标识
        protected string key;

        public OriginatorBase()
        {
            key = new Guid().ToString();
        }

        //待注入的持久机制
        protected IPersistenceStore<T> store;

        //把状态保存到备忘录
        public virtual void SaveCheckPoint(int version)
        {
            store.Store(key, version, state);
        }
        //从备忘录恢复之前的状态
        public virtual void Undo(int version)
        {
            state = store.Find(key, version);
        }
    }



    //模拟的持久对象
    //一个用于模拟的具体持久对象
    public class MementoryPersistenceStore<T>:IPersistenceStore<T>where T:IState
    {
        //模拟一个所有原发器类型共享的持久机制
        private static IDictionary<KeyValuePair<string, int>, string> store = new Dictionary<KeyValuePair<string, int>, string>();

        public void Store(string originatorID,int version,T target)
        {
            if ((target == null))
                throw new ArgumentException("target");
            KeyValuePair<string, int> key = new KeyValuePair<string, int>(originatorID, version);
            string value = SerializationHelper.SerializeObjectToString(target);
            if(store.ContainsKey(key))
                store[key]=value;//更新一个既有版本的备忘内容
            else
                store.Add(key,value);//增加一个新版本的备忘内容
        }
        //从持久化对象中获得备忘信息
        public T Find(string originatorID,int version)
        {
            KeyValuePair<string, int> key = new KeyValuePair<string, int>();
            string value;
            if (!store.TryGetValue(key, out value))
                throw new NullReferenceException();
            return SerializationHelper.DeserializeStringToObject<T>(value);
        }

    }


    //为了适应序列化和持久化修改的具体类型
    [Serializable]
    public struct Position
    {
        public int X;
        public int Y;
    }






    //*****************************************
    //小结
    //经典模式定义中处于职责划分的考虑，抽象出原发器，备忘录和负责人等三个对象，但实际项目中，尤其是随着计算的分布化，需要对状态进行很多约束和额外处理，而且实施中是否有必要定义独立的负责人，备忘录对象往往需要视状态本身的复杂性而定，很多时候一个Stack<T> IDictionay<K,T>就可以满足大多数需求了
    //由于基于Web的应用越来越盛行，应用规模越来越大，在大部分情况下简单HTTP调用可以视为无状态的分布式调用，如果需要传递其中操作对象的属性，有时候需要借助序列化，持久化机制才可以实现分布式调用的备忘录机制。
}
