using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MyBusiness.Models;
using MyBusiness.Service.Models;

namespace MyBusiness.Services;

public class EmailSender : IEmailSender
{
    #region Fields

    private readonly ApplicationDbContext db;
    private readonly IConfiguration configuration;

    #endregion

    #region Constructor

    public EmailSender(ApplicationDbContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }

    #endregion
    
    #region Methods

    public async Task SendToTeacher()
    {
        await CreateAndSendEmail();
    }

    #endregion

    #region Private methods

    private async Task CreateAndSendEmail()
    {
        var statistic = await GetStatistic();

        #region MessageBody

        var messageBody = 
            @$"<h2>Данные о 3-х крупнейших сделках:</h2>
            #threeBiggestDeals
            <h2>Общая статистика:</h2>
            <h3>Общее количество сделок за весь период: {statistic.CountDeals}</h3>
            <h3>Общая сумма по всем сделкам за весь период: {statistic.SumAmountDeals}</h3>";

        #endregion

        var threeBiggestDeals = string.Empty;
        
        foreach (var deal in statistic.ThreeBiggestDeals)
            threeBiggestDeals += $"<h3>{deal.Client} - {deal.Amount}</h3>";

        messageBody = messageBody.Replace("#threeBiggestDeals", threeBiggestDeals);

        MailAddress from = new MailAddress(configuration.GetValue<string>("EmailAddress")!, configuration.GetValue<string>("Name")!);
        
        MailAddress to = new MailAddress(configuration.GetValue<string>("EmailAddressTeacher")!);
        
        // Для тестирования
        //MailAddress to = new MailAddress(configuration.GetValue<string>("EmailAddressMe")!);

        MailMessage message = new MailMessage(from, to);
        
        message.Subject = configuration.GetValue<string>("Subject");
        message.Body = messageBody;
        message.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(configuration.GetValue<string>("SmtpHost")!, configuration.GetValue<int>("SmtpPort")!);

        smtpClient.Credentials = new NetworkCredential(configuration.GetValue<string>("EmailAddress")!, configuration.GetValue<string>("EmailPassword")!);
        smtpClient.EnableSsl = true;
        await smtpClient.SendMailAsync(message);
    }
    
    private async Task<StatisticModel> GetStatistic()
    {
        var threeBiggestDeals = await db.Deals
            .OrderByDescending(d => d.Amount)
            .Take(3)
            .ToListAsync();

        var countDeals = await db.Deals.CountAsync();

        var sumAmountDeals = await db.Deals.SumAsync(d => d.Amount);

        return new StatisticModel { ThreeBiggestDeals = threeBiggestDeals, CountDeals = countDeals, SumAmountDeals = sumAmountDeals };
    }

    #endregion
}