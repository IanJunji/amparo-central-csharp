namespace CentralAmparo.Modelos;

// Uma ação do operador sobre um alerta.
public class Atendimento
{
    public int Id { get; set; }
    public int AlertaId { get; set; }
    public string Operador { get; set; } = "";
    public AcaoAtendimento Acao { get; set; }
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }

    // Usado pelo JSON.
    public Atendimento() { }

    public Atendimento(int alertaId, string operador, AcaoAtendimento acao, string observacao)
    {
        if (observacao.Length > 500)
            throw new ArgumentException("A observação pode ter no máximo 500 caracteres.");

        AlertaId = alertaId;
        Operador = operador;
        Acao = acao;
        Observacao = observacao.Trim() == "" ? null : observacao.Trim();
        CriadoEm = DateTime.UtcNow;
    }

    public string AcaoTexto()
    {
        switch (Acao)
        {
            case AcaoAtendimento.LigouIdoso: return "Ligou para o idoso";
            case AcaoAtendimento.LigouContato: return "Ligou para o contato";
            case AcaoAtendimento.AcionouSamu: return "Acionou o SAMU";
            default: return "Encerrou o alerta";
        }
    }
}
