using System.Text.Json.Serialization;

namespace CentralAmparo.Modelos;

// Alerta de queda ou de botão de pânico. Quem cria o alerta é a API;
// a central só muda o status pelos métodos abaixo.
public class Alerta
{
    public int Id { get; set; }
    public int IdosoId { get; set; }
    public int DispositivoId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public double? ProbabilidadeQueda { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public List<Atendimento> Atendimentos { get; set; } = new();

    // private set: o status só muda pelos métodos. O [JsonInclude] deixa o JSON preencher.
    [JsonInclude]
    public StatusAlerta Status { get; private set; } = StatusAlerta.Aberto;

    [JsonIgnore]
    public bool EstaEncerrado => Status == StatusAlerta.Resolvido || Status == StatusAlerta.FalsoAlarme;

    [JsonIgnore]
    public string Prioridade
    {
        get
        {
            if (Tipo == TipoAlerta.BotaoPanico || ProbabilidadeQueda == null || ProbabilidadeQueda >= 0.8)
                return "Alta";
            if (ProbabilidadeQueda >= 0.5)
                return "Média";
            return "Baixa";
        }
    }

    public void IniciarAtendimento()
    {
        if (EstaEncerrado)
            throw new InvalidOperationException("Este alerta já foi encerrado.");
        if (Status == StatusAlerta.Aberto)
            MudarStatus(StatusAlerta.EmAtendimento);
    }

    public Atendimento RegistrarAcao(Operador operador, AcaoAtendimento acao, string observacao)
    {
        if (EstaEncerrado)
            throw new InvalidOperationException("Este alerta já foi encerrado.");

        var atendimento = new Atendimento(Id, operador.Nome, acao, observacao);
        Atendimentos.Add(atendimento);
        MudarStatus(StatusAlerta.EmAtendimento);
        return atendimento;
    }

    public Atendimento Encerrar(Operador operador, StatusAlerta resultado, string observacao)
    {
        if (Status != StatusAlerta.EmAtendimento)
            throw new InvalidOperationException("Inicie o atendimento antes de encerrar o alerta.");
        if (resultado == StatusAlerta.Resolvido && observacao.Trim() == "")
            throw new ArgumentException("Escreva na observação o que aconteceu antes de encerrar.");

        var atendimento = new Atendimento(Id, operador.Nome, AcaoAtendimento.Encerrou, observacao);
        Atendimentos.Add(atendimento);
        MudarStatus(resultado);
        return atendimento;
    }

    private void MudarStatus(StatusAlerta novo)
    {
        Status = novo;
        AtualizadoEm = DateTime.UtcNow;
    }

    public string TipoTexto() => Tipo == TipoAlerta.Queda ? "Queda" : "Botão de pânico";

    public string StatusTexto()
    {
        switch (Status)
        {
            case StatusAlerta.Aberto: return "Aberto";
            case StatusAlerta.EmAtendimento: return "Em atendimento";
            case StatusAlerta.Resolvido: return "Resolvido";
            default: return "Falso alarme";
        }
    }
}
