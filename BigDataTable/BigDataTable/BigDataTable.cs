using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace TreeZe.Sdk
{
    /// <summary>
    /// BigDataTable 数据集合
    /// </summary>
    [Serializable()]
    public sealed class BigDataTable : IEnumerable<BigDataItems>
    {
        #region "Private"

        /// <summary>
        /// 
        /// </summary>
        private ConcurrentBag<BigDataItems> _dic;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public BigDataTable()
        {
            _dic = new ConcurrentBag<BigDataItems>();
        }

        /// <summary>
        /// 获取所有集合的数量
        /// </summary>
        public int Count => _dic.Count;

        /// <summary>
        /// 获取指定序号值
        /// </summary>
        /// <param name="index"></param>
        public BigDataItems this[int index]
        {
            get
            {
                if (index < 0 || index >= _dic.Count)
                {
                    return new BigDataItems();
                }

                int cindex = 0;
                foreach (BigDataItems dr in _dic)
                {
                    if (cindex == index)
                    {
                        return dr;
                    }

                    cindex += 1;
                }
                
                return new BigDataItems();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        public void Add(BigDataItems dr)
        {
            _dic.Add(dr);
        }
        
        #region "XML"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        private void ReadXML(XmlReader r)
        {
            if (r.IsEmptyElement || !r.Read())
            {
                return;
            }

            bool hasitem = false;
            BigDataItems item = null;
            string n;
            while (r.Read())
            {
                switch (r.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!hasitem)
                        {
                            if (r.Name.Equals(BigDataConstant.FieldItem))
                            {
                                item = new BigDataItems();
                                hasitem = true;
                            }
                        }
                        else
                        {
                            if (r.Name.Equals(BigDataConstant.FieldItem))
                            {
                                item = new BigDataItems();
                            }
                            else
                            {
                                n = r.Name;
                                if (r.Read())
                                {
                                    item.Add(n, r.Value);
                                }
                            }
                        }

                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (r.Name.Equals(BigDataConstant.FieldItem))
                        {
                            _dic.Add(item);
                        }

                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        private void WriteXml(XmlWriter w)
        {
            w.WriteStartDocument();
            w.WriteStartElement(BigDataConstant.FieldCollection);

            foreach (BigDataItems b in _dic)
            {
                w.WriteStartElement(BigDataConstant.FieldItem);
                foreach (var a in b)
                {
                    w.WriteStartElement(a.Item1);
                    w.WriteString(a.Item2);
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            w.WriteEndElement();
            w.WriteEndDocument();
            w.Flush();
            w.Close();
        }

        #endregion

        #region "Enumerator"

        /// <summary>
        /// GetEnumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<BigDataItems> GetEnumerator()
        {
            foreach (var b in _dic)
            {
                yield return b;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region "Load"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <exception cref="Exception"></exception>
        public void Load(IDataReader r)
        {
            if (r == null)
            {
                return;
            }

            try
            {
                int cl = r.FieldCount;
                HashSet<string> hl = new HashSet<string>();
                for (int i = 0; i < cl; i++)
                {
                    hl.Add(r.GetName(i));
                }

                while (r.Read())
                {
                    BigDataItems dr = new BigDataItems();
                    foreach (string s in hl)
                    {
                        var index = r.GetOrdinal(s);
                        if (r.GetValue(index) != DBNull.Value)
                        {
                            dr.Add(s, r.GetValue(index));
                        }
                        else
                        {
                            dr.Add(s, "");
                        }
                    }

                    _dic.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void Load(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
            {
                return;
            }

            byte[] b = Encoding.UTF8.GetBytes(str);
            Load(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Load(byte[] b)
        {
            XmlReader r;
            try
            {
                if (b == null || b.Length == 0)
                {
                    return;
                }

                Stream s = new MemoryStream();
                r = XmlReader.Create(s);
                ReadXML(r);
            }
            catch (Exception ex)
            {
                throw new Exception("BigDataTable Load Byte data Error:" + ex);
            }
        }

        #endregion

        #region "ToData"

        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            StringWriterWithEncoding sw = new StringWriterWithEncoding(sb, Encoding.UTF8);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8, Indent = false, CloseOutput = true, NewLineOnAttributes = false
            };
            XmlWriter w = XmlWriter.Create(sw, settings);
            WriteXml(w);
            return sb.ToString();
        }

        /// <summary>
        /// To Byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            string s = ToString();
            return Encoding.UTF8.GetBytes(s);
        }

        #endregion
        
    }
}