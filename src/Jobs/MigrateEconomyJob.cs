﻿using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class MigrateEconomyJob : IJob
    {
        public void ExecuteMigration()
        {
            Logger.Log("Adding economy migration job to autoexec.yaml...");
            //todo check if job exists
        }
    }
}