using System.Diagnostics;
using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;

namespace Recriar.WinForms.Forms;

internal sealed class MainForm : Form
{
    private readonly IChildService _service;
    private readonly IErrorLogger _errorLogger;
    private readonly TextBox _search = new() { Width = 260, PlaceholderText = "Digite parte do nome" };
    private readonly Label _status = new() { AutoSize = true };
    private readonly Button _edit = new() { Text = "Editar ficha", AutoSize = true };
    private readonly Button _delete = new() { Text = "Excluir ficha", AutoSize = true };
    private readonly Button _export = new() { Text = "Exportar ficha", AutoSize = true };
    private readonly DataGridView _children = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        MultiSelect = false,
        RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
    };

    internal MainForm(IChildService service, IErrorLogger errorLogger)
    {
        _service = service;
        _errorLogger = errorLogger;
        Text = "RECRIAR | Cadastro de crianças";
        AutoScaleMode = AutoScaleMode.Font;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(880, 510);
        MinimumSize = new Size(740, 380);

        var searchButton = new Button { Text = "Pesquisar", AutoSize = true };
        var allButton = new Button { Text = "Listar todos", AutoSize = true };
        var newButton = new Button { Text = "Novo cadastro", AutoSize = true };
        var searchBar = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
        searchBar.Controls.AddRange([
            new Label { Text = "Nome da criança", AutoSize = true, Margin = new Padding(0, 7, 6, 0) },
            _search, searchButton, allButton
        ]);
        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
        actions.Controls.AddRange([newButton, _edit, _export, _delete]);

        _children.Columns.AddRange(
            Column("Ficha", nameof(ChildSummaryDto.Id), 15),
            Column("Nome da criança", nameof(ChildSummaryDto.Name), 45),
            Column("CPF", nameof(ChildSummaryDto.Cpf), 20),
            Column("Telefone", nameof(ChildSummaryDto.Phone), 20));

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(16) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label
        {
            Text = "Cadastros de crianças",
            Font = new Font(Font.FontFamily, 17, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 12)
        }, 0, 0);
        layout.Controls.Add(searchBar, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        layout.Controls.Add(_children, 0, 3);
        layout.Controls.Add(_status, 0, 4);
        Controls.Add(layout);

        AcceptButton = searchButton;
        searchButton.Click += (_, _) => LoadChildren(_search.Text);
        allButton.Click += (_, _) =>
        {
            _search.Clear();
            LoadChildren();
        };
        newButton.Click += (_, _) => OpenChild();
        _edit.Click += (_, _) => EditSelectedChild();
        _delete.Click += (_, _) => DeleteSelectedChild();
        _export.Click += (_, _) => ExportSelectedChild();
        _children.SelectionChanged += (_, _) => UpdateActions();
        _children.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                EditSelectedChild();
        };
        Shown += (_, _) => LoadChildren();
        UpdateActions();
    }

    private ChildSummaryDto? SelectedChild =>
        _children.SelectedRows.Count == 1
            ? _children.SelectedRows[0].DataBoundItem as ChildSummaryDto
            : null;

    private void LoadChildren(string? search = null)
    {
        try
        {
            var rows = search is null ? _service.GetAll() : _service.SearchByName(search);
            _children.DataSource = rows;
            _children.ClearSelection();
            _status.Text = rows.Count == 0
                ? "Nenhum cadastro encontrado."
                : $"Cadastros exibidos: {rows.Count}";
        }
        catch (Exception ex)
        {
            _children.DataSource = null;
            _status.Text = string.Empty;
            ShowError(ex);
        }

        UpdateActions();
    }

    private void OpenChild(int? id = null)
    {
        try
        {
            ChildDto child = id.HasValue ? _service.GetById(id.Value) : new ChildDto();
            var relationshipTypes = _service.GetRelationshipTypes();
            using var form = new ChildForm(_service, child, relationshipTypes);
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadChildren(_search.Text);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void EditSelectedChild()
    {
        if (SelectedChild is { } child)
            OpenChild(child.Id);
    }

    private void DeleteSelectedChild()
    {
        if (SelectedChild is not { } child)
            return;

        var answer = MessageBox.Show(this, $"Excluir a ficha {child.Id} de {child.Name}?",
            "Confirmar exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        if (answer != DialogResult.Yes)
            return;

        try
        {
            _service.Delete(child.Id);
            LoadChildren(_search.Text);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ExportSelectedChild()
    {
        if (SelectedChild is not { } child)
            return;

        string path;
        try
        {
            path = _service.ExportFile(child.Id);
        }
        catch (Exception ex)
        {
            ShowError(ex);
            return;
        }

        try
        {
            using var browser = Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            _errorLogger.LogError(ex, "Abertura da ficha exportada");
            ShowError(ex);
            MessageBox.Show(this, $"Ficha salva em:{Environment.NewLine}{path}", "Ficha exportada",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void UpdateActions()
    {
        bool hasSelection = SelectedChild is not null;
        _edit.Enabled = hasSelection;
        _delete.Enabled = hasSelection;
        _export.Enabled = hasSelection;
    }

    private void ShowError(Exception exception) =>
        MessageBox.Show(this, ErrorHandler.GetMessage(exception), "Atenção",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private static DataGridViewTextBoxColumn Column(string label, string property, float weight) => new()
    {
        HeaderText = label,
        DataPropertyName = property,
        FillWeight = weight
    };
}
