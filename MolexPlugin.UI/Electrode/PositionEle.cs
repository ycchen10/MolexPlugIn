﻿//==============================================================================
//  WARNING!!  This file is overwritten by the Block UI Styler while generating
//  the automation code. Any modifications to this file will be lost after
//  generating the code again.
//
//       Filename:  C:\Users\ycchen10\OneDrive - kochind.com\Desktop\MolexPlugIn-12.0\UI\PositionEle.cs
//
//        This file was generated by the NX Block UI Styler
//        Created by: ycchen10
//              Version: NX 11
//              Date: 03-10-2020  (Format: mm-dd-yyyy)
//              Time: 10:09 (Format: hh-mm)
//
//==============================================================================

//==============================================================================
//  Purpose:  This TEMPLATE file contains C# source to guide you in the
//  construction of your Block application dialog. The generation of your
//  dialog file (.dlx extension) is the first step towards dialog construction
//  within NX.  You must now create a NX Open application that
//  utilizes this file (.dlx).
//
//  The information in this file provides you with the following:
//
//  1.  Help on how to load and display your Block UI Styler dialog in NX
//      using APIs provided in NXOpen.BlockStyler namespace
//  2.  The empty callback methods (stubs) associated with your dialog items
//      have also been placed in this file. These empty methods have been
//      created simply to start you along with your coding requirements.
//      The method name, argument list and possible return values have already
//      been provided for you.
//==============================================================================

//------------------------------------------------------------------------------
//These imports are needed for the following template code
//------------------------------------------------------------------------------
using System;
using NXOpen;
using NXOpen.BlockStyler;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;
using MolexPlugin.DAL;
namespace MolexPlugin
{

