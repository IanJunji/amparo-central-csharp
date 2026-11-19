using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CentralAmparo.Modelos;

namespace CentralAmparo.Dados;

// Usa as rotas do contrato de integração da API.
// Antes de tudo, a central faz login e manda o token em todas as chamadas.
public class RepositorioApi : IRepositorio
{
    private readonly HttpClient _http;

    // Guardados só na memória, para entrar de novo quando o token vence.
    private string? _usuario;
    private string? _senha;

    private class RespostaLogin
    {
        public string Token { get; set; } = "";
        public string Nome { get; set; } = "";
    }

    public RepositorioApi(string urlBase)
    {
        _http = new HttpClient { BaseAddress = new Uri(urlBase), Timeout = TimeSpan.FromSeconds(10) };
    }

    public string UrlBase => _http.BaseAddress!.ToString();

    // Faz login e devolve o nome do operador que a API informou.
    public async Task<string> Entrar(string usuario, string senha)
    {
        var resposta = await _http.PostAsJsonAsync("/api/auth/login", new { usuario, senha }, OpcoesJson.Padrao);

        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            throw new ArgumentException("Usuário ou senha incorretos.");
        if (resposta.StatusCode == HttpStatusCode.BadRequest)
            throw new ArgumentException("Informe o usuário e a senha.");
        resposta.EnsureSuccessStatusCode();

        var login = await resposta.Content.ReadFromJsonAsync<RespostaLogin>(OpcoesJson.Padrao);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        _usuario = usuario;
        _senha = senha;
        return login.Nome;
    }

    // Faz a chamada. Se a API responder 401 (token vencido ou API reiniciada),
    // entra de novo com o mesmo usuário e tenta mais uma vez.
    private async Task<HttpResponseMessage> Enviar(Func<Task<HttpResponseMessage>> chamada)
    {
        var resposta = await chamada();
        if (resposta.StatusCode == HttpStatusCode.Unauthorized && _usuario != null)
        {
            await Entrar(_usuario, _senha!);
            resposta = await chamada();
        }

        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            throw new InvalidOperationException("A sessão expirou. Feche a central e entre de novo.");
        resposta.EnsureSuccessStatusCode();
        return resposta;
    }

    private async Task<T> Ler<T>(string rota)
    {
        var resposta = await Enviar(() => _http.GetAsync(rota));
        return (await resposta.Content.ReadFromJsonAsync<T>(OpcoesJson.Padrao))!;
    }

    public Task<List<Alerta>> ListarAlertas() => Ler<List<Alerta>>("/api/alertas");

    public Task<Alerta> ObterAlerta(int id) => Ler<Alerta>($"/api/alertas/{id}");

    public Task<List<Idoso>> ListarIdosos() => Ler<List<Idoso>>("/api/idosos");

    public Task<Idoso> ObterIdoso(int id) => Ler<Idoso>($"/api/idosos/{id}");

    public async Task CadastrarIdoso(Idoso idoso)
    {
        var corpo = new { idoso.Nome, idoso.DataNascimento, idoso.Bairro, idoso.Endereco, idoso.Observacoes };
        var resposta = await Enviar(() => _http.PostAsJsonAsync("/api/idosos", corpo, OpcoesJson.Padrao));

        var criado = await resposta.Content.ReadFromJsonAsync<Idoso>(OpcoesJson.Padrao);
        idoso.Id = criado!.Id;

        foreach (var contato in idoso.Contatos)
        {
            var corpoContato = new { contato.Nome, contato.Telefone, contato.Parentesco };
            await Enviar(() => _http.PostAsJsonAsync($"/api/idosos/{idoso.Id}/contatos", corpoContato, OpcoesJson.Padrao));
        }
    }

    public async Task AlterarStatus(Alerta alerta)
    {
        await Enviar(() => _http.PatchAsJsonAsync($"/api/alertas/{alerta.Id}/status",
            new { alerta.Status }, OpcoesJson.Padrao));
    }

    public async Task RegistrarAtendimento(Alerta alerta, Atendimento atendimento)
    {
        var corpo = new { atendimento.Operador, atendimento.Acao, atendimento.Observacao };
        await Enviar(() => _http.PostAsJsonAsync($"/api/alertas/{alerta.Id}/atendimentos", corpo, OpcoesJson.Padrao));
    }
}
