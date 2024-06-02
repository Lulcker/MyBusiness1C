using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyBusiness.Models.Entities;
using MyBusiness.Repositories;
using MyBusiness.Services;

namespace MyBusiness.Pages;

public partial class Index : ComponentBase
{
    #region Injects

    [Inject] private IDbProvider DbProvider { get; set; } = null!;

    [Inject] private IEmailSender EmailSender { get; set; } = null!;

    [Inject] private ISnackbar Messages { get; set; } = null!;

    #endregion

    #region Variable

    private List<Deal> _data = new();

    private bool IsFilled { get; set; } = true;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        _data = await DbProvider.FillDeals();
    }

    private async Task FillTableClick()
    {
        _data = await DbProvider.FillDeals();
        IsFilled = true;
        
        Messages.Add("Таблица заполнена", Severity.Success);
    }
    
    private async Task ClearTableClick()
    {
        _data = await DbProvider.ClearDeals();
        IsFilled = false;
        
        Messages.Add("Таблица очищена", Severity.Success);
    }

    private async Task SendToTeacherClick()
    {
        await EmailSender.SendToTeacher();
        
        Messages.Add("Сообщение с отчётом отправлено на почту", Severity.Success);
    }

    #endregion
}