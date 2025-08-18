using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Parsing;
using SyncfusionDrawing = Syncfusion.Drawing;
using MauiColor = Microsoft.Maui.Graphics.Color;
using MauiColors = Microsoft.Maui.Graphics.Colors;
using System.Text.RegularExpressions;
using Microsoft.Maui.Storage;

namespace VidaSimples
{
    public partial class ContasPage : ContentPage
    {

        public FileResult ArquivoSelecionado { get; set; }
        public ObservableCollection<Conta> Contas { get; set; }

        public ContasPage()
        {
            InitializeComponent();

            // Exemplo de dados com mais variedade
            Contas = new ObservableCollection<Conta>
            {
                new Conta { Nome = "Conta de Luz", Valor = 150.0, Vencimento = DateTime.Today.AddDays(5), Status = "Pendente" },
                new Conta { Nome = "Internet", Valor = 120.0, Vencimento = DateTime.Today.AddDays(10), Status = "Pendente" },
                new Conta { Nome = "Água", Valor = 80.0, Vencimento = DateTime.Today.AddDays(-2), Status = "Vencido" },
                new Conta { Nome = "Telefone", Valor = 65.0, Vencimento = DateTime.Today.AddDays(15), Status = "Pendente" },
                new Conta { Nome = "Cartão de Crédito", Valor = 280.0, Vencimento = DateTime.Today.AddDays(1), Status = "Pendente" }
            };

            ContasCollectionView.ItemsSource = Contas;
            AtualizarResumo();
        }

        private void AtualizarResumo()
        {
            int totalContas = Contas.Count;
            double valorTotal = 0;
            int pendentes = 0;
            int vencidas = 0;

            foreach (var conta in Contas)
            {
                valorTotal += conta.Valor;
                if (conta.Status == "Vencido") vencidas++;
                else if (conta.Status == "Pendente") pendentes++;
            }

            ResumoLabel.Text = $"Total: {totalContas} contas | Pendentes: {pendentes} | Vencidas: {vencidas}";
            TotalLabel.Text = $"Valor Total: {valorTotal:C}";
        }

        private async void OnAdicionarContaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Adicionar Conta", "Aqui você pode implementar a lógica para adicionar uma nova conta.", "OK");
        }

