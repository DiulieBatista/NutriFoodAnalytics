# NutriFoodAnalytics 

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/UI-WPF-blue.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Firebase](https://img.shields.io/badge/Backend-Firebase-orange.svg)](https://firebase.google.com/)
[![License](https://img.shields.io/badge/License-Educacional-green.svg)](#-licença)

O **NutriFoodAnalytics** é uma aplicação desktop de alta performance desenvolvida em C# utilizando **WPF (Windows Presentation Foundation)**. O sistema foi projetado com foco em análise nutricional avançada, permitindo que usuários consultem dados de alimentos em tempo real, gerenciem históricos alimentares e visualizem relatórios consolidados de macronutrientes.

A aplicação se integra dinamicamente com APIs REST públicas e centraliza a persistência de dados em nuvem híbrida através do ecossistema Firebase (**Realtime Database + Cloud Firestore**).

---

#  Tecnologias e Ferramentas

- **Linguagem:** C# (C-Sharp)
- **Framework Base:** .NET 8.0 / WPF (XAML)
- **Banco de Dados NoSQL:** Firebase Realtime Database & Cloud Firestore
- **Autenticação & Segurança:** Google Firebase Admin SDK
- **Consumo de API:** `HttpClient`
- **Processamento de JSON:** Newtonsoft.Json (Json.NET)
- **Arquitetura de Software:** Padrão em Camadas
- **Interface:** XAML moderno e responsivo

---

#  Funcionalidades Principais

-  **Consulta Inteligente** de alimentos
-  **Dashboard Nutricional** com informações detalhadas
-  **Sincronização em Nuvem** via Firebase
-  **Persistência de Dados** local e remota
-  **Relatórios Nutricionais**
-  **Interface Moderna em WPF**
-  **Operações Assíncronas** com `async/await`

---

#  Estrutura Arquitetural do Projeto

```text
NutriFoodAnalytics/
│
├── Assets/                   # Recursos visuais (Ícones, Imagens e Estilos)
├── Data/                     # Configuração e conexão Firebase
├── Models/                   # Entidades e modelos de dados
├── Services/                 # Integração com APIs externas
├── ViewModels/               # Regras de negócio e Data Binding
├── Views/                    # Interfaces gráficas WPF
│
├── App.xaml                  # Inicialização da aplicação
├── App.config                # Configurações globais
└── firebase-credentials.json # Credenciais privadas Firebase
```

---

#  Configuração do Firebase

## 1. Criar Projeto Firebase

Acesse:

 https://console.firebase.google.com/

---

## 2. Ativar Serviços

Ative:

- Firebase Realtime Database
- Cloud Firestore

---

## 3. Gerar Credenciais

No painel do Firebase:

```text
Configurações do Projeto
→ Contas de Serviço
→ Gerar Nova Chave Privada
```

Baixe o arquivo JSON e adicione no projeto:

```text
firebase-credentials.json
```

---

##  Importante

No Visual Studio configure:

```text
Copiar para Diretório de Saída:
→ Copiar se for mais novo
```

---

#  Dependências e Pacotes NuGet

Execute no Console do Gerenciador de Pacotes:

```powershell
# Firebase Realtime Database
Install-Package FirebaseDatabase.net

# Firestore e autenticação Google
Install-Package Google.Cloud.Firestore
Install-Package Google.Apis.Auth

# Configurações e JSON
Install-Package Microsoft.Extensions.Configuration
Install-Package Newtonsoft.Json
```

---

#  Consumo da API de Nutrição

A aplicação utiliza:

 https://api-ninjas.com/api/nutrition

## Endpoint Base

```http
GET https://api.api-ninjas.com/v1/nutrition?query=banana
```

---

#  Estrutura JSON Esperada

```json
[
  {
    "name": "banana",
    "calories": 88.7,
    "serving_size_g": 100.0,
    "fat_total_g": 0.3,
    "fat_saturated_g": 0.1,
    "protein_g": 1.1,
    "sodium_mg": 1,
    "potassium_mg": 358,
    "cholesterol_mg": 0,
    "carbohydrates_total_g": 22.8,
    "fiber_g": 2.6,
    "sugar_g": 12.2
  }
]
```

---

# Como Executar o Projeto

## 1. Instale o Visual Studio 2022

Com a carga:

```text
.NET Desktop Development
```

---

## 2. Clone o Repositório

```bash
git clone https://github.com/seuusuario/NutriFoodAnalytics.git
```

---

## 3. Abra a Solução

Abra o arquivo:

```text
NutriFoodAnalytics.sln
```

---

## 4. Configure as Chaves

Adicione:

- Sua API Key da API Ninjas
- O arquivo `firebase-credentials.json`

---

## 5. Execute

Pressione:

```text
F5
```

---

#  Objetivo Acadêmico

O projeto foi desenvolvido com finalidade educacional e prática, aplicando conceitos como:

- Consumo de APIs REST
- Manipulação de JSON
- Firebase com C#
- Desenvolvimento Desktop com WPF
- Persistência em banco NoSQL
- Arquitetura em camadas
- Programação assíncrona
- Organização de projetos profissionais

---

#  Desenvolvedora

## Diulie Mileide 

- Estudante de Desenvolvimento de Sistemas
