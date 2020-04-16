using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;

namespace MolexPlugin.DAL
{
    /// <summary>
    /// 刀路树操作类
    /// </summary>
    public class TreeOperInfoOperation
    {
        private Tree tree;

        public TreeOperInfoOperation(Tree tree)
        {
            this.tree = tree;
        }
        /// <summary>
        /// 添加刀路
        /// </summary>
        /// <param name="eleOper"></param>
        public void AddOperTree(AbstractElectrodeOperation eleOper)
        {
            DeleteAllNode();
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string png = dllPath.Replace("application\\", "Images\\");
            List<string> progs = eleOper.Oper.GroupBy(p => p.NameModel.ProgramName).OrderBy(p => p.Key).Select(p => p.Key).ToList();
            foreach (string ao in progs)
            {
                Node pNode = this.tree.CreateNode(ao);
                this.tree.InsertNode(pNode, null, null, Tree.NodeInsertOption.Last);
                pNode.SetColumnDisplayText(0, ao);
                List<AbstractCreateOperation> oprs = eleOper.Oper.FindAll(a => a.NameModel.ProgramName.Equals(ao));
                int i = 1;
                foreach (AbstractCreateOperation an in oprs)
                {
                    AddOpeTotree(an, pNode, null, i.ToString(), png);
                    i++;
                }
                pNode.Expand(Node.ExpandOption.Expand); //展开节点
            }
        }
        /// <summary>
        /// 获取所有程序Node
        /// </summary>
        /// <returns></returns>
        private List<Node> GetProgramNode()
        {
            List<Node> pro = new List<Node>();
            Node root = tree.RootNode;
            if (root == null)
                return pro;
            pro.Add(root);
            bool sibling = true;
            while (sibling)
            {
                root = root.NextSiblingNode;
                if (root != null)
                    pro.Add(root);
                else
                    sibling = false;
            }
            return pro;
        }
        /// <summary>
        /// 添加程序组
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        /// <returns></returns>
        public Node AddProgramNode(Node node, AbstractElectrodeOperation eleOper)
        {
            List<Node> nodes = GetProgramNode();
            if (nodes.Count != 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (node.Equals(nodes[i]))
                    {
                        string proName = "O000" + (i + 2).ToString();
                        Node newNode = this.tree.CreateNode(proName);
                        this.tree.InsertNode(newNode, null, node, Tree.NodeInsertOption.Last);
                        UpdateTree(eleOper);
                        return newNode;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 更新树
        /// </summary>
        /// <param name="eleOper"></param>
        private void UpdateTree(AbstractElectrodeOperation eleOper)
        {
            List<Node> nodes = GetProgramNode();
            for (int i = 0; i < nodes.Count; i++)
            {
                string oldProName = nodes[i].GetColumnDisplayText(0);
                string newProName = "O000" + (i + 1).ToString();
                nodes[i].SetColumnDisplayText(0, newProName);
                Node next = nodes[i].NextNode;
                if (next != null)
                {
                    List<AbstractCreateOperation> opers = eleOper.Oper.FindAll(a => a.NameModel.ProgramName.Equals(oldProName, StringComparison.CurrentCultureIgnoreCase));
                    foreach (AbstractCreateOperation ao in opers)
                    {
                        ao.SetProgramName(i + 1);
                    }
                }
            }
        }
        /// <summary>
        /// 删除全部Node
        /// </summary>
        private void DeleteAllNode()
        {
            foreach (Node nd in GetProgramNode())
            {
                this.tree.DeleteNode(nd);
            }
        }
        /// <summary>
        /// 删除程序组
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        public void DeleteProgramNode(Node node, AbstractElectrodeOperation eleOper)
        {
            string oldProName = node.GetColumnDisplayText(0);
            List<AbstractCreateOperation> opers = FindOperationForProNode(node, eleOper);
            foreach (AbstractCreateOperation ao in opers)
            {
                eleOper.Oper.Remove(ao);

            }
        }
        /// <summary>
        /// 添加刀路
        /// </summary>
        /// <param name="node">程序Node</param>
        /// <param name="eleOper"></param>
        /// <param name="oper"></param>
        public void AddOperation(AbstractElectrodeOperation eleOper, AbstractCreateOperation oper, Node node = null)
        {
            Node proNode = FindNodeForProName(oper.NameModel.ProgramName);
            string dllPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string png = dllPath.Replace("application\\", "Images\\");
            if (node != null)
            {
                AbstractCreateOperation ao = FindOperationForOperNode(node, eleOper);
                int index = eleOper.Oper.IndexOf(ao);
                eleOper.Oper.Insert(index, oper);
                if (proNode.Equals(node.ParentNode))
                    AddOpeTotree(oper, proNode, node, oper.NameModel.OperName, png);
                else
                    AddOpeTotree(oper, proNode, null, oper.NameModel.OperName, png);

            }
            else
                AddOpeTotree(oper, proNode, null, oper.NameModel.OperName, png);
        }
        /// <summary>
        /// 删除刀路
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        /// <param name="oper"></param>
        public void DeleteOperation(Node node, AbstractElectrodeOperation eleOper)
        {
            eleOper.Oper.Remove(FindOperationForOperNode(node, eleOper));
        }
        /// <summary>
        /// 通过刀路Node 查找刀路
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        /// <returns></returns>
        private AbstractCreateOperation FindOperationForOperNode(Node node, AbstractElectrodeOperation eleOper)
        {
            return eleOper.Oper.Find(a => a.Node.Equals(node));

        }
        /// <summary>
        /// 通过程序组Node 查找刀路
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        /// <returns></returns>
        private List<AbstractCreateOperation> FindOperationForProNode(Node node, AbstractElectrodeOperation eleOper)
        {
            return eleOper.Oper.FindAll(a => a.Node.Equals(node));

        }
        /// <summary>
        /// 通过程序组名查找Node
        /// </summary>
        /// <param name="proName"></param>
        /// <returns></returns>
        private Node FindNodeForProName(string proName)
        {
            List<Node> proNode = GetProgramNode();
            return proNode.Find(a => a.GetColumnDisplayText(0).Equals(proName, StringComparison.CurrentCultureIgnoreCase));
        }
        /// <summary>
        /// 把刀路插入到程序组下面
        /// </summary>
        /// <param name="ao"></param>
        /// <param name="pNode"></param>
        /// <param name="afterNode"></param>
        /// <param name="name"></param>
        /// <param name="pngPath"></param>
        /// <returns></returns>
        private Node AddOpeTotree(AbstractCreateOperation ao, Node pNode, Node afterNode, string name, string pngPath)
        {
            Node node = this.tree.CreateNode(name);
            this.tree.InsertNode(node, pNode, afterNode, Tree.NodeInsertOption.Last);
            node.SetColumnDisplayText(0, ao.NameModel.OperName);
            node.DisplayIcon = pngPath + ao.NameModel.PngName;
            node.SelectedIcon = pngPath + ao.NameModel.PngName;
            node.SetColumnDisplayText(1, ao.NameModel.ToolName);
            ao.Node = node;
            return node;
        }
    }
}
