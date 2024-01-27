using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function;

public class Produto
{
    public string Nome { get; set; }
}

public static class AprovadorDePedidos
{

    [FunctionName("IniciarAprovacaoProcessoHttp")]
    public static async Task<IActionResult> IniciarAprovacaoProcessoHttp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        var data = await req.Content.ReadAsAsync<dynamic>();
        string produto = data?.produto;

        string instanceId = await starter.StartNewAsync("OrquestradorAprovacaoPedido", null, produto);
        log.LogInformation("Iniciada orquestração com ID = '{instanceId}' para o produto '{produto}'.", instanceId, produto);

        var checkStatusResponse = starter.CreateCheckStatusResponse(req, instanceId);

        var result = await checkStatusResponse.Content.ReadAsStringAsync();
        return new OkObjectResult(result);
    }

    [FunctionName("OrquestradorAprovacaoPedido")]
    public static async Task<List<string>> ExecutarOrquestrador(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        string produto = context.GetInput<string>();

        var outputs = new List<string>();

        outputs.Add(await context.CallActivityAsync<string>("BuscarDetalhesPedido", produto));
        outputs.Add(await context.CallActivityAsync<string>("ProcessarPagamento", produto));
        outputs.Add(await context.CallActivityAsync<string>("EntregarProduto", produto));

        return outputs;
    }

    [FunctionName("BuscarDetalhesPedido")]
    public static string BuscarDetalhesPedido([ActivityTrigger] string produto, ILogger log)
    {
        log.LogInformation($"Buscando detalhes do pedido para: {produto}");
        return $"Detalhes do pedido: {produto}";
    }

    [FunctionName("ProcessarPagamento")]
    public static string ProcessarPagamento([ActivityTrigger] string produto, ILogger log)
    {
        log.LogInformation($"Processando pagamento para o produto: {produto}");
        return $"Pagamento aprovado!";
    }

    [FunctionName("EntregarProduto")]
    public static string EntregarProduto([ActivityTrigger] string produto, ILogger log)
    {
        log.LogInformation($"Produto em rota de entrega: {produto}");
        return $"Produto '{produto}' em rota de entrega!";
    }

}

