using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AppConfigTest
{
    class Program
    {
        static void Main(string[] args)
        {

            AppDomain saa = AppDomain.CreateDomain("aaa123");
            
            string appsetting1 = ConfigurationManager.AppSettings["appSettingKey1"].ToString();
            var appsetting2 = ConfigurationManager.AppSettings["appSettingKey2"];

            Console.WriteLine(appsetting1);
            Console.WriteLine(appsetting2.GetType());



            var settings = (MySettings1)ConfigurationManager.GetSection("MySettings1");

            Console.WriteLine(settings.Key1+settings.Key2);


            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach(var ss in config.Sections)
            {
                Console.WriteLine(ss.GetType());
            }

            foreach(var m in config.SectionGroups)
            {
                if(m.GetType()==typeof(MiaoTestGroup))
                {
                    Console.WriteLine("**************");
                    foreach(var s in (m as MiaoTestGroup).Sections)
                    {
                        Console.WriteLine(s.ToString());
                        
                    }
                }
            }


            Console.ReadLine();
        }


        //[System.Runtime.InteropServices.DllImport("kernel32")]
        //private static extern IntPtr LoadLibrary(string lpLibFileName);

        //[System.Runtime.InteropServices.DllImport("kernel32")]
        //private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        //[System.Runtime.InteropServices.DllImport("kernel32")]
        //private static extern IntPtr FreeLibrary(IntPtr hLibModule);

        
        
    }

    
    public class MySettings1 : ConfigurationSection
    {
        [ConfigurationProperty("key1", IsRequired = true)]
        public string Key1
        {
            get
            {
                return (string)this["key1"];
            }
        }

        [ConfigurationProperty("key2", IsRequired = true)]
        public string Key2
        {
            get
            {
                return (string)this["key2"];
            }
        }
    }


    public class MySettings2 : ConfigurationSection
    {
        [ConfigurationProperty("key1", IsRequired = true)]
        public string Key1
        {
            get
            {
                return (string)this["key1"];
            }
        }

        [ConfigurationProperty("key2", IsRequired = true)]
        public string Key2
        {
            get
            {
                return (string)this["key2"];
            }
        }
    }


    public sealed class MiaoTestGroup:ConfigurationSectionGroup
    {
        
    }

    public sealed class Class1 :ConfigurationSection
    {
        [ConfigurationProperty("key",IsRequired=true)]
        public string Key
        {
            get { return (string)base["key"]; }
            set { base["key"] = value; }
        }
    }
    public sealed class Class2 : ConfigurationSection
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return (string)base["key"]; }
            set { base["key"] = value; }
        }
    }








    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        SoftwareSettings softSettings = ConfigurationManager.GetSection("SoftwareSettings") as SoftwareSettings;

    //        foreach (string key in softSettings.LoadSettings.Settings.Keys)
    //        {
    //            Console.WriteLine("{0}={1}", key, softSettings.LoadSettings[key]);
    //        }
    //        Console.WriteLine("SavePath={0},SearchSubPath={1}", softSettings.PathSetting.SavePath, softSettings.PathSetting.SearchSubPath);
    //        Console.ReadLine();
    //    }
    //}  

    //public sealed class LoadSettingsCollection : ConfigurationElementCollection  
    //{  
    //    private IDictionary<string, bool> settings;  
  
    //    protected override ConfigurationElement CreateNewElement()  
    //    {  
    //        return new LoadSettingsElement();  
    //    }  
  
    //    protected override object GetElementKey(ConfigurationElement element)  
    //    {  
    //        LoadSettingsElement ep = (LoadSettingsElement)element;  
  
    //        return ep.Key;  
    //    }  
  
    //    protected override string ElementName  
    //    {  
    //        get  
    //        {  
    //            return base.ElementName;  
    //        }  
    //    }  
  
    //    public IDictionary<string, bool> Settings  
    //    {  
    //        get  
    //        {  
    //            if (settings == null)  
    //            {  
    //                settings = new Dictionary<string, bool>();  
    //                foreach (LoadSettingsElement e in this)  
    //                {  
    //                    settings.Add(e.Key, e.Value);  
    //                }  
    //            }  
    //            return settings;  
    //        }  
    //    }  
  
    //    public bool this[string key]  
    //    {  
    //        get  
    //        {  
    //            bool isLoad = true;  
    //            if (settings.TryGetValue(key, out isLoad))  
    //            {  
    //                return isLoad;  
    //            }  
    //            else  
    //            {  
    //                throw new ArgumentException("没有对'" + key + "'节点进行配置。");  
    //            }  
    //        }  
    //    }  
  
    //}  
  
    //public class LoadSettingsElement : ConfigurationElement  
    //{  
    //    [ConfigurationProperty("key", IsRequired = true)]  
    //    public string Key  
    //    {  
    //        get { return (string)base["key"]; }  
    //        set { base["key"] = value; }  
    //    }  
    //    [ConfigurationProperty("value", IsRequired = true)]  
    //    public bool Value  
    //    {  
    //        get { return (bool)base["value"]; }  
    //        set { base["value"] = value; }  
    //    }  
    //}  
  
    //public class PathSettingElement : ConfigurationElement  
    //{  
    //    /// <summary>  
    //    ///   
    //    /// </summary>  
    //    [ConfigurationProperty("SavePath", IsRequired = true)]  
    //    public string SavePath  
    //    {  
    //        get { return (string)base["SavePath"]; }  
    //        set { base["SavePath"] = value; }  
    //    }  
    //    /// <summary>  
    //    ///   
    //    /// </summary>  
    //    [ConfigurationProperty("SearchSubPath", IsRequired = false, DefaultValue = true)]  
    //    public bool SearchSubPath  
    //    {  
    //        get { return (bool)base["SearchSubPath"]; }  
    //        set { base["SearchSubPath"] = value; }  
    //    }  
    //}  
  
    ///// <summary>  
    ///// 对应config文件中的  
    ///// </summary>  
    //public sealed class SoftwareSettings : ConfigurationSection  
    //{  
    //    /// <summary>  
    //    /// 对应SoftwareSettings节点下的LoadSettings子节点  
    //    /// </summary>  
    //    [ConfigurationProperty("LoadSettings", IsRequired = true)]  
    //    public LoadSettingsCollection LoadSettings  
    //    {  
    //        get { return (LoadSettingsCollection)base["LoadSettings"]; }  
    //    }  
  
    //    /// <summary>  
    //    /// 对应SoftwareSettings节点下的PathSettings子节点，非必须  
    //    /// </summary>  
    //    [ConfigurationProperty("PathSettings", IsRequired = false)]  
    //    public PathSettingElement PathSetting  
    //    {  
    //        get { return (PathSettingElement)base["PathSettings"]; }  
    //        set { base["PathSettings"] = value; }  
    //    }  
  
    //}  

}
