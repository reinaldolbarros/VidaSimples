using Microsoft.Maui.Controls;
using System;

namespace VidaSimples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Login", "Bem-vindo ao VidaSimples!", "OK");
            await Navigation.PushAsync(new Dashboard());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Cadastro", "Ir para tela de cadastro.", "OK");
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Recuperar senha", "Ir para tela de recuperação de senha.", "OK");
        }
    }
}