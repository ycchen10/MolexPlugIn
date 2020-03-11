using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NXOpen;

namespace Basic
{
    /// <summary>
    /// 面上的边
    /// </summary>
    public class FaceLoopUtils
    {
        /// <summary>
        /// 查询面上的边
        /// </summary>
        /// <param name="faceTag"></param>
        /// <returns></returns>
        public static LoopList[] AskFaceLoops(NXOpen.Tag faceTag)
        {
            System.IntPtr loopT;
            NXOpen.Utilities.JAM.StartUFCall();
            int errorCode = UF_MODL_ask_face_loops(faceTag, out loopT);
            NXOpen.Utilities.JAM.EndUFCall();
            if (errorCode != 0)
            {
                throw NXOpen.NXException.Create(errorCode);
            }
            System.IntPtr ptr = loopT;
            List<LoopList> loopList = new List<LoopList>();
            while (ptr != IntPtr.Zero)
            {
                _loop_list loopListT = (_loop_list)Marshal.PtrToStructure(ptr, typeof(_loop_list));
                int count;
                errorCode = UF_MODL_ask_list_count(loopListT.edge_list, out count);
                NXOpen.Tag[] edgeArray = new NXOpen.Tag[count];
                for (int i = 0; i < count; i++)
                {
                    UF_MODL_ask_list_item(loopListT.edge_list, i, out edgeArray[i]);
                }
                //UF_MODL_delete_list(out loopListT.edge_list);
                loopList.Add(new LoopList { Type = loopListT.type, EdgeList = edgeArray });
                ptr = loopListT.next;
            }
            UF_MODL_delete_loop_list(out loopT);
            return loopList.ToArray();
        }
        [DllImport("libufun.dll", EntryPoint = "UF_MODL_ask_face_loops", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int UF_MODL_ask_face_loops(NXOpen.Tag face, out IntPtr loopList);

        [DllImport("libufun.dll", EntryPoint = "UF_MODL_ask_list_count", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int UF_MODL_ask_list_count(IntPtr list, out int count);

        [DllImport("libufun.dll", EntryPoint = "UF_MODL_ask_list_item", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int UF_MODL_ask_list_item(IntPtr list, int index, out NXOpen.Tag @object);

        [DllImport("libufun.dll", EntryPoint = "UF_MODL_delete_list", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int UF_MODL_delete_list(out IntPtr list);

        [DllImport("libufun.dll", EntryPoint = "UF_MODL_delete_loop_list", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int UF_MODL_delete_loop_list(out IntPtr list);


        internal struct _loop_list
        {
            public int type;
            public IntPtr edge_list;
            public IntPtr next;
        }

        public struct LoopList
        {
            /// <summary>
            /// Peripheral=1, Hole=2, Other=3
            /// </summary>
            public int Type;
            public NXOpen.Tag[] EdgeList;
        }
    }
}
