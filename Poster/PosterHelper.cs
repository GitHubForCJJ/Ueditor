using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;

namespace Poster
{
    public class PosterHelper
    {
        /// <summary>
        /// 生成带二维码海报
        /// </summary>
        /// <param name="url">二维码跳转链接</param>
        /// <param name="headImg">海报顶部图片</param>
        /// <param name="headTitle">海报顶部文字</param>
        /// <param name="headTitle">海报二维码中部文字</param>
        /// <param name="logoImg">海报二维码内的logo</param>
        /// <returns></returns>
        public static string CreatePoster(string url, string headImg, string headTitle, string bodyTitle, string logoImg)
        {
            var posPath = "";
            try
            {
                //先画个最大的黑色背景
                Bitmap ob = new Bitmap(414, 736);
                Graphics g = Graphics.FromImage(ob);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(ColorTranslator.FromHtml("#333333"));

                //画一个白的底板
                Bitmap white = new Bitmap(ob.Width, ob.Height - 15);
                Graphics gw = Graphics.FromImage(white);
                gw.Clear(Color.White);
                g.DrawImage(white, new Rectangle(0, 0, white.Width, white.Height));
                white?.Dispose();
                gw?.Dispose();
                //顶部图片
                Bitmap head = new Bitmap(headImg);
                //图片高度
                int headHeight = head.Height * ob.Width / head.Width;//保持横纵比例
                g.DrawImage(head, 0, 0, ob.Width, headHeight);
                head?.Dispose();
                //画顶部图片 中的文字
                SolidBrush txtBrush = new SolidBrush(Color.White);
                StringFormat txtFromat = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };
                g.DrawString(headTitle, new Font("黑体", 20, FontStyle.Bold), txtBrush, new Point(ob.Width / 2, 50), txtFromat);
                //二维码上部文字描述
                txtBrush = new SolidBrush(ColorTranslator.FromHtml("#333333"));
                //处理文字不能超过40个
                string bodyStr = "";
                if (bodyTitle.Length > 40)
                {
                    bodyTitle = bodyTitle.Substring(0, 40);
                }
                if (bodyTitle.Length > 20)
                {
                    bodyStr += bodyTitle.Substring(0, 20);
                    bodyStr += "\r\n";
                    bodyStr += bodyTitle.Substring(0, 20);
                }
                else
                {
                    bodyStr = bodyTitle;
                }
                g.DrawString(bodyStr, new Font("黑体", 20, FontStyle.Bold), txtBrush, new Point(ob.Width / 2, headHeight + 20), txtFromat);
                //二维码地址
                var encoder = new QrEncoder(ErrorCorrectionLevel.M);//设置二维码的容错率
                QrCode qrCode = encoder.Encode(url);
                GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(23, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
                //创建存储二维码的图片
                Bitmap map3 = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
                Graphics g2 = Graphics.FromImage(map3);
                render.Draw(g2, qrCode.Matrix);
                //将logo添加到二维码图片上
                Image logo = Image.FromFile(logoImg);
                int logowidth = (int)(dSize.CodeWidth * 0.1);
                Point iconPoint = new Point((map3.Width - logowidth) / 2, (map3.Height - logowidth) / 2);
                g2.DrawImage(logo, iconPoint.X, iconPoint.Y, logowidth, logowidth);
                //将二维码图片画到最开始的那张图片上
                g.DrawImage(map3, new Rectangle(ob.Width / 4, headHeight + 75, ob.Width / 2, ob.Width / 2));
                map3?.Dispose();
                g2?.Dispose();
                logo?.Dispose();
                //处理长按复制文字
                txtBrush = new SolidBrush(ColorTranslator.FromHtml("#9F9F9F"));
                g.DrawString("长按识别二维码查看产品详情", new Font("黑体", 10), txtBrush, new Point(ob.Width / 2, headHeight + ob.Width / 2 + 95), txtFromat);

                string savePath = $"D:{Path.DirectorySeparatorChar}Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}.png";
                ob.Save(savePath, ImageFormat.Png);
                ob?.Dispose();
                g?.Dispose();
                posPath = savePath;

            }
            catch (Exception ex)
            {
                posPath = "";
            }
            return posPath;
        }
    }
}
