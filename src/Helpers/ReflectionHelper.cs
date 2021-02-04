using System;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class ReflectionHelper
    {
        public static object GetPropertyValue(this object o, string propertyName)
        {
            var property = o.GetType().GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                throw new Exception($"Failed to find {propertyName} in Type {o.GetType()}");
            }

            return property.GetValue(o);
        }

        public static void SetPropertyValue(this object o, string propertyName, object value)
        {
            var property = o.GetType().GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
            {
                throw new Exception($"Failed to find {propertyName} in Type {o.GetType()}");
            }

            property.SetValue(o, value);
        }
    }
}