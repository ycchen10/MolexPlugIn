﻿//==============================================================================
//  WARNING!!  This file is overwritten by the Block UI Styler while generating
//  the automation code. Any modifications to this file will be lost after
//  generating the code again.
//
//       Filename:  C:\Users\ycchen10\OneDrive - kochind.com\Desktop\MolexPlugIn-12.0\UI\DeleteEle.cs
//
//        This file was generated by the NX Block UI Styler
//        Created by: ycchen10
//              Version: NX 11
//              Date: 03-10-2020  (Format: mm-dd-yyyy)
//              Time: 08:40 (Format: hh-mm)
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
using System.IO;
using NXOpen.UF;
using Basic;
using MolexPlugin.Model;
namespace MolexPlugin
{


    //------------------------------------------------------------------------------
    //Represents Block Styler application class
    //------------------------------------------------------------------------------
    public class DeleteEle
    {
        //class members
        private static Session theSession = null;
        private static UI theUI = null;
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group group0;// Block type: Group
        private NXOpen.BlockStyler.SelectObject selection_Ele;// Block type: Selection

        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public DeleteEle()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                theDlxFileName = "DeleteEle.dlx";
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
                selection_Ele = (NXOpen.BlockStyler.SelectObject)theDialog.TopBlock.FindBlock("selection_Ele");

                Selection.MaskTriple maskComp = new Selection.MaskTriple()
                {
                    Type = 63,
                    Subtype = 1,
                    SolidBodySubtype = 0
                };
                Selection.MaskTriple[] masks = { maskComp };
                selection_Ele.SetSelectionFilter(Selection.SelectionAction.ClearAndEnableSpecific, masks);//过滤只选择组件
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
                //---- Enter your callback code here -----

                Session.UndoMarkId markId = Session.GetSession().SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "删除电极");
                ElectrodeModel ele = new ElectrodeModel();
                NXOpen.Assemblies.Component eleComp = this.selection_Ele.GetSelectedObjects()[0] as NXOpen.Assemblies.Component;
                Part elePart = eleComp.Prototype as Part;
                ele.GetModelForPart(elePart);
                int ok = theUI.NXMessageBox.Show("删除", NXOpen.NXMessageBox.DialogType.Question, elePart.Name + "电极是否删除");
                if (ok == 1)
                {
                    string path = elePart.FullPath;
                    elePart.Close(NXOpen.BasePart.CloseWholeTree.False, NXOpen.BasePart.CloseModified.UseResponses, null);
                    AssmbliesUtils.DeleteComponent(eleComp);
                    LayerUtils.MoveDisplayableObject(ele.EleInfo.EleNumber + 10, LayerUtils.GetAllObjectsOnLayer(ele.EleInfo.EleNumber + 100));
                    if (File.Exists(path))
                        File.Delete(path);
                }
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
                if (block == selection_Ele)
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

    }
}
