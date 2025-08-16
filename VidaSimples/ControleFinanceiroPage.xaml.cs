using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace VidaSimples
{
    public partial class ControleFinanceiroPage : ContentPage
    {
        public ControleFinanceiroViewModel ViewModel => BindingContext as ControleFinanceiroViewModel;

        public ControleFinanceiroPage()
        {
            InitializeComponent();
            BindingContext = new ControleFinanceiroViewModel();
        }

        private async void OnAdicionarLancamentoClicked(object sender, EventArgs e)
        {
            var vm = ViewModel;
            string tipo = await DisplayActionSheet("Tipo", "Cancelar", null, "Entrada", "Saída");
            if (tipo == "Cancelar" || string.IsNullOrEmpty(tipo)) return;

            string valorStr = await DisplayPromptAsync("Valor", "Informe o valor:");
            if (!decimal.TryParse(valorStr, out decimal valor)) return;

            string categoria = await DisplayActionSheet("Categoria", "Cancelar", null, vm.Categorias.ToArray());
            if (categoria == "Cancelar" || string.IsNullOrEmpty(categoria)) return;

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

        private async void OnAdicionarCategoriaClicked(object sender, EventArgs e)
        {
            var vm = ViewModel;
            string novaCategoria = NovaCategoriaEntry.Text?.Trim();
            if (string.IsNullOrEmpty(novaCategoria))
            {
                await DisplayAlert("Atenção", "Digite o nome da nova categoria.", "OK");
                return;
            }
            if (vm.Categorias.Any(c => string.Equals(c, novaCategoria, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert("Atenção", "Categoria já existente.", "OK");
                return;
            }
            vm.Categorias.Add(novaCategoria);
            NovaCategoriaEntry.Text = "";
            await DisplayAlert("Sucesso", $"Categoria '{novaCategoria}' adicionada!", "OK");
        }

        private async void OnExportarDadosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Exportar", "Funcionalidade de exportação ainda não implementada.", "OK");
        }

     

        //aqui
        private async void OnVisualizarGraficoClicked(object sender, EventArgs e)
        {
            var vm = ViewModel;
            var somaPorCategoria = vm.GetSomaPorCategoria();
            var saldo = vm.SaldoAtual;
            await Navigation.PushAsync(new GraficoPizzaPage(somaPorCategoria, saldo));
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

        public ObservableCollection<string> Categorias { get; set; } = new ObservableCollection<string>
        {
            "Alimentação",
            "Transporte",
            "Saúde",
            "Lazer",
            "Vestuário",
            "Educação",
            "Investimentos"
        };
        private string categoriaSelecionada;
        public string CategoriaSelecionada
        {
            get => categoriaSelecionada;
            set
            {
                if (categoriaSelecionada != value)
                {
                    categoriaSelecionada = value;
                    FiltrarLancamentos();
                    OnPropertyChanged(nameof(CategoriaSelecionada));
                }
            }
        }

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
                new Lancamento{ Tipo="Saída", Valor=250, Categoria="Alimentação", Descricao="Compras mercado", Observacao="Supermercado X", Data=DateTime.Now.AddDays(-8)},
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
                (string.IsNullOrEmpty(CategoriaSelecionada) || l.Categoria == CategoriaSelecionada) &&
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

        //aqui
        public Dictionary<string, decimal> GetSomaPorCategoria()
        {
            // Somente Saídas agrupadas por categoria
            return LancamentosFiltrados
                .Where(l => l.Tipo == "Saída")
                .GroupBy(l => l.Categoria)
                .ToDictionary(grp => grp.Key, grp => grp.Sum(l => l.Valor));
        }
    }
}