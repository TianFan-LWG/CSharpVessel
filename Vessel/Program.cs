using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace 自定义容器
{
        class MyClass1
        {
                public int C;
        }
        class MyClass
        {
                public static int A;
                public int Num;
                public MyClass1 C1;
                private int B;
                public MyClass()
                {

                }

                public void SetB(int b)=> B = b;


                public int GetB => B;
        }
        class Program
        {
                static void Main(string[] args)
                {
                        //TestDataPair();
                        //TestDataTree();
                        //TestRingQueue();
                        //ObjectPool<MyClass> aPool = new ObjectPool<MyClass>(10);

                        Console.ReadKey();
                }





                static void TestRingQueue()
                {
                        Console.Title = "环队列测试";
                        RingQueue<int> queue = new RingQueue<int>(10);
                        queue.Enqueue(666);
                        queue.Enqueue(777);
                        var n = queue.Peek();
                        var n1 = queue.Dequeue();
                        Console.WriteLine(n);
                        Console.WriteLine(n1);
                        for (int i = 0; i < 15; i++)
                        {
                                queue.Enqueue(i);
                        }

                        foreach (var temp in queue)
                        {
                                Console.WriteLine(temp);
                        }

                        Console.WriteLine("=======================");

                        while (!queue.IsEmpty)
                        {
                                Console.WriteLine(queue.Dequeue());
                        }
                        Console.WriteLine("=======================");
                        Console.WriteLine("10000000次入队出队：");

                        RingQueue<int> queue1 = new RingQueue<int>(10001);
                        Queue<int> queue2 = new Queue<int>();
                      
                        int max = 10000000;
                        Random random=new Random();
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        
                        for (int i = 0; i < max; i++)
                        {
                                queue2.Enqueue(random.Next());
                        }

                        while (queue2.Count>0)
                        {
                                queue2.Dequeue();
                        }
                        stopwatch.Stop();
                        Console.WriteLine("原队列：{0}",stopwatch.Elapsed);
                        
                        stopwatch.Restart();
                        for (int i = 0; i < max; i++)
                        {
                                queue1.Enqueue(random.Next());
                        }

                        while (!queue1.IsEmpty)
                        {
                                queue1.Dequeue();
                        }
                        stopwatch.Stop();
                        Console.WriteLine("环队列：{0}", stopwatch.Elapsed);
                }

                static void TestDataTree()
                {
                        Console.Title = "树列测试";
                        DataTree<int> testTree = new DataTree<int>("Root", 1000);

                        testTree.AddNode("AAA",555).AddNode("bbb",666).AddNode("ccc",777);
                        testTree.AddNode("bbb",111).AddNode("ccc",222).AddNode("ddd",333);

                        Console.WriteLine(testTree.GetNodeByUri("./AAA/bbb/ccc")?.Data);
                        Console.WriteLine(testTree.GetNodeByUri("/bbb/CCC")?.Data);
                }

                static void TestDataPair()
                {
                        Console.Title = "数据对测试";
                        //能允许多个相同名字数据存在的 数据对 容器。相当于允许相同Key的Dictionary<key,value> 
                        var datas = new DataPair<string, int>();
                        datas.Add("A", 10);
                        datas.Add("A", 11);
                        datas.Add("A", 12);
                        datas.Add("A", 13);
                        datas.Add("A", 14);
                        datas.Add("A", 15);
                        datas.AddValues(datas,true);

                        datas["B"] = 11;
                        datas["B"] = 111;
                        datas["B"] = 1111;
                        datas["B"] = 11111;
                        datas["C"] = 12;
                        datas["D"] = 13;
                        datas["E"] = 14;
                        datas["F"] = 15;
                        datas.RemoveLastByName("A");//"A" = 15 被移除
                        datas.SetName(11, "SSS", 1);//匹配组{"A" = 11,"B" = 11}；"B" = 11 被修改为 "SSS" = 11


                        Console.WriteLine("数据D={0}", datas["D"]);

                        int num = datas["E"] - 4;
                        Console.WriteLine("datas[\"E\"] - 4={0}", num);
                        //遍历输出
                        foreach (var nameValuePair in datas) Console.WriteLine(nameValuePair);
                }
        }
}
