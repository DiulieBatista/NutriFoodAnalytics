using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace NutriFoodAnalytics.Data
{
    public class DataFirebase
    {
        /// <summary>
        /// Propriedade usada para realizar as operações no banco Firestore.
        /// </summary>
        public FirestoreDb Database { get; private set; }

        // Mudamos o construtor para receber a string do ID e do caminho do JSON diretamente,
        // evitando erros de injeção de dependência do IConfiguration no WPF.
        public DataFirebase(string projectId, string jsonPath)
        {
            try
            {
                if (string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentException("O ProjectId não pode ser nulo ou vazio.");
                }

                if (string.IsNullOrEmpty(jsonPath))
                {
                    throw new ArgumentException("O caminho do arquivo JSON (JsonPath) não pode ser nulo ou vazio.");
                }

                /// Monta o caminho considerando a pasta 'ConfigWPF' dentro do diretório de execução
                string caminhoCompleto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWPF", jsonPath);

                /// Validação de existência do arquivo físico
                if (!File.Exists(caminhoCompleto))
                {
                    throw new FileNotFoundException($"Arquivo de credenciais do Firebase não encontrado no caminho: {caminhoCompleto}");
                }

                // Carrega a credencial do arquivo JSON
                var credential = GoogleCredential.FromFile(caminhoCompleto);

                // Inicializa o banco de dados
                Database = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    Credential = credential
                }.Build();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro Crítico ao inicializar FirestoreContext: {ex.Message}", ex);
            }
        }
    }
}