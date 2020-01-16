using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TaskWork
{
    /// <summary>
    /// 抽象任务类
    /// </summary>
    public abstract class BaseWork
    {
        /// <summary>
        /// 时间计时器
        /// </summary>
        System.Timers.Timer Timer = new System.Timers.Timer();
        /// <summary>
        /// 运行成功次数
        /// </summary>
        /// <value>
        /// The run success.
        /// </value>
        public int RunSuccess { get; set; }
        /// <summary>
        /// 1按周期  2按时间点
        /// </summary>
        /// <value>
        /// The type of the run.
        /// </value>
        public int RunType { get; set; }
        /// <summary>
        /// 运行周期时间
        /// </summary>
        public int RunCycleTime = 60;
        /// <summary>
        /// 运行时间点
        /// </summary>
        public string RunTime = "02:00:00";

        /// <summary>
        /// 按周期执行工作 多少分钟
        /// </summary>
        /// <param name="runCycle">The run cycle.</param>
        public void RunWork(int runCycle)
        {
            RunType = 1;
            RunCycleTime = runCycle;
            Timer.Interval = RunCycleTime * 60 * 1000;
            Timer.Elapsed += Timer1_Elapsed;
            Timer.Enabled = true;
            Timer.Start();
            Timer1_Elapsed(null, null);//立即执行一次
        }

        /// <summary>
        /// 按时间点执行工作
        /// </summary>
        /// <param name="runTime">The run time.</param>
        public void RunWork(string runTime)
        {
            RunType = 2;
            RunTime = runTime;
            Timer.Interval = RunCycleTime * 60 * 1000;
            Timer.Elapsed += Timer1_Elapsed;
            Timer.Enabled = true;
            Timer.Start();
            Timer1_Elapsed(null, null);//立即执行一次
        }

        /// <summary>
        /// 周期执行工作
        /// </summary>
        public void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (RunType == 1)
                {
                    Timer.Stop();
                    RunSuccess++;
                    Dowork();
                }
                else
                {
                    DateTime now = DateTime.Now;
                    DateTime date = DateTime.Parse(now.ToString("yyyy-MM-dd") + " " + RunTime);
                    if (now > date && now < date.AddHours(1))
                    {
                        Timer.Stop();
                        RunSuccess++;
                        Dowork();
                    }
                    else
                    {
                        Console.WriteLine("执行时间点未到暂不执行任务");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //
            }
            finally
            {
                Timer.Start();
            }
        }

        /// <summary>
        /// 抽象任务
        /// </summary>
        public abstract void Dowork();
    }
}
