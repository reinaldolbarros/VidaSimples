using Microsoft.Maui.Controls;

namespace VidaSimples;

public partial class AnexarBoletoPage : ContentPage
{
    public AnexarBoletoPage()
    {
        InitializeComponent();
    }

    private async void OnEscolherArquivoClicked(object sender, EventArgs e)
    {
        var file = await FilePicker.Default.PickAsync();
        if (file != null)
        {
            lblStatus.Text = $"Arquivo selecionado: {file.FileName}";
            await DisplayAlert("Arquivo", $"Arquivo {file.FileName} selecionado!", "OK");
        }
    }
}