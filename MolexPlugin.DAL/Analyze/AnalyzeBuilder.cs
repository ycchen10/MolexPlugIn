using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;


namespace MolexPlugin.DAL
{
    public class AnalyzeBuilder
    {
        private NXObject obj;

        private List<AnalyzeFaceSlopeAndRadius> analyze = new List<AnalyzeFaceSlopeAndRadius>();
        /// <summary>
        /// 倒扣
        /// </summary>
        public bool IsBackOff { get; private set; } = false;
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinRadius { get; private set; } = 99999;

        public AnalyzeBuilder(NXObject obj)
        {
            this.obj = obj;
        }
        /// <summary>
        /// 分析面
        /// </summary>
        /// <param name="face"></param>
        /// <param name="vec"></param>
        private void AnalyzeFace(Face face, Vector3d vec)
        {
            AnalyzeFaceSlopeAndRadius af = new AnalyzeFaceSlopeAndRadius(face);
            af.AnalyzeFace(vec);
            analyze.Add(af);
            if (this.MinRadius > af.MinRadius)
                this.MinRadius = af.MinRadius;
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="face"></param>
        /// <param name="color"></param>
        private void SetColor(Face face, int color)
        {
            UFSession theUFSession = UFSession.GetUFSession();
            theUFSession.Obj.SetColor(face.Tag, color);
        }
        /// <summary>
        /// 获取倒扣
        /// </summary>
        private void GetBackOff()
        {
            analyze.Sort();//排序（最后一个为底面）
            if (analyze.Count == 1)
            {
                if ((analyze[0].MaxSlope > Math.Round(Math.PI / 2, 3) && analyze[0].MaxSlope <= Math.Round(Math.PI, 3)) || analyze[0].ResultsNum > 0)
                {
                    SetColor(analyze[0].face, 186);//倒扣
                    this.IsBackOff = true;
                }
            }
            else
            {
                for (int i = 0; i < analyze.Count - 2; i++)
                {
                    if ((analyze[i].MaxSlope > Math.Round(Math.PI / 2, 3) && analyze[i].MaxSlope <= Math.Round(Math.PI, 3)) || analyze[i].ResultsNum > 0)
                    {
                        SetColor(analyze[i].face, 186);//倒扣
                        this.IsBackOff = true;
                    }
                }
            }
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetSlopeEqualColor()
        {
            foreach (AnalyzeFaceSlopeAndRadius ar in this.analyze)
            {
                if (UMathUtils.IsEqual(ar.MaxSlope, ar.MinSlope))
                {
                    if ((ar.MaxSlope < Math.Round(Math.PI / 2, 3) && ar.MaxSlope > 0) && ar.ResultsNum == 0)
                    {
                        SetColor(ar.face, 36);//斜度
                        continue;
                    }

                    if ((ar.MaxSlope == Math.Round(Math.PI / 2, 3)) && ar.ResultsNum == 0)
                    {
                        SetColor(ar.face, 211); //垂直
                        continue;
                    }
                    if ((UMathUtils.IsEqual(ar.MaxSlope, 0) || ar.MaxSlope == Math.Round(Math.PI, 3)) && ar.ResultsNum == 0)
                    {
                        SetColor(ar.face, 25); //平面
                        continue;
                    }
                }
                else if (!(ar.MaxSlope > Math.Round(Math.PI / 2, 3) && ar.MaxSlope <= Math.Round(Math.PI, 3)) && ar.ResultsNum > 0)
                    SetColor(ar.face, 36);//斜度
            }
        }


        public void Analyze(Vector3d vec)
        {
            analyze.Clear();
            if (obj is Face)
                this.AnalyzeFace(obj as Face, vec);
            if (obj is Body)
            {
                Body body = obj as Body;
                foreach (Face face in body.GetFaces())
                {
                    this.AnalyzeFace(face, vec);
                }
            }
            this.GetBackOff();
        }

    }
}
