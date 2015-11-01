using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20102ChainOfResponsibilityConfigurationRestricted
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    //15.5增加配置约束

    /*
     * 设计方法：
     * 1，按照模块复用的考虑，我们为CoR增加一个独立的配置节
     * 2，逻辑上把一组IHandler的操作定义为一个Channel，并作为CoR下面的一个配直节
     * 3，为每个IHandler定义一个相应的配置元素
     * 4，客户程序通过一个名为Runtime的类访问基于配置的CoR，客户程序所以来的是Runtime类提供的Head属性，而不是Channel中每个具体的IHandler
     * 
     * 
     * Client---------->untility----------->System.Configuration
     *                  Runtime
     *                  +Hed:IHandler
     * 
     */

    /*******************************************************************************
     * App.Config
     * <?xml version="1.0" encoding="utf-8">
     * <configuration>
     *      <configSections>
     *          <sectionGroup name="12345.12345" type="...">
     *              <section name="channel" type="...">
     *      </configSecitons>
     * </configuration>
     * <12345.12345>
     *      <channel>
     *          <handlers>
     *              <add type="...">
     *          </handlers>
     *      </channel>
     * </12345.12345>
     * 
     */

     //相应的OCM（Object-Configuration Mapping)类型
     //定义每个IHandler的配置单元

    public interface IHandler
    {

    }
    class HandlerConfigurationElement:ConfigurationElement
    {
        [ConfigurationProperty("type",IsRequired=true,IsKey=true)]
        public string Type
        {
            get
            {
                return base["type"] as string;
            }
        }

        public IHandler CreateInstance()
        {
            return (IHandler)(Activator.CreateInstance(System.Type.GetType(Type)));
        }
    }
    //配置元素集合
    [ConfigurationCollection(typeof(HandlerConfigurationElement),CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class HandlerConfigurationElementCollection:ConfigurationElementCollection
    {
        public const string Name = "handlers";

        //定义配置元素的集合操作
        public HandlerConfigurationElement this[int index]
        {
            get
            {
                return (HandlerConfigurationElement)base.BaseGet(index);
            }
        }

        public new HandlerConfigurationElement this[string name]
        {
            get
            {
                return base.BaseGet(name) as HandlerConfigurationElement;
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new HandlerConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as HandlerConfigurationElement).Type;
        }
    }


    //配直节
    class ChannelConfigurationSection:ConfigurationSection
    {
        public const string Name = "channel";
        [ConfigurationProperty(HandlerConfigurationElementCollection.Name,IsRequired=true)]
        public HandlerConfigurationElementCollection handlers
        {
            get
            {
                return this[HandlerConfigurationElementCollection.Name] as HandlerConfigurationElementCollection;
            }
        }
    }


    //配直节组
    class CoRConfigurationSectionGroup:ConfigurationSectionGroup
    {
        public const string Name = "12345.12345";
        public CoRConfigurationSectionGroup():base()
        {

        }
        [ConfigurationProperty(ChannelConfigurationSection.Name,IsRequired=true)]
        public ChannelConfigurationSection Channel
        {
            get
            {
                return Sections[ChannelConfigurationSection.Name] as ChannelConfigurationSection;
            }
        }
    }



    //C#服务于客户程序的Runtime类型
    //职责链环境的运行类
    public class Runtime
    {
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        /// <summary>
        /// 通过访问配置文件编排后的各个处理类
        /// 1，通过配置文件的层次关系获得对应的<handlers>节点
        /// 2，解析并组装职责链
        /// </summary>
        public IHandler Head
        {
            get
            {
                CoRConfigurationSectionGroup group = config.GetSectionGroup(CoRConfigurationSectionGroup.Name) as CoRConfigurationSectionGroup;
                HandlerConfigurationElementCollection coll = group.Channel.handlers;


                if (coll.Count == 0) return null;
                if (coll.Count == 1) return coll[0].CreateInstance();//头结点

                IHandler head = coll[0].CreateInstance();//头结点
                //关于链表的操作
                IHandler current = head;
                for(int i=1;i<coll.Count;i++)
                {
                    IHandler handler = coll[i].CreateInstance();
                    current.Successor = handler;
                    current = handler;
                }
            }
        }
    }

    public class TestClass
    {
        
        public void TestMethod()
        {
            Runtime runtime = new Runtime();
            IHandler head = runtime.Head;
            Request request = new Request(20, PurchaseType.Discount);
            head.HandleRequest(request);



        }
    }

    /*
     * 其实，在真正的项目环境中，为了保证应用的开放性，我们往往无法知道到底有哪些IHandler类型被编排到CoR中，而且随着应用的修改，每个IHandler可能都会独立变化，但他们的执行模型相对稳定，因此通过一个配置文件可以比较好地找到开发过程与运维过程的平衡。
     * 
     * 另外，为了便于客户程序使用，可以使用一个外围的Runtime类型封装CoR的组装过程，而客户程序只通过head属相来访问。
     * 
     * 这也为后续的扩展提供了便利，预料到CoR会运行在不同的环境下，那么完全可以考虑将Runtime类型也配置花，这样可以更具不同的运行环境，加载相应的IHandler Channel，在此有一个例子，即NET对资源文件的使用，它的每个IHandler都是具体某个区域语言资源文件中的各个资源项。
     * 
     * 
     * 15.6小结
     * 
     * CoR给了我们设计类似童泰系统一个常见的处理措施。它的是指同样可通过一个触点王城客户程序与多个操作间的耦合关系，并且借助链表的动态性能力，也为编排这个触点后的一组操作提供可能。
     */
}
