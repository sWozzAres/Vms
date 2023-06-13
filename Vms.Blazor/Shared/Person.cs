using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Blazor.Shared;

public record Person : ICopyable<Person>
{
    [Required]
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Range(18, 100)]
    public int Age { get; set; }

    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "This form only allows administrators.")]
    public bool IsAdministrator { get; set; }

    [Required]
    public DateTime? BirthDate { get; set; }

    [Required]
    public string? Notes { get; set; } = null!;

    [Required]
    public int? Status { get; set; }

    [Required]
    [Range(typeof(Manufacturer), nameof(Manufacturer.SpaceX), nameof(Manufacturer.VirginGalactic), ErrorMessage = "Pick a manufacturer.")]
    public Manufacturer TheManufacturer { get; set; } = Manufacturer.None;

    // [Required, EnumDataType(typeof(Color))]
    // public Color? TheColor { get; set; } = null;

    // [Required, EnumDataType(typeof(Engine))]
    // public Engine? TheEngine { get; set; } = null;

    public enum Manufacturer { None, SpaceX, NASA, ULA, VirginGalactic }
    public enum Color { ImperialRed, SpacecruiserGreen, StarshipBlue, VoyagerOrange }
    public enum Engine { Ion, Plasma, Fusion, Warp }

    public void CopyFrom(Person source)
    {
        Name = source.Name;
        Email = source.Email;
        Age = source.Age;
        IsAdministrator = source.IsAdministrator;
        BirthDate = source.BirthDate;
        Notes = source.Notes;
        Status = source.Status;
        TheManufacturer = source.TheManufacturer;
    }
}