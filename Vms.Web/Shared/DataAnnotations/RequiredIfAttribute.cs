﻿namespace Vms.Web.Shared.DataAnnotations;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Adapted from https://steven-giesel.com/blogPost/f6be6a72-1497-4ad5-8197-343966761b77
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object? _isValue;

    public RequiredIfAttribute(string propertyName, object? isValue)
    {
        _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        _isValue = isValue;
    }

    public override string FormatErrorMessage(string name)
    {
        //var errorMessage = $"Property {name} is required when {_propertyName} is {_isValue}.";
        var errorMessage = $"The {name} field is required.";
        return ErrorMessage ?? errorMessage;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);
        var property = validationContext.ObjectType.GetProperty(_propertyName) ?? throw new NotSupportedException($"Can't find {_propertyName} on searched type: {validationContext.ObjectType.Name}");
        var requiredIfTypeActualValue = property.GetValue(validationContext.ObjectInstance);

        if (requiredIfTypeActualValue == null && _isValue != null)
        {
            return ValidationResult.Success;
        }

        if (requiredIfTypeActualValue == null || requiredIfTypeActualValue.Equals(_isValue))
        {
            return value == null || value is string stringValue && string.IsNullOrWhiteSpace(stringValue)
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
                : ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}