using CentralAmparo.Modelos;

namespace CentralAmparo.Telas;

// Primeira tela: o operador digita o nome, que vai em cada atendimento registrado.
public class FormIdentificacao : Form
{
    private readonly string _arquivoNome = Path.Combine(AppContext.BaseDirectory, "ultimo-operador.txt");
    private readonly TextBox txtNome = new();

    public Operador? Operador { get; private set; }

    public FormIdentificacao()
    {
        Text = "Central Amparo - Identificação";
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(440, 200);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var lblTitulo = new Label { Text = "Central Amparo", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        var lblNome = new Label { Text = "Digite seu nome para começar:", Location = new Point(20, 65), AutoSize = true };
        txtNome.Location = new Point(20, 92);
        txtNome.Width = 400;

        var btnEntrar = new Button { Text = "Entrar", Location = new Point(220, 140), Size = new Size(100, 35) };
        var btnSair = new Button { Text = "Sair", Location = new Point(320, 140), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
        btnEntrar.Click += BtnEntrar_Click;

        Controls.AddRange(new Control[] { lblTitulo, lblNome, txtNome, btnEntrar, btnSair });
        AcceptButton = btnEntrar;
        CancelButton = btnSair;

        // Lembra o último nome usado.
        if (File.Exists(_arquivoNome))
            txtNome.Text = File.ReadAllText(_arquivoNome);
    }

    private void BtnEntrar_Click(object? sender, EventArgs e)
    {
        try
        {
            Operador = new Operador(txtNome.Text);
            File.WriteAllText(_arquivoNome, Operador.Nome);
            DialogResult = DialogResult.OK;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
