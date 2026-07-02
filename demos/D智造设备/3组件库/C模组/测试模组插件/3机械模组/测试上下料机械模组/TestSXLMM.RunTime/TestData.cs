using Griffins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSXLMM.RunTime
{
    /// <summary>
    /// 测试数据
    /// </summary>
    internal static class TestData
    {
       public static void SimpleTest(LoadUnloadUIObj loadUnloadUIObj)
        {
            //过程1：模拟属性值改变
            loadUnloadUIObj.TestParam1 = 2;
            loadUnloadUIObj.TestMaterialBox.MaterialBoxName = "已修改值的测试料盒名称";
            loadUnloadUIObj.MaterialContainers.FirstOrDefault().Value.ContainerName = "已修改的容器名称";
            GFBaseTypeObjPropPathValueList changedProvalues = loadUnloadUIObj.GetChangedGFBaseTypeObjPropPathValue();

            //过程2：【后端产生属性值改变通知】或【前端调用设置属性值】传递参数：changedProvalues

            //过程3-1：后端：将发生改变的属性值列表填充到已有对象
            loadUnloadUIObj.SetGFBaseTypeObjPropPathValue(changedProvalues);

            //过程3-2：前端：两种方式更新修改的属性值：
            //1、前端有自己的数据结构，自己处理将改变的属性值赋值到自己的对象。
            //2、定义前端自己的对象和后端的LoadUnloadUIObj结构保持一致，同样调用SetGFBaseTypeObjPropPathValue设置修改的值
            //3、前端的结构和后端共用。调用SetGFBaseTypeObjPropPathValue设置修改的值
            //loadUnloadUIObj.SetGFBaseTypeObjPropPathValue(changedProvalues);
        }

        public static void OtherTest(LoadUnloadUIObj loadUnloadUIObj)
        {
            //合并测试:
            GFBaseTypePropValueList gFBaseTypePropValues = loadUnloadUIObj.ToGFBaseTypePropValues();
            //mergTest(gFBaseTypePropValues);
            //mergTest1(gFBaseTypePropValues, loadUnloadUIObj);
            //mergChangedValuesTest1(gFBaseTypePropValues);
            //mergChangedValuesTest2(gFBaseTypePropValues);
            //mergChangedValuesTest3(gFBaseTypePropValues);
            ////mergChangedValuesTest4(gFBaseTypePropValues);
            //testUIDataObjPropPathValueList(loadUnloadUIObj);
        }
        private static void mergTest(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            //MaterialContainers[1000].ContainerName
            GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = new GFBaseTypeObjPropPathValueList();
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器xxx")
            });
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs[materialBox0]", "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒1xxxx")
            });
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "TestMaterialBox", "ContainerName" }),
                Value = new GriffinsBaseValue("容器Testxxx")
            });
            gFBaseTypePropValues.Merge(gFBaseTypeObjPropPathValues);
            //测试Merge GetLeafGFBaseTypeObjPropPathValues
            GFBaseTypeObjPropPathValueList gfBaseTypeObjPropPathValues =gFBaseTypePropValues.GetLeafGFBaseTypeObjPropPathValues();
            LoadUnloadUIObj loadUnloadUIObj = LoadUnloadUIObj.FromGFBaseTypePropValues<LoadUnloadUIObj>(gFBaseTypePropValues) ;
            //测试ToGFBaseTypePropValues
            GFBaseTypePropValueList sources = gfBaseTypeObjPropPathValues.ToGFBaseTypePropValues();
            LoadUnloadUIObj loadUnloadUIObjUp1 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(sources);

        }

        private static void mergTest1(GFBaseTypePropValueList targets, LoadUnloadUIObj loadUnloadUIObj)
        {
            loadUnloadUIObj.TestMaterialBox.MaterialBoxName = "Test料盒";
            MaterialContainer materialContainer = loadUnloadUIObj.MaterialContainers.FirstOrDefault().Value;
            materialContainer.ContainerName = "新值容器名称";
            MaterialBox materialBox=materialContainer.MaterialBoxs.FirstOrDefault().Value;
            materialBox.MaterialBoxName = "新的料盒名称";
            GFBaseTypePropValueList sources = loadUnloadUIObj.ToGFBaseTypePropValues();
            targets.Merge(sources);
            //还原对象验证：
            LoadUnloadUIObj loadUnloadUIObjUp = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(targets);
        }
        /// <summary>
        /// 测试属性为对象的合并
        /// </summary>
        /// <param name="targets"></param>
        private static void mergChangedValuesTest1(GFBaseTypePropValueList targets)
        {
            string testMaterialBox = nameof(LoadUnloadUIObj.TestMaterialBox);
            GFBaseTypeObjPropPathValueList changedValues = new GFBaseTypeObjPropPathValueList();
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { testMaterialBox, "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒xxx")
            });
           
            //合并容器1的属性值
            var objInstPropPath = new ObjInstPropPath(new string[] { testMaterialBox });
            //容器1的旧值
            MPPropertyID mPPropertyID = new MPPropertyID(testMaterialBox);
            var newValue1 = GFBaseTypeObjPropPathValueList.MergeGriffinsBaseValueOfGFBaseTypePropValueList(objInstPropPath, null, changedValues);
            GFBaseTypePropValueList new1s = new GFBaseTypePropValueList() {new GFBaseTypePropValue()
            {
                PropertyID=mPPropertyID,
                Value=newValue1
            } };
            //还原对象验证：
            LoadUnloadUIObj newOb1 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(new1s);
        }
        /// <summary>
        /// 测试属性为字典的某个对象的合并
        /// </summary>
        /// <param name="targets"></param>
        private static void mergChangedValuesTest2(GFBaseTypePropValueList targets)
        {
            //属性为字典的某个项对象：容器0
            var objInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]" });

            GFBaseTypeObjPropPathValueList changedValues = new GFBaseTypeObjPropPathValueList();
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器xxx")
            });
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs[materialBox0]", "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒1xxxx")
            });
            MPPropertyID mPPropertyID = new MPPropertyID(nameof(LoadUnloadUIObj.MaterialContainers));
            var newValue1 = GFBaseTypeObjPropPathValueList.MergeGriffinsBaseValueOfGFBaseTypePropValueList(objInstPropPath, null, changedValues);
            GFBaseTypePropValue newValue = targets.Find(mPPropertyID);
            var objnewDict = newValue.Value.ToObject<GFBaseTypePropValueListDict>();
            objnewDict["Containers0"]= newValue1.ToObject<GFBaseTypePropValueList>();
            GriffinsBaseValue newGriffinsBaseValue = objnewDict.ToGriffinsBaseValue();
            newValue.Value = newGriffinsBaseValue;
            ////还原对象验证：
            LoadUnloadUIObj newOb1 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(targets);

            //合并旧值验证
            GFBaseTypePropValue oldValue = targets.Find(mPPropertyID);
            var propValuesDict = new GFBaseTypePropValueListDict();
            var objDict = oldValue.Value.ToObject<GFBaseTypePropValueListDict>();
            GFBaseTypePropValueList objValues = objDict["Containers0"];
            GriffinsBaseValue oldGriffinsBaseValue = objValues.ToGriffinsBaseValue();
            changedValues.Clear();
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器yyy")
            });
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs[materialBox0]", "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒yyyy")
            });
            var newValue2 = GFBaseTypeObjPropPathValueList.MergeGriffinsBaseValueOfGFBaseTypePropValueList(objInstPropPath, oldGriffinsBaseValue, changedValues);

            GFBaseTypePropValue newValue11 = targets.Find(mPPropertyID);
            var objnewDict11 = newValue11.Value.ToObject<GFBaseTypePropValueListDict>();
            objnewDict11["Containers0"] = newValue2.ToObject<GFBaseTypePropValueList>();
            GriffinsBaseValue newGriffinsBaseValue2 = objnewDict11.ToGriffinsBaseValue();
            newValue11.Value = newGriffinsBaseValue2;
            ////还原对象验证：
            LoadUnloadUIObj newOb122 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(targets);
        }
        /// <summary>
        /// 测试属性为字典的合并
        /// </summary>
        /// <param name="targets"></param>
        private static void mergChangedValuesTest3(GFBaseTypePropValueList targets)
        {
            //属性为字典
            string materialContainers = nameof(LoadUnloadUIObj.MaterialContainers);
            GFBaseTypeObjPropPathValueList changedValues = new GFBaseTypeObjPropPathValueList();
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器xxx")
            });
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs[materialBox0]", "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒1xxxx")
            });

            //合并容器列表的属性值
            //var objInstPropPath = new ObjInstPropPath(new string[] { materialContainers });
            var objInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs" });
            //容器1
            MPPropertyID mPPropertyID = new MPPropertyID(materialContainers);
            var newValue1 = GFBaseTypeObjPropPathValueList.MergeGriffinsBaseValueOfGFBaseTypePropValueListDict(objInstPropPath, null, changedValues);
            GFBaseTypePropValueList new1s = new GFBaseTypePropValueList() {new GFBaseTypePropValue()
            {
                PropertyID=mPPropertyID,
                Value=newValue1
            } };
            //还原对象验证：
            LoadUnloadUIObj newOb1 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(new1s);

            //合并旧值验证
            changedValues.Clear();
            changedValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器yyy")
            });
            GFBaseTypePropValue oldValue = targets.Find(mPPropertyID);
            GriffinsBaseValue oldGriffinsBaseValue = oldValue.Value;
            var newValue2 = GFBaseTypeObjPropPathValueList.MergeGriffinsBaseValueOfGFBaseTypePropValueListDict(objInstPropPath, oldGriffinsBaseValue, changedValues);

            GFBaseTypePropValueList new2s = new GFBaseTypePropValueList() {new GFBaseTypePropValue()
            {
                PropertyID=mPPropertyID,
                Value=newValue2
            } };
            //还原对象验证：
            LoadUnloadUIObj newOb2 = GFPropObjBase.FromGFBaseTypePropValues<LoadUnloadUIObj>(new2s);
        }

        private static void testUIDataObjPropPathValueList(LoadUnloadUIObj loadUnloadUIObj)
        {
            GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValues = new GFBaseTypeObjPropPathValueList();
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" }),
                Value = new GriffinsBaseValue("容器xxx")
            });
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "MaterialBoxs[materialBox0]", "MaterialBoxName" }),
                Value = new GriffinsBaseValue("料盒1xxxx")
            });
            gFBaseTypeObjPropPathValues.Add(new GFBaseTypeObjPropPathValue()
            {
                ObjInstPropPath = new ObjInstPropPath(new string[] { "TestMaterialBox", "ContainerName" }),
                Value = new GriffinsBaseValue("容器Testxxx")
            });

            UIDataObjPropPathValueList uIDataObjPropPathValues = new UIDataObjPropPathValueList();
            uIDataObjPropPathValues.FromGFBaseTypeObjPropPathValues("alise1",DateTime.Now, gFBaseTypeObjPropPathValues);
            UIDataObjPropPathValueList data1 =uIDataObjPropPathValues.GetUIDataObjPropPathValues("alise1");
            ObjInstPropPath objInstPropPath=new ObjInstPropPath(new string[] { "MaterialContainers[Containers0]", "ContainerName" });
            UIDataObjPropPathValue data2 =uIDataObjPropPathValues.GetUIDataObjPropPathValue("alise1", objInstPropPath);

            GFBaseTypePropValueList source = new GFBaseTypePropValueList();
            UIDataObjPropValueList uIDataObjPropValues = new UIDataObjPropValueList();
            GFBaseTypePropValueList gFBaseTypePropValues = loadUnloadUIObj.ToGFBaseTypePropValues();
            uIDataObjPropValues.FromGFBaseTypePropValues("alise1", DateTime.Now, gFBaseTypePropValues);
            UIDataObjPropPathValueList data3 =uIDataObjPropValues.ToUIDataObjPropPathValue();
        }

    }
}
