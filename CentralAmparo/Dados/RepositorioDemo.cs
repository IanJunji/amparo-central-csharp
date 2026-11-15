using System.Text.Json;
using CentralAmparo.Modelos;

namespace CentralAmparo.Dados;

// Modo demonstração: guarda tudo no arquivo dados-demo.json, ao lado do executável.
// Na primeira vez cria o arquivo com dados fictícios.
public class RepositorioDemo : IRepositorio
{
    private class Arquivo
    {
        public List<Idoso> Idosos { get; set; } = new();
        public List<Alerta> Alertas { get; set; } = new();
    }

    private readonly string _caminho = Path.Combine(AppContext.BaseDirectory, "dados-demo.json");
    private readonly Arquivo _dados;

    public RepositorioDemo()
    {
        if (File.Exists(_caminho))
            _dados = JsonSerializer.Deserialize<Arquivo>(File.ReadAllText(_caminho), OpcoesJson.Padrao)!;
        else
        {
            _dados = CriarDadosIniciais();
            Salvar();
        }
    }

    private void Salvar()
    {
        File.WriteAllText(_caminho, JsonSerializer.Serialize(_dados, OpcoesJson.Padrao));
    }

    public Task<List<Alerta>> ListarAlertas() => Task.FromResult(_dados.Alertas.ToList());

    public Task<Alerta> ObterAlerta(int id) => Task.FromResult(_dados.Alertas.First(a => a.Id == id));

    public Task<List<Idoso>> ListarIdosos() => Task.FromResult(_dados.Idosos.ToList());

    public Task<Idoso> ObterIdoso(int id) => Task.FromResult(_dados.Idosos.First(i => i.Id == id));

    public Task CadastrarIdoso(Idoso idoso)
    {
        idoso.Id = _dados.Idosos.Count == 0 ? 1 : _dados.Idosos.Max(i => i.Id) + 1;
        foreach (var contato in idoso.Contatos)
            contato.IdosoId = idoso.Id;
        _dados.Idosos.Add(idoso);
        Salvar();
        return Task.CompletedTask;
    }

    // O alerta já foi alterado pelos métodos da classe Alerta; aqui só grava o arquivo.
    public Task AlterarStatus(Alerta alerta)
    {
        Salvar();
        return Task.CompletedTask;
    }

    public Task RegistrarAtendimento(Alerta alerta, Atendimento atendimento)
    {
        atendimento.Id = _dados.Alertas.SelectMany(a => a.Atendimentos).Max(a => a.Id) + 1;
        Salvar();
        return Task.CompletedTask;
    }

    // Faz o papel da pulseira: abre um alerta novo para um idoso sorteado.
    public void SimularQueda()
    {
        var sorteio = new Random();
        var idoso = _dados.Idosos[sorteio.Next(_dados.Idosos.Count)];
        var panico = sorteio.Next(4) == 0;

        _dados.Alertas.Add(new Alerta
        {
            Id = _dados.Alertas.Max(a => a.Id) + 1,
            IdosoId = idoso.Id,
            DispositivoId = idoso.Id,
            Tipo = panico ? TipoAlerta.BotaoPanico : TipoAlerta.Queda,
            ProbabilidadeQueda = panico ? null : Math.Round(0.55 + sorteio.NextDouble() * 0.44, 2),
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
        Salvar();
    }

    private static Arquivo CriarDadosIniciais()
    {
        var agora = DateTime.UtcNow;
        var dados = new Arquivo();

        dados.Idosos.Add(new Idoso
        {
            Id = 1, Nome = "Maria Aparecida Fictícia", DataNascimento = new DateOnly(1941, 3, 12),
            Bairro = "Vila Esperança", Endereco = "Rua das Acácias, 120",
            Observacoes = "Hipertensa. Usa andador dentro de casa.",
            Contatos =
            {
                new ContatoEmergencia { Id = 1, IdosoId = 1, Nome = "Joana Fictícia", Telefone = "11900000001", Parentesco = "filha" },
                new ContatoEmergencia { Id = 2, IdosoId = 1, Nome = "Seu Antônio Exemplo", Telefone = "11900000002", Parentesco = "vizinho" }
            }
        });
        dados.Idosos.Add(new Idoso
        {
            Id = 2, Nome = "José Benedito Exemplo", DataNascimento = new DateOnly(1938, 11, 2),
            Bairro = "Jardim Primavera", Endereco = "Travessa dos Ipês, 45",
            Observacoes = "Diabético.",
            Contatos = { new ContatoEmergencia { Id = 3, IdosoId = 2, Nome = "Carlos Exemplo", Telefone = "11900000003", Parentesco = "filho" } }
        });
        dados.Idosos.Add(new Idoso
        {
            Id = 3, Nome = "Lourdes Teste da Silva", DataNascimento = new DateOnly(1945, 7, 25),
            Bairro = "Vila Esperança", Endereco = "Rua dos Girassóis, 300",
            Contatos = { new ContatoEmergencia { Id = 4, IdosoId = 3, Nome = "Rita Teste", Telefone = "11900000004", Parentesco = "sobrinha" } }
        });

        dados.Alertas.Add(new Alerta
        {
            Id = 1, IdosoId = 1, DispositivoId = 1, Tipo = TipoAlerta.Queda, ProbabilidadeQueda = 0.93,
            CriadoEm = agora.AddMinutes(-8), AtualizadoEm = agora.AddMinutes(-8)
        });
        dados.Alertas.Add(new Alerta
        {
            Id = 2, IdosoId = 2, DispositivoId = 2, Tipo = TipoAlerta.BotaoPanico,
            CriadoEm = agora.AddMinutes(-2), AtualizadoEm = agora.AddMinutes(-2)
        });
        dados.Alertas.Add(new Alerta
        {
            Id = 3, IdosoId = 3, DispositivoId = 3, Tipo = TipoAlerta.Queda, ProbabilidadeQueda = 0.61,
            CriadoEm = agora.AddMinutes(-25), AtualizadoEm = agora.AddMinutes(-25)
        });

        return dados;
    }
}
