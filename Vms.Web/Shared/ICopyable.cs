using System.Reflection;

namespace Vms.Web.Shared;

public interface ICopyable<T>
{
    public void CopyFrom(T source)
    {
        Type recordType = typeof(T);
        PropertyInfo[] properties = recordType.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object? sourceValue = property.GetValue(source);
            property.SetValue(this, sourceValue);
        }
    }
}
