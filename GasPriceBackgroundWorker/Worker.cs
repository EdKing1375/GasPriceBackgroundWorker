using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using GasPriceBackgroundWorker.Jobs;
using Microsoft.Extensions.Configuration;
using System;
using GasPriceBackgroundWorker.Services;
using GasPriceBackgroundWorker.Repository;

namespace GasPriceBackgroundWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IScheduler _scheduler;
        private StdSchedulerFactory _stdSchedulerFactory;
        private readonly IConfiguration _configuration;
        private readonly IGasPriceService _service;
        private readonly IPricePerWeekRepository _repo;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IGasPriceService service, IPricePerWeekRepository repo)
        {
            _logger = logger;
            _configuration = configuration;
            _service = service;
            _repo = repo;
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
            int.TryParse(_configuration["DelayInMins"], out int delayInMinutes);
            _stdSchedulerFactory = new StdSchedulerFactory();
            _scheduler = await _stdSchedulerFactory.GetScheduler();

            await _scheduler.Start();

            IJobDetail getGasPrices = JobBuilder.Create<GetGassPrice>()
                .WithIdentity("job", "group1")
                .Build();

            getGasPrices.JobDataMap.Add("logger", _logger);
            getGasPrices.JobDataMap.Add("service", _service);
            getGasPrices.JobDataMap.Add("repo", _repo);
            getGasPrices.JobDataMap.Add("config", _configuration);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(config => config
                    .WithIntervalInMinutes(delayInMinutes)
                    .WithMisfireHandlingInstructionIgnoreMisfires()
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(getGasPrices, trigger, stoppingToken);

        }
    }
}
