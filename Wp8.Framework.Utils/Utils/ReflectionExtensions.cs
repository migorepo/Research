using System;

namespace Wp8.Framework.Utils.Utils
{
    public static class ReflectionExtensions
    {
        public static string GetName(this Type type)
        {
            var fullName = type.FullName;
            var index = fullName.LastIndexOf('.');
            return index != -1 ? fullName.Substring(index + 1) : fullName;
        }

        public static object CreateGenericObject(this Type type, Type innerType, params object[] args)
        {
            var specificType = type.MakeGenericType(new[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        public static void Clone(this object source, object target)
        {
            var targetType = target.GetType();
            foreach (var p in source.GetType().GetProperties())
            {
                var tp = targetType.GetProperty(p.Name);
                if (tp != null && p.CanWrite)
                {
                    var value = p.GetValue(source, null);
                    tp.SetValue(target, value, null);
                }
            }
        }
    }
}
