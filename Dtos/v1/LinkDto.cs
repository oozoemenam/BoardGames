namespace BoardGames.Dtos.v1;

public record LinkDto(string? Href, string? Rel, string? Type)
{
    public string? Href { get; private set; } = Href;
    public string? Rel { get; private set; } = Rel;
    public string? Type { get; private set; } = Type;
}
