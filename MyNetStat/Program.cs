using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyNetStat
{
    class Program
    {
        //定义一个静态列表，保存收集到的IP地址
        static List<string> iPEndPoints = new List<string>();

        //保存IP地址列表的文件名
        static string netFileName = "net.txt";
        static void Main(string[] args)
        {
            //程序启动的时候，如果保存列表的文件已经存在，则更改该文件名
            if (File.Exists(netFileName))
            {
                File.Move(netFileName, DateTime.Now.Ticks + "_" + netFileName);
            }

            //默认收集连接到本机Oracle端口的计算机IP
            int localPort = 1521;
            if (args.Length == 0)
            {
                localPort = 1521;
            }
            //如果用户指定了端口，则收集指定的端口，否则收集Oracle端口
            else if (int.TryParse(args[0], out localPort) == false)
            {
                localPort = 1521;
            }

            Console.WriteLine("检测本机本地端口:" + localPort);

            //上次检测后，现有列表中的IP数量
            int lastCount = 0;
            while (true)
            {
                //检测端口
                GetPort(localPort);

                //检测后，如果目前列表中的IP数量比上一次多，则保存一个记录文件
                if (iPEndPoints.Count() > lastCount)
                {
                    //先对记录排序
                    iPEndPoints.Sort();

                    //更新记录数量
                    lastCount = iPEndPoints.Count();

                    //创建一个字符串缓冲区
                    StringBuilder stringBuilder = new StringBuilder();
                    
                    //先添加一行内容
                    stringBuilder.AppendLine("检测本机本地端口:" + localPort);

                    //所有检测到的IP地址
                    foreach (var item in iPEndPoints)
                    {
                        stringBuilder.AppendLine(item);
                    }

                    //将新的记录写入文件
                    File.WriteAllText("net.txt", stringBuilder.ToString());

                    //输出目前记录数量
                    Console.WriteLine("iPEndPoints:\t" + lastCount);
                }

                //暂停5秒
                Thread.Sleep(5000);
            }
        }

        private static void GetPort(int localPort = 0)
        {
            Console.Write(".");

            //本计算机网络连接信息
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            
            //TCP连接信息
            TcpConnectionInformation[] tcpConnectionInformation = ipGlobalProperties.GetActiveTcpConnections();

            //甄别TCP连接信息
            foreach (TcpConnectionInformation tci in tcpConnectionInformation)
            {
                //如果指定了非零端口
                if (localPort != 0)
                {
                    //如果该TCP连接信息中的端口，不是指定的端口，忽略
                    if (tci.LocalEndPoint.Port != localPort) continue;
                }

                //该连接中的IP地址
                string localEndPoint = tci.RemoteEndPoint.Address.ToString();

                //检查是否已经在记录中，如果存在，则忽略
                if (iPEndPoints.Contains(localEndPoint)) continue;

                //向记录中添加新的IP地址
                iPEndPoints.Add(localEndPoint);

                //输出
                Console.WriteLine(localEndPoint);
            }
        }
    }


}
