using System;
using System.Linq;

namespace BinaryDataExplorer
{
    public static class TypeExtensions
    {
        // TODO: Needs to be recursive since KH uses a lot of Archive<Archive<File>>
        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;

            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');

                if (iBacktick > 0)
                    friendlyName = friendlyName.Remove(iBacktick);

                friendlyName += $"<{String.Join(",", type.GetGenericArguments().Select(p => p.Name))}>";
            }

            return friendlyName;
        }
    }
}