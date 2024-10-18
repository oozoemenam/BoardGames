using BoardGames.Data;
using BoardGames.Dtos.v1;
using BoardGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices;

namespace BoardGames.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class BoardGamesController(ApplicationDbContext context, ILogger<BoardGamesController> logger) : ControllerBase
{
    [HttpGet("GetBoardGames")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDto<BoardGame[]>> Get(
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = "Name",
        string? sortOrder = "ASC",
        string? filterQuery = null
        )
    {
        var query = context.BoardGames.AsQueryable();
        if (!string.IsNullOrEmpty(filterQuery))
        {
            query = query.Where(b => b.Name.Contains(filterQuery));
        }
        var recordCount = await query.CountAsync();
        query = query
            .OrderBy($"{sortColumn} {sortOrder}")
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
        return new RestDto<BoardGame[]>()
        {
            Data = await query.ToArrayAsync(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            RecordCount = recordCount,
            Links =
            [
                new LinkDto(Url.Action(null, "BoardGames", new { pageIndex, pageSize }, Request.Scheme)!, "self", "GET"),
            ]
        };
    }

    [HttpPost(Name = "UpdateBoardGame")]
    [ResponseCache(NoStore = true)] 
    public async Task<RestDto<BoardGame?>> Post(BoardGameDto dto)
    {
        var boardGame = await context.BoardGames
            .Where(b => b.Id == dto.Id)
            .FirstOrDefaultAsync();
        if (boardGame != null)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                boardGame.Name = dto.Name;
            if (dto.Year.HasValue && dto.Year.Value > 0)
                boardGame.Year = dto.Year.Value;
            boardGame.LastModifiedDate = DateTime.Now;
            context.BoardGames.Update(boardGame);
            await context.SaveChangesAsync();
        }
        return new RestDto<BoardGame?>()
        {
            Data = boardGame,
            Links =
            [
                new LinkDto(Url.Action(null, "BoardGames", dto, Request.Scheme)!, "self", "POST"),
            ]
        };
    }

    [HttpDelete(Name = "DeleteBoardGames")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDto<BoardGame?>> Delete(int id)
    {
        var boardGame = await context.BoardGames
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
        if (boardGame != null)
        {
            context.BoardGames.Remove(boardGame);
            await context.SaveChangesAsync();
        }
        return new RestDto<BoardGame?>()
        {
            Data = boardGame,
            Links = [
                new LinkDto(Url.Action(null, "BoardGames", id, Request.Scheme)!, "self", "DELETE"),
                ]
        };
    }
}
