using System.Xml.Linq;

namespace Tools.Extensions
{
    public static class XmlExtensions
    {
        public static T GetAttributeValue<T>(this XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName) ??
                throw new Exception($"Xml attribute '{attributeName}' could not be found.");

            return (T)Convert.ChangeType(attribute.Value, typeof(T));
        }

        public static bool TryGetAttributeValue<T>(this XElement element, string attributeName, out T attributeValue)
        {
            attributeValue = default!;

            XAttribute? attribute = element.Attribute(attributeName);
            if (attribute is null)
                return false;

            attributeValue = (T)Convert.ChangeType(attribute.Value, typeof(T));
            return true;
        }
    }
}