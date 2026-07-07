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

    private readonly BindingSource _ticketsBindingSource = new();

    private readonly TextBox _apiBaseTextBox = new() { Text = "http://localhost:5298", Width = 220 };
    private readonly DataGridView _ticketsGrid = new();
    private readonly ComboBox _requesterComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _departmentComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _priorityComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _titleTextBox = new();
    private readonly TextBox _descriptionTextBox = new() { Multiline = true, Height = 70, ScrollBars = ScrollBars.Vertical };
    private readonly ToolStripStatusLabel _statusLabel = new("Pronto");

    public Form1()
    {
        InitializeComponent();
        BuildLayout();
        Shown += async (_, _) => await LoadInitialDataAsync();
    }

    private void BuildLayout()
    {
        Text = "CoopDesk Legacy - Atendimento Interno";
        Width = 1180;
        Height = 720;
        MinimumSize = new Size(980, 620);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(12)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        var refreshButton = new Button { Text = "Atualizar", Width = 100 };
        refreshButton.Click += async (_, _) => await LoadTicketsAsync();

        var startButton = new Button { Text = "Atender", Width = 90 };
        startButton.Click += async (_, _) => await ChangeSelectedTicketStatusAsync(TicketStatus.InProgress, "Atendimento iniciado pelo legado WinForms.");

        var resolveButton = new Button { Text = "Resolver", Width = 90 };
        resolveButton.Click += async (_, _) => await ChangeSelectedTicketStatusAsync(TicketStatus.Resolved, "Chamado resolvido pelo legado WinForms.");

        var closeButton = new Button { Text = "Fechar", Width = 90 };
        closeButton.Click += async (_, _) => await ChangeSelectedTicketStatusAsync(TicketStatus.Closed, "Chamado fechado pelo legado WinForms.");

        toolbar.Controls.AddRange([
            new Label { Text = "API:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
            _apiBaseTextBox,
            refreshButton,
            startButton,
            resolveButton,
            closeButton
        ]);

        ConfigureGrid();

        var form = BuildCreateTicketForm();

        var statusStrip = new StatusStrip();
        statusStrip.Items.Add(_statusLabel);

        root.Controls.Add(toolbar, 0, 0);
        root.Controls.Add(_ticketsGrid, 0, 1);
        root.Controls.Add(form, 0, 2);
        root.Controls.Add(statusStrip, 0, 3);

        Controls.Add(root);
    }

    private void ConfigureGrid()
    {
        _ticketsGrid.Dock = DockStyle.Fill;
        _ticketsGrid.AutoGenerateColumns = false;
        _ticketsGrid.AllowUserToAddRows = false;
        _ticketsGrid.AllowUserToDeleteRows = false;
        _ticketsGrid.ReadOnly = true;
        _ticketsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _ticketsGrid.MultiSelect = false;
        _ticketsGrid.DataSource = _ticketsBindingSource;

        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Titulo", DataPropertyName = nameof(TicketSummaryDto.Title), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Prioridade", DataPropertyName = nameof(TicketSummaryDto.Priority), Width = 100 });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = nameof(TicketSummaryDto.Status), Width = 120 });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Solicitante", DataPropertyName = nameof(TicketSummaryDto.RequesterName), Width = 160 });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Area", DataPropertyName = nameof(TicketSummaryDto.DepartmentName), Width = 140 });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Responsavel", DataPropertyName = nameof(TicketSummaryDto.AssignedAgentName), Width = 160 });
        _ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Criado em UTC", DataPropertyName = nameof(TicketSummaryDto.CreatedAtUtc), Width = 150 });
    }

    private Control BuildCreateTicketForm()
    {
        var group = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Abrir chamado"
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Padding = new Padding(10)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        _priorityComboBox.DataSource = Enum.GetValues<TicketPriority>();
        _priorityComboBox.SelectedItem = TicketPriority.Medium;

        AddLabeledControl(layout, "Titulo", _titleTextBox, 0, 0);
        AddLabeledControl(layout, "Prioridade", _priorityComboBox, 2, 0);
        AddLabeledControl(layout, "Solicitante", _requesterComboBox, 0, 1);
        AddLabeledControl(layout, "Area", _departmentComboBox, 2, 1);
        AddLabeledControl(layout, "Descricao", _descriptionTextBox, 0, 2);
        layout.SetColumnSpan(_descriptionTextBox, 3);

        var createButton = new Button { Text = "Abrir chamado", Width = 140, Anchor = AnchorStyles.Right };
        createButton.Click += async (_, _) => await CreateTicketAsync();
        layout.Controls.Add(createButton, 3, 3);

        group.Controls.Add(layout);
        return group;
    }

    private static void AddLabeledControl(TableLayoutPanel layout, string label, Control control, int column, int row)
    {
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, column, row);
        control.Dock = DockStyle.Fill;
        layout.Controls.Add(control, column + 1, row);
    }

    private async Task LoadInitialDataAsync()
    {
        await LoadReferenceDataAsync();
        await LoadTicketsAsync();
    }

    private async Task LoadReferenceDataAsync()
    {
        try
        {
            var departments = await _httpClient.GetFromJsonAsync<List<LookupItemDto>>(BuildUri("api/reference-data/departments"), _jsonOptions) ?? [];
            var collaborators = await _httpClient.GetFromJsonAsync<List<LookupItemDto>>(BuildUri("api/reference-data/collaborators"), _jsonOptions) ?? [];

            _departmentComboBox.DataSource = departments;
            _departmentComboBox.DisplayMember = nameof(LookupItemDto.Name);
            _departmentComboBox.ValueMember = nameof(LookupItemDto.Id);

            _requesterComboBox.DataSource = collaborators;
            _requesterComboBox.DisplayMember = nameof(LookupItemDto.Name);
            _requesterComboBox.ValueMember = nameof(LookupItemDto.Id);
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel carregar cadastros: {exception.Message}");
        }
    }

    private async Task LoadTicketsAsync()
    {
        try
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<TicketSummaryDto>>(BuildUri("api/tickets"), _jsonOptions) ?? [];
            _ticketsBindingSource.DataSource = tickets;
            SetStatus($"{tickets.Count} chamado(s) carregado(s).");
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel carregar chamados: {exception.Message}");
        }
    }

    private async Task CreateTicketAsync()
    {
        if (_requesterComboBox.SelectedValue is not Guid requesterId || _departmentComboBox.SelectedValue is not Guid departmentId)
        {
            SetStatus("Selecione solicitante e area.");
            return;
        }

        var request = new CreateTicketRequest(
            _titleTextBox.Text,
            _descriptionTextBox.Text,
            (TicketPriority)_priorityComboBox.SelectedItem!,
            requesterId,
            departmentId,
            null);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(BuildUri("api/tickets"), request, _jsonOptions);
            response.EnsureSuccessStatusCode();
            _titleTextBox.Clear();
            _descriptionTextBox.Clear();
            await LoadTicketsAsync();
            SetStatus("Chamado criado.");
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel criar chamado: {exception.Message}");
        }
    }

    private async Task ChangeSelectedTicketStatusAsync(TicketStatus status, string notes)
    {
        if (_ticketsBindingSource.Current is not TicketSummaryDto selectedTicket)
        {
            SetStatus("Selecione um chamado.");
            return;
        }

        try
        {
            var request = new ChangeTicketStatusRequest(status, notes, Environment.UserName);
            using var message = new HttpRequestMessage(HttpMethod.Patch, BuildUri($"api/tickets/{selectedTicket.Id}/status"))
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };
            using var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            await LoadTicketsAsync();
            SetStatus($"Chamado atualizado para {status}.");
        }
        catch (Exception exception)
        {
            SetStatus($"Nao foi possivel atualizar chamado: {exception.Message}");
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
