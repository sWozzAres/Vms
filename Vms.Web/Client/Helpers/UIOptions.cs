using Utopia.Blazor.Component;
using Vms.Web.Shared;

namespace Vms.Web.Client.Helpers;

public static class UIOptions
{
    public static List<SelectOption<string?>> BuildCompanyOptions(IEnumerable<CompanyListModel>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Code, m.Name)).ToList();
        options.Insert(0, new(null, "-- Select Company --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildMakeOptions(IEnumerable<VehicleMakeShortListModel>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Make, m.Make)).ToList();
        options.Insert(0, new(null, "-- Select Make --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildModelOptions(IEnumerable<VehicleModelShortListModel>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Model, m.Model)).ToList();
        options.Insert(0, new(null, "-- Select Model --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildRefusalReasonOptions(IEnumerable<RefusalReasonDto>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Code, m.Name)).ToList();
        options.Insert(0, new(null, "-- Select Reason --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildNonArrivalReasonOptions(IEnumerable<NonArrivalReasonDto>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Code, m.Name)).ToList();
        options.Insert(0, new(null, "-- Select Reason --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildConfirmBookedRefusalReasonOptions(IEnumerable<ConfirmBookedRefusalReasonDto>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Code, m.Name)).ToList();
        options.Insert(0, new(null, "-- Select Reason --"));
        return options;
    }
    public static List<SelectOption<string?>> BuildRescheduleReasonOptions(IEnumerable<RescheduleReasonDto>? list)
    {
        var options = list is null
            ? new List<SelectOption<string?>>()
            : list.Select(m => new SelectOption<string?>(m.Code, m.Name)).ToList();
        options.Insert(0, new(null, "-- Select Reason --"));
        return options;
    }
}
