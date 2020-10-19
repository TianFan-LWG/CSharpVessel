using System.Collections;
using System.Collections.Generic;

namespace 自定义容器
{
        /// <summary>
        ///         数据对
        /// </summary>
        /// <typeparam name="TName">数据名</typeparam>
        /// <typeparam name="TValue">数据值</typeparam>
        public class NameValuePair<TName, TValue>
        {
                /// <summary>
                ///         构造函数
                /// </summary>
                /// <param name="name">数据名</param>
                /// <param name="value">数据值</param>
                public NameValuePair(TName name, TValue value)
                {
                        Name = name;
                        Value = value;
                }

                /// <summary>
                ///         数据名
                /// </summary>
                public TName Name { get; set; }

                /// <summary>
                ///         数据值
                /// </summary>
                public TValue Value { get; set; }

                /// <summary>
                ///         重写ToString()方法
                /// </summary>
                /// <returns>[Name]=Value</returns>
                public override string ToString()
                {
                        return $"[{Name}]={Value}";
                }
        }

        /// <summary>
        ///         数据对容器
        /// </summary>
        /// <typeparam name="TName">数据名</typeparam>
        /// <typeparam name="TValue">数据值</typeparam>
        public class DataPair<TName, TValue> : IEnumerable<NameValuePair<TName, TValue>>
        {
                private readonly List<NameValuePair<TName, TValue>> _data;

                /// <summary>
                ///         数据个数
                /// </summary>
                public int Count => _data.Count;

                /// <summary>
                ///         索引器
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>数据值</returns>
                public TValue this[TName name]
                {
                        get => GetFirstValue(name);
                        set => Add(name, value);
                }

                #region Other

                public IEnumerator<NameValuePair<TName, TValue>> GetEnumerator()
                {
                        //foreach (var nameValuePair in _Data)
                        //{
                        //        yield return nameValuePair;
                        //}

                        //return _Data.GetEnumerator();

                        return new DataPairEnumerator(this);
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                //迭代器
                private class DataPairEnumerator : IEnumerator<NameValuePair<TName, TValue>>
                {
                        private DataPair<TName, TValue> _data;
                        private int _position = -1;

                        public DataPairEnumerator(DataPair<TName, TValue> data)
                        {
                                _data = data;
                        }

                        public NameValuePair<TName, TValue> Current => _data._data[_position];

                        object IEnumerator.Current => Current;

                        public void Dispose()
                        {
                                _position = -1;
                                _data = null;
                        }

                        public bool MoveNext()
                        {
                                if (_position < _data._data.Count - 1)
                                {
                                        _position++;
                                        return true;
                                }

                                return false;
                        }

                        public void Reset()
                        {
                                _position = -1;
                        }
                }

                #endregion Other

                #region 构造函数

                /// <summary>
                ///         默认构造
                /// </summary>
                public DataPair()
                {
                        _data = new List<NameValuePair<TName, TValue>>();
                }

                /// <summary>
                ///         基于已有对象构造
                /// </summary>
                /// <param name="dataPair">数据对象</param>
                /// <param name="copy">是否拷贝内容，false则直接引用</param>
                public DataPair(DataPair<TName, TValue> dataPair, bool copy = false)
                {
                        _data = new List<NameValuePair<TName, TValue>>();
                        if (copy)
                                foreach (var data in dataPair)
                                {
                                        _data.Add(new NameValuePair<TName, TValue>(data.Name, data.Value));
                                }
                        else
                                foreach (var data in dataPair)
                                        _data.Add(data);
                }

                /// <summary>
                ///         基于已有数据构造
                /// </summary>
                /// <param name="datas">已有数据</param>
                /// <param name="copy">是否拷贝内容，false则直接引用</param>
                public DataPair(IEnumerable<NameValuePair<TName, TValue>> datas, bool copy = false)
                {
                        _data = new List<NameValuePair<TName, TValue>>();
                        if (datas == null)
                                return;
                        if (copy)
                        {
                                foreach (var data in datas)
                                        _data.Add(new NameValuePair<TName, TValue>(data.Name, data.Value));
                        }
                        else
                        {
                                foreach (var data in datas)
                                        _data.Add(data);
                        }
                }

                #endregion 构造函数

                #region 添加

                /// <summary>
                ///         添加数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <param name="value">数据值</param>
                public void Add(TName name, TValue value)
                {
                        _data.Add(new NameValuePair<TName, TValue>(name, value));
                }

                /// <summary>
                ///         从列表添加数据对
                /// </summary>
                /// <param name="datas">数据列表</param>
                /// <param name="copy">是否拷贝内容，false则直接引用</param>
                public void AddValues(IEnumerable<NameValuePair<TName, TValue>> datas, bool copy = false)
                {
                        if (datas == null) return;
                        if (Equals(datas, this))
                        {
                                var temp = datas;
                                datas = new DataPair<TName, TValue>(temp);
                        }

                        if (copy)
                                foreach (var data in datas) _data.Add(new NameValuePair<TName, TValue>(data.Name, data.Value));
                        else
                                foreach (var data in datas) _data.Add(data);
                }

                #endregion 添加

                #region 获取

                /// <summary>
                ///         获取所有同名的数据值
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>数据值列表</returns>
                public List<TValue> GetValuesByName(TName name)
                {
                        var values = new List<TValue>();
                        foreach (var nv in _data)
                                if (nv.Name.Equals(name))
                                        values.Add(nv.Value);
                        return values;
                }

                /// <summary>
                ///         获取所有同值的数据名
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>数据名列表</returns>
                public List<TName> GetNamesByValue(TValue value)
                {
                        var names = new List<TName>();
                        foreach (var nv in _data)
                                if (nv.Value.Equals(value))
                                        names.Add(nv.Name);
                        return names;
                }

                //=========================================================================//

                /// <summary>
                ///         获取首个匹配数据名的数据值
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>数据值</returns>
                public TValue GetFirstValue(TName name)
                {
                        foreach (var nv in _data)
                                if (nv.Name.Equals(name))
                                        return nv.Value;
                        return default;
                }

                /// <summary>
                ///         获取首个匹配数据值的数据名
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>数据名</returns>
                public TName GetFirstName(TValue value)
                {
                        foreach (var nv in _data)
                                if (nv.Value.Equals(value))
                                        return nv.Name;
                        return default;
                }

                //=========================================================================//

                /// <summary>
                ///         获取最后一个匹配数据名的数据值
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>数据值</returns>
                public TValue GetLastValue(TName name)
                {
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Name.Equals(name))
                                        return _data[i].Value;

                        return default;
                }

