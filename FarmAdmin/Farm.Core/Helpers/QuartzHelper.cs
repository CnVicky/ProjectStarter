using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Infrastructure.Helpers
{
    public class QuartzHelper
    {
        /// <summary>
        /// 时间间隔执行任务
        /// </summary>
        /// <typeparam name="T">任务类，必须实现IJob接口</typeparam>
        /// <param name="seconds">时间间隔(单位：秒)</param>
        public static void ExecuteInterval<T>(int seconds) where T : IJob
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();

            //IJobDetail job = JobBuilder.Create<T>().WithIdentity("job1", "group1").Build();
            IJobDetail job = JobBuilder.Create<T>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();
        }

        /// <summary>
        /// 指定时间执行任务
        /// </summary>
        /// <typeparam name="T">任务类，必须实现IJob接口</typeparam>
        /// <param name="cronExpression">cron表达式，即指定时间点的表达式</param>
        public static void ExecuteByCron<T>(string cronExpression) where T : IJob
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();

            IJobDetail job = JobBuilder.Create<T>().Build();

            //DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddSeconds(1), 2);
            //DateTimeOffset endTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddYears(2), 3);

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                //.StartAt(startTime).EndAt(endTime)
                .WithCronSchedule(cronExpression)
                .Build();

            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();

            //Thread.Sleep(TimeSpan.FromDays(2));
            //scheduler.Shutdown();
        }
    }
}
