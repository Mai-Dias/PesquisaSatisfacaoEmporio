# Pesquisa de Satisfação Empório Quatro Estrelas

Formulário progressivo de pesquisa de satisfação com integração Bonifiq (clube de fidelidade) e salvando respostas em SQLite.

## Como rodar local

1. Instale o [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Restaure pacotes:  
   `dotnet restore`
3. Crie o banco de dados:  
   `dotnet ef database update`
4. Rode a aplicação:  
   `dotnet run`
5. Acesse [https://localhost:5001](https://localhost:5001) ou [http://localhost:5000](http://localhost:5000)

## Deploy na Render

- Use o arquivo padrão de deploy, marque como "Web Service .NET".
- Banco SQLite já incluído.
