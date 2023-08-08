using System.Reflection;

namespace Vms.Web.Shared;



public static class TypeHelper
{
    public static void DeepCopy<T>(T source, T destination)
    {
        Type recordType = typeof(T);
        PropertyInfo[] properties = recordType.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var sourceValue = property.GetValue(source);
            var destValue = property.GetValue(destination);

            if (property.PropertyType.IsPrimitive)
            {
                property.SetValue(destination, sourceValue);
            }
            else if (property.PropertyType.IsEnum)
            {
                property.SetValue(destination, sourceValue);
            }
            else if (property.PropertyType == typeof(string))
            {
                if (sourceValue is null)
                {
                    property.SetValue(destination, null);
                }
                else
                {
                    property.SetValue(destination, new String((string)sourceValue));
                }
            }
            else if (property.PropertyType.IsArray)
            {

            }
            else if (property.PropertyType.IsClass)
            {
                DeepCopy(sourceValue, destValue);
            }
            else
            {
                property.SetValue(destination, sourceValue);
            }

            //if (!IsCustomClassProperty(property.PropertyType))
            //{
            //    object? sourceValue = property.GetValue(source);
            //    property.SetValue(destination, sourceValue);
            //}
        }
    }



    public static T? DeepClone<T>(T? obj)
    {
        if (obj == null)
            return default;

        Type type = obj.GetType();

        if (type.IsPrimitive || type == typeof(string))
            return obj;

        if (type.IsArray)
        {
            Type elementType = Type.GetType(type.FullName?.Replace("[]", string.Empty) ?? throw new InvalidOperationException("Type has no fullname."))
                ?? throw new InvalidOperationException("Unknown array type.");
            var array = obj as Array;
            Array copiedArray = Array.CreateInstance(elementType, array!.Length);
            for (int i = 0; i < array.Length; i++)
                copiedArray.SetValue(DeepClone(array.GetValue(i)), i);

            return (T)Convert.ChangeType(copiedArray, obj.GetType());
        }

        if (type.IsClass)
        {
            T copiedObject = (T?)Activator.CreateInstance(obj.GetType()) ?? throw new InvalidOperationException("Failed to create instance.");
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
                field.SetValue(copiedObject, DeepClone(field.GetValue(obj)));

            return copiedObject;
        }

        throw new ArgumentException("Unknown type");
    }
}