        private async void OnAnexarBoletoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AnexarBoletoPage(Contas));
        }

        private async void OnInserirManualClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InserirManualPage());
        }

        // NOVA FUNCIONALIDADE: Ler PDF
        private async void OnLerPdfClicked(object sender, EventArgs e)
        {
            try
            {
                // Permitir ao usuário escolher um arquivo PDF
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { ".pdf" } },
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "pdf" } },
                    });

                PickOptions options = new()
                {
                    PickerTitle = "Selecione um arquivo PDF para ler",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    await LerConteudoPdfAsync(result);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao selecionar arquivo: {ex.Message}", "OK");
            }
        }

        private async Task LerConteudoPdfAsync(FileResult arquivo)
        {
            try
            {
                using var stream = await arquivo.OpenReadAsync();

                // Carregar o documento PDF
                using (PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream))
                {
                    StringBuilder textoCompleto = new StringBuilder();
                    List<ContaPdf> contasEncontradas = new List<ContaPdf>();

                    // Extrair texto de todas as páginas
                    for (int i = 0; i < loadedDocument.PageCount; i++)
                    {
                        PdfLoadedPage page = loadedDocument.Pages[i] as PdfLoadedPage;
                        string textoPage = page.ExtractText();
                        textoCompleto.AppendLine($"--- PÁGINA {i + 1} ---");
                        textoCompleto.AppendLine(textoPage);
                        textoCompleto.AppendLine();

                        // Tentar identificar contas no texto (lógica básica)
                        IdentificarContasNoTexto(textoPage, contasEncontradas);
                    }

                    // Mostrar resultado
                    await MostrarResultadoLeituraPdf(loadedDocument.PageCount, textoCompleto.ToString(), contasEncontradas);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro ao Ler PDF", $"Não foi possível ler o arquivo: {ex.Message}", "OK");
            }
        }

        private void IdentificarContasNoTexto(string texto, List<ContaPdf> contasEncontradas)
        {
            try
            {
                // Padrões básicos para identificar valores monetários e datas
                var padraoValor = new System.Text.RegularExpressions.Regex(@"R\$\s*(\d+[.,]\d{2})");
                var padraoData = new System.Text.RegularExpressions.Regex(@"(\d{2}[/.-]\d{2}[/.-]\d{4})");

                var linhas = texto.Split('\n');
                string beneficiario = ExtrairBeneficiario(texto);
                foreach (string linha in linhas)
                {
                    var matchValor = padraoValor.Match(linha);
                    var matchData = padraoData.Match(linha);

                    if (matchValor.Success)
                    {
                        string valorTexto = matchValor.Groups[1].Value.Replace(",", ".");
                        if (double.TryParse(valorTexto, out double valor))
                        {
                            ContaPdf conta = new ContaPdf
                            {
                                LinhaOriginal = linha.Trim(),
                                Valor = valor,
                                DataEncontrada = matchData.Success ? matchData.Groups[1].Value : "",
                                PossivelDescricao = ExtrairDescricaoLinha(linha, matchValor.Index),
                                Beneficiario = beneficiario
                            };

                            contasEncontradas.Add(conta);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignorar erros de parsing - apenas continuar
            }
        }

        private string ExtrairDescricaoLinha(string linha, int posicaoValor)
        {
            try
            {
                // Pegar texto antes do valor como possível descrição
                string descricao = linha.Substring(0, Math.Max(0, posicaoValor)).Trim();

                // Limitar tamanho da descrição
                if (descricao.Length > 50)
                    descricao = descricao.Substring(0, 50) + "...";

                return string.IsNullOrWhiteSpace(descricao) ? "Descrição não identificada" : descricao;
            }
            catch
            {
                return "Descrição não identificada";
            }
        }
        private string ExtrairBeneficiario(string texto)
        {
            // Procura "Beneficiário: Nome" ou "Cedente: Nome"
            var padrao = new System.Text.RegularExpressions.Regex(@"(Benefici[aá]rio|Cedente)\s*[:\-]?\s*(.+)", RegexOptions.IgnoreCase);
            var match = padrao.Match(texto);
            if (match.Success)
                return match.Groups[2].Value.Trim();
            return "";
        }
        private async Task MostrarResultadoLeituraPdf(int numeroPaginas, string textoCompleto, List<ContaPdf> contasEncontradas)
        {
            // Preparar resumo
            string resumo = $"📄 ANÁLISE DO PDF\n\n";
            resumo += $"📊 Páginas: {numeroPaginas}\n";
            resumo += $"💰 Possíveis contas encontradas: {contasEncontradas.Count}\n\n";

            if (contasEncontradas.Any())
            {
                resumo += "💡 CONTAS IDENTIFICADAS:\n";
                foreach (var conta in contasEncontradas.Take(5)) // Mostrar apenas 5 primeiras
                {
                    resumo += $"• {conta.PossivelDescricao}\n";
                    resumo += $"  Valor: R$ {conta.Valor:F2}";
                    if (!string.IsNullOrEmpty(conta.DataEncontrada))
                        resumo += $" | Data: {conta.DataEncontrada}";
                    resumo += "\n\n";
                }

                if (contasEncontradas.Count > 5)
                    resumo += $"... e mais {contasEncontradas.Count - 5} contas encontradas\n\n";
            }

            // Opções para o usuário
            string acao = await DisplayActionSheet(
                "PDF Analisado com Sucesso!",
                "Fechar",
                null,
                "Ver Resumo",
                "Ver Texto Completo",
                "Importar Contas Encontradas");

            switch (acao)
            {
                case "Ver Resumo":
                    await DisplayAlert("Resumo da Análise", resumo, "OK");
                    break;

                case "Ver Texto Completo":
                    // Limitar texto para não travar a interface
                    string textoLimitado = textoCompleto.Length > 2000
                        ? textoCompleto.Substring(0, 2000) + "\n\n[Texto truncado...]"
                        : textoCompleto;
                    await DisplayAlert("Conteúdo Completo do PDF", textoLimitado, "OK");
                    break;

                case "Importar Contas Encontradas":
                    await ImportarContasEncontradas(contasEncontradas);
                    break;
            }
        }

        private async Task ImportarContasEncontradas(List<ContaPdf> contasEncontradas)
        {
            if (!contasEncontradas.Any())
            {
                await DisplayAlert("Aviso", "Nenhuma conta foi identificada para importar.", "OK");
                return;
            }

            bool confirmar = await DisplayAlert(
                "Importar Contas",
                $"Deseja importar {contasEncontradas.Count} contas encontradas para sua lista?",
                "Sim", "Não");

            if (confirmar)
            {
                int importadas = 0;
                foreach (var contaPdf in contasEncontradas)
                {
                    // Tentar parsear data
                    DateTime vencimento = DateTime.Today.AddDays(30); // Padrão: 30 dias
                    if (!string.IsNullOrEmpty(contaPdf.DataEncontrada))
                    {
                        if (DateTime.TryParse(contaPdf.DataEncontrada.Replace('/', '-'), out DateTime dataParsed))
                            vencimento = dataParsed;
                    }

                    var novaConta = new Conta
                    {
                        Nome = contaPdf.PossivelDescricao,
                        Valor = contaPdf.Valor,
                        Vencimento = vencimento,
                        Status = vencimento < DateTime.Today ? "Vencido" : "Pendente",
                        
                    };

                    Contas.Add(novaConta);
                    importadas++;
                }

                AtualizarResumo();
                await DisplayAlert("Sucesso!", $"{importadas} contas foram importadas com sucesso!", "OK");
            }
        }
        private async void OnGerarRelatorioPdfClicked(object sender, EventArgs e)
        {
            try
            {
                // Mostrar indicador de carregamento
                await DisplayAlert("Gerando PDF", "Aguarde, gerando seu relatório...", "OK");

                string caminhoArquivo = await GerarRelatorioPdfAsync();

                if (!string.IsNullOrEmpty(caminhoArquivo))
                {
                    // Perguntar se quer compartilhar
                    bool compartilhar = await DisplayAlert("PDF Gerado!",
                        "Relatório gerado com sucesso! Deseja compartilhar?",
                        "Compartilhar", "Fechar");

                    if (compartilhar)
                    {
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            Title = "Relatório de Contas - Vida Simples",
                            File = new ShareFile(caminhoArquivo)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao gerar PDF: {ex.Message}", "OK");
            }
        }

        private async Task<string> GerarRelatorioPdfAsync()
        {
            try
            {
                // Criar documento PDF
                using (PdfDocument document = new PdfDocument())
                {
                    // Adicionar página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Definir fontes
                    PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold);
                    PdfFont subtitleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
                    PdfFont smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

                    float yPosition = 50;

                    // Cabeçalho
                    graphics.DrawString("💰 RELATÓRIO DE CONTAS", titleFont, PdfBrushes.DarkBlue, new SyncfusionDrawing.PointF(50, yPosition));
                    yPosition += 35;

                    graphics.DrawString("Vida Simples - Controle Financeiro", subtitleFont, PdfBrushes.Gray, new SyncfusionDrawing.PointF(50, yPosition));
                    yPosition += 20;

                    graphics.DrawString($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont, PdfBrushes.Black, new SyncfusionDrawing.PointF(50, yPosition));
                    yPosition += 30;

                    // Resumo
                    double valorTotal = Contas.Sum(c => c.Valor);
                    int pendentes = Contas.Count(c => c.Status == "Pendente");
                    int vencidas = Contas.Count(c => c.Status == "Vencido");

                    // Caixa de resumo
                    SyncfusionDrawing.RectangleF resumoRect = new SyncfusionDrawing.RectangleF(50, yPosition, page.Size.Width - 100, 80);
                    graphics.DrawRectangle(new PdfSolidBrush(SyncfusionDrawing.Color.FromArgb(240, 248, 255)), resumoRect);
                    graphics.DrawRectangle(PdfPens.LightGray, resumoRect);

                    graphics.DrawString("📊 RESUMO FINANCEIRO", subtitleFont, PdfBrushes.DarkBlue, new SyncfusionDrawing.PointF(60, yPosition + 15));
                    graphics.DrawString($"Total de contas: {Contas.Count}", normalFont, PdfBrushes.Black, new SyncfusionDrawing.PointF(60, yPosition + 35));
                    graphics.DrawString($"Valor total: {valorTotal:C}", normalFont, PdfBrushes.Black, new SyncfusionDrawing.PointF(60, yPosition + 50));
                    graphics.DrawString($"Pendentes: {pendentes} | Vencidas: {vencidas}", normalFont,
                        vencidas > 0 ? PdfBrushes.Red : PdfBrushes.Green, new SyncfusionDrawing.PointF(250, yPosition + 35));

                    yPosition += 100;

                    // Título da tabela
                    graphics.DrawString("📋 DETALHAMENTO DAS CONTAS", subtitleFont, PdfBrushes.DarkBlue, new SyncfusionDrawing.PointF(50, yPosition));
                    yPosition += 30;

                    // Criar tabela
                    PdfGrid pdfGrid = new PdfGrid();

                    // Preparar dados
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Descrição");
                    dataTable.Columns.Add("Valor");
                    dataTable.Columns.Add("Vencimento");
                    dataTable.Columns.Add("Status");
                    dataTable.Columns.Add("Dias para vencer");

                    foreach (var conta in Contas.OrderBy(c => c.Vencimento))
                    {
                        int diasParaVencer = (conta.Vencimento - DateTime.Today).Days;
                        string diasTexto = diasParaVencer < 0 ? $"{Math.Abs(diasParaVencer)} dias em atraso" :
                                          diasParaVencer == 0 ? "Vence hoje" :
                                          $"{diasParaVencer} dias";

                        dataTable.Rows.Add(
                            conta.Nome,
                            conta.Valor.ToString("C"),
                            conta.Vencimento.ToString("dd/MM/yyyy"),
                            conta.Status,
                            diasTexto
                        );
                    }

                    pdfGrid.DataSource = dataTable;

                    // Estilizar cabeçalho
                    PdfGridCellStyle headerStyle = new PdfGridCellStyle();
                    headerStyle.BackgroundBrush = new PdfSolidBrush(SyncfusionDrawing.Color.FromArgb(70, 130, 180));
                    headerStyle.TextBrush = PdfBrushes.White;
                    headerStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 11, PdfFontStyle.Bold);

                    // Aplicar estilo ao cabeçalho
                    PdfGridRow headerRow = pdfGrid.Headers[0];
                    for (int i = 0; i < headerRow.Cells.Count; i++)
                    {
                        headerRow.Cells[i].Style = headerStyle;
                    }

                    // Estilizar linhas de dados
                    for (int i = 0; i < pdfGrid.Rows.Count; i++)
                    {
                        PdfGridRow row = pdfGrid.Rows[i];

                        // Alternar cores das linhas
                        if (i % 2 == 0)
                        {
                            row.Style.BackgroundBrush = new PdfSolidBrush(SyncfusionDrawing.Color.FromArgb(248, 248, 248));
                        }

                        // Colorir status
                        string status = row.Cells[3].Value.ToString();
                        if (status == "Vencido")
                        {
                            row.Cells[3].Style.TextBrush = PdfBrushes.Red;
                            row.Cells[3].Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                        }
                        else if (status == "Pendente")
                        {
                            row.Cells[3].Style.TextBrush = PdfBrushes.Orange;
                        }

                        // Estilizar fonte
                        row.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    }

                    // Configurar layout da tabela
                    pdfGrid.Style.Font = normalFont;

                    // Desenhar a tabela
                    PdfGridLayoutResult result = pdfGrid.Draw(page, new SyncfusionDrawing.PointF(50, yPosition));
                    yPosition = result.Bounds.Bottom + 20;

                    // Rodapé
                    yPosition += 20;
                    graphics.DrawLine(PdfPens.Gray, 50, yPosition, page.Size.Width - 50, yPosition);
                    yPosition += 10;

                    graphics.DrawString("Relatório gerado automaticamente pelo app Vida Simples", smallFont,
                        PdfBrushes.Gray, new SyncfusionDrawing.PointF(50, yPosition));
                    graphics.DrawString($"© {DateTime.Now.Year} - Todos os direitos reservados", smallFont,
                        PdfBrushes.Gray, new SyncfusionDrawing.PointF(page.Size.Width - 200, yPosition));

                    // Salvar arquivo
                    string fileName = $"relatorio_contas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        document.Save(fileStream);
                    }

                    return filePath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro interno: {ex.Message}", "OK");
                return null;
            }
        }
        private async void OnExtrairDadosClicked(object sender, EventArgs e)
        {
            if (ArquivoSelecionado != null)
            {
                // Chame aqui o método que lê e extrai dados do PDF/imagem
                await LerConteudoPdfAsync(ArquivoSelecionado);
                // Depois, atualize a lista de contas ou mostre os dados extraídos
            }
            else
            {
                await DisplayAlert("Atenção", "Selecione um arquivo primeiro.", "OK");
            }
        }
        public static string ClassificarTipoConta(string texto)
        {
            texto = texto.ToLowerInvariant();
            if (texto.Contains("energia") || texto.Contains("energisa") || texto.Contains("eletricidade") || texto.Contains("luz"))
                return "Energia";
            if (texto.Contains("água") || texto.Contains("saneamento") || texto.Contains("brk") || texto.Contains("sabesp"))
                return "Água";
            if (texto.Contains("telefone") || texto.Contains("vivo") || texto.Contains("tim") || texto.Contains("claro"))
                return "Telefone";
            if (texto.Contains("internet") || texto.Contains("net") || texto.Contains("oi fibra") || texto.Contains("fibra"))
                return "Internet";
            if (texto.Contains("cartão"))
                return "Cartão de Crédito";
            // Adicione outros tipos conforme necessário
            return "Outros";
        }
    }

    public class Conta
    {
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
        public DateTime Vencimento { get; set; }
        public string Status { get; set; } = "Pendente";

        // Propriedade para cor do status (para o XAML)
        public MauiColor StatusColor => Status switch
        {
            "Vencido" => MauiColors.Red,
            "Pago" => MauiColors.Green,
            "Pendente" => MauiColors.Orange,
            _ => MauiColors.Gray
        };
    }

    // Classe auxiliar para contas encontradas em PDF
    public class ContaPdf
    {
        public string LinhaOriginal { get; set; }
        public string PossivelDescricao { get; set; }
        public double Valor { get; set; }
        public string DataEncontrada { get; set; }
        public string Beneficiario { get; set; }
    }
}