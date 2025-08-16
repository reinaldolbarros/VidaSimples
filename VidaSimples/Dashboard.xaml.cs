using Microsoft.Maui.Controls;
using System;

namespace VidaSimples
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            // Exemplo: abre alerta, voc� pode adaptar para abrir uma tela de cadastro
            DisplayAlert("Adicionar", "Bot�o de adicionar pressionado!", "OK");
        }

        private void OnDashboardClicked(object sender, EventArgs e)
        {
            // Exemplo: j� est� no dashboard, pode n�o fazer nada ou mostrar alerta
            DisplayAlert("Dashboard", "Voc� j� est� no Dashboard.", "OK");
        }

        private async void OnContasClicked(object sender, EventArgs e)
        {
            // Navegue para a p�gina de contas (substitua pelo seu c�digo de navega��o real)
            await Navigation.PushAsync(new ContasPage());
        }

        private async void OnAgendaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendaPage());
        }

        private async void OnAssistenteClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssistentePage());
        }

        private async void OnConfigClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConfigPage());
        }

        private async void OnControleFinanceiroClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new ControleFinanceiroPage());
        }
    }
}