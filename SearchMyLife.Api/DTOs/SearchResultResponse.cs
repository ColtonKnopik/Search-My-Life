namespace SearchMyLife.Api.DTOs;

public class SearchResultResponse : EntryResponse
{
    public double Score { get; set; }
    public string? RelevanceReason { get; set; }
}

public class SearchResponse
{
    public string Overview { get; set; } = string.Empty;
    public List<SearchResultResponse> TopResults { get; set; } = [];
    public List<SearchResultResponse> OtherResults { get; set; } = [];
}
