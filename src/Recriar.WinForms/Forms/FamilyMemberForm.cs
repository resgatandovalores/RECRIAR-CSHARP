using Recriar.Application.Dtos;
using Recriar.Domain.Entities;

namespace Recriar.WinForms.Forms;

internal sealed class FamilyMemberForm : Form
{
    private readonly ComboBox _relationship = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _otherLabel = new() { Text = "Qual parentesco?", AutoSize = true };
    private readonly TextBox _other = new() { AccessibleName = "Qual parentesco?" };
    private readonly TextBox _name = new() { AccessibleName = "Nome do familiar" };
    private readonly TextBox _age = new() { AccessibleName = "Idade do familiar" };

    internal FamilyMemberDto Member { get; private set; }

    internal FamilyMemberForm(List<RelationshipType> relationships, FamilyMemberDto member)
    {
        Member = member;
        Text = "Familiar";
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(460, 210);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;

        _relationship.AccessibleName = "Parentesco";
        _relationship.DisplayMember = nameof(RelationshipType.Name);
        _relationship.ValueMember = nameof(RelationshipType.Id);
        _relationship.DataSource = relationships;
        _name.Text = member.Name;
        _age.Text = member.Age;
        _other.Text = member.OtherRelationship;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(12) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(layout, 0, new Label { Text = "Parentesco", AutoSize = true }, _relationship);
        AddRow(layout, 1, _otherLabel, _other);
        AddRow(layout, 2, new Label { Text = "Nome", AutoSize = true }, _name);
        AddRow(layout, 3, new Label { Text = "Idade", AutoSize = true }, _age);

        var apply = new Button { Text = "Aplicar", AutoSize = true };
        var cancel = new Button { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft
        };
        actions.Controls.AddRange([apply, cancel]);
        layout.Controls.Add(actions, 0, 4);
        layout.SetColumnSpan(actions, 2);
        Controls.Add(layout);

        // Data binding can select the first item while the dialog is being created.
        Load += (_, _) =>
        {
            _relationship.SelectedValue = member.RelationshipTypeId;
            UpdateOtherField();
        };
        _relationship.SelectedValueChanged += (_, _) => UpdateOtherField();
        AcceptButton = apply;
        CancelButton = cancel;
        apply.Click += (_, _) =>
        {
            int relationshipId = _relationship.SelectedValue is int id ? id : 0;
            Member = new FamilyMemberDto
            {
                Id = member.Id,
                Name = _name.Text,
                Age = _age.Text,
                RelationshipTypeId = relationshipId,
                OtherRelationship = relationshipId == RelationshipType.OtherId ? _other.Text : string.Empty
            };
            DialogResult = DialogResult.OK;
        };
    }

    private void UpdateOtherField()
    {
        bool isOther = _relationship.SelectedValue is int id && id == RelationshipType.OtherId;
        _otherLabel.Visible = _other.Visible = isOther;
    }

    private static void AddRow(TableLayoutPanel layout, int row, Label label, Control input)
    {
        label.Anchor = AnchorStyles.Left;
        input.Dock = DockStyle.Fill;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(input, 1, row);
    }
}
