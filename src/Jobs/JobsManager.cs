using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class JobsManager
    {
        private readonly HashSet<IJob> _jobs;

        public JobsManager()
        {
            _jobs = new HashSet<IJob>();
        }

        public void RegisterJob(IJob migration)
        {
            _jobs.Add(migration);
        }

        public IReadOnlyCollection<IJob> GetJobs()
        {
            return _jobs;
        }

        public void Migrate(string[] preventionFlags = null)
        {
            for (int i = 0; i < _jobs.Count; i++)
            {
                var job = _jobs.ElementAt(i);
                
                var jobName = job.GetType().Name;
                if (preventionFlags != null)
                {
                    if (job.GetType().HasPreventAttribute())
                    {
                        var preventionFlagsForThisJob = job.GetType().GetPreventionCommands();
                        if (preventionFlags.Intersect(preventionFlagsForThisJob).Any())
                        {
                            Logger.Log($"Preventing {jobName}");
                            continue;
                        }
                    }
                }
                Logger.Log($"Executing {jobName}");
                try
                {
                    job.ExecuteMigration();
                }
                catch (Exception e)
                {
                    Logger.Log($"Caught exception while executing {jobName}:");
                    Logger.LogException(e);
                    Logger.Log("Reverting all possible jobs.");
                    Revert(i, preventionFlags);
                    return;
                }
            }

            Logger.Log("Migration successfully finished. You can restart your server now.");
        }

        private void Revert(int stoppedAt, string[] preventionFlags = null)
        {
            for (int i = stoppedAt; i >= 0; i--)
            {
                var job = _jobs.ElementAt(i);
                if (job is IRevertable revertableJob)
                {
                    var jobName = revertableJob.GetType().Name;
                    if (preventionFlags != null)
                    {
                        if (job.GetType().HasPreventAttribute())
                        {
                            var preventionFlag = job.GetType().GetPreventionCommand();
                            if (preventionFlags.Contains(preventionFlag))
                            {
                                Logger.Log($"Recognized {preventionFlag}, preventing reverting of {jobName} because it never happened.");
                                continue;
                            }
                        }
                    }
                    Logger.Log($"Reverting {jobName}");
                    try
                    {
                        revertableJob.Revert();
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"Caught exception while reverting {jobName}:");
                        Logger.LogException(e);
                    }
                }
            }
            Logger.Log("OpenMod installation has failed. No changes were made. Please report this issue on the OpenMod discord: https://discord.gg/J9KYMaMaTN");
        }
    }
}