using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;

namespace Recriar.WinForms.Forms;

internal sealed class ChildForm : Form
{
    private readonly IChildService _service;
    private readonly int _childId;
    private readonly List<RelationshipType> _relationshipTypes;
    private readonly List<FamilyMemberDto> _familyMembers;
    private readonly TextBox _name = new();
    private readonly TextBox _age = new();
    private readonly TextBox _cpf = new();
    private readonly TextBox _rg = new();
    private readonly TextBox _address = new();
    private readonly TextBox _neighborhood = new();
    private readonly TextBox _city = new();
    private readonly TextBox _school = new();
    private readonly TextBox _mother = new();
    private readonly TextBox _father = new();
    private readonly TextBox _phone = new();
    private readonly TextBox _date = new() { PlaceholderText = "dd/mm/aaaa" };
    private readonly TextBox _guardianName = new();
    private readonly TextBox _guardianRg = new();
    private readonly TextBox _guardianCpf = new();
    private readonly TextBox _emergencyName = new();
    private readonly TextBox _emergencyPhone = new();
    private readonly TextBox _healthCenter = new();
    private readonly TextBox _fileNumber = new();
    private readonly TextBox _allergy = new();
    private readonly ListView _familyList = new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        MultiSelect = false,
        HideSelection = false
    };
    private readonly Button _addFamily = new() { Text = "Adicionar familiar", AutoSize = true };
    private readonly Button _editFamily = new() { Text = "Editar", AutoSize = true };
    private readonly Button _removeFamily = new() { Text = "Remover", AutoSize = true };
    private readonly Label _familyCount = new() { AutoSize = true };

    internal ChildForm(IChildService service, ChildDto child, List<RelationshipType> relationshipTypes)
    {
        _service = service;
        _childId = child.Id;
        _relationshipTypes = relationshipTypes;
        _familyMembers = [.. child.FamilyMembers];
        Text = child.Id == 0 ? "Novo cadastro" : $"Editar ficha {child.Id}";
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(740, 590);
        MinimumSize = new Size(580, 450);
        ShowInTaskbar = false;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(CreateFieldsTab("Criança",
            ("Nome *", _name, child.Name),
            ("Idade *", _age, child.Age),
            ("CPF *", _cpf, child.Cpf),
            ("RG *", _rg, child.Rg),
            ("Endereço", _address, child.Address),
            ("Bairro *", _neighborhood, child.Neighborhood),
            ("Cidade *", _city, child.City),
            ("Escola", _school, child.School),
            ("Telefone *", _phone, child.Phone),
            ("Data de cadastro *", _date, child.RegistrationDate)));
        tabs.TabPages.Add(CreateFieldsTab("Responsáveis e emergência",
            ("Nome da mãe *", _mother, child.MotherName),
            ("Nome do pai *", _father, child.FatherName),
            ("Nome do responsável *", _guardianName, child.GuardianName),
            ("RG do responsável *", _guardianRg, child.GuardianRg),
            ("CPF do responsável *", _guardianCpf, child.GuardianCpf),
            ("Contato de emergência *", _emergencyName, child.EmergencyContactName),
            ("Telefone de emergência *", _emergencyPhone, child.EmergencyContactPhone)));
        tabs.TabPages.Add(CreateFieldsTab("Saúde",
            ("Centro de saúde", _healthCenter, child.HealthCenter),
            ("Número da ficha de saúde", _fileNumber, child.FileNumber),
            ("Alergia", _allergy, child.Allergy)));
        tabs.TabPages.Add(CreateFamilyTab());

        var save = new Button { Text = "Salvar ficha", AutoSize = true };
        var cancel = new Button { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft
        };
        actions.Controls.AddRange([save, cancel]);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(12) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Text = "* Campo obrigatório. Use apenas números em CPF, RG e telefone."
        }, 0, 0);
        layout.Controls.Add(tabs, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        Controls.Add(layout);

        AcceptButton = save;
        CancelButton = cancel;
        save.Click += (_, _) => SaveChild();
    }

    private static TabPage CreateFieldsTab(string title, params (string Label, TextBox Input, string Value)[] fields)
    {
        var tab = new TabPage(title) { AutoScroll = true };
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2,
            RowCount = fields.Length, Padding = new Padding(12)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 205));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (int index = 0; index < fields.Length; index++)
        {
            var (label, input, value) = fields[index];
            input.Text = value;
            input.AccessibleName = label.TrimEnd(' ', '*');
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(3, 5, 3, 5);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, index);
            layout.Controls.Add(input, 1, index);
        }

        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage CreateFamilyTab()
    {
        var tab = new TabPage("Composição familiar");
        _familyList.Columns.Add("Parentesco", 155);
        _familyList.Columns.Add("Nome", 260);
        _familyList.Columns.Add("Idade", 70);

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
        actions.Controls.AddRange([_addFamily, _editFamily, _removeFamily]);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(_familyCount, 0, 0);
        layout.Controls.Add(_familyList, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        tab.Controls.Add(layout);

        _addFamily.Click += (_, _) => OpenFamilyMember();
        _editFamily.Click += (_, _) => EditSelectedFamilyMember();
        _familyList.DoubleClick += (_, _) => EditSelectedFamilyMember();
        _removeFamily.Click += (_, _) =>
        {
            if (_familyList.SelectedIndices.Count == 1)
            {
                _familyMembers.RemoveAt(_familyList.SelectedIndices[0]);
                RefreshFamily();
            }
        };
        _familyList.SelectedIndexChanged += (_, _) => UpdateFamilyActions();

        RefreshFamily();
        return tab;
    }

    private void EditSelectedFamilyMember()
    {
        if (_familyList.SelectedIndices.Count == 1)
            OpenFamilyMember(_familyList.SelectedIndices[0]);
    }

    private void OpenFamilyMember(int? index = null)
    {
        var member = index.HasValue ? _familyMembers[index.Value] : new FamilyMemberDto();
        using var editor = new FamilyMemberForm(_relationshipTypes, member);
        if (editor.ShowDialog(this) != DialogResult.OK)
            return;

        if (index.HasValue)
            _familyMembers[index.Value] = editor.Member;
        else
            _familyMembers.Add(editor.Member);

        RefreshFamily(index ?? (_familyMembers.Count - 1));
    }

    private void RefreshFamily(int? selectedIndex = null)
    {
        _familyList.Items.Clear();
        foreach (var member in _familyMembers)
        {
            string relationship = member.RelationshipTypeId == RelationshipType.OtherId
                ? member.OtherRelationship
                : _relationshipTypes.Find(type => type.Id == member.RelationshipTypeId)?.Name ?? "—";
            _familyList.Items.Add(new ListViewItem([relationship, member.Name, member.Age]));
        }

        if (selectedIndex.HasValue)
            _familyList.Items[selectedIndex.Value].Selected = true;

        _familyCount.Text = $"Familiares na ficha: {_familyMembers.Count} de {ChildValidator.MaxFamilyMembers}";
        UpdateFamilyActions();
    }

    private void UpdateFamilyActions()
    {
        _addFamily.Enabled = _familyMembers.Count < ChildValidator.MaxFamilyMembers;
        _editFamily.Enabled = _removeFamily.Enabled = _familyList.SelectedIndices.Count == 1;
    }

    private void SaveChild()
    {
        var child = new ChildDto
        {
            Id = _childId,
            Name = _name.Text,
            Age = _age.Text,
            Cpf = _cpf.Text,
            Rg = _rg.Text,
            Address = _address.Text,
            Neighborhood = _neighborhood.Text,
            City = _city.Text,
            School = _school.Text,
            MotherName = _mother.Text,
            FatherName = _father.Text,
            Phone = _phone.Text,
            RegistrationDate = _date.Text,
            GuardianName = _guardianName.Text,
            GuardianRg = _guardianRg.Text,
            GuardianCpf = _guardianCpf.Text,
            EmergencyContactName = _emergencyName.Text,
            EmergencyContactPhone = _emergencyPhone.Text,
            HealthCenter = _healthCenter.Text,
            FileNumber = _fileNumber.Text,
            Allergy = _allergy.Text,
            FamilyMembers = _familyMembers
        };

        try
        {
            if (_childId == 0)
            {
                int id = _service.Register(child);
                MessageBox.Show(this, $"Cadastro realizado. Ficha número {id}.", "RECRIAR",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _service.Update(_childId, child);
            }

            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ErrorHandler.GetMessage(ex), "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
