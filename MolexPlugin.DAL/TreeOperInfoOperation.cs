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
        private static string png = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("application\\", "Images\\");
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
            List<string> progs = eleOper.Oper.GroupBy(p => p.NameModel.ProgramName).OrderBy(p => p.Key).Select(p => p.Key).ToList();
            foreach (string ao in progs)
            {
                Node pNode = this.tree.CreateNode(ao);
                this.tree.InsertNode(pNode, null, null, Tree.NodeInsertOption.Last);
                pNode.SetColumnDisplayText(0, ao);
                pNode.SetColumnDisplayText(2, eleOper.EleModel.EleInfo.EleName);
                List<AbstractCreateOperation> oprs = eleOper.Oper.FindAll(a => a.NameModel.ProgramName.Equals(ao));

                foreach (AbstractCreateOperation an in oprs)
                {
                    AddOpeTotree(an, pNode, null, png);
                }
                pNode.Expand(Node.ExpandOption.Expand); //展开节点
            }
        }
        /// <summary>
        /// 获取所有程序Node
        /// </summary>
        /// <returns></returns>
        public List<Node> GetProgramNode()
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
                        newNode.SetColumnDisplayText(0, proName);
                        newNode.SetColumnDisplayText(2, eleOper.EleModel.EleInfo.EleName);
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
        public void UpdateTree(AbstractElectrodeOperation eleOper)
        {

            List<Node> nodes = GetProgramNode();
            for (int i = 0; i < nodes.Count; i++)
            {
                string name = "O000" + (i + 1).ToString();
                nodes[i].SetColumnDisplayText(0, name);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                foreach (AbstractCreateOperation ao in FindOperationForProNode(nodes[i], eleOper))
                {
                    ao.SetProgramName(i + 1);
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
            this.tree.DeleteNode(node);
            UpdateTree(eleOper);
        }
        /// <summary>
        /// 添加刀路
        /// </summary>
        /// <param name="node">程序Node</param>
        /// <param name="eleOper"></param>
        /// <param name="oper"></param>
        public void AddOperation(AbstractElectrodeOperation eleOper, AbstractCreateOperation oper)
        {
            Node proNode = FindNodeForProName(oper.NameModel.ProgramName);
            Node nextNode1 = proNode.NextNode;
            if (nextNode1 == null)
            {
                eleOper.Oper.Insert(eleOper.Oper.Count, oper);
                AddOpeTotree(oper, proNode, null, png);
                return;
            }
            bool sibling = true;
            while (sibling)
            {
                Node nextNode2;
                nextNode2 = nextNode1.NextSiblingNode;
                if (nextNode2 == null)
                {
                    sibling = false;
                    break;
                }
                else
                {
                    nextNode1 = nextNode2;
                }
            }
            AbstractCreateOperation ao = FindOperationForOperNode(nextNode1, eleOper);
            int index = eleOper.Oper.IndexOf(ao);
            eleOper.Oper.Insert(index + 1, oper);
            AddOpeTotree(oper, proNode, nextNode1, png);
        }
        public void AddOperation(AbstractElectrodeOperation eleOper, AbstractCreateOperation oper, Node node)
        {
            Node nextNode1 = node.FirstChildNode;
            if (nextNode1 == null)
            {
                eleOper.Oper.Insert(eleOper.Oper.Count, oper);
                AddOpeTotree(oper, node, null, png);
                return;
            }
            bool sibling = true;
            while (sibling)
            {
                Node nextNode2;
                nextNode2 = nextNode1.NextSiblingNode;
                if (nextNode2 == null)
                {
                    sibling = false;
                    break;
                }
                else
                {
                    nextNode1 = nextNode2;
                }
            }
            AbstractCreateOperation ao = FindOperationForOperNode(nextNode1, eleOper);
            int index = eleOper.Oper.IndexOf(ao);
            eleOper.Oper.Insert(index + 1, oper);
            AddOpeTotree(oper, node, nextNode1, png);
            node.Expand(Node.ExpandOption.Expand); //展开节点
        }
        /// <summary>
        /// 拷贝程序
        /// </summary>
        /// <param name="eleOper"></param>
        /// <param name="node"></param>
        public void CopyOperation(AbstractElectrodeOperation eleOper, Node node)
        {
            AbstractCreateOperation ao = FindOperationForOperNode(node, eleOper);
            AbstractCreateOperation copy = ao.CopyOperation();
            int index = eleOper.Oper.IndexOf(ao);
            eleOper.Oper.Insert(index + 1, copy);
            AddOpeTotree(copy, node.ParentNode, node, png);
        }
        /// <summary>
        /// 更新刀路
        /// </summary>
        /// <param name="eleOper"></param>
        /// <param name="ao"></param>
        public void UpdateOperation(AbstractElectrodeOperation eleOper, AbstractCreateOperation ao)
        {
            if (ao.NameModel.ProgramName.Equals(ao.Node.ParentNode.GetColumnDisplayText(0)))
            {
                ao.Node.SetColumnDisplayText(0, ao.NameModel.OperName);
                ao.Node.SetColumnDisplayText(1, ao.NameModel.ToolName);
            }
            else
            {
                eleOper.Oper.Remove(ao);
                this.tree.DeleteNode(ao.Node);
                this.AddOperation(eleOper, ao);
            }
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
            this.tree.DeleteNode(node);
        }
        /// <summary>
        /// 移动刀路
        /// </summary>
        /// <param name="eleOper"></param>
        /// <param name="node"></param>
        /// <param name="afterNode"></param>
        public void MoveOperation(AbstractElectrodeOperation eleOper, Node node, Node afterNode)
        {
            AbstractCreateOperation ao1 = FindOperationForOperNode(node, eleOper);
            AbstractCreateOperation ao2 = FindOperationForOperNode(afterNode, eleOper);
            this.AddOpeTotree(ao1, afterNode.ParentNode, afterNode, png);
            eleOper.Oper.Remove(ao1);
            int index = eleOper.Oper.IndexOf(ao2);
            eleOper.Oper.Insert(index + 1, ao1);
            this.tree.DeleteNode(node);
        }
        /// <summary>
        /// 程序组向上移动
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        public void MoveUpProgram(Node node, AbstractElectrodeOperation eleOper)
        {
            Node previous = node.PreviousSiblingNode;
            if (previous != null)
            {
                List<AbstractCreateOperation> aos = FindOperationForProNode(previous, eleOper);
                Node newNode = this.tree.CreateNode(previous.GetColumnDisplayText(0));
                this.tree.InsertNode(newNode, null, node, Tree.NodeInsertOption.Last);
                newNode.SetColumnDisplayText(0, previous.GetColumnDisplayText(0));
                newNode.SetColumnDisplayText(2, eleOper.EleModel.EleInfo.EleName);
                if (aos.Count != 0)
                {
                    foreach (AbstractCreateOperation an in aos)
                    {
                        AddOpeTotree(an, newNode, null, png);
                    }
                }
                newNode.Expand(Node.ExpandOption.Expand); //展开节点
                this.tree.DeleteNode(previous);
                UpdateTree(eleOper);
            }
        }
        /// <summary>
        /// 程序组往下移动
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        public void MoveDownProgram(Node node, AbstractElectrodeOperation eleOper)
        {
            Node sibling = node.NextSiblingNode;
            if (sibling != null)
            {
                List<AbstractCreateOperation> aos = FindOperationForProNode(node, eleOper);
                Node newNode = this.tree.CreateNode(node.GetColumnDisplayText(0));
                this.tree.InsertNode(newNode, null, sibling, Tree.NodeInsertOption.Last);
                newNode.SetColumnDisplayText(0, node.GetColumnDisplayText(0));
                newNode.SetColumnDisplayText(2, eleOper.EleModel.EleInfo.EleName);
                if (aos.Count != 0)
                {
                    foreach (AbstractCreateOperation an in aos)
                    {
                        AddOpeTotree(an, newNode, null, png);
                    }
                }
                newNode.Expand(Node.ExpandOption.Expand); //展开节点
                this.tree.DeleteNode(node);
                UpdateTree(eleOper);
            }
        }
        /// <summary>
        /// 操作往上移动
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        public void MoveUpOperation(Node node, AbstractElectrodeOperation eleOper)
        {
            Node previous = node.PreviousSiblingNode;
            if (previous != null)
            {
                AbstractCreateOperation ao = FindOperationForOperNode(previous, eleOper);
                AddOpeTotree(ao, node.ParentNode, node, png);
                int index = eleOper.Oper.IndexOf(ao);
                eleOper.Oper.Remove(ao);
                eleOper.Oper.Insert(index + 1, ao);
                this.tree.DeleteNode(previous);
            }
        }
        /// <summary>
        /// 操作往下移动
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        public void MoveDownOperation(Node node, AbstractElectrodeOperation eleOper)
        {
            Node sibling = node.NextSiblingNode;
            if (sibling != null)
            {
                AbstractCreateOperation ao = FindOperationForOperNode(node, eleOper);
                AddOpeTotree(ao, node.ParentNode, sibling, png);
                int index = eleOper.Oper.IndexOf(ao);
                eleOper.Oper.Remove(ao);
                eleOper.Oper.Insert(index - 1, ao);
                this.tree.DeleteNode(node);
            }
        }
        /// <summary>
        /// 判断Node是否是程序组
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool NodeIsProgram(Node node)
        {
            string toolName = node.GetColumnDisplayText(1);
            if (toolName == "")
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 判断Node是否是操作
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool NodeIsOperation(Node node)
        {
            string toolName = node.GetColumnDisplayText(1);
            if (toolName != "")
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 通过刀路Node 查找刀路
        /// </summary>
        /// <param name="node"></param>
        /// <param name="eleOper"></param>
        /// <returns></returns>
        public AbstractCreateOperation FindOperationForOperNode(Node node, AbstractElectrodeOperation eleOper)
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
            List<AbstractCreateOperation> ao = new List<AbstractCreateOperation>();
            Node next = node.FirstChildNode;
            if (next != null)
            {
                ao.Add(FindOperationForOperNode(next, eleOper));
                bool sibling = true;
                while (sibling)
                {
                    next = next.NextSiblingNode;
                    if (next != null)
                    {
                        ao.Add(FindOperationForOperNode(next, eleOper));
                    }
                    else
                        sibling = false;
                }
            }
            return ao;

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
        private Node AddOpeTotree(AbstractCreateOperation ao, Node pNode, Node afterNode, string pngPath)
        {
            Node node = this.tree.CreateNode(ao.NameModel.OperName);
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
