using MyBusiness.Models.Entities;

namespace MyBusiness.Repositories;

public interface IDbProvider
{
    Task<List<Deal>> FillDeals();

    Task<List<Deal>> ClearDeals();
}