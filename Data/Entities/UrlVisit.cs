using System;
using System.ComponentModel.DataAnnotations;

namespace UrlSamurai.Data.Entities;

public class UrlVisit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string ShortId { get; set; } = string.Empty;

    public string? Country { get; set; }

    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    
    public Urls? Url { get; set; }
}