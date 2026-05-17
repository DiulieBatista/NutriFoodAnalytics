using NutriFoodAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriFoodAnalytics.Service
{
    public class AlimentoService
    {

        private const string BaseUrl = "http://apinutrifood.runasp.net/api/AlimentoValidado";

        private static readonly HttpClient _httpClient;

        static AlimentoService()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Cabeçalhos essenciais para simular um navegador e evitar bloqueios da hospedagem
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
        };

        // ───────────────────────────────────────
        // BUSCAR TODOS
        // ───────────────────────────────────────
        public async Task<List<Alimento>> BuscarTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);

                // Se der erro 404 (Rota não encontrada) ou 500 (Erro no banco), avisa na tela do WPF
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro técnico no servidor. Código: {(int)response.StatusCode} ({response.ReasonPhrase}). Verifique se a rota precisa ou não do prefixo '/api'.");
                }

                var json = await response.Content.ReadAsStringAsync();

                // Garante que o servidor não devolveu um HTML por engano
                if (json.Trim().StartsWith("<"))
                {
                    throw new Exception("O servidor respondeu com uma página web (HTML) em vez de dados brutos (JSON). Certifique-se de que a rota está correta.");
                }

                return JsonSerializer.Deserialize<List<Alimento>>(json, _jsonOptions)
                       ?? new List<Alimento>();
            }
            catch (HttpRequestException ex)
            {
                string detalhe = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Erro de comunicação HTTP: {detalhe}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Erro ao processar os dados recebidos (JSON inválido): {ex.Message}", ex);
            }
        }
    }
}