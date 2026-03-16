using System.ComponentModel.DataAnnotations;

namespace SearchMyLife.Api.DTOs;

public class AnalyzeRequest
{
    [Required]
    public string Plaintext { get; set; } = string.Empty;
}
