namespace Vms.Web.Shared;

public class Person : ICopyable<Person>
{
    public int Id { get; set; }
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

    [Required, Range(0,1)]
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
}