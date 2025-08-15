using Microsoft.Maui.Controls;
using System;

namespace VidaSimples
{
    public partial class ConfigPage : ContentPage
    {
        public string Nome { get; set; } = "Seu Nome";
        public string Email { get; set; } = "email@exemplo.com";
        public bool NotificacoesAtivas { get; set; } = true;

        public ConfigPage()
        {
            InitializeComponent();
            BindingContext = this;
            TemaPicker.SelectedIndex = 0;
        }

        private async void OnSalvarPerfilClicked(object sender, EventArgs e)
        {
            Nome = NomeEntry.Text;
            Email = EmailEntry.Text;
            await DisplayAlert("Perfil", "Perfil atualizado!", "OK");
        }

        private async void OnBackupClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Backup", "Backup realizado com sucesso!", "OK");
        }

        private async void OnRestaurarClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Restauração", "Dados restaurados com sucesso!", "OK");
        }

        private void OnTemaChanged(object sender, EventArgs e)
        {
            switch (TemaPicker.SelectedIndex)
            {
                case 0:
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case 1:
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                default:
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }
        }
    }
}