using Microsoft.Maui.Controls;

namespace VidaSimples;

public partial class InserirManualPage : ContentPage
{
    public InserirManualPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarManualClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Sucesso", "Conta/Boleto adicionado!", "OK");
        await Navigation.PopAsync();
    }
}