    //------------------------------------------------------------------------------
    //Represents Block Styler application class
    //------------------------------------------------------------------------------
    public class PositionEle
    {
        //class members
        private static Session theSession = null;
        private static UI theUI = null;
        private Part workPart;
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group group0;// Block type: Group
        private NXOpen.BlockStyler.SelectObject SeleElePart;// Block type: Selection
        private NXOpen.BlockStyler.StringBlock StrName;// Block type: String
        private NXOpen.BlockStyler.Group group;// Block type: Group
        private NXOpen.BlockStyler.DoubleBlock double_x;// Block type: Double
        private NXOpen.BlockStyler.DoubleBlock double_y;// Block type: Double
        private NXOpen.BlockStyler.DoubleBlock double_z;// Block type: Double
        private WorkModel work = new WorkModel();
        private AssembleModel assemble;
        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public PositionEle()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                workPart = theSession.Parts.Work;
                theDlxFileName = "PositionEle.dlx";
                theDialog = theUI.CreateDialog(theDlxFileName);
                theDialog.AddApplyHandler(new NXOpen.BlockStyler.BlockDialog.Apply(apply_cb));
                theDialog.AddOkHandler(new NXOpen.BlockStyler.BlockDialog.Ok(ok_cb));
                theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
                theDialog.AddFilterHandler(new NXOpen.BlockStyler.BlockDialog.Filter(filter_cb));
                theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
                theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                throw ex;
            }
        }



        //------------------------------------------------------------------------------
        //This method shows the dialog on the screen
        //------------------------------------------------------------------------------
        public NXOpen.UIStyler.DialogResponse Show()
        {
            try
            {

                string partType = AttributeUtils.GetAttrForString(workPart, "PartType");
                if (!partType.Equals("Work"))
                {
                    theUI.NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "请切换到Work档下！");
                    return 0;
                }
                work.GetModelForPart(workPart);
                string asm = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber + "-ASM";
                string name = work.MoldInfo.MoldNumber + "-" + work.MoldInfo.WorkpieceNumber;

                foreach (Part part in theSession.Parts)
                {
                    if (part.Name.Equals(asm))
                    {
                        assemble = AssembleSingleton.Instance().GetAssemble(name);
                    }
                }
                if (assemble == null)
                {
                    theUI.NXMessageBox.Show("错误！", NXMessageBox.DialogType.Error, "无法找到ASM档！");
                    return 0;
                }
                theDialog.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Method Name: Dispose
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            if (theDialog != null)
            {
                theDialog.Dispose();
                theDialog = null;
            }
        }

        //------------------------------------------------------------------------------
        //---------------------Block UI Styler Callback Functions--------------------------
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Callback Name: initialize_cb
        //------------------------------------------------------------------------------
        public void initialize_cb()
        {
            try
            {
                group0 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group0");
                SeleElePart = (NXOpen.BlockStyler.SelectObject)theDialog.TopBlock.FindBlock("SeleElePart");
                StrName = (NXOpen.BlockStyler.StringBlock)theDialog.TopBlock.FindBlock("StrName");
                group = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group");
                double_x = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("double_x");
                double_y = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("double_y");
                double_z = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("double_z");
                Selection.MaskTriple maskComp = new Selection.MaskTriple()
                {
                    Type = 63,
                    Subtype = 1,
                    SolidBodySubtype = 0
                };
                Selection.MaskTriple[] masks = { maskComp };
                SeleElePart.SetSelectionFilter(Selection.SelectionAction.ClearAndEnableSpecific, masks);//过滤只选择组件
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: dialogShown_cb
        //This callback is executed just before the dialog launch. Thus any value set 
        //here will take precedence and dialog will be launched showing that value. 
        //------------------------------------------------------------------------------
        public void dialogShown_cb()
        {
            try
            {
                //---- Enter your callback code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: apply_cb
        //------------------------------------------------------------------------------
        public int apply_cb()
        {
            int errorCode = 0;
            try
            {
                //---- Enter your callback code here 
                bool anyPartsModified1;
                NXOpen.PartSaveStatus partSaveStatus1;
                Session.GetSession().Parts.SaveAll(out anyPartsModified1, out partSaveStatus1);
                NXOpen.Assemblies.Component eleComp = this.SeleElePart.GetSelectedObjects()[0] as NXOpen.Assemblies.Component;
                string name = eleComp.Name + "-" + this.StrName.Value.ToUpper();
                if (!this.EleDuplicationOfName(name))
                {
                    this.StrName.Value = "";
                    return 0;
                }
                ElectrodeModel eleModel = new ElectrodeModel();
                eleModel.GetModelForPart(eleComp.Prototype as Part);
                name = work.WorkpieceDirectoryPath + name + ".prt";
                Vector3d movePt = new Vector3d(double_x.Value, double_y.Value, double_z.Value);               
                NXOpen.Assemblies.Component copyComp = AssmbliesUtils.MoveCompCopyPart(eleComp, movePt, work.WorkMatr);
                AssmbliesUtils.MakeUnique(copyComp, name);
                ElectrodeInfo newInfo = this.SetEleInfo(eleModel.EleInfo);
                newInfo.SetAttribute(copyComp.Prototype as Part);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                errorCode = 1;
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return errorCode;
        }

        //------------------------------------------------------------------------------
        //Callback Name: update_cb
        //------------------------------------------------------------------------------
        public int update_cb(NXOpen.BlockStyler.UIBlock block)
        {
            try
            {
                if (block == SeleElePart)
                {
                    //---------Enter your code here-----------
                }
                else if (block == StrName)
                {
                    //---------Enter your code here-----------
                    if (this.SeleElePart.GetSelectedObjects().Length > 0)
                    {
                        NXOpen.Assemblies.Component eleComp = this.SeleElePart.GetSelectedObjects()[0] as NXOpen.Assemblies.Component;
                        string name = eleComp.Name + "-" + this.StrName.Value.ToUpper();
                        if (!this.EleDuplicationOfName(name))
                        {
                            this.StrName.Value = "";
                        }
                    }
                }
                else if (block == double_x)
                {
                    //---------Enter your code here-----------
                }
                else if (block == double_y)
                {
                    //---------Enter your code here-----------
                }
                else if (block == double_z)
                {
                    //---------Enter your code here-----------
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Callback Name: ok_cb
        //------------------------------------------------------------------------------
        public int ok_cb()
        {
            int errorCode = 0;
            try
            {
                errorCode = apply_cb();
                //---- Enter your callback code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                errorCode = 1;
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return errorCode;
        }

        //------------------------------------------------------------------------------
        //Callback Name: filter_cb
        //------------------------------------------------------------------------------
        public int filter_cb(NXOpen.BlockStyler.UIBlock block, NXOpen.TaggedObject selectedObject)
        {

            Part part = (selectedObject as NXOpen.Assemblies.Component).Prototype as Part;
            string partType = AttributeUtils.GetAttrForString(part, "PartType");
            if (!partType.Equals("Electrode"))
                return UFConstants.UF_UI_SEL_REJECT;
            return (NXOpen.UF.UFConstants.UF_UI_SEL_ACCEPT);
        }

        //------------------------------------------------------------------------------
        //Function Name: GetBlockProperties
        //Returns the propertylist of the specified BlockID
        //------------------------------------------------------------------------------
        public PropertyList GetBlockProperties(string blockID)
        {
            PropertyList plist = null;
            try
            {
                plist = theDialog.GetBlockProperties(blockID);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return plist;
        }


        private ElectrodeInfo SetEleInfo(ElectrodeInfo info)
        {
            info.Positioning = this.StrName.Value.ToUpper();
            info.EleSetValue[0] = info.EleSetValue[0] + this.double_x.Value;
            info.EleSetValue[1] = info.EleSetValue[1] + this.double_y.Value;
            info.EleSetValue[2] = info.EleSetValue[2] + this.double_z.Value;
            info.Positioning = this.StrName.Value.ToUpper();
            return info;
        }
        /// <summary>
        /// 判断电极是否重名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool EleDuplicationOfName(string name)
        {

            foreach (ElectrodeModel eleModel in assemble.Electrodes)
            {
                if (name.Equals(eleModel.AssembleName))
                {
                    theUI.NXMessageBox.Show("错误", NXMessageBox.DialogType.Error, "电极重名！");
                    return false;
                }
            }
            return true;
        }
    }
}
