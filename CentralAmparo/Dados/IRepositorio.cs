using System.Text.Json;
using System.Text.Json.Serialization;
using CentralAmparo.Modelos;

namespace CentralAmparo.Dados;

// O que a central lê e grava. Tem duas versões:
// RepositorioApi (backend) e RepositorioDemo (arquivo JSON local, sem backend).
public interface IRepositorio
{
    Task<List<Alerta>> ListarAlertas();
    Task<Alerta> ObterAlerta(int id);
    Task<List<Idoso>> ListarIdosos();
    Task<Idoso> ObterIdoso(int id);
    Task CadastrarIdoso(Idoso idoso);
    Task AlterarStatus(Alerta alerta);
    Task RegistrarAtendimento(Alerta alerta, Atendimento atendimento);
}

public static class OpcoesJson
{
    // camelCase e enums como EM_ATENDIMENTO, igual ao contrato.
    public static readonly JsonSerializerOptions Padrao = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper) }
    };
}
