using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace Basic
{
    public class PullFaceUtils
    {
        public static NXObject CreatePullFace(Vector3d vec, double pull,params Face[] faces)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            NXOpen.Features.PullFace nullNXOpen_Features_PullFace = null;
            NXOpen.Features.PullFaceBuilder pullFaceBuilder1;
            pullFaceBuilder1 = workPart.Features.CreatePullFaceBuilder(nullNXOpen_Features_PullFace);

            pullFaceBuilder1.Motion.DistanceAngle.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            pullFaceBuilder1.Motion.DistanceAngle.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.IsPercentUsed = true;

            NXOpen.Point3d origin1 = new NXOpen.Point3d(0.0, 0.0, 0.0);

            NXOpen.Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vec, NXOpen.SmartObject.UpdateOption.WithinModeling);

            pullFaceBuilder1.Motion.DistanceVector = direction1;

            NXOpen.FaceDumbRule faceDumbRule;
            faceDumbRule = workPart.ScRuleFactory.CreateRuleFaceDumb(faces);

            NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            rules1[0] = faceDumbRule;
            pullFaceBuilder1.FaceToPull.ReplaceRules(rules1, false);

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = pull.ToString();

            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = pullFaceBuilder1.Commit();
                return nXObject1;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("PullFaceUtils:CreatePullFace:      " + ex.Message);
                return null;
            }
            finally
            {
                pullFaceBuilder1.Destroy();
            }


        }
    }
}
