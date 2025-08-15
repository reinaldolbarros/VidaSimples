using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace VidaSimples
{
    public partial class ContasPage : ContentPage
    {
        public ObservableCollection<Conta> Contas { get; set; }

        public ContasPage()
        {
            InitializeComponent();
        
            // Exemplo de dados
            Contas = new ObservableCollection<Conta>
            {
                new Conta { Nome = "Luz", Valor = 120.0, Vencimento = DateTime.Today },
                new Conta { Nome = "Internet", Valor = 80.0, Vencimento = DateTime.Today.AddDays(1) }
            };

            ContasCollectionView.ItemsSource = Contas;
        }

        private async void OnAdicionarContaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Adicionar Conta", "Aqui você pode implementar a lógica para adicionar uma nova conta.", "OK");
        }
    }

    public class Conta
    {
        public string Nome { get; set; }
        public double Valor { get; set; }
        public DateTime Vencimento { get; set; }
    }
}