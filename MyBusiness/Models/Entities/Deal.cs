namespace MyBusiness.Models.Entities;

public class Deal
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;

    public DateTime CreationDateTime { get; set; }

    public string Client { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Stage { get; set; } = string.Empty;
    
    public double Amount { get; set; }

    public string Currency { get; set; } = string.Empty;
}