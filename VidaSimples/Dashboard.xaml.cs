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
            // Exemplo: abre alerta, você pode adaptar para abrir uma tela de cadastro
            DisplayAlert("Adicionar", "Botão de adicionar pressionado!", "OK");
        }

        private void OnDashboardClicked(object sender, EventArgs e)
        {
            // Exemplo: já está no dashboard, pode não fazer nada ou mostrar alerta
            DisplayAlert("Dashboard", "Você já está no Dashboard.", "OK");
        }

        private async void OnContasClicked(object sender, EventArgs e)
        {
            // Navegue para a página de contas (substitua pelo seu código de navegação real)
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
            DisplayAlert("Configurações", "Ir para a tela de Configurações.", "OK");
            await Navigation.PushAsync(new ConfigPage());
        }

        private async void OnControleFinanceiroClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new ControleFinanceiroPage());
        }
    }
}