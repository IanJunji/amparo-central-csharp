using System.Text.Json.Serialization;

namespace CentralAmparo.Modelos;

public class Idoso
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public DateOnly DataNascimento { get; set; }
    public string Bairro { get; set; } = "";
    public string Endereco { get; set; } = "";
    public string? Observacoes { get; set; }
    public List<ContatoEmergencia> Contatos { get; set; } = new();

    // Usado pelo JSON.
    public Idoso() { }

    public Idoso(string nome, DateOnly dataNascimento, string bairro, string endereco, string observacoes)
    {
        nome = nome.Trim();
        bairro = bairro.Trim();

        if (nome == "")
            throw new ArgumentException("Informe o nome do idoso.");
        if (nome.Length > 120)
            throw new ArgumentException("O nome pode ter no máximo 120 caracteres.");
        if (nome.Any(char.IsDigit))
            throw new ArgumentException("O nome não pode ter números.");
        if (bairro == "")
            throw new ArgumentException("Informe o bairro.");

        Nome = nome;
        DataNascimento = dataNascimento;
        Bairro = bairro;
        Endereco = endereco.Trim();
        Observacoes = observacoes.Trim() == "" ? null : observacoes.Trim();

        // O programa atende pessoas com 60 anos ou mais.
        if (Idade < 60)
            throw new ArgumentException("O programa atende pessoas com 60 anos ou mais.");
    }

    [JsonIgnore]
    public int Idade
    {
        get
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var idade = hoje.Year - DataNascimento.Year;
            if (DataNascimento > hoje.AddYears(-idade))
                idade--;
            return idade;
        }
    }
}
