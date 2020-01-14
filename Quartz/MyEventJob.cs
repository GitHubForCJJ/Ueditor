using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace QuartzDemo
{
    public class MyEventJob : IJob
    {
        /// <summary>
        /// 作业调度定时执行的方法
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>Task.</returns>
        /// <remarks>The implementation may wish to set a  result object on the
        /// JobExecutionContext before this method exits.  The result itself
        /// is meaningless to Quartz, but may be informative to
        /// <see cref="T:Quartz.IJobListener" />s or
        /// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
        /// execution.</remarks>
        public virtual void Execute(IJobExecutionContext context)
        {
            Task.Run(() =>
            {
                EveryTask();
            });
        }

        /// <summary>
        ///任务实现 直接调用服务或者具体逻辑代码
        /// </summary>
        public void EveryTask()
        {
            try
            {
                Console.WriteLine($"每2分钟一次 ：执行时间{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

