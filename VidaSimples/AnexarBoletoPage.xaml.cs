using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace VidaSimples;

public partial class AnexarBoletoPage : ContentPage, INotifyPropertyChanged
{
    // No início da classe:
    public ObservableCollection<Conta> ContasExternas { get; set; }
    public AnexarBoletoPage(ObservableCollection<Conta> contasExternas)
    {
        InitializeComponent();
        BindingContext = this;
        ContasExternas = contasExternas;
    }
    private FileResult _arquivoSelecionado;

    public FileResult ArquivoSelecionado
    {
        get => _arquivoSelecionado;
        set
        {
            _arquivoSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ArquivoSelecionadoExiste));
        }
    }

    public bool ArquivoSelecionadoExiste => ArquivoSelecionado != null;

    public event PropertyChangedEventHandler PropertyChanged;

   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void OnEscolherArquivoClicked(object sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione o boleto (PDF)",
                FileTypes = FilePickerFileType.Pdf
            });

            if (file != null)
            {
                ArquivoSelecionado = file;
                lblStatus.Text = $"Arquivo selecionado: {file.FileName}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao selecionar arquivo: {ex.Message}", "OK");
        }
    }

    private async void OnExtrairDadosClicked(object sender, EventArgs e)
    {
        if (ArquivoSelecionado == null)
        {
            await DisplayAlert("Atenção", "Selecione um arquivo primeiro.", "OK");
            return;
        }

        try
        {
            using var stream = await ArquivoSelecionado.OpenReadAsync();
            using var pdfDoc = new Syncfusion.Pdf.Parsing.PdfLoadedDocument(stream);
            StringBuilder textoCompleto = new StringBuilder();

            // Extrai texto de todas as páginas do PDF
            for (int i = 0; i < pdfDoc.PageCount; i++)
            {
                var page = pdfDoc.Pages[i] as PdfLoadedPage;
                string textoPage = page.ExtractText();
                textoCompleto.AppendLine(textoPage);
            }

            string texto = textoCompleto.ToString();

            // Extrai valor (R$ ...)
            double valor = ExtrairValor(texto);

            // Extrai vencimento (data dd/MM/yyyy)
            string vencimento = ExtrairVencimento(texto);

            // Classifica tipo automaticamente com método do ContasPage
            string tipoConta = ContasPage.ClassificarTipoConta(texto);

            // Cria nova conta
            DateTime vencimentoCorreto;
            if (!DateTime.TryParseExact(vencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out vencimentoCorreto))
                vencimentoCorreto = DateTime.Today.AddDays(30);

            var novaConta = new Conta
            {
                Nome = tipoConta,
                Valor = valor,
                Vencimento = vencimentoCorreto,
                Status = vencimentoCorreto < DateTime.Today ? "Vencido" : "Pendente",
                Tipo = tipoConta
            };

            // Adiciona à lista principal vinda do ContasPage
            ContasExternas.Add(novaConta);

            // Atualiza status e exibe mensagem
            lblStatus.Text = $"Conta classificada como: {tipoConta}\nValor: R$ {valor:F2}\nVencimento: {(string.IsNullOrEmpty(vencimento) ? "Não encontrado" : vencimento)}";
            await DisplayAlert("Conta Adicionada", $"A conta foi classificada como: {tipoConta} e adicionada à sua lista!", "OK");

            // Volta para a tela principal de contas
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao extrair dados do boleto: {ex.Message}", "OK");
        }
    }

    private double ExtrairValor(string texto)
    {
        var padrao = new Regex(@"R\$\s*(\d+[.,]\d{2})");
        var match = padrao.Match(texto);
        if (match.Success)
        {
            string valorTexto = match.Groups[1].Value.Replace(",", ".");
            if (double.TryParse(valorTexto, out double valor))
                return valor;
        }
        return 0;
    }

    // Substitua seu método antigo pelo abaixo:

    // Substitua seu método antigo pelo abaixo:

    private string ExtrairVencimento(string texto)
    {
        var padrao = new Regex(@"(\d{2}[/\-\.]\d{2}[/\-\.]\d{4})");
        var match = padrao.Match(texto);
        if (match.Success)
        {
            DateTime venc;
            // Tenta converter para DateTime usando os formatos mais comuns brasileiros
            if (DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out venc) ||
                DateTime.TryParseExact(match.Groups[1].Value, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out venc) ||
                DateTime.TryParseExact(match.Groups[1].Value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out venc))
            {
                // Retorna no formato dd/MM/yyyy
                return venc.ToString("dd/MM/yyyy");
            }
            // Se não conseguir, retorna o texto original
            return match.Groups[1].Value;
        }
        return "";
    }
}