using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace VidaSimples
{
    public partial class AssistentePage : ContentPage
    {
        ObservableCollection<ChatItem> ChatItens { get; set; }

        public AssistentePage()
        {
            InitializeComponent();

            ChatItens = new ObservableCollection<ChatItem>
            {
                new ChatItem { Mensagem = "Ol�! Sou seu assistente virtual. Como posso ajudar hoje?" }
            };

            ChatCollectionView.ItemsSource = ChatItens;
            // Exemplos est�ticos para resumo, alertas e sugest�es
            ResumoLabel.Text = "Resumo do m�s: 3 contas pagas, 2 compromissos realizados";
            AlertasLabel.Text = "Conta de �gua vence amanh�!";
            SugestoesLabel.Text = "Prioridade: Confirmar consulta m�dica e pagar conta de luz.";
        }

        private void OnEnviarClicked(object sender, System.EventArgs e)
        {
            var texto = ChatEntry.Text;
            if (!string.IsNullOrWhiteSpace(texto))
            {
                ChatItens.Add(new ChatItem { Mensagem = $"Voc�: {texto}" });
                // Aqui voc� pode implementar a l�gica do chatbot.
                ChatItens.Add(new ChatItem { Mensagem = $"Assistente: Comando \"{texto}\" recebido." });
                ChatEntry.Text = "";
            }
        }

        public class ChatItem
        {
            public string Mensagem { get; set; }
        }
    }
}