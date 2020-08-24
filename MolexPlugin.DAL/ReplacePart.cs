using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.Assemblies;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;
using System.Text.RegularExpressions;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 修改电极装配档组件名称
    /// </summary>
    public class ReplacePart
    {
        /// <summary>
        /// 替换模号件号
        /// </summary>
        /// <param name="model"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Part Replace(AbstractModel model, MoldInfoModel info)
        {
            Session theSession = Session.GetSession();
            UFSession theUFSession = UFSession.GetUFSession();
            Part workPart = theSession.Parts.Work;
            Component ct = model.GetPartComp(workPart);
            string OldName = model.MoldInfo.MoldNumber + "-" + model.MoldInfo.WorkpieceNumber;
            string newName = info.MoldNumber + "-" + info.WorkpieceNumber;
            string newPartPath = model.WorkpiecePath.Replace(OldName, newName);
            if (File.Exists(newPartPath))
            {
                File.Delete(newPartPath);
            }
            File.Move(model.WorkpiecePath, newPartPath);
            if (ct != null)
            {
                model.PartTag.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                if (Basic.AssmbliesUtils.ReplaceComp(ct, newPartPath, ct.Name.Replace(OldName, newName)))
                {
                    return ct.Prototype as Part;
                }
                return null;
            }
            else
            {
                model.PartTag.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                //NXOpen.PartLoadStatus partLoadStatus1;
                //BasePart part = theSession.Parts.OpenBaseDisplay(newPartPath, out partLoadStatus1);
                Tag partTag;
                UFPart.LoadStatus error_status;
                theUFSession.Part.Open(newPartPath, out partTag, out error_status);
                return NXObjectManager.Get(partTag) as Part;
            }
        }
        /// <summary>
        /// 修改电极名
        /// </summary>
        /// <param name="model"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static Part ReplaceElectrode(ElectrodeModel model, string newName)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Component ct = model.GetPartComp(workPart);
            string OldName = model.EleInfo.EleName;
            string newPartPath = model.WorkpiecePath.Replace(OldName, newName);
            File.Move(model.WorkpiecePath, newPartPath);
            if (ct != null)
            {
                model.PartTag.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                if (Basic.AssmbliesUtils.ReplaceComp(ct, newPartPath, ct.Name.Replace(OldName, newName)))
                {
                    Part elePart = ct.Prototype as Part;
                    string dwgPath = model.WorkpieceDirectoryPath + model.AssembleName + "_dwg.prt";
                    AttributeUtils.AttributeOperation("EleName", newName, elePart);
                    AttributeUtils.AttributeOperation("EleNumber", GetEleNumber(newName), elePart);
                    if (File.Exists(dwgPath))
                    {
                        UFSession theUFSession = UFSession.GetUFSession();
                        Tag partTag;
                        UFPart.LoadStatus error_status;
                        File.Move(dwgPath, dwgPath.Replace(OldName, newName));
                        theUFSession.Part.Open(dwgPath.Replace(OldName, newName), out partTag, out error_status);
                        AttributeUtils.AttributeOperation("EleName", newName, NXObjectManager.Get(partTag) as Part);

                    }
                    return elePart;
                }
                return null;
            }
            return null;
        }
        public static Part Replace(Part pt, string newName)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            string oldName = pt.Name;
            string oldPath = pt.FullPath;
            string newPath = oldPath.Replace(oldName, newName);
            Component ct = AssmbliesUtils.GetPartComp(workPart, pt);
            File.Move(oldPath, newPath);
            if (ct != null)
            {
                pt.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                if (Basic.AssmbliesUtils.ReplaceComp(ct, newPath, newName))
                {
                    return ct.OwningPart as Part;
                }
                return null;
            }
            return null;
        }

        private static int GetEleNumber(string eleName)
        {
            string name = eleName.Substring(eleName.LastIndexOf("E"));

            MatchCollection match = Regex.Matches(name, @"\d+");
            int result;
            if (match.Count != 0 && int.TryParse(match[0].Value, out result))
            {
                return result;
            }
            return 0;
        }
    }
}
