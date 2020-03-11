
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 边数据
    /// </summary>
    public class EdgeUtils
    {
        /// <summary>
        /// 获得圆弧的数据
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="arcData"></param>
        /// <returns></returns>
        public static bool GetArcData(Edge edge, out NXOpen.UF.UFEval.Arc arcData, ref string errorMsg)
        {
            if (edge.SolidEdgeType != Edge.EdgeType.Circular)
            {
                arcData = new NXOpen.UF.UFEval.Arc();
                errorMsg = "该边不是圆弧";
                return false;
            }

            NXOpen.UF.UFSession theUfSession = NXOpen.UF.UFSession.GetUFSession();
            IntPtr eval;
            theUfSession.Eval.Initialize(edge.Tag, out eval);


            try
            {
                theUfSession.Eval.AskArc(eval, out arcData);
            }
            catch (Exception ex)
            {
                arcData = new NXOpen.UF.UFEval.Arc();
                errorMsg = ex.Message;
                LogMgr.WriteLog("EdgeUtils:GetArcData:" + ex.Message);
                return false;
            }
            finally
            {
                theUfSession.Eval.Free(eval);
            }

            return true;
        }


        public static ArcEdgeData GetArcData(Edge edge, ref string errorMsg)
        {
            if (edge.SolidEdgeType != Edge.EdgeType.Circular)
            {
                errorMsg = errorMsg + "该边不是圆弧";
                return null;
            }

            NXOpen.UF.UFSession theUfSession = NXOpen.UF.UFSession.GetUFSession();
            IntPtr eval;
            theUfSession.Eval.Initialize(edge.Tag, out eval);

            NXOpen.UF.UFEval.Arc arc;
            try
            {
                theUfSession.Eval.AskArc(eval, out arc);
                ArcEdgeData arcData = new ArcEdgeData(edge);
                arcData.Center = new Point3d(arc.center[0], arc.center[1], arc.center[2]);
                arcData.Radius = arc.radius;

                arcData.IsWholeCircle = UMathUtils.IsEqual(Math.PI * 2, Math.Abs(arc.limits[1] - arc.limits[0]));
                arcData.Angle = Math.Abs(arc.limits[1] - arc.limits[0]);
                return arcData;
            }
            catch (Exception ex)
            {
                errorMsg += ex.Message;
                LogMgr.WriteLog("EdgeUtils:GetArcData1:"+edge.Tag.ToString() + ex.Message);
                return null;
            }
            finally
            {
                theUfSession.Eval.Free(eval);
            }
        }

        /// <summary>
        /// 获得直线的数据
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="lineData"></param>
        /// <returns></returns>
        public static bool GetLineData(Edge edge, out NXOpen.UF.UFEval.Line lineData, ref string errorMsg)
        {
            if (edge.SolidEdgeType != Edge.EdgeType.Linear)
            {
                lineData = new NXOpen.UF.UFEval.Line();

                errorMsg = "该边不是直线!";
                return false;
            }

            NXOpen.UF.UFSession theUfSession = NXOpen.UF.UFSession.GetUFSession();
            IntPtr eval;
            theUfSession.Eval.Initialize(edge.Tag, out eval);
            try
            {
                theUfSession.Eval.AskLine(eval, out lineData);
            }
            catch (Exception ex)
            {
                lineData = new NXOpen.UF.UFEval.Line();
                errorMsg = ex.Message;
                LogMgr.WriteLog("EdgeUtils:GetLineData:" + ex.Message);
                return false;
            }
            finally
            {
                theUfSession.Eval.Free(eval);
            }

            return true;
        }


        public static double GetArcRadius(Edge edge)
        {
            NXOpen.UF.UFSession theUfSession = NXOpen.UF.UFSession.GetUFSession();
            IntPtr eval;
            theUfSession.Eval.Initialize(edge.Tag, out eval);

            NXOpen.UF.UFEval.Arc arcData;
            try
            {
                theUfSession.Eval.AskArc(eval, out arcData);

                return arcData.radius;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                theUfSession.Eval.Free(eval);
            }
        }
    }

    public class ArcEdgeData : IComparable<ArcEdgeData>
    {
        /// <summary>
        /// 边
        /// </summary>
        public Edge Edge { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// 圆心
        /// </summary>
        public Point3d Center { get; set; }
        /// <summary>
        /// 是否是整圆
        /// </summary>
        public bool IsWholeCircle { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }
        public ArcEdgeData(Edge edge)
        {
            this.Edge = edge;
            this.IsWholeCircle = true;
        }


        public int CompareTo(ArcEdgeData other)
        {
            if (UMathUtils.Equals(this.Center.Z, other.Center.Z))
            {
                if (this.Radius > other.Radius)
                    return -1;
                else
                    return 1;
            }
            else if (this.Center.Z > other.Center.Z)
                return 1;

            return -1;
        }
    }
}
