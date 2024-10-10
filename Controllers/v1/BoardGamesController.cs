using BoardGames.Dtos.v1;
using BoardGames.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoardGames.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class BoardGamesController : ControllerBase
{
    private readonly ILogger<BoardGamesController> _logger;

    public BoardGamesController(ILogger<BoardGamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetBoardGames")]
    public RestDto<BoardGame[]> Get()
    {
        return new RestDto<BoardGame[]>()
        {
            Data =
            [
            new BoardGame { Id = 1, Name="Chess", Year = 1500 },
            new BoardGame { Id = 2, Name = "Checkers", Year = 1000 },
            new BoardGame { Id = 3, Name = "Monopoly", Year = 1935 }
        ],
            Links =
            [
                new LinkDto(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET"),
            ]
        };
    }
}
