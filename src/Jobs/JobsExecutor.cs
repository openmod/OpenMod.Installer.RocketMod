﻿using Rocket.Core.Logging;
using System;
using System.Collections.Generic;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public static class JobsExecutor
    {
        public static bool Execute(List<IJob> jobsToExecute, string[] args)
        {
            var executedJobs = new List<IJob>();

            for (var i = 0; i < jobsToExecute.Count; i++)
            {
                var job = jobsToExecute[i];

                var jobName = job.GetType().Name;

                try
                {
                    executedJobs.Add(job);
                    job.ExecuteMigration(args);
                }
                catch (Exception e)
                {
                    Logger.Log($"Caught exception while executing job \"{jobName}\":");
                    Logger.LogException(e);
                    Logger.Log("Reverting...");

                    Revert(executedJobs);

                    jobsToExecute.Clear();
                    Logger.Log("OpenMod installation has failed. No changes were made. Please report this issue on the OpenMod discord: https://discord.gg/J9KYMaMaTN.");
                    Logger.Log("To install OpenMod manually, visit https://openmod.github.io/openmod-docs/userdoc/installation/unturned.html.");
                    return false;
                }
            }

            Logger.Log("Migration successfully finished. Restart your server to complete installation.");
            return true;
        }

        private static void Revert(IReadOnlyList<IJob> jobs)
        {
            for (var i = (jobs.Count - 1); i >= 0; i--)
            {
                var job = jobs[i];
                if (job is not IReversibleJob reversibleJob)
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
