using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public static class JobsExecutor
    {
        public static void Execute(List<IJob> jobsToExecute)
        {
            var executedJobs = new List<IJob>();

            for (var i = 0; i < jobsToExecute.Count; i++)
            {
                var job = jobsToExecute.ElementAt(i);
                
                var jobName = job.GetType().Name;
                
                try
                {
                    executedJobs.Add(job);
                    job.ExecuteMigration();
                }
                catch (Exception e)
                {
                    Logger.Log($"Caught exception while executing job \"{jobName}\":");
                    Logger.LogException(e);
                    Logger.Log("Reverting...");

                    Revert(executedJobs);

                    jobsToExecute.Clear();
                    Logger.Log("OpenMod installation has failed. No changes were made. Please report this issue on the OpenMod discord: https://discord.gg/J9KYMaMaTN.");
                    return;
                }
            }

            Logger.Log("Migration successfully finished. Restart your server to finish installation.");
        }

        private static void Revert(IReadOnlyList<IJob> jobs)
        {
            for (var i = jobs.Count; i >= 0; i--)
            {
                var job = jobs[i];
                if (!(job is IReversibleJob reversibleJob))
                {
                    continue;
                }

                var jobName = reversibleJob.GetType().Name;
                    
                try
                {
                    reversibleJob.Revert();
                }
                catch (Exception e)
                {
                    Logger.Log($"Exception occurred while reverting job \"{jobName}\":");
                    Logger.LogException(e);
                }
            }
        }
    }
}