using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Basic
{
    public class AttributeUtils : ClassItem
    {
        #region 写和修改属性
        /// <summary>
        /// 属性操作
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="value">值</param>
        /// <param name="attrObj"></param>
        /// <returns></returns>
        public static bool AttributeOperation(string title, string value = "", params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;
            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.StringValue = value;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }

        public static bool AttributeOperation(string title, string[] value, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;

            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.IsArray = true;
            for (int i = 0; i < value.Length; i++)
            {
                attributePropertiesBuilder1.ArrayIndex = i;
                attributePropertiesBuilder1.StringValue = value[i];
                attributePropertiesBuilder1.CreateAttribute();
            }

            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }
        public static bool AttributeOperation(string title, int value = 0, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer;
            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.IntegerValue = value;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }

        public static bool AttributeOperation(string title, int[] value, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer;
            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.IsArray = true;
            for (int i = 0; i < value.Length; i++)
            {
                attributePropertiesBuilder1.ArrayIndex = i;
                attributePropertiesBuilder1.IntegerValue = value[i];
                attributePropertiesBuilder1.CreateAttribute();
            }

            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }

        public static bool AttributeOperation(string title, double value = 0, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Number;
            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.NumberValue = value;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }

        public static bool AttributeOperation(string title, double[] value, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Number;
            attributePropertiesBuilder1.Title = title;
            attributePropertiesBuilder1.IsArray = true;
            for (int i = 0; i < value.Length; i++)
            {
                attributePropertiesBuilder1.ArrayIndex = i;
                attributePropertiesBuilder1.NumberValue = value[i];
                attributePropertiesBuilder1.CreateAttribute();
            }

            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }

        public static bool AttributeOperation(string title, bool value = true, params NXObject[] attrObj)
        {

            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = theSession.AttributeManager.CreateAttributePropertiesBuilder(attrObj[0].OwningPart, attrObj, NXOpen.AttributePropertiesBuilder.OperationType.None);
            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Boolean;
            attributePropertiesBuilder1.Title = title;
            if (value)
                attributePropertiesBuilder1.BooleanValue = AttributePropertiesBaseBuilder.BooleanValueOptions.True;
            else
                attributePropertiesBuilder1.BooleanValue = AttributePropertiesBaseBuilder.BooleanValueOptions.False;
            try
            {
                NXOpen.NXObject nXObject1;
                nXObject1 = attributePropertiesBuilder1.Commit();
                return true;
            }
            catch (Exception ex)
            {
                LogMgr.WriteLog("AttributeUtils:AttributeOperation:" + ex.Message);
                return false;
            }
            finally
            {

                attributePropertiesBuilder1.Destroy();
            }

        }
        #endregion
        #region 获取属性
        /// <summary>
        /// 获取字符串属性
        /// </summary>
        /// <param name="objTag">需要获取属性的Tag值</param>
        /// <param name="title">属性标题</param>
        /// <returns>属性值</returns>
        public static string GetAttrForString(NXObject obj, string title)
        {
            string value = "";
            bool hasAttribute = false;
            try
            {
                theUFSession.Attr.GetStringUserAttribute(obj.Tag, title, UFConstants.UF_ATTR_NOT_ARRAY, out value, out hasAttribute);  //获取属性
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
            }
            finally
            {
                if (!hasAttribute)
                    LogMgr.WriteLog("未获取" + title + "属性");
            }
            return value;

        }

        public static string GetAttrForString(NXObject obj, string title, int index)
        {

            try
            {
                return obj.GetStringUserAttribute(title, index);  //获取属性
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
                return " ";
            }

        }
        /// <summary>
        /// 获取int属性
        /// </summary>
        /// <param name="objTag">需要获取属性的Tag值</param>
        /// <param name="title">属性标题</param>
        /// <returns>属性值</returns>
        public static int GetAttrForInt(NXObject obj, string title)
        {
            int value = 0;
            bool hasAttribute = false;
            try
            {
                theUFSession.Attr.GetIntegerUserAttribute(obj.Tag, title, UFConstants.UF_ATTR_NOT_ARRAY, out value, out hasAttribute);
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
            }
            finally
            {
                if (!hasAttribute)
                    LogMgr.WriteLog("未获取" + title + "属性");
            }
            return value;

        }
        public static int GetAttrForInt(NXObject obj, string title, int index)
        {

            try
            {
                return obj.GetIntegerUserAttribute(title, index);  //获取属性
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
                return 0;
            }

        }

        /// <summary>
        /// 获取bool属性
        /// </summary>
        /// <param name="objTag">需要获取属性的Tag值</param>
        /// <param name="title">属性标题</param>
        /// <returns>属性值</returns>
        public static bool GetAttrForBool(NXObject obj, string title)
        {
            bool value = false;
            bool hasAttribute = false;
            try
            {
                theUFSession.Attr.GetBoolUserAttribute(obj.Tag, title, UFConstants.UF_ATTR_NOT_ARRAY, out value, out hasAttribute);
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
            }
            finally
            {
                if (!hasAttribute)
                    LogMgr.WriteLog("未获取" + title + "属性");
            }
            return value;

        }
        public static bool GetAttrForBool(NXObject obj, string title, int index)
        {

            try
            {
                return obj.GetBooleanUserAttribute(title, index);  //获取属性
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
                return false;
            }

        }
        /// <summary>
        /// 获取double属性
        /// </summary>
        /// <param name="objTag">需要获取属性的Tag值</param>
        /// <param name="title">属性标题</param>
        /// <returns>属性值</returns>
        public static double GetAttrForDouble(NXObject obj, string title)
        {
            double value = 0;
            Tag temp;
            bool hasAttribute = false;
            try
            {

                theUFSession.Attr.GetRealUserAttribute(obj.Tag, title, UFConstants.UF_ATTR_NOT_ARRAY, out value, out temp, out hasAttribute);
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
            }
            finally
            {
                if (!hasAttribute)
                    LogMgr.WriteLog("未获取" + title + "属性");
            }
            return value;

        }
        public static double GetAttrForDouble(NXObject obj, string title, int index)
        {

            try
            {
                return obj.GetRealUserAttribute(title, index);  //获取属性
            }
            catch
            {
                LogMgr.WriteLog("获取" + title + "属性函数错误");
                return 0;
            }

        }

        #endregion

    }
}
