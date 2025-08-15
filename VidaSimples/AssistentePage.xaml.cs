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
                new ChatItem { Mensagem = "Olá! Sou seu assistente virtual. Como posso ajudar hoje?" }
            };

            ChatCollectionView.ItemsSource = ChatItens;
            // Exemplos estáticos para resumo, alertas e sugestões
            ResumoLabel.Text = "Resumo do mês: 3 contas pagas, 2 compromissos realizados";
            AlertasLabel.Text = "Conta de água vence amanhã!";
            SugestoesLabel.Text = "Prioridade: Confirmar consulta médica e pagar conta de luz.";
        }

        private void OnEnviarClicked(object sender, System.EventArgs e)
        {
            var texto = ChatEntry.Text;
            if (!string.IsNullOrWhiteSpace(texto))
            {
                ChatItens.Add(new ChatItem { Mensagem = $"Você: {texto}" });
                // Aqui você pode implementar a lógica do chatbot.
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