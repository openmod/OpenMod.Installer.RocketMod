namespace OpenMod.Installer.RocketMod.Jobs
{
    public abstract class NuGetInstallJob : IJob
    {
        private readonly string _packageId;

        protected NuGetInstallJob(string packageId)
        {
            _packageId = packageId;
        }

        public void ExecuteMigration()
        {
            throw new System.NotImplementedException();
        }
    }
}