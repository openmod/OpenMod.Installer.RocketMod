using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class TaskWrapper<TResult>
    {
        protected PropertyInfo m_Result;
        protected Task m_Instance;

        public Task Instance => m_Instance;

        public async Task<TResult> GetResult()
        {
            await m_Instance;

            var resultObj = m_Result.GetValue(m_Instance);
            return resultObj switch
            {
                null => default,
                TResult result => result,
                _ => (TResult)Activator.CreateInstance(typeof(TResult), resultObj)
            };
        }

        public TaskWrapper(Task instance)
        {
            m_Instance = instance;
            m_Result = instance.GetType().GetProperty("Result");
        }
    }
}