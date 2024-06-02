using MyBusiness.Models.Entities;

namespace MyBusiness.Service.Models;

public class StatisticModel
{
    public List<Deal> ThreeBiggestDeals { get; set; } = new List<Deal>();

    public int CountDeals { get; set; }

    public double SumAmountDeals { get; set; }
}