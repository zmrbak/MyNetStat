using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
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
        static bool isExitedHandler = false;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
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

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (isExitedHandler == true) return;
            isExitedHandler=true;
            WirteConfig();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (isExitedHandler == true) return;
            isExitedHandler = true;
            WirteConfig();
        }

        private static void WirteConfig()
        {
            string fw = "fw.json";
            string orignfw = "";
            if (File.Exists(fw))
            {
                orignfw = File.ReadAllText(fw);
            }
            else
            {
                orignfw = "{\r\n    \"Name\": \"Oracle金盘访问\",\r\n    \"Description\": \"允许访问本机Oracle服务\",\r\n    \"Enabled\": true,\r\n    \"Action\": 1,\r\n    \"Direction\": 1,\r\n    \"LocalAddresses\": \"*\",\r\n    \"Protocol\": 6,\r\n    \"LocalPorts\": [\r\n      {\r\n        \"PortNumber\": 1521,\r\n        \"Description\": \"Oracle Server\"\r\n      }\r\n    ],\r\n    \"InterfaceTypes\": \"ALL\",\r\n    \"RemoteAddresses\": [\r\n      {\r\n        \"IpAddress\": \"192.168.240.0/20\",\r\n        \"Description\": \"临时全部开启\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"10.0.0.0/8\",\r\n        \"Description\": \"临时全部开启\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.29\",\r\n        \"Description\": \"金盘备份\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.139\",\r\n        \"Description\": \"IC空间\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.156\",\r\n        \"Description\": \"OPAC\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.157\",\r\n        \"Description\": \"金盘中间件\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.215\",\r\n        \"Description\": \"RFID\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.230\",\r\n        \"Description\": \"毕业季\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.247\",\r\n        \"Description\": \"微信门禁\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.158\",\r\n        \"Description\": \"超星馆藏收割\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.243.1\",\r\n        \"Description\": \"总服务台1\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.243.2\",\r\n        \"Description\": \"总服务台3\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.249.201\",\r\n        \"Description\": \"门禁闸机控制1\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.249.202\",\r\n        \"Description\": \"门禁闸机控制2\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.124\",\r\n        \"Description\": \"书窝\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.130\",\r\n        \"Description\": \"旧主页\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.139\",\r\n        \"Description\": \"IC空间\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.156\",\r\n        \"Description\": \"opac\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.157\",\r\n        \"Description\": \"金盘中间件\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.215\",\r\n        \"Description\": \"Rfid\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.218\",\r\n        \"Description\": \"门禁闸机\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"192.168.250.247\",\r\n        \"Description\": \"微信门禁\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"172.17.30.239\",\r\n        \"Description\": \"---\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"172.19.74.136\",\r\n        \"Description\": \"旅游规划学院图情室\"\r\n      },\r\n      {\r\n        \"IpAddress\": \"202.115.133.24\",\r\n        \"Description\": \"---\"\r\n      }\r\n    ],\r\n    \"RemotePorts\": \"*\"\r\n  }";
            }

            var rules = JsonSerializer.Deserialize<FireWall>(orignfw);

            foreach (var item in iPEndPoints)
            {
                if (rules.RemoteAddresses.Count(x => x.Equals(item)) > 0) continue;
                rules.RemoteAddresses.Add(new FireWall.Remoteaddress { IpAddress = item });
            }
            // 配置 JsonSerializerOptions 以支持中文和缩进
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // 启用缩进
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All) // 支持中文
            };
            File.WriteAllText(fw, JsonSerializer.Serialize(rules,options));
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
