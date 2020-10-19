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

                /// <summary>
                /// 当前元素数量
                /// </summary>
                public int Count => _full ? _RingSize : (_front - _rear + _RingSize) % _RingSize;

                private int _RingSize;

                /// <summary>
                /// 队列容量
                /// </summary>
                public int RingSize
                {
                        get => _RingSize;
                        private set => _RingSize = value;
                }

                //头部，尾部
                private int _front,
                _rear;

                //是否队满
                private bool _full;

                /// <summary>
                /// 是否为空
                /// </summary>
                public bool IsEmpty => _rear == _front && !_full;

                /// <summary>
                /// 构造函数
                /// </summary>
                /// <param name="ringSize">队列大小</param>
                public RingQueue(int ringSize)
                {
                        RingSize = ringSize;
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

                                RingSize = queue.Count;
                                _datas = queue.ToArray();
                                _full = true;
                                return;
                        }
                        RingSize = ringSize;
                        _datas = new T[ringSize];
                        int index = 0;
                        foreach (var data in collection)
                        {
                                if (index >= ringSize)
                                {
                                        _full = true;
                                        index = 0;
                                        break;
                                }
                                _datas[index++] = data;
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
                        RingSize = queue.Count;
                        _datas = queue.ToArray();
                        _full = true;
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
                        private bool _full;

                        public RingQueueEnumerator(RingQueue<T> data)
                        {
                                _data = data;
                                _position = data._rear - 1;
                                _full = data._full;
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
                                _position = (_position + 1) % _data._RingSize;
                                if (_full) //队满
                                {
                                        if (_position == _data._front)
                                        {
                                                _full = false;
                                        }
                                }
                                else
                                {
                                        if (_position == _data._front)
                                                return false;
                                }

                                return true;
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
                        _datas[_front] = item;
                        //移动队头
                        _front = (_front + 1) % _RingSize;
                        if (_full) //队满移动队尾
                                _rear = _front;

                        if (_front == _rear) //队满
                                _full = true;
                }

                /// <summary>
                /// 出队
                /// </summary>
                /// <returns>数据</returns>
                public T Dequeue()
                {
                        if (_rear == _front && !_full) //队空
                                return default(T);
                        T result = _datas[_rear];
                        //移动队尾
                        _rear = (_rear + 1) % _RingSize;
                        _full = false; //只要有出队就取消队满状态
                        return result;
                }

                /// <summary>
                /// 尝试出队
                /// </summary>
                /// <param name="result">出队结果</param>
                /// <returns>是否成功</returns>
                public bool TryDequeue(out T result)
                {
                        if (_rear == _front && !_full)
                        {
                                result = default(T);
                                return false;
                        }
                        result = _datas[_rear];
                        //移动队尾
                        _rear = (_rear + 1) % _RingSize;
                        _full = false; //只要有出队就取消队满状态
                        return true;
                }

                /// <summary>
                /// 返回队头
                /// </summary>
                /// <returns>队头数据</returns>
                public T Peek()
                {
                        if (_rear == _front && !_full)
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
                        if (_rear == _front && !_full)
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
                        int index = _rear;
                        do
                        {
                                if (_datas[index].Equals(value))
                                        return true;
                                index = (index + 1) % _RingSize;
                        } while (index != _front);

                        return false;
                }

                /// <summary>
                /// 清除队列（填充默认值）
                /// </summary>
                public void Clear(T fill)
                {
                        _rear = _front;
                        _full = false;
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
                        if (_full) //队满直接前移 清除队满状态
                        {
                                _rear = (_rear + distance) % _RingSize;
                                _full = false;
                                return;
                        }

                        //前移距离
                        distance = distance % _RingSize;

                        if (distance >= Count)
                        {
                                _rear = _front;
                        }
                        else
                        {
                                _rear = (_rear + distance) % _RingSize;
                        }
                }

                /// <summary>
                /// 向前移动队头，用于取填充的默认值
                /// </summary>
                /// <param name="distance">距离</param>
                public void PushFront(int distance)
                {
                        if (distance <= 0)
                                return;
                        if (_full) //队满直接前移并且推动队尾
                        {
                                _front = (_front + distance) % _RingSize;
                                _rear = _front;
                                return;
                        }
                        //前移距离
                        distance = distance % _RingSize;

                        //空余数量
                        int e_count = _rear == _front ? _RingSize : (_rear - _front + _RingSize) % _RingSize;
                        _front = (_front + distance) % _RingSize;
                        if (e_count <= distance) //队满
                        {
                                _rear = _front;
                                _full = true;
                        }
                }

                /// <summary>
                /// 清除队列
                /// </summary>
                public void Clear()
                {
                        _rear = _front;
                        _full = false;
                }
        }
}