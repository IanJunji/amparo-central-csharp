using CentralAmparo.Dados;
using CentralAmparo.Modelos;

namespace CentralAmparo.Telas;

// Primeira tela no modo API: o operador entra com usuário e senha.
// O nome que vai nos atendimentos é o que a API devolve no login.
public class FormLogin : Form
{
    private readonly RepositorioApi _repositorio;
    private readonly string _arquivoUsuario = Path.Combine(AppContext.BaseDirectory, "ultimo-usuario.txt");
    private readonly TextBox txtUsuario = new();
    private readonly TextBox txtSenha = new();
    private readonly Button btnEntrar = new();

    public Operador? Operador { get; private set; }

    public FormLogin(RepositorioApi repositorio)
    {
        _repositorio = repositorio;

        Text = "Central Amparo - Login";
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(440, 290);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var lblTitulo = new Label { Text = "Central Amparo", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        var lblUsuario = new Label { Text = "Usuário:", Location = new Point(20, 65), AutoSize = true };
        txtUsuario.SetBounds(20, 92, 400, 30);
        var lblSenha = new Label { Text = "Senha:", Location = new Point(20, 130), AutoSize = true };
        txtSenha.SetBounds(20, 157, 400, 30);
        txtSenha.UseSystemPasswordChar = true;
        var lblServidor = new Label { Text = "Servidor: " + repositorio.UrlBase, Location = new Point(20, 195), AutoSize = true, ForeColor = Color.DimGray };

        btnEntrar.Text = "Entrar";
        btnEntrar.SetBounds(220, 235, 100, 35);
        var btnSair = new Button { Text = "Sair", Location = new Point(320, 235), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
        btnEntrar.Click += BtnEntrar_Click;

        Controls.AddRange(new Control[] { lblTitulo, lblUsuario, txtUsuario, lblSenha, txtSenha, lblServidor, btnEntrar, btnSair });
        AcceptButton = btnEntrar;
        CancelButton = btnSair;

        // Lembra o último usuário (nunca a senha).
        if (File.Exists(_arquivoUsuario))
        {
            txtUsuario.Text = File.ReadAllText(_arquivoUsuario);
            ActiveControl = txtSenha;
        }
    }

    private async void BtnEntrar_Click(object? sender, EventArgs e)
    {
        var usuario = txtUsuario.Text.Trim();
        if (usuario == "" || txtSenha.Text == "")
        {
            MessageBox.Show("Informe o usuário e a senha.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnEntrar.Enabled = false;
        try
        {
            var nome = await _repositorio.Entrar(usuario, txtSenha.Text);
            Operador = new Operador(nome);
            File.WriteAllText(_arquivoUsuario, usuario);
            DialogResult = DialogResult.OK;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSenha.Clear();
            txtSenha.Focus();
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            MessageBox.Show("Não foi possível falar com a API em " + _repositorio.UrlBase +
                ". Confira se ela está rodando.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnEntrar.Enabled = true;
        }
    }
}
