using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TreeZe.Sdk
{
     /// <summary>
    /// BigDataTable 行数据集合
    /// </summary>
    [Serializable()]
    public sealed class BigDataItems : ISerializable,IEnumerable<Tuple<string,string>>
    {
        private readonly ConcurrentDictionary<string, string> _item;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public BigDataItems(SerializationInfo info, StreamingContext context)
        {
            _item = (ConcurrentDictionary<string, string>) info.GetValue(BigDataConstant.DataItem,
                typeof(ConcurrentDictionary<string, string>));
            if (_item == null)
            {
                _item = new ConcurrentDictionary<string, string>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BigDataItems()
        {
            _item = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(string name, T value)
        {
            string v = string.Empty;
            if (value != null)
            {
                v = value.ToString();
            }
            Add(name,v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            name = name.ToLower();
            if (!_item.ContainsKey(name))
            {
                _item.TryAdd(name, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public string this[string key]
        {
            get
            {
                key = key.ToLower();
                if (key.Length == 0)
                {
                    return string.Empty;
                }
                
                if (_item.ContainsKey(key))
                {
                    return _item[key];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, string value)
        {
            if (_item.ContainsKey(key))
            {
                _item[key]=value;
            }
            else
            {
                _item.TryAdd(key, value);
            }
        }
        
        /// <summary>
        /// 总数量
        /// </summary>
        public int Count => _item.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Tuple<string, string>> GetEnumerator()
        {
            foreach (var t in _item)
            {
                yield return new Tuple<string, string>(t.Key,t.Value);
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
             StringBuilder sb=new StringBuilder();
             if (_item.Count > 0)
             {
                 sb.Append(string.Concat("<",BigDataConstant.FieldItem,">"));
                 foreach (var k in _item)
                 {
                     sb.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", k.Key, k.Value);
                 }
                 
                 sb.Append(string.Concat("</",BigDataConstant.FieldItem,">"));
             }

             return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
      
        
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(BigDataConstant.DataItem,_item,typeof(ConcurrentDictionary<string,string>));
        } 
    }
}