using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;

class XMLFileController {
    private XmlDocument Document = null;
    private Stack<XmlElement> NodeList = new Stack<XmlElement>();
    private string FileName = null;

    /* 打开文件
       param[FileName]：文件名
     */
    public void Open(string FileName) {
        Document = new XmlDocument();
        this.FileName = FileName;
        if(File.Exists(FileName)) {
            Document.Load(FileName);
            XmlElement root = Document.DocumentElement;
            NodeList.Push(root);
        } else {
            XmlNode node = Document.CreateXmlDeclaration("1.0", "utf-8", "");
            Document.AppendChild(node);
            XmlElement root = Document.CreateElement("root");
            Document.AppendChild(root);
            NodeList.Push(root);
        }
    }
    /* 进入一个父结点
       param[NodeName]：结点名称
     */
    public void BeginParentNode(string NodeName) {
        XmlElement currentNode = NodeList.Peek();
        XmlNodeList child = currentNode.GetElementsByTagName(NodeName);
        if(child.Count > 0)
            NodeList.Push((XmlElement)(child[0]));
        else {
            XmlElement node = Document.CreateElement(NodeName);
            currentNode.AppendChild(node);
            NodeList.Push(node);
        }
    }
    // 结束父结点
    public void EndParentNode() {
        if(NodeList.Count > 1)
            NodeList.Pop();
    }
    /* 设置当前父结点的属性
       param[key]：属性名称
       param[value]：属性的值
     */
    public void SetAttribute(string key, string value) {
        XmlElement currentNode = NodeList.Peek();
        currentNode.SetAttribute(key, value);
    }
    /* 获取当前父结点的属性
       param[key]：属性名称
       return：返回属性的值
     */
    public string GetAttribute(string key) {
        XmlElement currentNode = NodeList.Peek();
        return currentNode.GetAttribute(key);
    }
    /* 移除当前父结点属性
       param[key]：属性名称
     */
    public void RemoveAttribute(string key) {
        XmlElement currentNode = NodeList.Peek();
        currentNode.RemoveAttribute(key);
    }
    /* 移除当前父结点的指定子结点
       param[nodeName]：结点名称
     */
    public void RemoveChildNode(string nodeName = null) {
        XmlElement currentNode = NodeList.Peek();
        if(nodeName == null) 
            currentNode.RemoveAll();
        else {
            XmlNodeList child = currentNode.GetElementsByTagName(nodeName);
            currentNode.RemoveChild(child[0]);
        }
    }
    /* 设置当前父结点的指定子结点的值
       param[key]：结点名称
       param[value]：结点的值
     */
    public void SetValue(string key, string value) {
        XmlElement currentNode = NodeList.Peek();
        XmlNodeList child = currentNode.GetElementsByTagName(key);
        if(child.Count > 0)
            child[0].InnerText = value;
        else {
            XmlElement node = Document.CreateElement(key);
            node.InnerText = value;
            currentNode.AppendChild(node);
        }
    }
    /* 获取前父结点的指定子结点的值
       param[key]：结点名称
       return：结点的值
     */
    public string GetValue(string key) {
        XmlElement currentNode = NodeList.Peek();
        string result = "";
        XmlNodeList child = currentNode.GetElementsByTagName(key);
        //if(child.Count > 0 && child[0].NodeType == XmlNodeType.Text)
        result = child[0].InnerText;
        return result;
    }
    //关闭并保存
    public void Close() {
        if(Document != null)
            Document.Save(FileName);
        Document = null;
        FileName = null;
        NodeList.Clear();
    }
    //获取当前结点的所有子节点
    public LinkedList<string> GetChildren() {
        LinkedList<string> result = new LinkedList<string>();
        XmlElement currentNode = NodeList.Peek();
        XmlNodeList child = currentNode.ChildNodes;
        foreach(XmlNode c in child) {
            result.AddLast(c.Name);
        }
        return result;
    }
}
