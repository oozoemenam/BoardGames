using BoardGames.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGames.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("v{verion:apiVersion}/[controller]")]
public class BoardGamesController : ControllerBase
{
    private readonly ILogger<BoardGamesController> _logger;

    public BoardGamesController(ILogger<BoardGamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetBoardGames")]
    public Dtos.v2.RestDto<BoardGame[]> Get()
    {
        return new Dtos.v2.RestDto<BoardGame[]>()
        {
            Items =
            [
            new BoardGame { Id = 1, Name="Chess", Year = 1500 },
            new BoardGame { Id = 2, Name = "Checkers", Year = 1000 },
            new BoardGame { Id = 3, Name = "Monopoly", Year = 1935 }
        ],
            Links =
            [
                new Dtos.v1.LinkDto(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET"),
            ]
        };
    }
}
