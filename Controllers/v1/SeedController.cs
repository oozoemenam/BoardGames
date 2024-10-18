
using BoardGames.Data;
using BoardGames.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BoardGames.Controllers.v1;

[ApiController]
[Route("[controller]")]
public class SeedController(
    ApplicationDbContext context,
    ILogger<SeedController> logger,
    IWebHostEnvironment env
    ) : ControllerBase
{
    [HttpPut(Name = "Seed")]
    [ResponseCache(NoStore = true)]
    public async Task<JsonResult> Put()
    {
        var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true,
            Delimiter = ";",
        };
        using var reader = new StreamReader(Path.Combine(env.ContentRootPath, "Data/board_games.csv"));
        using var csv = new CsvReader(reader, config);
        var existingBoardGames = await context.BoardGames.ToDictionaryAsync(b => b.Id);
        var existingDomains = await context.Domains.ToDictionaryAsync(d => d.Name);
        var existingMechanics = await context.Mechanics.ToDictionaryAsync(m => m.Name);
        var now = DateTime.Now;

        var records = csv.GetRecords<BoardGameRecord>();
        var skippedRows = 0;
        foreach (var record in records)
        {
            if (!record.ID.HasValue
                || string.IsNullOrEmpty(record.Name)
                || existingBoardGames.ContainsKey(record.ID.Value))
            {
                skippedRows++;
                continue;
            }
            var boardGame = new BoardGame()
            {
                Id = record.ID.Value,
                Name = record.Name,
                BGGRank = record.BGGRank ?? 0,
                ComplexityAverage = record.ComplexityAverage ?? 0,
                MaxPlayers = record.MaxPlayers ?? 0,
                MinAge = record.MinAge ?? 0,
                MinPlayers = record.MinPlayers ?? 0,
                OwnedUsers = record.OwnedUsers ?? 0,
                PlayTime = record.PlayTime ?? 0,
                RatingAverage = record.RatingAverage ?? 0,
                UsersRated = record.UsersRated ?? 0,
                Year = record.YearPublished ?? 0,
                CreatedDate = now,
                LastModifiedDate = now,
            };
            context.BoardGames.Add(boardGame);

            if (!string.IsNullOrEmpty(record.Domains))
            {
                foreach (var domainName in record.Domains
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var domain = existingDomains.GetValueOrDefault(domainName);
                    if (domain == null)
                    {
                        domain = new Domain()
                        {
                          Name = domainName,
                          CreatedDate = now,
                          LastModifiedDate = now,
                        };
                        context.Domains.Add(domain);    
                        existingDomains.Add(domainName, domain);    
                    }
                    context.BoardGames_Domains.Add(new BoardGames_Domains()
                    {
                        BoardGame = boardGame,
                        Domain = domain,
                        CreatedDate = now
                    });
                }
            }

            if (!string.IsNullOrEmpty(record.Mechanics))
            {
                foreach (var mechanicName in record.Mechanics
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase))
                {
                    var mechanic = existingMechanics.GetValueOrDefault(mechanicName);   
                    if (mechanic == null)
                    {
                        mechanic = new Mechanic()
                        {
                            Name = mechanicName,
                            CreatedDate = now,
                            LastModifiedDate = now,
                        };
                        context.Mechanics.Add(mechanic);    
                        existingMechanics.Add(mechanicName, mechanic);  
                    }
                    context.BoardGames_Mechanics.Add(new BoardGames_Mechanics()
                    {
                        BoardGame = boardGame,
                        Mechanic = mechanic,
                        CreatedDate = now
                    });
                }
            }
        }

        //using var transaction = context.Database.BeginTransaction();
        //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames ON");
        await context.SaveChangesAsync();
        //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames OFF");
        //transaction.Commit();   

        return new JsonResult(new
        {
            BoardGames = context.BoardGames.Count(),
            Domains = context.Domains.Count(),
            Mechanics = context.Mechanics.Count(),
            SkippedRows = skippedRows
        });
    }
}
