using System.Reflection;

namespace ConsoleApp1;

public interface ICopyable<T>
{
    public void CopyFrom(T source);
    //public void CopyFrom(T source)
    //{
    //    Type recordType = typeof(T);
    //    PropertyInfo[] properties = recordType.GetProperties();

    //    foreach (PropertyInfo property in properties)
    //    {
    //        if (!IsCustomClassProperty(property.PropertyType))
    //        {
    //            object? sourceValue = property.GetValue(source);
    //            property.SetValue(this, sourceValue);
    //        }
    //    }
    //}

    //private bool IsCustomClassProperty(Type propertyType)
    //    => !propertyType.IsPrimitive && !propertyType.IsEnum && propertyType != typeof(string) && !propertyType.IsArray;
}

