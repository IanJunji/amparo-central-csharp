using System.Text.Json;
using CentralAmparo.Dados;
using CentralAmparo.Telas;

namespace CentralAmparo;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // appsettings.json diz se usa a API ou o modo demonstração.
        var config = JsonSerializer.Deserialize<Configuracao>(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")), OpcoesJson.Padrao)!;

        // Modo API: login com usuário e senha. Modo demo: só o nome do operador.
        if (config.Modo == "Api")
        {
            var api = new RepositorioApi(config.ApiUrl);
            var login = new FormLogin(api);
            if (login.ShowDialog() != DialogResult.OK)
                return;
            Application.Run(new FormFila(api, login.Operador!));
        }
        else
        {
            var identificacao = new FormIdentificacao();
            if (identificacao.ShowDialog() != DialogResult.OK)
                return;
            Application.Run(new FormFila(new RepositorioDemo(), identificacao.Operador!));
        }
    }
}

public class Configuracao
{
    public string Modo { get; set; } = "Demo";
    public string ApiUrl { get; set; } = "http://localhost:5080";
}
