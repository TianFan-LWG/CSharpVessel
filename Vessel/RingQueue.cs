using System.Collections;
using System.Collections.Generic;

namespace 自定义容器
{
        /// <summary>
        /// 环队列
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        public class RingQueue<T> : IEnumerable<T>
        {
                private T[] _datas;

                private int _Count;
                /// <summary>
                /// 当前元素数量
                /// </summary>
                public int Count => _Count;

                private int _RingSize;

                /// <summary>
                /// 队列容量
                /// </summary>
                public int RingSize => _RingSize;

                //头部，尾部
                private int _front,
                _rear;

                /// <summary>
                /// 是否为空
                /// </summary>
                public bool IsEmpty => _Count <= 0;

                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="ringSize">队列大小</param>
                public RingQueue(int ringSize)
                {
                        _RingSize = ringSize;
                        _datas = new T[ringSize];
                }

                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="collection">原有数据</param>
                /// <param name="ringSize">队列大小</param>
                public RingQueue(IEnumerable<T> collection, int ringSize = 0)
                {
                        if (ringSize <= 0)
                        {
                                Queue<T> queue = new Queue<T>();
                                foreach (var item in collection)
                                {
                                        queue.Enqueue(item);
                                }

                                _RingSize = queue.Count;
                                _datas = queue.ToArray();
                                _Count = _RingSize;
                                return;
                        }
                        _RingSize = ringSize;
                        _datas = new T[ringSize];
                        int index = 0;
                        foreach (var data in collection)
                        {
                                if (index >= ringSize)
                                {
                                        index = 0;
                                        break;
                                }
                                _datas[index++] = data;
                                _Count++;
                        }

                        _front = index;
                }

                /// <summary>
                /// 构造函数
                /// 将已有的普通队列转为环队列
                /// </summary>
                /// <param name="queue">原队列</param>
                public RingQueue(Queue<T> queue)
                {
                        _RingSize = queue.Count;
                        _datas = queue.ToArray();
                        _Count = _RingSize;
                }

                #region 迭代器

                public IEnumerator<T> GetEnumerator()
                {
                        return new RingQueueEnumerator(this);
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                //迭代器
                private class RingQueueEnumerator : IEnumerator<T>
                {
                        private RingQueue<T> _data;
                        private int _position;
                        private int _count;

                        public RingQueueEnumerator(RingQueue<T> data)
                        {
                                _data = data;
                                _position = data._rear - 1;
                                _count = data._Count;
                        }

                        public T Current => _data._datas[_position];

                        object IEnumerator.Current => Current;

                        public void Dispose()
                        {
                                _position = -1;
                                _data = null;
                        }

                        public bool MoveNext()
                        {
                                _position++;
                                if (_position >= _data.RingSize)
                                {
                                        _position = 0;
                                }

                                return _count-- > 0;
                        }

                        public void Reset()
                        {
                                _position = _data._rear - 1;
                        }
                }

                #endregion 迭代器

                /// <summary>
                /// 入队
                /// </summary>
                /// <param name="item">数据</param>
                public void Enqueue(T item)
                {
                        //移动队头
                        _datas[_front++] = item;
                        _Count++;
                        if (_front >= _RingSize)
                        {
                                _front = 0;
                        }

                        if (_Count >= _RingSize)
                        {
                                _Count = _RingSize;
                        }
                }

                /// <summary>
                /// 出队
                /// </summary>
                /// <returns>数据</returns>
                public T Dequeue()
                {
                        if (_Count <= 0) //队空
                                return default(T);
                        //移动队尾
                        T result = _datas[_rear++];
                        _Count--;
                        if (_rear >= _RingSize)
                        {
                                _rear = 0;
                        }
                        return result;
                }

                /// <summary>
                /// 尝试出队
                /// </summary>
                /// <param name="result">出队结果</param>
                /// <returns>是否成功</returns>
                public bool TryDequeue(out T result)
                {
                        if (_Count <= 0)
                        {
                                result = default(T);
                                return false;
                        }
                        //移动队尾
                        result = _datas[_rear++];
                        _Count--;
                        if (_rear >= _RingSize)
                        {
                                _rear = 0;
                        }
                        return true;
                }

                /// <summary>
                /// 返回队头
                /// </summary>
                /// <returns>队头数据</returns>
                public T Peek()
                {
                        if (_Count <= 0)
                                return default(T);
                        int index = (_front - 1 + _RingSize) % _RingSize;
                        return _datas[index];
                }

                /// <summary>
                /// 尝试返回队头
                /// </summary>
                /// <param name="result">返回数据</param>
                /// <returns>是否成功</returns>
                public bool TryPeek(out T result)
                {
                        if (_Count <= 0)
                        {
                                result = default(T);
                                return false;
                        }
                        int index = (_front - 1 + _RingSize) % _RingSize;
                        result = _datas[index];
                        return true;
                }

                /// <summary>
                /// 返回是否包含指定元素
                /// </summary>
                /// <param name="value">给定元素</param>
                /// <returns>是否包含</returns>
                public bool Contains(T value)
                {
                        if (_Count <= 0)
                                return false;

                        int index = _rear;
                        do
                        {
                                if (_datas[index].Equals(value))
                                        return true;
                                index++;
                                if (index >= _RingSize)
                                        index = 0;
                        } while (index != _front);

                        return false;
                }

                /// <summary>
                /// 清除队列（填充默认值）
                /// </summary>
                public void Clear(T fill)
                {
                        _rear = _front;
                        _Count = _RingSize;
                        for (int i = 0; i < _RingSize; i++)
                        {
                                _datas[i] = fill;
                        }
                }

                /// <summary>
                /// 向前移动队尾，用于抛弃值
                /// </summary>
                /// <param name="distance">距离</param>
                public void PushRear(int distance)
                {
                        if (distance <= 0)
                                return;
                        //前移距离
                        distance = distance % _RingSize;

                        if (distance >= _Count)
                        {
                                _rear = _front;
                                _Count = 0;
                        }
                        else
                        {
                                _rear = (_rear + distance) % _RingSize;
                                _Count -= distance;
                        }
                }

                /// <summary>
                /// 清除队列
                /// </summary>
                public void Clear()
                {
                        _rear = _front;
                        _Count = 0;
                }
        }
}