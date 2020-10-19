using System;
using System.Collections.Generic;

namespace 自定义容器
{
        /// <summary>
        /// 数据树
        /// </summary>
        /// <typeparam name="T">存储类型</typeparam>
        public class DataTree<T>
        {
                private string _Name;

                /// <summary>
                /// 节点名，自动全小写
                /// </summary>
                public string Name { get => _Name; set { _Name = value.ToLower(); } }

                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="name">节点名</param>
                public DataTree(string name)
                {
                        Name = name;
                        Root = this;
                }
                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="name">节点名</param>
                /// <param name="data">节点数据</param>
                public DataTree(string name, T data)
                {
                        Name = name;
                        Data = data;
                        Root = this;
                }

                /// <summary>
                /// 模拟路径获取节点
                /// </summary>
                /// <param name="Uri">数据路径</param>
                /// <returns>节点</returns>
                public DataTree<T> GetNodeByUri(string Uri)
                {
                        if (string.IsNullOrEmpty(Uri)) return null;
                        string[] names = Uri.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (names.Length < 1) return null;
                        DataTree<T> rnode = FindNode(names[0]);

                        for (int i = 1; i < names.Length; i++)
                        {
                                if (rnode == null) return null;
                                rnode = rnode.FindNode(names[i]);
                        }

                        return rnode;
                }

                /// <summary>
                /// 查找子节点（默认"."返回当前节点，".."返回父节点，"..."返回根节点）
                /// </summary>
                /// <param name="nodeName">子节点名（不区分大小写）</param>
                /// <returns>子节点</returns>
                public DataTree<T> FindNode(string nodeName)
                {
                        if (string.IsNullOrEmpty(nodeName)) return null;
                        if (Nodes == null || Nodes.Count < 1)
                        {
                                if (nodeName.Equals(".")) return this;
                                if (nodeName.Equals("..")) return Parent;
                                if (nodeName.Equals("...")) return Root;
                                return null;
                        }

                        nodeName = nodeName.ToLower();//全部转为小写
                        if (Nodes.ContainsKey(nodeName))
                        {
                                return Nodes[nodeName];
                        }

                        if (nodeName.Equals(".")) return this;
                        if (nodeName.Equals("..")) return Parent;
                        if (nodeName.Equals("...")) return Root;
                        return null;
                }

                /// <summary>
                /// 父结点
                /// </summary>
                public DataTree<T> Parent { get; private set; }

                /// <summary>
                /// 根节点
                /// </summary>
                public DataTree<T> Root { get; private set; }

                /// <summary>
                /// 结点数据
                /// </summary>
                public T Data { get; set; }

                /// <summary>
                /// 子结点（Key自动全小写，手动添加请用小写Key,否则无法通过路径查询）
                /// </summary>
                public Dictionary<string, DataTree<T>> Nodes { get; private set; }

                /// <summary>
                /// 添加结点
                /// </summary>
                /// <param name="node">结点</param>
                public DataTree<T> AddNode(DataTree<T> node)
                {
                        if (Nodes == null)
                        {
                                Nodes = new Dictionary<string, DataTree<T>>();
                        }
                        if (Nodes.ContainsKey(node.Name))
                        {
                                Nodes[node.Name].RemoveAll();
                        }
                        node.Parent = this;
                        node.Root = Root;
                        Nodes[node.Name] = node;
                        return node;
                }

                /// <summary>
                /// 添加结点
                /// </summary>
                /// <param name="name">节点名</param>
                /// <param name="data">节点数据</param>
                public DataTree<T> AddNode(string name, T data = default(T))
                {
                        name = name.ToLower();
                        if (Nodes == null)
                        {
                                Nodes = new Dictionary<string, DataTree<T>>();
                        }
                        if (Nodes.ContainsKey(name))
                        {
                                Nodes[name].RemoveAll();
                        }
                        var node = new DataTree<T>(name, data);
                        node.Parent = this;
                        node.Root = Root;
                        Nodes[node.Name] = node;
                        return node;
                }

                /// <summary>
                /// 添加多个结点
                /// </summary>
                /// <param name="nodes">结点集合</param>
                public void AddNodes(List<DataTree<T>> nodes)
                {
                        if (Nodes == null)
                        {
                                Nodes = new Dictionary<string, DataTree<T>>();
                        }
                        foreach (DataTree<T> node in nodes)
                        {
                                if (Nodes.ContainsKey(node.Name))
                                {
                                        Nodes[node.Name].RemoveAll();
                                }
                                node.Parent = this;
                                node.Root = Root;
                                Nodes[node.Name] = node;
                        }
                }

                /// <summary>
                /// 移除指定名字的结点
                /// </summary>
                /// <param name="nodename">节点名字</param>
                public DataTree<T> Remove(string nodename)
                {
                        if (Nodes == null) return null;
                        nodename = nodename.ToLower();
                        DataTree<T> node = null;
                        if (!Nodes.ContainsKey(nodename)) return node;
                        node = Nodes[nodename];
                        Nodes.Remove(nodename);
                        return node;
                }

                /// <summary>
                /// 清空所有结点
                /// </summary>
                public void RemoveAll()
                {
                        Nodes?.Clear();
                }

                public override string ToString()
                {
                        return $"{Name} : {Data}";
                }
        }
}