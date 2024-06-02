using Microsoft.EntityFrameworkCore;
using MyBusiness.Models;
using MyBusiness.Models.Entities;
using Newtonsoft.Json.Linq;

namespace MyBusiness.Repositories;

public class DbProvider : IDbProvider
{
    #region Fields

    private readonly ApplicationDbContext db;
    private readonly IConfiguration configuration;

    #endregion

    #region Constructor

    public DbProvider(ApplicationDbContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }

    #endregion

    #region Methods

    public async Task<List<Deal>> FillDeals()
    {
        await SaveFillDeals();
        
        return await db.Deals.OrderBy(d => d.CreationDateTime).ToListAsync();
    }

    public async Task<List<Deal>>  ClearDeals()
    {
        await SaveClearDeals();

        return await db.Deals.OrderBy(d => d.CreationDateTime).ToListAsync();
    }

    #endregion
    
    #region Private methods

    private async Task SaveFillDeals()
    {
        using HttpClient client = new HttpClient();
        
        var response = await client.GetAsync(configuration.GetValue<string>("UrlResource"));
        
        JArray jArray = JArray.Parse(await response.Content.ReadAsStringAsync());
        
        List<Deal> deals = new List<Deal>();
        
        foreach (var item in jArray)
        {
            deals.Add(new Deal
            {
                Id = Guid.NewGuid(),
                Code = item.Value<string>("Код")!,
                CreationDateTime = item.Value<DateTime>("ДатаСоздания"),
                Client = item.Value<string>("Клиент")!,
                Name = item.Value<string>("Наименование")!,
                Stage = item.Value<string>("Стадия")!,
                Amount = item.Value<double>("Сумма"),
                Currency = item.Value<string>("Валюта")!
            });
        }

        // Если не пришли данные, но не обновляем базу данных
        if (!deals.Any())
        {
            return;
        }

        await db.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Deals""");
        await db.Deals.AddRangeAsync(deals);
        await db.SaveChangesAsync();
    }

    private async Task SaveClearDeals()
    {
        await db.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Deals""");
        await db.SaveChangesAsync();
    }

    #endregion
}