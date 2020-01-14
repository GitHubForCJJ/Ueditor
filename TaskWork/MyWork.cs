using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWork
{
    public class MyWork : BaseWork
    {
        public override void Dowork()
        {
            Console.WriteLine($"每3分钟执行一次{DateTime.Now.ToString("yyyy-MM-dd hh:mm")}运行成功次数{RunSuccess}");
        }
    }
}
