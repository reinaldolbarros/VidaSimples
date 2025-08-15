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
            DisplayAlert("Contas", "Ir para a tela de Contas.", "OK");
            await Navigation.PushAsync(new ContasPage());
        }

        private async void OnAgendaClicked(object sender, EventArgs e)
        {
            DisplayAlert("Agenda", "Ir para a tela de Agenda.", "OK");
            await Navigation.PushAsync(new AgendaPage());
        }

        private async void OnAssistenteClicked(object sender, EventArgs e)
        {
            DisplayAlert("Assistente", "Ir para a tela de Assistente Virtual.", "OK");
            await Navigation.PushAsync(new AssistentePage());
        }

        private async void OnConfigClicked(object sender, EventArgs e)
        {
            DisplayAlert("Configura��es", "Ir para a tela de Configura��es.", "OK");
            await Navigation.PushAsync(new ConfigPage());
        }

        private async void OnControleFinanceiroClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new ControleFinanceiroPage());
        }
    }
}