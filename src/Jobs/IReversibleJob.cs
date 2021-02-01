namespace OpenMod.Installer.RocketMod.Jobs
{
    public interface IReversibleJob : IJob
    {
        void Revert();
    }
}