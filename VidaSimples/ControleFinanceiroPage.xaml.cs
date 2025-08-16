using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace VidaSimples
{
    public partial class ControleFinanceiroPage : ContentPage
    {
        public ControleFinanceiroPage()
        {
            InitializeComponent();
            BindingContext = new ControleFinanceiroViewModel();
        }

        private async void OnAdicionarLancamentoClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ControleFinanceiroViewModel;
            string tipo = await DisplayActionSheet("Tipo", "Cancelar", null, "Entrada", "Saída");
            if (tipo == "Cancelar" || string.IsNullOrEmpty(tipo)) return;

            string valorStr = await DisplayPromptAsync("Valor", "Informe o valor:");
            if (!decimal.TryParse(valorStr, out decimal valor)) return;

            string categoria = await DisplayPromptAsync("Categoria", "Informe a categoria:");
            string descricao = await DisplayPromptAsync("Descrição", "Informe uma descrição:");
            string observacao = await DisplayPromptAsync("Observação", "Informe uma observação (opcional):");

            vm.AdicionarLancamento(new Lancamento
            {
                Tipo = tipo,
                Valor = valor,
                Categoria = categoria,
                Descricao = descricao,
                Observacao = observacao,
                Data = DateTime.Now
            });
        }

        private async void OnExportarDadosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Exportar", "Funcionalidade de exportação ainda não implementada.", "OK");
        }

        private async void OnAnexarBoletoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AnexarBoletoPage());
        }

        private async void OnInserirManualClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InserirManualPage());
        }
    }

    public class Lancamento
    {
        public string Tipo { get; set; } // "Entrada" ou "Saída"
        public decimal Valor { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
    }

    public class ControleFinanceiroViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Lancamento> Lancamentos { get; set; }
        public ObservableCollection<Lancamento> LancamentosFiltrados { get; set; }

        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Todos", "Entrada", "Saída" };
        private string tipoSelecionado = "Todos";
        public string TipoSelecionado
        {
            get => tipoSelecionado;
            set
            {
                if (tipoSelecionado != value)
                {
                    tipoSelecionado = value;
                    FiltrarLancamentos();
                    OnPropertyChanged(nameof(TipoSelecionado));
                }
            }
        }

        private DateTime dataSelecionada = DateTime.Now;
        public DateTime DataSelecionada
        {
            get => dataSelecionada;
            set
            {
                if (dataSelecionada != value)
                {
                    dataSelecionada = value;
                    FiltrarLancamentos();
                    OnPropertyChanged(nameof(DataSelecionada));
                }
            }
        }

        public decimal SaldoAtual => LancamentosFiltrados.Sum(l => l.Tipo == "Entrada" ? l.Valor : -l.Valor);
        public decimal TotalEntradas => LancamentosFiltrados.Where(l => l.Tipo == "Entrada").Sum(l => l.Valor);
        public decimal TotalSaidas => LancamentosFiltrados.Where(l => l.Tipo == "Saída").Sum(l => l.Valor);

        public ControleFinanceiroViewModel()
        {
            Lancamentos = new ObservableCollection<Lancamento>
            {
                new Lancamento{ Tipo="Entrada", Valor=1500, Categoria="Salário", Descricao="Salário mensal", Observacao="Agosto", Data=DateTime.Now.AddDays(-10)},
                new Lancamento{ Tipo="Saída", Valor=250, Categoria="Mercado", Descricao="Compras mercado", Observacao="Supermercado X", Data=DateTime.Now.AddDays(-8)},
                new Lancamento{ Tipo="Saída", Valor=400, Categoria="Aluguel", Descricao="Aluguel", Observacao="Apartamento", Data=DateTime.Now.AddDays(-4)},
                new Lancamento{ Tipo="Entrada", Valor=200, Categoria="Freelancer", Descricao="Freela", Observacao="", Data=DateTime.Now.AddDays(-2)},
            };

            LancamentosFiltrados = new ObservableCollection<Lancamento>();
            FiltrarLancamentos();
        }

        public void FiltrarLancamentos()
        {
            var filtrados = Lancamentos.Where(l =>
                (TipoSelecionado == "Todos" || l.Tipo == TipoSelecionado) &&
                l.Data.Month == DataSelecionada.Month &&
                l.Data.Year == DataSelecionada.Year);

            LancamentosFiltrados.Clear();
            foreach (var l in filtrados)
                LancamentosFiltrados.Add(l);

            OnPropertyChanged(nameof(SaldoAtual));
            OnPropertyChanged(nameof(TotalEntradas));
            OnPropertyChanged(nameof(TotalSaidas));
        }

        public void AdicionarLancamento(Lancamento lancamento)
        {
            Lancamentos.Add(lancamento);
            FiltrarLancamentos();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}