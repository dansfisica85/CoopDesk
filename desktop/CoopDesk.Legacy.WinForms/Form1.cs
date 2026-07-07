using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoopDesk.Application.Dtos;
using CoopDesk.Domain.Enums;

namespace CoopDesk.Legacy.WinForms;

public partial class Form1 : Form
{
    private readonly HttpClient _httpClient = new();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly TextBox _apiBaseTextBox = new() { Text = "http://localhost:5298", Width = 260 };
    private readonly TextBox _nameTextBox = new();
    private readonly TextBox _emailTextBox = new();
    private readonly ComboBox _departmentComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _problemTypeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _descriptionTextBox = new() { Multiline = true, Height = 140, ScrollBars = ScrollBars.Vertical };
    private readonly Label _protocolLabel = new() { Text = "Nenhuma solicitacao enviada nesta sessao.", AutoSize = true };
    private readonly ToolStripStatusLabel _statusLabel = new("Pronto");

    public Form1()
    {
        InitializeComponent();
        BuildLayout();
        Shown += async (_, _) => await LoadReferenceDataAsync();
    }

    private void BuildLayout()
    {
        Text = "CoopDesk Client - Solicitar Suporte";
        Width = 760;
        Height = 620;
        MinimumSize = new Size(680, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(16)
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));

        var title = new Label
        {
            Text = "Solicitacao de suporte",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 16, FontStyle.Bold),
            Anchor = AnchorStyles.Left
        };

        var apiPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        var reloadButton = new Button { Text = "Recarregar", Width = 90 };
        reloadButton.Click += async (_, _) => await LoadReferenceDataAsync();

        apiPanel.Controls.AddRange([
            new Label { Text = "API:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
            _apiBaseTextBox,
            reloadButton
        ]);

        header.Controls.Add(title, 0, 0);
        header.Controls.Add(apiPanel, 1, 0);

        var form = BuildSupportRequestForm();

        var resultPanel = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Protocolo"
        };
        _protocolLabel.Dock = DockStyle.Fill;
        _protocolLabel.Padding = new Padding(10);
        resultPanel.Controls.Add(_protocolLabel);

        var statusStrip = new StatusStrip();
        statusStrip.Items.Add(_statusLabel);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(form, 0, 1);
        root.Controls.Add(resultPanel, 0, 2);
        root.Controls.Add(statusStrip, 0, 3);

        Controls.Add(root);
    }

    private Control BuildSupportRequestForm()
    {
        var group = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Dados da solicitacao"
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(14)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        AddLabeledControl(layout, "Nome", _nameTextBox, 0);
        AddLabeledControl(layout, "E-mail", _emailTextBox, 1);
        AddLabeledControl(layout, "Setor", _departmentComboBox, 2);
        AddLabeledControl(layout, "Problema", _problemTypeComboBox, 3);
        AddLabeledControl(layout, "Descricao", _descriptionTextBox, 4);

        var submitButton = new Button
        {
            Text = "Enviar solicitacao",
            Width = 150,
            Height = 34,
            Anchor = AnchorStyles.Right
        };
        submitButton.Click += async (_, _) => await SubmitSupportRequestAsync();
        layout.Controls.Add(submitButton, 1, 5);

        group.Controls.Add(layout);
        return group;
    }

    private static void AddLabeledControl(TableLayoutPanel layout, string label, Control control, int row)
    {
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        control.Dock = DockStyle.Fill;
        layout.Controls.Add(control, 1, row);
    }

    private async Task LoadReferenceDataAsync()
    {
        try
        {
            SetStatus("Carregando dados de apoio...");

            var departments = await _httpClient.GetFromJsonAsync<List<LookupItemDto>>(BuildUri("api/reference-data/departments"), _jsonOptions) ?? [];
            var problemTypes = await _httpClient.GetFromJsonAsync<List<ProblemTypeDto>>(BuildUri("api/reference-data/problem-types"), _jsonOptions) ?? [];

            _departmentComboBox.DataSource = departments;
            _departmentComboBox.DisplayMember = nameof(LookupItemDto.Name);
            _departmentComboBox.ValueMember = nameof(LookupItemDto.Id);

            _problemTypeComboBox.DataSource = problemTypes;
            _problemTypeComboBox.DisplayMember = nameof(ProblemTypeDto.Name);
            _problemTypeComboBox.ValueMember = nameof(ProblemTypeDto.Value);

            SetStatus("Dados carregados. Preencha a solicitacao.");
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel carregar dados: {exception.Message}");
        }
    }

    private async Task SubmitSupportRequestAsync()
    {
        if (_departmentComboBox.SelectedValue is not Guid departmentId ||
            _problemTypeComboBox.SelectedValue is not string problemTypeValue ||
            !Enum.TryParse<SupportProblemType>(problemTypeValue, out var problemType))
        {
            SetStatus("Selecione setor e tipo de problema.");
            return;
        }

        var request = new CreateSupportRequest(
            _nameTextBox.Text,
            _emailTextBox.Text,
            departmentId,
            problemType,
            _descriptionTextBox.Text);

        try
        {
            SetStatus("Enviando solicitacao...");
            using var response = await _httpClient.PostAsJsonAsync(BuildUri("api/support-requests"), request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SupportRequestResponseDto>(_jsonOptions);
            if (result is null)
            {
                SetStatus("Solicitacao enviada, mas a API nao retornou protocolo.");
                return;
            }

            _descriptionTextBox.Clear();
            _protocolLabel.Text = $"Protocolo: {result.Protocol}\nStatus: {result.Status}\nCriado em UTC: {result.CreatedAtUtc:dd/MM/yyyy HH:mm}";
            SetStatus("Solicitacao enviada com sucesso.");
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel enviar: {exception.Message}");
        }
    }

    private Uri BuildUri(string relativePath)
    {
        var baseUrl = _apiBaseTextBox.Text.Trim().TrimEnd('/');
        return new Uri($"{baseUrl}/{relativePath.TrimStart('/')}");
    }

    private void SetStatus(string message)
    {
        _statusLabel.Text = message;
    }
}
