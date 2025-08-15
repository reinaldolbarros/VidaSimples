using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace VidaSimples
{
    public partial class AgendaPage : ContentPage
    {
        ObservableCollection<Compromisso> Compromissos { get; set; }
        DateTime dataAtual = DateTime.Today;
        string modo = "Mensal";

        public AgendaPage()
        {
            InitializeComponent();
            ModoPicker.SelectedIndex = 0;
            AtualizarPeriodoLabel();
            CarregarCompromissos();
            MostrarSugestaoHorariosLivres();
        }

        void AtualizarPeriodoLabel()
        {
            if (modo == "Mensal")
                PeriodoLabel.Text = dataAtual.ToString("MMMM yyyy");
            else
            {
                var inicioSemana = dataAtual.AddDays(-(int)dataAtual.DayOfWeek);
                var fimSemana = inicioSemana.AddDays(6);
                PeriodoLabel.Text = $"{inicioSemana:dd/MM} - {fimSemana:dd/MM}";
            }
        }

        void CarregarCompromissos()
        {
            // Exemplo estático, troque por dados reais
            Compromissos = new ObservableCollection<Compromisso>
            {
                new Compromisso { Titulo = "Reunião Equipe", Horario = dataAtual.AddHours(10), Descricao = "Sala 2" },
                new Compromisso { Titulo = "Dentista", Horario = dataAtual.AddHours(16), Descricao = "Clínica Central" },
            };
            CompromissosCollectionView.ItemsSource = Compromissos;
        }

        void MostrarSugestaoHorariosLivres()
        {
            // Simulação: horários livres não ocupados
            var ocupados = Compromissos.Select(c => c.Horario.Hour).ToList();
            var sugestoes = new[] { 9, 15, 18 }
                .Where(h => !ocupados.Contains(h))
                .Select(h => $"{h:00}:00");
            SugestaoLabel.Text = "Você tem horários livres às " + string.Join(", ", sugestoes);
        }

        private void OnModoChanged(object sender, EventArgs e)
        {
            modo = ModoPicker.SelectedIndex == 0 ? "Mensal" : "Semanal";
            AtualizarPeriodoLabel();
            // Aqui você pode alterar os compromissos exibidos de acordo com o modo
        }

        private void OnPrevClicked(object sender, EventArgs e)
        {
            if (modo == "Mensal")
                dataAtual = dataAtual.AddMonths(-1);
            else
                dataAtual = dataAtual.AddDays(-7);
            AtualizarPeriodoLabel();
            CarregarCompromissos();
            MostrarSugestaoHorariosLivres();
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            if (modo == "Mensal")
                dataAtual = dataAtual.AddMonths(1);
            else
                dataAtual = dataAtual.AddDays(7);
            AtualizarPeriodoLabel();
            CarregarCompromissos();
            MostrarSugestaoHorariosLivres();
        }

        private async void OnAdicionarCompromissoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Novo Compromisso", "Abrir tela de cadastro de compromisso.", "OK");
        }
    }

    public class Compromisso
    {
        public string Titulo { get; set; }
        public DateTime Horario { get; set; }
        public string Descricao { get; set; }
    }
}