using System.ComponentModel.DataAnnotations;

namespace SearchMyLife.Api.DTOs;

public class CreateEntryRequest
{
    public string? Title { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? Iv { get; set; }
    public string? Salt { get; set; }
}
