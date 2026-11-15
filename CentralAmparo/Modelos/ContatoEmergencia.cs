namespace CentralAmparo.Modelos;

public class ContatoEmergencia
{
    public int Id { get; set; }
    public int IdosoId { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Parentesco { get; set; } = "";

    // Usado pelo JSON.
    public ContatoEmergencia() { }

    public ContatoEmergencia(string nome, string telefone, string parentesco)
    {
        nome = nome.Trim();
        // Guarda só os números: "(11) 98765-4321" vira "11987654321".
        telefone = new string(telefone.Where(char.IsDigit).ToArray());

        if (nome.Length < 2)
            throw new ArgumentException("Informe o nome do contato.");
        if (telefone.Length < 10 || telefone.Length > 11)
            throw new ArgumentException("O telefone precisa ter DDD + número (10 ou 11 dígitos).");

        Nome = nome;
        Telefone = telefone;
        Parentesco = parentesco.Trim();
    }
}
