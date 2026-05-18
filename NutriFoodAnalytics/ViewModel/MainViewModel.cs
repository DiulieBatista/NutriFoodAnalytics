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
            Resultados.Clear();

            var alimentos =
                await _repository.BuscarTodosAsync();

            string pergunta =
                Pergunta.ToLower();

            // PROCURA O ALIMENTO
            var alimentoEncontrado =
                alimentos.FirstOrDefault(a =>
                    pergunta.Contains(a.Nome.ToLower()));

            // NÃO ENCONTROU
            if (alimentoEncontrado == null)
            {
                Resposta =
                    "Alimento não encontrado.";

                return;
            }

            // MOSTRA SOMENTE O ITEM ENCONTRADO
            Resultados.Add(alimentoEncontrado);


            // CALORIAS
            if (pergunta.Contains("caloria"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Calorias} calorias.";
            }

            // PROTEÍNA
            else if (pergunta.Contains("proteína") ||
                     pergunta.Contains("proteina"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Proteina}g de proteína.";
            }

            // AÇÚCAR
            else if (pergunta.Contains("açúcar") ||
                     pergunta.Contains("acucar"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Acucar}g de açúcar.";
            }

            // CARBOIDRATOS
            else if (pergunta.Contains("carboidrato"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Carboidratos}g de carboidratos.";
            }

            // GORDURA TOTAL
            else if (pergunta.Contains("gordura total"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.GorduraTotal}g de gordura total.";
            }

            // GORDURA SATURADA
            else if (pergunta.Contains("gordura saturada"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.GorduraSaturada}g de gordura saturada.";
            }

            // FIBRAS
            else if (pergunta.Contains("fibra"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Fibras}g de fibras.";
            }

            // SÓDIO
            else if (pergunta.Contains("sódio") ||
                     pergunta.Contains("sodio"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Sodio}mg de sódio.";
            }

            // POTÁSSIO
            else if (pergunta.Contains("potássio") ||
                     pergunta.Contains("potassio"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Potassio}mg de potássio.";
            }

            // COLESTEROL
            else if (pergunta.Contains("colesterol"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.Colesterol}mg de colesterol.";
            }

            // PORÇÃO
            else if (pergunta.Contains("porção") ||
                     pergunta.Contains("porcao"))
            {
                Resposta =
                    $"{alimentoEncontrado.Nome} possui " +
                    $"{alimentoEncontrado.PorcaoGramas}g por porção.";
            }

            // SEM ATRIBUTO
            else
            {
                Resposta =
                    $"Alimento encontrado: {alimentoEncontrado.Nome}.";
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