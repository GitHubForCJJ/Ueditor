using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace WebSocketSer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer wssv = new WebSocketServer(6688);
            wssv.AddWebSocketService<GoChart>("/GoChart");
            wssv.Start();
            Console.WriteLine("服务器启动，按任意键终止服务器。");
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
