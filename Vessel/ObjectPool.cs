using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace 自定义容器
{
        public static class CloneTools
        {
                /// <summary>
                /// 深拷贝
                /// </summary>
                /// <typeparam name="Obj">泛型类型</typeparam>
                /// <param name="obj">参考对象</param>
                /// <returns>拷贝结果</returns>
                public static Obj DeepCopy<Obj>(Obj obj)
                {
                        if (obj == null)
                                return default(Obj);
                        //如果是字符串或值类型则直接返回
                        if (obj is string || obj.GetType().IsValueType) return obj;
                        object retval = Activator.CreateInstance(obj.GetType());
                        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        foreach (FieldInfo field in fields)
                        {
                                try
                                {
                                        object o = DeepCopy(field.GetValue(obj));
                                        field.SetValue(retval, o);
                                }
                                catch
                                {
                                        object o = field.GetValue(obj);
                                        field.SetValue(retval, o);
                                        
                                }
                        }
                        return (Obj)retval;
                }
        }

        /// <summary>
        /// 对象池类
        /// </summary>
        /// <typeparam name="T">泛型类型必须为“引用类型”并且“有无参构造函数”</typeparam>
        public class ObjectPool<T> where T : class, new()
        {
                private T _Template;
                //备用池
                private RingQueue<T> _Pool;
                //已用池
                private List<T> _UsedPool;

                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="poolSize">池子最大容量</param>
                /// <param name="fill">初始化时是否填满池子</param>
                /// <param name="template">克隆模板(模板不为空时将自动调用CloneTools.DeepCopy进行拷贝)</param>
                public ObjectPool(int poolSize, bool fill = false, T template = null)
                {
                        _Template = template;
                        _Pool = new RingQueue<T>(poolSize);
                        if (fill)
                        {
                                if (template == null)
                                {
                                        for (int i = 0; i < poolSize; i++)
                                        {
                                                _Pool.Enqueue(new T());
                                        }
                                }
                                else
                                {
                                        for (int i = 0; i < poolSize; i++)
                                        {
                                                _Pool.Enqueue(CloneTools.DeepCopy(template));
                                        }
                                }
                        }

                        _UsedPool = new List<T>(_Pool.RingSize);
                }

                /// <summary>
                /// 构造函数
                /// 从已初始化好的集合创建对象池
                /// </summary>
                /// <param name="collection">指定集合</param>
                public ObjectPool(IEnumerable<T> collection)
                {
                        _Pool = new RingQueue<T>(collection);
                        _Template = null;
                        _UsedPool = new List<T>(_Pool.RingSize);
                }
                /// <summary>
                /// 获取实例,如果已取出的数量超出池子的容量将返回null
                /// </summary>
                /// <returns>对象实例</returns>
                public T GetObject()
                {
                        if (_UsedPool.Count >= _Pool.RingSize)
                                return null;
                        T result;
                        if (!_Pool.TryDequeue(out result))
                        {
                                if (_Template == null)
                                {
                                        result = new T();
                                }
                                else
                                {
                                        result = CloneTools.DeepCopy(_Template);
                                }
                        }
                        _UsedPool.Add(result);
                        return result;
                }

                /// <summary>
                /// 回收对象,无法回收由外部创建的对象
                /// </summary>
                /// <param name="obj">对象实例</param>
                public void RecycleObject(T obj)
                {
                        if (obj == null || !_UsedPool.Contains(obj))
                                return;
                        _UsedPool.Remove(obj);
                        _Pool.Enqueue(obj);
                }

                /// <summary>
                /// 清空池子
                /// </summary>
                public void Clear()
                {
                        _Pool.Clear();
                        _UsedPool.Clear();
                }

                /// <summary>
                /// 回收所有
                /// </summary>
                public void RecycleOAll()
                {
                        foreach (var obj in _UsedPool)
                        {
                                _Pool.Enqueue(obj);
                        }
                        _UsedPool.Clear();
                }
        }
}