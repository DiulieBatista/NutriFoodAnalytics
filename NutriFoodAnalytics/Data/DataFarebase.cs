using Firebase.Database;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace NutriFoodAnalytics.Data
{
    public static class DataFirebase
    {
        private const string FirebaseUrl = "https://nutrifoodanalytics-default-rtdb.firebaseio.com/";

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
                // Inicializa a autenticação com o arquivo de chave de serviço
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        // Lê o arquivo serviceAccountKey.json que fica na raiz do projeto
                        Credential = GoogleCredential.FromFile("nutrifoodwpf-firebase-admin.json")
                    });
                }

                // Cria o cliente apontando para a URL do banco
                _client = new FirebaseClient(FirebaseUrl, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () =>
                    {
                        // Gera um token de autenticação usando o Admin 
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