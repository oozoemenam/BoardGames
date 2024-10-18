using System.ComponentModel.DataAnnotations;

namespace BoardGames.Dtos.v1;

public class BoardGameDto
{
    [Required] public int Id { get; set; }
    public string? Name { get; set; }
    public int? Year { get; set; }

}
