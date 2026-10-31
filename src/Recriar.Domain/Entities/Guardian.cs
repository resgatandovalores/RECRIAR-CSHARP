namespace Recriar.Domain.Entities;

/// <summary>
/// The legal guardian responsible for a child.
/// Translated from the C fields nomeResponsavel, rgResponsavel and cpfResponsavel.
/// </summary>
public class Guardian
{
    public int Id { get; set; }

    /// <summary>Full name of the guardian. (C: nomeResponsavel)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>RG document number, digits only. (C: rgResponsavel)</summary>
    public string Rg { get; set; } = string.Empty;

    /// <summary>CPF document number, digits only. (C: cpfResponsavel)</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Id of the child this guardian is responsible for.</summary>
    public int ChildId { get; set; }

    /// <summary>The child this guardian is responsible for.</summary>
    public Child? Child { get; set; }
}
