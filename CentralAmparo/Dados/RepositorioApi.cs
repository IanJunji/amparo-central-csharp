using System.Net.Http.Json;
using CentralAmparo.Modelos;

namespace CentralAmparo.Dados;

// Usa as rotas do CONTRATO-INTEGRACAO.md.
public class RepositorioApi : IRepositorio
{
    private readonly HttpClient _http;

    public RepositorioApi(string urlBase)
    {
        _http = new HttpClient { BaseAddress = new Uri(urlBase), Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task<List<Alerta>> ListarAlertas()
    {
        return await _http.GetFromJsonAsync<List<Alerta>>("/api/alertas", OpcoesJson.Padrao) ?? new();
    }

    public async Task<Alerta> ObterAlerta(int id)
    {
        return (await _http.GetFromJsonAsync<Alerta>($"/api/alertas/{id}", OpcoesJson.Padrao))!;
    }

    public async Task<List<Idoso>> ListarIdosos()
    {
        return await _http.GetFromJsonAsync<List<Idoso>>("/api/idosos", OpcoesJson.Padrao) ?? new();
    }

    public async Task<Idoso> ObterIdoso(int id)
    {
        return (await _http.GetFromJsonAsync<Idoso>($"/api/idosos/{id}", OpcoesJson.Padrao))!;
    }

    public async Task CadastrarIdoso(Idoso idoso)
    {
        var corpo = new { idoso.Nome, idoso.DataNascimento, idoso.Bairro, idoso.Endereco, idoso.Observacoes };
        var resposta = await _http.PostAsJsonAsync("/api/idosos", corpo, OpcoesJson.Padrao);
        resposta.EnsureSuccessStatusCode();

        var criado = await resposta.Content.ReadFromJsonAsync<Idoso>(OpcoesJson.Padrao);
        idoso.Id = criado!.Id;

        foreach (var contato in idoso.Contatos)
        {
            var corpoContato = new { contato.Nome, contato.Telefone, contato.Parentesco };
            var r = await _http.PostAsJsonAsync($"/api/idosos/{idoso.Id}/contatos", corpoContato, OpcoesJson.Padrao);
            r.EnsureSuccessStatusCode();
        }
    }

    public async Task AlterarStatus(Alerta alerta)
    {
        var resposta = await _http.PatchAsJsonAsync($"/api/alertas/{alerta.Id}/status",
            new { alerta.Status }, OpcoesJson.Padrao);
        resposta.EnsureSuccessStatusCode();
    }

    public async Task RegistrarAtendimento(Alerta alerta, Atendimento atendimento)
    {
        var corpo = new { atendimento.Operador, atendimento.Acao, atendimento.Observacao };
        var resposta = await _http.PostAsJsonAsync($"/api/alertas/{alerta.Id}/atendimentos", corpo, OpcoesJson.Padrao);
        resposta.EnsureSuccessStatusCode();
    }
}
