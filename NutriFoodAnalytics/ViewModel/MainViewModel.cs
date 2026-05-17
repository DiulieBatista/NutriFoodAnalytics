using NutriFoodAnalytics.Command;
using NutriFoodAnalytics.Models;
using NutriFoodAnalytics.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NutriFoodAnalytics.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AlimentoRepository _repository;
        private string _pergunta = string.Empty;
        private string _resposta = string.Empty;

        public string Pergunta
        {
            get => _pergunta;
            set
            {
                _pergunta = value;
                OnPropertyChanged();
            }
        }

        public string Resposta
        {
            get => _resposta;
            set
            {
                _resposta = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Alimento> Resultados { get; set; }
        public ICommand PesquisarCommand { get; set; }
        public ICommand GerarPdfCommand { get; set; }

        // NOVO: Comando para listar absolutamente tudo
        public ICommand ListarTudoCommand { get; set; }

        public MainViewModel()
        {
            _repository = new AlimentoRepository();
            Resultados = new ObservableCollection<Alimento>();

            PesquisarCommand = new RelayCommand(async () => await PesquisarAsync());
            GerarPdfCommand = new RelayCommand(async () => await GerarPdfAsync());

            // Inicializa o novo comando
            ListarTudoCommand = new RelayCommand(async () => await ListarTudoAsync());
        }

        // ─────────────────────────────────────────────────────────────────
        // NOVO: MÉTODO GET ALL (TRAZ TUDO SEM FILTRO)
        // ─────────────────────────────────────────────────────────────────
        private async Task ListarTudoAsync()
        {
            try
            {
                Resultados.Clear();
                Pergunta = string.Empty; // Limpa a caixa de texto para fazer sentido
                Resposta = "Carregando todos os alimentos do banco...";

                // Busca a lista completa diretamente da API
                var dadosBrutos = await _repository.BuscarTodosAsync();

                if (dadosBrutos == null || !dadosBrutos.Any())
                {
                    Resposta = "Nenhum alimento cadastrado no banco de dados.";
                    return;
                }

                // Remove duplicados pelo nome para garantir o grid limpo
                var dadosUnicos = dadosBrutos.DistinctBy(a => a.Nome?.ToLower().Trim()).ToList();

                // Adiciona TODOS os alimentos direto na tabela da tela
                foreach (var alimento in dadosUnicos)
                {
                    Resultados.Add(alimento);
                }

                Resposta = $"Exibindo todos os {Resultados.Count} alimento(s) cadastrados.";
            }
            catch (Exception ex)
            {
                Resposta = $"Erro ao listar tudo: {ex.Message}";
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // PESQUISA FILTRADA (Mantida para quando digitar algo)
        // ─────────────────────────────────────────────────────────────────
        private async Task PesquisarAsync()
        {
            try
            {
                Resultados.Clear();
                Resposta = "Buscando dados na API...";

                if (string.IsNullOrWhiteSpace(Pergunta))
                {
                    Resposta = "Por favor, digite o nome de um alimento para pesquisar.";
                    return;
                }

                var dadosBrutos = await _repository.BuscarTodosAsync();

                if (dadosBrutos == null || !dadosBrutos.Any())
                {
                    Resposta = "Nenhum dado foi retornado pela API.";
                    return;
                }

                var dadosUnicos = dadosBrutos.DistinctBy(a => a.Nome?.ToLower().Trim()).ToList();
                var palavrasChave = Pergunta.Trim().ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var alimento in dadosUnicos)
                {
                    if (alimento.Nome == null) continue;

                    string nomeAlimento = alimento.Nome.ToLower();
                    bool corresponde = palavrasChave.Any(palavra => nomeAlimento.Contains(palavra));

                    if (corresponde)
                    {
                        Resultados.Add(alimento);
                    }
                }

                if (Resultados.Count == 0)
                {
                    Resposta = $"Nenhum alimento encontrado correspondente a '{Pergunta}'.";
                }
                else
                {
                    Resposta = $"{Resultados.Count} resultado(s) encontrado(s).";
                }
            }
            catch (Exception ex)
            {
                Resposta = $"Erro de Conexão: {ex.Message}";
            }
        }

        // ─────────────────────────────────────
        // PDF DINÂMICO
        // ─────────────────────────────────────
        private async Task GerarPdfAsync()
        {
            if (Resultados == null || !Resultados.Any())
            {
                MessageBox.Show("Nenhum dado encontrado para gerar o PDF.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string carimboTempo = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string nomeArquivo = $"RelatorioNutricional_{carimboTempo}.pdf";
                string caminhoPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);

                await Task.Run(() =>
                {
                    QuestPDF.Settings.License = LicenseType.Community;

                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(30);
                            page.Header().Text("RELATÓRIO NUTRICIONAL").FontSize(20).Bold();

                            page.Content().Column(col =>
                            {
                                foreach (var item in Resultados)
                                {
                                    col.Item()
                                        .PaddingVertical(10)
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .Padding(10)
                                        .Column(card =>
                                        {
                                            card.Item().Text(item.Nome).FontSize(16).Bold();

                                            PropertyInfo[] propriedades = item.GetType().GetProperties();
                                            foreach (var prop in propriedades)
                                            {
                                                if (prop.Name == "Id") continue;
                                                var valor = prop.GetValue(item);
                                                if (valor == null) continue;

                                                card.Item().Text($"{prop.Name}: {valor}");
                                            }
                                        });
                                }
                            });

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                            });
                        });
                    })
                    .GeneratePdf(caminhoPdf);
                });

                Process.Start(new ProcessStartInfo(caminhoPdf) { UseShellExecute = true });
            }
            catch (IOException)
            {
                MessageBox.Show("Não foi possível gerar o PDF porque o documento anterior ainda está aberto.", "Arquivo em Uso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}