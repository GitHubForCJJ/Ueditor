using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzDemo
{
class Program
    {
        /// <summary>
        /// The scheduler
        /// </summary>
        static IScheduler _scheduler = null;
        static void Main(string[] args)
        {
            //周期执行时间设置
            string cycleTime = @"* 0/2 * ? * *";

            //调度器工厂
            ISchedulerFactory sf = new StdSchedulerFactory();

            if (_scheduler == null)
            {
                //创建调度器
                _scheduler = sf.GetScheduler();
            }
            _scheduler.Start();

            //创建任务对象
            Quartz.IJobDetail job1 = JobBuilder.Create<MyEventJob>().WithIdentity("job1", "group1").Build();
            //创建触发器
            ITrigger trigger1 = TriggerBuilder.Create().StartAt(DateTime.Now.AddSeconds(5)).WithIdentity("trigger1", "group1").StartNow().WithCronSchedule(cycleTime).Build();
            //添加指定的任务  并关联指定的触发器
            _scheduler.ScheduleJob(job1, trigger1);

            Console.ReadLine();
        }
    }
}
