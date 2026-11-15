using CentralAmparo.Dados;
using CentralAmparo.Modelos;

namespace CentralAmparo.Telas;

// Tela principal: lista de alertas. Os abertos aparecem primeiro e em vermelho.
public class FormFila : Form
{
    private readonly IRepositorio _repositorio;
    private readonly Operador _operador;

    private readonly DataGridView grade = new();
    private readonly CheckBox chkEncerrados = new();
    private readonly Label lblSituacao = new();
    private readonly System.Windows.Forms.Timer timer = new();

    public FormFila(IRepositorio repositorio, Operador operador)
    {
        _repositorio = repositorio;
        _operador = operador;

        Text = "Central Amparo - Fila de alertas";
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(1150, 600);
        StartPosition = FormStartPosition.CenterScreen;

        // Barra de cima
        var painel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(8) };
        var btnAbrir = new Button { Text = "Abrir alerta", Size = new Size(140, 35) };
        var btnAtualizar = new Button { Text = "Atualizar", Size = new Size(120, 35) };
        var btnCadastrar = new Button { Text = "Cadastrar idoso", Size = new Size(160, 35) };
        chkEncerrados.Text = "Mostrar encerrados";
        chkEncerrados.AutoSize = true;
        chkEncerrados.Margin = new Padding(10, 10, 10, 0);

        btnAbrir.Click += (s, e) => AbrirAlerta();
        btnAtualizar.Click += async (s, e) => await Atualizar();
        btnCadastrar.Click += async (s, e) =>
        {
            new FormCadastroIdoso(_repositorio).ShowDialog(this);
            await Atualizar();
        };
        chkEncerrados.CheckedChanged += async (s, e) => await Atualizar();

        painel.Controls.AddRange(new Control[] { btnAbrir, btnAtualizar, btnCadastrar });

        // Só no modo demonstração: botão que faz o papel da pulseira.
        if (repositorio is RepositorioDemo demo)
        {
            var btnSimular = new Button { Text = "Simular queda (demo)", Size = new Size(220, 35) };
            btnSimular.Click += async (s, e) =>
            {
                demo.SimularQueda();
                await Atualizar();
            };
            painel.Controls.Add(btnSimular);
        }
        painel.Controls.Add(chkEncerrados);

        // Tabela
        grade.Dock = DockStyle.Fill;
        grade.ReadOnly = true;
        grade.AllowUserToAddRows = false;
        grade.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grade.MultiSelect = false;
        grade.RowHeadersVisible = false;
        grade.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grade.Columns.Add("id", "Nº");
        grade.Columns["id"]!.FillWeight = 40;
        grade.Columns.Add("tipo", "Tipo");
        grade.Columns["tipo"]!.FillWeight = 120;
        grade.Columns.Add("idoso", "Idoso");
        grade.Columns.Add("bairro", "Bairro");
        grade.Columns.Add("status", "Situação");
        grade.Columns.Add("prioridade", "Prioridade");
        grade.Columns.Add("chance", "Chance de queda");
        grade.Columns["chance"]!.FillWeight = 130;
        // O cabeçalho quebra a linha em vez de cortar o texto.
        grade.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grade.Columns.Add("recebido", "Recebido em");
        grade.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) AbrirAlerta(); };

        lblSituacao.Dock = DockStyle.Bottom;
        lblSituacao.Height = 30;

        Controls.Add(grade);
        Controls.Add(painel);
        Controls.Add(lblSituacao);

        // Atualiza a lista sozinha a cada 10 segundos.
        timer.Interval = 10000;
        timer.Tick += async (s, e) => await Atualizar();
        Load += async (s, e) =>
        {
            await Atualizar();
            timer.Start();
        };
    }

    private async Task Atualizar()
    {
        try
        {
            var alertas = await _repositorio.ListarAlertas();
            var idosos = await _repositorio.ListarIdosos();

            if (!chkEncerrados.Checked)
                alertas = alertas.Where(a => !a.EstaEncerrado).ToList();

            // Abertos primeiro, depois em atendimento, depois encerrados.
            // Dentro de cada grupo, a prioridade mais alta primeiro e, empatando, o mais recente.
            alertas = alertas.OrderBy(a => a.Status)
                .ThenBy(a => a.OrdemPrioridade)
                .ThenByDescending(a => a.CriadoEm)
                .ToList();

            grade.Rows.Clear();
            foreach (var alerta in alertas)
            {
                var idoso = idosos.FirstOrDefault(i => i.Id == alerta.IdosoId);
                var chance = alerta.ProbabilidadeQueda == null ? "-" : alerta.ProbabilidadeQueda.Value.ToString("P0");

                int linha = grade.Rows.Add(alerta.Id, alerta.TipoTexto(), idoso?.Nome ?? "?", idoso?.Bairro ?? "?",
                    alerta.StatusTexto(), alerta.Prioridade, chance, alerta.CriadoEm.ToLocalTime().ToString("dd/MM HH:mm"));

                if (alerta.Status == StatusAlerta.Aberto)
                    grade.Rows[linha].DefaultCellStyle.BackColor = Color.MistyRose;
                else if (alerta.Status == StatusAlerta.EmAtendimento)
                    grade.Rows[linha].DefaultCellStyle.BackColor = Color.LightYellow;
            }

            lblSituacao.ForeColor = Color.Black;
            lblSituacao.Text = $"Operador: {_operador.Nome}. {alertas.Count} alerta(s). Atualizado às {DateTime.Now:HH:mm:ss}.";
        }
        catch (Exception ex)
        {
            lblSituacao.ForeColor = Color.Red;
            lblSituacao.Text = "Erro ao buscar os alertas: " + ex.Message;
        }
    }

    private async void AbrirAlerta()
    {
        if (grade.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione um alerta na lista.");
            return;
        }

        int id = (int)grade.SelectedRows[0].Cells["id"].Value!;
        timer.Stop();
        new FormDetalheAlerta(_repositorio, _operador, id).ShowDialog(this);
        timer.Start();
        await Atualizar();
    }
}
