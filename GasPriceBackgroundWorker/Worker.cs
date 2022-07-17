using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using GasPriceBackgroundWorker.Jobs;
using Microsoft.Extensions.Configuration;

namespace GasPriceBackgroundWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IScheduler _scheduler;
        private StdSchedulerFactory _stdSchedulerFactory;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ScheduleJobs(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
            await _scheduler.Shutdown();

        }
        private async Task ScheduleJobs(CancellationToken stoppingToken)
        {
            int.TryParse( _configuration["DelayInMins"], out int delayInMinutes);
            _stdSchedulerFactory = new StdSchedulerFactory();
            _scheduler = await _stdSchedulerFactory.GetScheduler();

            await _scheduler.Start();

            IJobDetail getGasPrices = JobBuilder.Create<GetGassPrice>()
                .WithIdentity("job1", "group1")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("triggerOne", "group")
                .StartNow()
                .WithSimpleSchedule(config => config.WithIntervalInMinutes(delayInMinutes).WithMisfireHandlingInstructionIgnoreMisfires().RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(getGasPrices, trigger, stoppingToken);

        }
    }
}
