namespace BoardGames.Dtos.v1;

public class RestDto<T>
{
    public List<LinkDto> Links { get; set; } = new();
    public T Data { get; set; } = default!;
}
