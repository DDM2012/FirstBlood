using System;
using System.Data;
using System.IO;
using System.Xml;

namespace XML
{
    class XMLProcessor
    {
        #region "Constructor"
        public XMLProcessor()
        { }

        public XMLProcessor(string xmlPath)
        {
            this._xmlPath = xmlPath;
        }
        #endregion

        #region "Properties"
        private string _xmlPath;
        public string XmlPath { get; set; }
        #endregion

        private XmlDocument LoadXmlDoc()
        {
            string xmlPath = XmlPath;
            XmlDocument xmlDoc = new XmlDocument();
            string fileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + xmlPath;
            if (File.Exists(fileName)) xmlDoc.Load(fileName);
            return xmlDoc;
        }

        private static XmlDocument LoadXmlDoc(string xmlPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string fileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + xmlPath;
            if (File.Exists(fileName)) xmlDoc.Load(fileName);
            return xmlDoc;
        }

        private static string GetXmlFullPath(string xmlPath)
        {
            if (xmlPath.IndexOf(":") > 0)
            {
                return xmlPath;
            }
            else
            {
                return System.Web.HttpContext.Current.Server.MapPath(xmlPath);
            }
        }

        public string Read(string node)
        {
            string value = "";
            try
            {
                XmlDocument doc = LoadXmlDoc();
                XmlNode xn = doc.SelectSingleNode(node);
                value = xn.InnerText;
            }
            catch { }
            return value;
        }

        public static string Read(string path, string node)
        {
            string value = "";
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = xn.InnerText;
            }
            catch { }
            return value;
        }

        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        public string[] ReadAllChildallValue(string node)
        {
            int i = 0;
            string[] str = { };
            XmlDocument doc = LoadXmlDoc();
            XmlNode xn = doc.SelectSingleNode(node);
            XmlNodeList nodelist = xn.ChildNodes;  //得到该节点的子节点
            if (nodelist.Count > 0)
            {
                str = new string[nodelist.Count];
                foreach (XmlElement el in nodelist)//读元素值
                {
                    str[i] = el.Value;
                    i++;
                }
            }
            return str;
        }

        public XmlNodeList ReadAllChild(string node)
        {
            XmlDocument doc = LoadXmlDoc();
            XmlNode xn = doc.SelectSingleNode(node);
            XmlNodeList nodelist = xn.ChildNodes;  //得到该节点的子节点
            return nodelist;
        }

        public DataView GetDataViewByXml(string strWhere, string strSort)
        {
            try
            {
                string XMLFile = this.XmlPath;
                string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + XMLFile;
                DataSet ds = new DataSet();
                ds.ReadXml(filename);
                DataView dv = new DataView(ds.Tables[0]); //创建DataView来完成排序或筛选操作	
                if (strSort != null)
                {
                    dv.Sort = strSort; //对DataView中的记录进行排序
                }
                if (strWhere != null)
                {
                    dv.RowFilter = strWhere; //对DataView中的记录进行筛选，找到我们想要的记录
                }
                return dv;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSet GetDataSetByXml(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables.Count > 0)
                {
                    return ds;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static void Insert(string path, string node, string element, string[][] strList)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = doc.CreateElement(element);
                string strAttribute = "";
                string strValue = "";
                for (int i = 0; i < strList.Length; i++)
                {
                    for (int j = 0; j < strList[i].Length; j++)
                    {
                        if (j == 0)
                            strAttribute = strList[i][j];
                        else
                            strValue = strList[i][j];
                    }
                    if (strAttribute.Equals(""))
                        xe.InnerText = strValue;
                    else
                        xe.SetAttribute(strAttribute, strValue);
                }
                xn.AppendChild(xe);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static bool WriteXmlByDataSet(string strXmlPath, string[] Columns, string[] ColumnValue)
        {
            try
            {
                //根据传入的XML路径得到.XSD的路径，两个文件放在同一个目录下
                string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
                DataSet ds = new DataSet();
                ds.ReadXmlSchema(GetXmlFullPath(strXsdPath)); //读XML架构，关系到列的数据类型
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                DataTable dt = ds.Tables[0];
                DataRow newRow = dt.NewRow();                 //在原来的表格基础上创建新行
                for (int i = 0; i < Columns.Length; i++)      //循环给一行中的各个列赋值
                {
                    newRow[Columns[i]] = ColumnValue[i];
                }
                dt.Rows.Add(newRow);
                dt.AcceptChanges();
                ds.AcceptChanges();
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Update(string node, string value)
        {
            try
            {
                XmlDocument doc = LoadXmlDoc();
                XmlNode xn = doc.SelectSingleNode(node);
                xn.InnerText = value;
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + XmlPath);
            }
            catch { }
        }

        public static void Update(string path, string node, string value)
        {
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                xn.InnerText = value;
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static bool UpdateXmlRow(string strXmlPath, string[] Columns, string[] ColumnValue, string strWhereColumnName, string strWhereColumnValue)
        {
            try
            {
                string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
                DataSet ds = new DataSet();
                ds.ReadXmlSchema(GetXmlFullPath(strXsdPath));//读XML架构，关系到列的数据类型
                ds.ReadXml(GetXmlFullPath(strXmlPath));

                //先判断行数
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //如果当前记录为符合Where条件的记录
                        if (ds.Tables[0].Rows[i][strWhereColumnName].ToString().Trim().Equals(strWhereColumnValue))
                        {
                            //循环给找到行的各列赋新值
                            for (int j = 0; j < Columns.Length; j++)
                            {
                                ds.Tables[0].Rows[i][Columns[j]] = ColumnValue[j];
                            }
                            ds.AcceptChanges();                     //更新DataSet
                            ds.WriteXml(GetXmlFullPath(strXmlPath));//重新写入XML文件
                            return true;
                        }
                    }

                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Delete(string path, string node)
        {
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                xn.ParentNode.RemoveChild(xn);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = LoadXmlDoc(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory.ToString() + path);
            }
            catch { }
        }

        public static bool DeleteXmlAllRows(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                }
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteXmlRowByIndex(string strXmlPath, int iDeleteRow)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[iDeleteRow].Delete();
                }
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteXmlRows(string strXmlPath, string strColumn, string[] ColumnValue)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //判断行多还是删除的值多，多的for循环放在里面
                    if (ColumnValue.Length > ds.Tables[0].Rows.Count)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < ColumnValue.Length; j++)
                            {
                                if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
                                {
                                    ds.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < ColumnValue.Length; j++)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
                                {
                                    ds.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                    }
                    ds.WriteXml(GetXmlFullPath(strXmlPath));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