                /// <summary>
                ///         获取最后一个匹配数据值的数据名
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>数据名</returns>
                public TName GetLastName(TValue value)
                {
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Value.Equals(value))
                                        return _data[i].Name;

                        return default;
                }

                //=========================================================================//

                /// <summary>
                ///         通过索引获取数据对
                /// </summary>
                /// <param name="index">索引位置</param>
                /// <returns>数据对</returns>
                public NameValuePair<TName, TValue> GetByIndex(int index)
                {
                        if (_data.Count - 1 < index || index < 0) return default;

                        return _data[index];
                }

                #endregion 获取

                #region 移除

                /// <summary>
                ///         清空容器
                /// </summary>
                public void Clear()
                {
                        _data.Clear();
                }

                //=========================================================================//

                /// <summary>
                ///         移除所有同名的数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>已经移除的数据值列表</returns>
                public List<TValue> RemoveAllByName(TName name)
                {
                        var values = new List<TValue>();
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Name.Equals(name))
                                {
                                        values.Add(_data[i].Value);
                                        _data.RemoveAt(i);
                                }

                        return values;
                }

                /// <summary>
                ///         移除所有同值的数据对
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>已经移除的数据名列表</returns>
                public List<TName> RemoveAllByValue(TValue value)
                {
                        var names = new List<TName>();
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Value.Equals(value))
                                {
                                        names.Add(_data[i].Name);
                                        _data.RemoveAt(i);
                                }

                        return names;
                }

                //=========================================================================//
                /// <summary>
                ///         移除最后一个匹配数据名的数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>已移除的数据值</returns>
                public TValue RemoveLastByName(TName name)
                {
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Name.Equals(name))
                                {
                                        var value = _data[i].Value;
                                        _data.RemoveAt(i);
                                        return value;
                                }

                        return default;
                }

                /// <summary>
                ///         移除最后一个匹配数据值的数据对
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>已移除的数据名</returns>
                public TName RemoveLastByValue(TValue value)
                {
                        for (var i = _data.Count - 1; i >= 0; i--)
                                if (_data[i].Name.Equals(value))
                                {
                                        var name = _data[i].Name;
                                        _data.RemoveAt(i);
                                        return name;
                                }

                        return default;
                }

                //=========================================================================//

                /// <summary>
                ///         移除首个匹配数据名的数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <returns>已移除的数据值</returns>
                public TValue RemoveFirstByName(TName name)
                {
                        if (_data.Count < 1) return default;

                        var index = -1;
                        for (var i = 0; i < _data.Count; i++)
                                if (_data[i].Name.Equals(name))
                                {
                                        index = i;
                                        break;
                                }

                        if (index > -1)
                        {
                                var value = _data[index].Value;
                                _data.RemoveAt(index);
                                return value;
                        }

                        return default;
                }

                /// <summary>
                ///         移除首个匹配数据值的数据对
                /// </summary>
                /// <param name="value">数据值</param>
                /// <returns>已移除的数据名</returns>
                public TName RemoveFirstByValue(TValue value)
                {
                        if (_data.Count < 1) return default;

                        var index = -1;
                        for (var i = 0; i < _data.Count; i++)
                                if (_data[i].Value.Equals(value))
                                {
                                        index = i;
                                        break;
                                }

                        if (index > -1)
                        {
                                var name = _data[index].Name;
                                _data.RemoveAt(index);
                                return name;
                        }

                        return default;
                }

                //=========================================================================//

                /// <summary>
                ///         移除匹配组指定位置的数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <param name="index">索引位置</param>
                /// <returns>已移除的数据值</returns>
                public TValue RemoveAtByName(TName name, int index)
                {
                        if (_data.Count - 1 < index || index < 0) return default;

                        var j = 0;
                        var k = -1;
                        for (var i = 0; i < _data.Count; i++)
                                if (_data[i].Name.Equals(name))
                                        if (j++ == index)
                                        {
                                                k = i;
                                                break;
                                        }

                        if (k > -1)
                        {
                                var value = _data[k].Value;
                                _data.RemoveAt(k);
                                return value;
                        }

                        return default;
                }

                /// <summary>
                ///         移除匹配组指定位置的数据对
                /// </summary>
                /// <param name="value">数据值</param>
                /// <param name="index">索引位置</param>
                /// <returns>已移除的数据名</returns>
                public TName RemoveAtByValue(TValue value, int index)
                {
                        if (_data.Count - 1 < index || index < 0) return default;

                        var j = 0;
                        var k = -1;
                        for (var i = 0; i < _data.Count; i++)
                                if (_data[i].Value.Equals(value))
                                        if (j++ == index)
                                        {
                                                k = i;
                                                break;
                                        }

                        if (k > -1)
                        {
                                var name = _data[k].Name;
                                _data.RemoveAt(k);
                                return name;
                        }

                        return default;
                }

                //=========================================================================//

                /// <summary>
                ///         移除整个容器内指定位置的数据对
                /// </summary>
                /// <param name="index">索引位置</param>
                /// <returns>已移除的数据对</returns>
                public NameValuePair<TName, TValue> RemoveAt(int index)
                {
                        if (index < 0 || index > _data.Count - 1) return default;

                        var data = _data[index];
                        _data.RemoveAt(index);
                        return data;
                }

                #endregion 移除

                #region 修改

                /// <summary>
                ///         修改指定索引位置的数据对
                /// </summary>
                /// <param name="name">数据名</param>
                /// <param name="value">数据值</param>
                /// <param name="index">索引位置</param>
                public void SetData(int index, TName name, TValue value)
                {
                        if (index > _data.Count - 1 || index < 0) return;
                        _data[index].Name = name;
                        _data[index].Value = value;
                }

                /// <summary>
                ///         修改匹配数据名的数据值
                /// </summary>
                /// <param name="name">数据名</param>
                /// <param name="value">数据值</param>
                /// <param name="index">匹配组的索引位置</param>
                public void SetValue(TName name, TValue value, int index = -1)
                {
                        int i = 0;
                        foreach (var data in _data)
                        {
                                if (data.Name.Equals(name))
                                {
                                        if (i++ >= index)
                                        {
                                                data.Value = value;
                                                break;
                                        }
                                }
                        }
                }

                /// <summary>
                ///         修改匹配数据值的数据名
                /// </summary>
                /// <param name="value">数据值</param>
                /// <param name="name">数据名</param>
                /// <param name="index">匹配组的索引位置</param>
                public void SetName(TValue value, TName name, int index = -1)
                {
                        int i = 0;
                        foreach (var data in _data)
                        {
                                if (data.Value.Equals(value))
                                {
                                        if (i++ >= index)
                                        {
                                                data.Name = name;
                                                break;
                                        }
                                }
                        }
                }

                #endregion 修改
        }
}