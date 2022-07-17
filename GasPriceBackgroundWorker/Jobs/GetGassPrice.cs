using System;
using Quartz;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GasPriceBackgroundWorker.Jobs
{
    [DisallowConcurrentExecution]
    public class GetGassPrice : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
