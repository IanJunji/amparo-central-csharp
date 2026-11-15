namespace CentralAmparo.Modelos;

// No JSON viram QUEDA, BOTAO_PANICO, EM_ATENDIMENTO etc., como no contrato.

public enum TipoAlerta
{
    Queda,
    BotaoPanico
}

public enum StatusAlerta
{
    Aberto,
    EmAtendimento,
    Resolvido,
    FalsoAlarme
}

public enum AcaoAtendimento
{
    LigouIdoso,
    LigouContato,
    AcionouSamu,
    Encerrou
}
