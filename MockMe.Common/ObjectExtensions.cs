using System.ComponentModel;

namespace MockMe.Common
{
    public static class ObjectExtensions
    {
        public static string Description<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            return source.ToString();
        }
    }
}
