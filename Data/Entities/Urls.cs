using System.ComponentModel.DataAnnotations;

namespace UrlSamurai.Data.Entities;

public class Urls
{
    public int Id { get; set; }
    
    public string? OwnerId { get; set; }  // nullable FK

    public ApplicationUser? Owner { get; set; }

    [Required(ErrorMessage = "URL is required")]
    public string UrlValue { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? ShortId { get; set; }

    public int NumericId { get; set; }
}