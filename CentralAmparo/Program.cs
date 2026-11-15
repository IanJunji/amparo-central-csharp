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

        IRepositorio repositorio;
        if (config.Modo == "Api")
            repositorio = new RepositorioApi(config.ApiUrl);
        else
            repositorio = new RepositorioDemo();

        var identificacao = new FormIdentificacao();
        if (identificacao.ShowDialog() != DialogResult.OK)
            return;

        Application.Run(new FormFila(repositorio, identificacao.Operador!));
    }
}

public class Configuracao
{
    public string Modo { get; set; } = "Demo";
    public string ApiUrl { get; set; } = "http://localhost:5080";
}
