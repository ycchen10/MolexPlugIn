using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Assemblies;

namespace Basic
{
    /// <summary>
    /// 装配间隙分析
    /// </summary>
    public class ClearanceAnalysisUtils
    {
        /// <summary>
        /// 创建间隙分析
        /// </summary>
        /// <param name="setName">间隙分析名字</param>
        /// <param name="oneObjects">第一个工件</param>
        /// <param name="twoObjects">等二个工件</param>
        /// <returns></returns>
        public static ClearanceSet CreateClearanceAnalysis(string setName, NXOpen.Assemblies.Component oneObjects, params Component[] twoObjects)
        {
            Part workPart = Session.GetSession().Parts.Work;
            NXOpen.Assemblies.ClearanceSet nullNXOpen_Assemblies_ClearanceSet = null;
            NXOpen.Assemblies.ClearanceAnalysisBuilder clearanceAnalysisBuilder1;
            clearanceAnalysisBuilder1 = workPart.AssemblyManager.CreateClearanceAnalysisBuilder(nullNXOpen_Assemblies_ClearanceSet);
            clearanceAnalysisBuilder1.CalculationMethod = NXOpen.Assemblies.ClearanceAnalysisBuilder.CalculationMethodType.ExactifLoaded;
            clearanceAnalysisBuilder1.ClearanceSetName = setName;
            clearanceAnalysisBuilder1.TotalCollectionCount = NXOpen.Assemblies.ClearanceAnalysisBuilder.NumberOfCollections.Two;
            clearanceAnalysisBuilder1.CollectionOneRange = NXOpen.Assemblies.ClearanceAnalysisBuilder.CollectionRange.SelectedObjects;
            clearanceAnalysisBuilder1.CollectionTwoRange = NXOpen.Assemblies.ClearanceAnalysisBuilder.CollectionRange.SelectedObjects;
            clearanceAnalysisBuilder1.ClearanceBetween = NXOpen.Assemblies.ClearanceAnalysisBuilder.ClearanceBetweenEntity.Components;
            clearanceAnalysisBuilder1.SaveInterferenceGeometry = true;
            clearanceAnalysisBuilder1.IsCalculatePenetrationDepth = true;
            bool added1;
            added1 = clearanceAnalysisBuilder1.CollectionOneObjects.Add(oneObjects);
            bool added2;
            added2 = clearanceAnalysisBuilder1.CollectionTwoObjects.Add(twoObjects);

            try
            {

                return clearanceAnalysisBuilder1.Commit() as ClearanceSet;
            }
            catch (NXException ex)
            {
                LogMgr.WriteLog("ClearanceAnalysisUtils.CreateClearanceAnalysis" + ex.Message);
                return null;
            }
            finally
            {
                clearanceAnalysisBuilder1.Destroy();
            }

        }
        /// <summary>
        /// 获取干涉数据
        /// </summary>
        /// <param name="setObj"></param>
        /// <param name="ct1"></param>
        /// <param name="ct2"></param>
        /// <returns></returns>
        public static InterferenceData GetInterenceData(ClearanceSet setObj, Component ct1, Component ct2)
        {
            ClearanceSet.InterferenceType type;
            bool newInterference;
            DisplayableObject[] interfBodies;
            Point3d point1;
            Point3d point2;
            string text;
            int interfNum;
            int config;
            int depthResult;
            double depth;
            Vector3d direction;
            Point3d minPoint;
            Point3d maxPoint;
            setObj.PerformAnalysis(ClearanceSet.ReanalyzeOutOfDateExcludedPairs.False);
            setObj.GetInterferenceData(ct1, ct2, out type,
                                                 out newInterference,
                                                 out interfBodies,
                                                 out point1,
                                                 out point2,
                                                 out text,
                                                 out interfNum,
                                                 out config,
                                                 out depthResult,
                                                 out depth,
                                                 out direction,
                                                 out minPoint,
                                                 out maxPoint);
            return new InterferenceData()
            {
                Type = type,
                NewInterference = newInterference,
                InterfBodies = interfBodies.ToList(),
                Point1 = point1,
                Point2 = point2,
                Text = text,
                InterfNum = interfNum,
                Config = config,
                DepthResult = depthResult,
                Depth = depth,
                Direction = direction,
                MaxPoint = maxPoint,
                MinPoint = minPoint
            };

        }
    }
    /// <summary>
    /// 干涉数据
    /// </summary>
    public class InterferenceData
    {
        /// <summary>
        /// 干涉类型
        /// </summary>
        public ClearanceSet.InterferenceType Type { get; set; }
        /// <summary>
        /// 新干涉
        /// </summary>
        public bool NewInterference { get; set; }
        /// <summary>
        /// 干涉体列表
        /// </summary>
        public List<DisplayableObject> InterfBodies { get; set; } = new List<DisplayableObject>();
        /// <summary>
        /// 第一个体上的点
        /// </summary>
        public Point3d Point1 { get; set; }
        /// <summary>
        /// 第二个物体上的点
        /// </summary>
        public Point3d Point2 { get; set; }
        /// <summary>
        /// 与干涉有关的文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 唯一的干涉编号
        /// </summary>
        public int InterfNum { get; set; }
        /// <summary>
        /// 配置索引
        /// </summary>
        public int Config { get; set; }
        /// <summary>
        /// 穿透深度计算的结果状态
        /// </summary>
        public int DepthResult { get; set; }
        /// <summary>
        /// 穿透深度
        /// </summary>
        public double Depth { get; set; }
        /// <summary>
        /// 指示穿透方向的单位向量
        /// </summary>
        public Vector3d Direction { get; set; }
        /// <summary>
        /// 干涉区域最小处的点
        /// </summary>
        public Point3d MinPoint { get; set; }
        /// <summary>
        /// 干涉区域最大的点
        /// </summary>
        public Point3d MaxPoint { get; set; }
    }
}
