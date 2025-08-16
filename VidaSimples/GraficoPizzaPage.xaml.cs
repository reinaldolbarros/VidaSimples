using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace VidaSimples
{
    public class DadoGrafico
    {
        public string Categoria { get; set; }
        public decimal Valor { get; set; }
    }

    public partial class GraficoPizzaPage : ContentPage
    {
        public ObservableCollection<DadoGrafico> DadosGrafico { get; set; }

        public GraficoPizzaPage(Dictionary<string, decimal> somaPorCategoria, decimal saldo)
        {
            InitializeComponent();

            DadosGrafico = new ObservableCollection<DadoGrafico>();

            foreach (var kvp in somaPorCategoria)
                DadosGrafico.Add(new DadoGrafico { Categoria = kvp.Key, Valor = kvp.Value });

            DadosGrafico.Add(new DadoGrafico { Categoria = "Saldo", Valor = saldo });

            BindingContext = this;
        }
    }
}