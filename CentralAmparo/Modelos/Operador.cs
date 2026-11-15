namespace CentralAmparo.Modelos;

public class Operador
{
    public string Nome { get; private set; }

    public Operador(string nome)
    {
        nome = nome.Trim();
        if (nome.Length < 3)
            throw new ArgumentException("Digite seu nome (pelo menos 3 letras).");
        if (nome.Any(char.IsDigit))
            throw new ArgumentException("O nome não pode ter números.");
        Nome = nome;
    }
}
