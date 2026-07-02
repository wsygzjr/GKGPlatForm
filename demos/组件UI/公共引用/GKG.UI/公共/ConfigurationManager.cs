using Avalonia.Layout;
using Avalonia.Media;
using GF_Gereric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml.Linq;

namespace GKG.UI;

internal static class ConfigurationManager
{
    private static BaseControlCfgList _baseControlCfgs=null!;
    internal static BaseControlCfgList BaseControlCfgs
    {
        get
        {
            if(_baseControlCfgs==null)
                loadIni();
            return _baseControlCfgs!;
        }
    }
    private static  FileSystemWatcher? _fileWatcher;
    private static readonly Subject<BaseControlCfgList> _configChanged = new Subject<BaseControlCfgList>();

    /// <summary>
    /// 配置变更通知
    /// </summary>
    public static IObservable<BaseControlCfgList> ConfigChanged => _configChanged.AsObservable();
    static ConfigurationManager()
    {
        InitializeFileWatcher();
        loadIni();
    }
    /// <summary>
    /// 初始化文件监听器
    /// </summary>
    private static void InitializeFileWatcher()
    {
        string cfgPath = getCfgFileName();
        string? configDir = Path.GetDirectoryName(cfgPath);
        if (string.IsNullOrEmpty(configDir)) return;

        _fileWatcher = new FileSystemWatcher(configDir)
        {
            Filter = Path.GetFileName(cfgPath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        // 订阅文件变更事件
        _fileWatcher.Changed += (s, e) =>
        {
            Observable.Timer(TimeSpan.FromMilliseconds(300))
                      .Subscribe(_ => ReloadAndNotify());
        };

        _fileWatcher.Created += (s, e) => ReloadAndNotify();
        _fileWatcher.Renamed += (s, e) => ReloadAndNotify();
    }
    /// <summary>
    /// 重新加载配置并通知变更
    /// </summary>
    private static void ReloadAndNotify()
    {
        _baseControlCfgs = null!;
    }

    private static void loadIni()
    {
        try
        {
            _baseControlCfgs = new BaseControlCfgList();
            string cfgFileName = getCfgFileName();

            _baseControlCfgs.LoadFromFile(cfgFileName);
        }
        catch
        {
            setDefaultData();
            Save();
        }
    }
   
    private static void setDefaultData()
    {
        _baseControlCfgs.Add(new BaseControlCfg
        {
            TotalWidthLevel = "Small",
            TotalWidth = 200,
            LableTotal = 100,
        });
        _baseControlCfgs.Add(new BaseControlCfg
        {
            TotalWidthLevel = "Medium",
            TotalWidth = 400,
            LableTotal = 150,
        });
        _baseControlCfgs.Add(new BaseControlCfg
        {
            TotalWidthLevel = "Large",
            TotalWidth = 600,
            LableTotal = 200,
        });
        _baseControlCfgs.Add(new BaseControlCfg
        {
            TotalWidthLevel = "ExtraLarge",
            TotalWidth = 800,
            LableTotal = 300,
        });
    }
    public static void Save()
    {
        string cfgFileName = getCfgFileName();
        _baseControlCfgs.SaveToFile(cfgFileName);
    }
    private static string getCfgFileName()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return Path.Combine(path, "BaseControlCfg.AppCfg");
    }


    #region  内部类型
    /// <summary>
    /// 人员字段显示信息配置信息
    /// </summary>
    public class BaseControlCfg :  IGriffinsObjXml
    {
        /// <summary>
        /// 控件整体宽度级别
        /// </summary>
        public string TotalWidthLevel { set; get; } = "";
        /// <summary>
        /// 控件整体宽度
        /// </summary>
        public double TotalWidth { set; get; }
        /// <summary>
        /// 左侧标签宽度
        /// </summary>
        public double LableTotal { set; get; }
        #region IGriffinsObjXml 成员

        void IGriffinsObjXml.ReadFromXmlInfo(GriffinsXmlInfo xmlInfo)
        {
            this.TotalWidthLevel = xmlInfo.ReadString("TotalWidthLevel", String.Empty);
            this.TotalWidth = xmlInfo.ReadDouble("TotalWidth", 200);
            this.LableTotal = xmlInfo.ReadDouble("LableTotal", 100);
        }
        void IGriffinsObjXml.WriteToXmlInfo(GriffinsXmlInfo xmlInfo)
        {
            xmlInfo.Write("TotalWidthLevel", this.TotalWidthLevel);
            xmlInfo.Write("TotalWidth", this.TotalWidth);
            xmlInfo.Write("LableTotal", this.LableTotal);
        }

        #endregion
    }

    /// <summary>
    ///用户字段配置信息配置信息列表
    /// </summary>
    public class BaseControlCfgList : List<BaseControlCfg>, IGriffinsObjXml
    {
        /// <summary>
        /// 从文件中加载
        /// </summary>
        /// <returns>true:加载成功，false:加载失败</returns>
        public bool LoadFromFile(string fileName)
        {
            return ObjXmlFormatter.DeserializeFromFile(this, fileName, "BaseControlCfgList");
        }
        /// <summary>
        /// 把内容写到文件中
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public void SaveToFile(string fileName)
        {
            ObjXmlFormatter.SerializeToFile(this, fileName, "BaseControlCfgList");
        }
        #region IGriffinsObjXml 成员

        void IGriffinsObjXml.ReadFromXmlInfo(GriffinsXmlInfo xmlInfo)
        {
            this.Clear();
            ObjXmlFormatter.AddListObjItemFromXmlInfo(xmlInfo, this, typeof(BaseControlCfg), "BaseControlCfg");
        }

        void IGriffinsObjXml.WriteToXmlInfo(GriffinsXmlInfo xmlInfo)
        {
            ObjXmlFormatter.WriteAllListObjItemToXmlInfo(xmlInfo, this, "BaseControlCfg");
        }

        #endregion
    }

    #endregion

}
