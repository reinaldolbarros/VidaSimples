using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace VidaSimples
{
    public partial class ControleFinanceiroPage : ContentPage
    {
        public class Lancamento
        {
            public string Tipo { get; set; } // Receita ou Despesa
            public decimal Valor { get; set; }
            public string Descricao { get; set; }
            public DateTime Data { get; set; }
        }

        ObservableCollection<Lancamento> lancamentos = new ObservableCollection<Lancamento>();

        public ControleFinanceiroPage()
        {
            InitializeComponent();
            LancamentosCollectionView.ItemsSource = lancamentos;
        }

        private async void OnAdicionarLancamentoClicked(object sender, EventArgs e)
        {
            string tipo = await DisplayActionSheet("Tipo", "Cancelar", null, "Receita", "Despesa");
            if (tipo == "Cancelar" || string.IsNullOrEmpty(tipo)) return;

            string valorStr = await DisplayPromptAsync("Valor", "Informe o valor:");
            if (!decimal.TryParse(valorStr, out decimal valor)) return;

            string descricao = await DisplayPromptAsync("Descrição", "Informe uma descrição:");

            lancamentos.Add(new Lancamento
            {
                Tipo = tipo,
                Valor = valor,
                Descricao = descricao,
                Data = DateTime.Now
            });
        }
    }
}