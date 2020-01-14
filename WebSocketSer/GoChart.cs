using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketSer
{
    public class GoChart : WebSocketBehavior
    {
        public static Dictionary<string, object> Clients = new Dictionary<string, object>();
        /// <summary>
        ///链接
        /// </summary>
        protected override void OnOpen()
        {
            Console.WriteLine($"客户端[{this.ID}]连上了");
            base.OnOpen();
        }

        /// <summary>
        ///消息处理
        /// </summary>
        protected override void OnMessage(MessageEventArgs e)
        {
            string data = e.Data;
            /*JToken param = JToken.Parse(data);
              得到用户数据 进行逻辑处理
             */
            string id = this.ID;

            SendAsync(JsonConvert.SerializeObject(new { code = 200, msg = "收到你的数据：" + data }), null);
        }

        /// <summary>
        ///关闭
        /// </summary>
        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"客户端[{this.ID}]关闭了");
            base.OnClose(e);
        }

        /// <summary>
        ///错误
        /// </summary>
        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine($"客户端[{this.ID}]出错:{e.Message}");
            base.OnError(e);
        }
    }
}
