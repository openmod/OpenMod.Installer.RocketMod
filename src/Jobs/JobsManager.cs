using System.Collections.Generic;
using OpenMod.Installer.RocketMod.Migrations;

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
    }
}