using CentralAmparo.Dados;
using CentralAmparo.Modelos;

namespace CentralAmparo.Telas;

// Atendimento de um alerta: dados do idoso, contatos, histórico e botões de ação.
public class FormDetalheAlerta : Form
{
    private readonly IRepositorio _repositorio;
    private readonly Operador _operador;
    private readonly int _alertaId;
    private Alerta? _alerta;

    private readonly Label lblAlerta = new();
    private readonly Label lblIdoso = new();
    private readonly ListBox lstContatos = new();
    private readonly ListBox lstHistorico = new();
    private readonly TextBox txtObservacao = new();
    private readonly Button btnIniciar = new();
    private readonly Button btnLigouIdoso = new();
    private readonly Button btnLigouContato = new();
    private readonly Button btnSamu = new();
    private readonly Button btnResolvido = new();
    private readonly Button btnFalsoAlarme = new();

    public FormDetalheAlerta(IRepositorio repositorio, Operador operador, int alertaId)
    {
        _repositorio = repositorio;
        _operador = operador;
        _alertaId = alertaId;

        Text = "Alerta nº " + alertaId;
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(900, 600);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        // Coluna da esquerda: alerta, idoso e contatos, um embaixo do outro.
        // Os textos crescem conforme o conteúdo, então nada fica cortado.
        var colunaEsquerda = new FlowLayoutPanel
        {
            Location = new Point(10, 10),
            Size = new Size(420, 520),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        lblAlerta.AutoSize = true;
        lblAlerta.MaximumSize = new Size(390, 0);
        lblAlerta.Margin = new Padding(10, 5, 3, 10);
        lblIdoso.AutoSize = true;
        lblIdoso.MaximumSize = new Size(390, 0);
        lblIdoso.Margin = new Padding(10, 0, 3, 10);
        var lblContatos = new Label { Text = "Contatos de emergência:", AutoSize = true, Margin = new Padding(10, 0, 3, 3) };
        lstContatos.Size = new Size(390, 120);
        lstContatos.Margin = new Padding(10, 0, 3, 3);
        colunaEsquerda.Controls.AddRange(new Control[] { lblAlerta, lblIdoso, lblContatos, lstContatos });

        // Coluna da direita: histórico, observação e botões
        var lblHistorico = new Label { Text = "Histórico do atendimento:", Location = new Point(450, 15), AutoSize = true };
        lstHistorico.Location = new Point(450, 40);
        lstHistorico.Size = new Size(430, 200);
        lstHistorico.HorizontalScrollbar = true;
        var lblObservacao = new Label { Text = "Observação (vai junto com a próxima ação):", Location = new Point(450, 250), AutoSize = true };
        txtObservacao.Location = new Point(450, 275);
        txtObservacao.Size = new Size(430, 80);
        txtObservacao.Multiline = true;
        txtObservacao.MaxLength = 500;

        ConfigurarBotao(btnIniciar, "Iniciar atendimento", 450, 370);
        ConfigurarBotao(btnLigouIdoso, "Liguei para o idoso", 670, 370);
        ConfigurarBotao(btnLigouContato, "Liguei para o contato", 450, 415);
        ConfigurarBotao(btnSamu, "Acionei o SAMU (192)", 670, 415);
        ConfigurarBotao(btnResolvido, "Encerrar: resolvido", 450, 460);
        ConfigurarBotao(btnFalsoAlarme, "Encerrar: falso alarme", 670, 460);
        var btnFechar = new Button { Text = "Fechar", Location = new Point(670, 540), Size = new Size(210, 38), DialogResult = DialogResult.Cancel };

        btnIniciar.Click += BtnIniciar_Click;
        btnLigouIdoso.Click += async (s, e) => await RegistrarAcao(AcaoAtendimento.LigouIdoso);
        btnLigouContato.Click += async (s, e) => await RegistrarAcao(AcaoAtendimento.LigouContato);
        btnSamu.Click += async (s, e) => await RegistrarAcao(AcaoAtendimento.AcionouSamu);
        btnResolvido.Click += async (s, e) => await Encerrar(StatusAlerta.Resolvido);
        btnFalsoAlarme.Click += async (s, e) => await Encerrar(StatusAlerta.FalsoAlarme);

        Controls.AddRange(new Control[] { colunaEsquerda, lblHistorico, lstHistorico,
            lblObservacao, txtObservacao, btnFechar });
        CancelButton = btnFechar;

        Load += async (s, e) => await Carregar();
    }

    private void ConfigurarBotao(Button botao, string texto, int x, int y)
    {
        botao.Text = texto;
        botao.Location = new Point(x, y);
        botao.Size = new Size(210, 38);
        Controls.Add(botao);
    }

    private async Task Carregar()
    {
        try
        {
            _alerta = await _repositorio.ObterAlerta(_alertaId);
            var idoso = await _repositorio.ObterIdoso(_alerta.IdosoId);

            var chance = _alerta.ProbabilidadeQueda == null ? "-" : _alerta.ProbabilidadeQueda.Value.ToString("P0");
            lblAlerta.Text = $"{_alerta.TipoTexto()} - {_alerta.StatusTexto()}\n" +
                             $"Prioridade: {_alerta.Prioridade}\n" +
                             $"Chance de queda: {chance}\n" +
                             $"Recebido em: {_alerta.CriadoEm.ToLocalTime():dd/MM/yyyy HH:mm}";

            lblIdoso.Text = $"{idoso.Nome}, {idoso.Idade} anos\n" +
                            $"Endereço: {idoso.Endereco}\n" +
                            $"Bairro: {idoso.Bairro}\n" +
                            $"Saúde: {idoso.Observacoes ?? "nada informado"}";

            lstContatos.Items.Clear();
            foreach (var c in idoso.Contatos)
                lstContatos.Items.Add($"{c.Nome} ({c.Parentesco}) - {c.Telefone}");

            lstHistorico.Items.Clear();
            foreach (var a in _alerta.Atendimentos)
            {
                var observacao = a.Observacao == null ? "" : ": " + a.Observacao;
                lstHistorico.Items.Add($"{a.CriadoEm.ToLocalTime():HH:mm} {a.AcaoTexto()} - {a.Operador}{observacao}");
            }

            // Só deixa clicar no que faz sentido para o status atual.
            btnIniciar.Enabled = _alerta.Status == StatusAlerta.Aberto;
            btnLigouIdoso.Enabled = !_alerta.EstaEncerrado;
            btnLigouContato.Enabled = !_alerta.EstaEncerrado;
            btnSamu.Enabled = !_alerta.EstaEncerrado;
            btnResolvido.Enabled = _alerta.Status == StatusAlerta.EmAtendimento;
            btnFalsoAlarme.Enabled = _alerta.Status == StatusAlerta.EmAtendimento;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Não foi possível abrir o alerta: " + ex.Message, "Erro");
            Close();
        }
    }

    private async void BtnIniciar_Click(object? sender, EventArgs e)
    {
        try
        {
            _alerta!.IniciarAtendimento();
            await _repositorio.AlterarStatus(_alerta);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
        await Carregar();
    }

    private async Task RegistrarAcao(AcaoAtendimento acao)
    {
        try
        {
            var atendimento = _alerta!.RegistrarAcao(_operador, acao, txtObservacao.Text);
            await _repositorio.RegistrarAtendimento(_alerta, atendimento);
            txtObservacao.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
        await Carregar();
    }

    private async Task Encerrar(StatusAlerta resultado)
    {
        // Confere a observação antes de pedir confirmação, para não perguntar à toa.
        if (resultado == StatusAlerta.Resolvido && txtObservacao.Text.Trim() == "")
        {
            MessageBox.Show("Escreva na observação o que aconteceu antes de encerrar.", "Atenção");
            txtObservacao.Focus();
            return;
        }

        var texto = resultado == StatusAlerta.Resolvido ? "RESOLVIDO" : "FALSO ALARME";
        if (MessageBox.Show($"Encerrar este alerta como {texto}?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return;

        try
        {
            var atendimento = _alerta!.Encerrar(_operador, resultado, txtObservacao.Text);

            // A API marca RESOLVIDO quando recebe a ação ENCERROU. Falso alarme é só troca de status.
            if (resultado == StatusAlerta.Resolvido)
                await _repositorio.RegistrarAtendimento(_alerta, atendimento);
            else
                await _repositorio.AlterarStatus(_alerta);
            txtObservacao.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Atenção");
        }
        await Carregar();
    }
}
