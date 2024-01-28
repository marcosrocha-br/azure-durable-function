# Azure Durable Functions

## Projeto
A aplicação é um exemplo simples de uso do recurso Azure Durable Functions, uma extensão do Azure Functions!

Utilizando o padrão Function Chaining ou Encadeamento de Funções, demonstra o uso do template Durable simulando o processo de aprovação de um pedido
através de uma HTTP Trigger e salva os logs em uma Conta de Armazenamento posteriormente.

# Como executar
Sugerimos utilizar o <a href="https://www.postman.com/">Postman</a> para fazer as requisições.

<br>

1. Faça uma requisição do tipo POST utilizando o endpoint da inicialização do orquestrador:
https://myorderfunctions.azurewebsites.net/api/IniciarAprovacaoProcesso



2. Forneça um produto no body


