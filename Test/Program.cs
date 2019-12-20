using Poster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string ceshiimg = @"D:\Temp\a.jpg";
            string logo= @"D:\Temp\wechat.png";
            var path=PosterHelper.CreatePoster("www.baidu.com", ceshiimg, "第一张海报","这是一个测试海报的正文标题",logo);
            Console.WriteLine(path);
            Console.ReadKey();
        }
    }
}
