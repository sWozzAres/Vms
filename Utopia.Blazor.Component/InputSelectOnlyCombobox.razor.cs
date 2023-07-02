using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Utopia.Blazor.Component;

[CascadingTypeParameter(nameof(TValue))]
public partial class InputSelectOnlyCombobox<TValue>
{
    [Parameter]
    public string LabelText { get; set; } = null!;

    [Parameter]
    public List<SelectOption<TValue>> Options { get; set; } = null!;

    [Parameter] public string ParsingErrorMessage { get; set; } = "The {0} field is not valid.";

    protected override bool TryParseValueFromString(string? value,
    [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.InvariantCulture, out result))
        {
            validationErrorMessage = null;
            return true;
        }
        else
        {
            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, ParsingErrorMessage, 
                DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
    }
}
