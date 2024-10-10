using BoardGames.Dtos.v1;

namespace BoardGames.Dtos.v2;

public class RestDto<T>
{
    public List<LinkDto> Links { get; set; } = new();
    public T Items { get; set; } = default!;
}
