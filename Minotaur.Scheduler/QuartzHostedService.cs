using Microsoft.Extensions.Hosting;
using Quartz;

namespace Minotaur.Scheduler
{
    public class QuartzHostedService : IHostedService
    {
        private readonly IScheduler _scheduler;
        private readonly ConfirmationCodeHandler _confirmationCodeHandler;

        public QuartzHostedService(IScheduler scheduler, ConfirmationCodeHandler confirmationCodeHandler)
        {
            _scheduler = scheduler;
            _confirmationCodeHandler = confirmationCodeHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the job to run every 10 minutes
            var jobDetail = JobBuilder.Create<ConfirmationCodeHandler>()
                .WithIdentity("ConfirmationCodeJob")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("ConfirmationCodeTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMinutes(1))
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
