using System.ComponentModel.DataAnnotations;

namespace SearchMyLife.Api.DTOs;

public class SearchRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
}
