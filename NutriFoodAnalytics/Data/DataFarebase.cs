using System;
using System.IO;
using FirebaseAdmin;
using Firebase.Database;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace NutriFoodWPF.Data
{
    public class DatabaseFirestore
    {
        // ATUALIZADO: Sua nova URL do projeto limpo
        private const string FirebaseUrl = "https://nutrifoodanalytics-5f6a2-default-rtdb.firebaseio.com/";

        // Instância única do cliente (padrão Singleton)
        private static FirebaseClient? _client;

        /// <summary>
        /// Retorna o cliente do Firebase pronto para uso.
        /// Cria a conexão apenas na primeira chamada.
        /// </summary>
        public static FirebaseClient GetClient()
        {
            if (_client == null)
            {
                // Inicializa a autenticação com o arquivo de chave de serviço do novo projeto
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                      
                        Credential = GoogleCredential.FromFile("firebase-credentials.json")
                    });
                }

                // Cria o cliente apontando para a nova URL do banco
                _client = new FirebaseClient(FirebaseUrl, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () =>
                    {
                        // Gera um token de autenticação seguro usando o Admin SDK
                        string token = await FirebaseAuth.DefaultInstance
                            .CreateCustomTokenAsync("nutrifoodapp");
                        return token;
                    }
                });
            }

            return _client;
        }
    }
}