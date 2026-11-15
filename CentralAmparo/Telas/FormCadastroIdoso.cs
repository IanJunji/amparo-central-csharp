using CentralAmparo.Dados;
using CentralAmparo.Modelos;

namespace CentralAmparo.Telas;

// Cadastro de idoso com os contatos de emergência.
public class FormCadastroIdoso : Form
{
    private readonly IRepositorio _repositorio;
    private readonly List<ContatoEmergencia> _contatos = new();

    private readonly TextBox txtNome = new();
    private readonly DateTimePicker dtpNascimento = new();
    private readonly TextBox txtBairro = new();
    private readonly TextBox txtEndereco = new();
    private readonly TextBox txtObservacoes = new();
    private readonly TextBox txtContatoNome = new();
    private readonly TextBox txtContatoTelefone = new();
    private readonly TextBox txtContatoParentesco = new();
    private readonly ListBox lstContatos = new();

    public FormCadastroIdoso(IRepositorio repositorio)
    {
        _repositorio = repositorio;

        Text = "Cadastrar idoso";
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(560, 640);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        AdicionarCampo("Nome completo *", txtNome, 15);
        AdicionarCampo("Data de nascimento *", dtpNascimento, 75);
        AdicionarCampo("Bairro *", txtBairro, 135);
        AdicionarCampo("Endereço", txtEndereco, 195);
        AdicionarCampo("Observações de saúde", txtObservacoes, 255);

        dtpNascimento.Format = DateTimePickerFormat.Short;
        dtpNascimento.Value = DateTime.Today.AddYears(-75);
        txtNome.MaxLength = 120;

        var lblContatos = new Label { Text = "Contatos de emergência (pelo menos um) *", Location = new Point(20, 320), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
        var lblCNome = new Label { Text = "Nome", Location = new Point(20, 350), AutoSize = true };
        var lblCTelefone = new Label { Text = "Telefone com DDD", Location = new Point(220, 350), AutoSize = true };
        var lblCParentesco = new Label { Text = "Parentesco", Location = new Point(390, 350), AutoSize = true };
        txtContatoNome.SetBounds(20, 375, 190, 30);
        txtContatoTelefone.SetBounds(220, 375, 160, 30);
        txtContatoParentesco.SetBounds(390, 375, 150, 30);

        var btnAdicionar = new Button { Text = "Adicionar contato", Location = new Point(20, 415), Size = new Size(170, 35) };
        var btnRemover = new Button { Text = "Remover contato", Location = new Point(200, 415), Size = new Size(170, 35) };
        lstContatos.SetBounds(20, 460, 520, 100);

        var btnSalvar = new Button { Text = "Salvar", Location = new Point(320, 585), Size = new Size(110, 38) };
        var btnCancelar = new Button { Text = "Cancelar", Location = new Point(430, 585), Size = new Size(110, 38), DialogResult = DialogResult.Cancel };

        btnAdicionar.Click += BtnAdicionar_Click;
        btnRemover.Click += BtnRemover_Click;
        btnSalvar.Click += BtnSalvar_Click;

        Controls.AddRange(new Control[] { lblContatos, lblCNome, lblCTelefone, lblCParentesco, txtContatoNome,
            txtContatoTelefone, txtContatoParentesco, btnAdicionar, btnRemover, lstContatos, btnSalvar, btnCancelar });
        CancelButton = btnCancelar;
    }

    private void AdicionarCampo(string rotulo, Control campo, int y)
    {
        Controls.Add(new Label { Text = rotulo, Location = new Point(20, y), AutoSize = true });
        campo.SetBounds(20, y + 25, 520, 30);
        Controls.Add(campo);
    }

    private void BtnAdicionar_Click(object? sender, EventArgs e)
    {
        try
        {
            var contato = new ContatoEmergencia(txtContatoNome.Text, txtContatoTelefone.Text, txtContatoParentesco.Text);
            if (_contatos.Any(c => c.Telefone == contato.Telefone))
            {
                MessageBox.Show("Este telefone já está na lista.", "Atenção");
                return;
            }

            _contatos.Add(contato);
            lstContatos.Items.Add($"{contato.Nome} ({contato.Parentesco}) - {contato.Telefone}");
            txtContatoNome.Clear();
            txtContatoTelefone.Clear();
            txtContatoParentesco.Clear();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
    }

    private void BtnRemover_Click(object? sender, EventArgs e)
    {
        int indice = lstContatos.SelectedIndex;
        if (indice < 0)
            return;
        _contatos.RemoveAt(indice);
        lstContatos.Items.RemoveAt(indice);
    }

    private async void BtnSalvar_Click(object? sender, EventArgs e)
    {
        try
        {
            var idoso = new Idoso(txtNome.Text, DateOnly.FromDateTime(dtpNascimento.Value),
                txtBairro.Text, txtEndereco.Text, txtObservacoes.Text);

            if (_contatos.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um contato de emergência.", "Atenção");
                return;
            }
            idoso.Contatos.AddRange(_contatos);

            await _repositorio.CadastrarIdoso(idoso);
            MessageBox.Show($"{idoso.Nome} foi cadastrado(a).", "Cadastro salvo");
            DialogResult = DialogResult.OK;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Não foi possível salvar: " + ex.Message, "Erro");
        }
    }
}
