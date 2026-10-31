namespace Recriar.Application.Dtos;

/// <summary>
/// The few data shown in lists and search results.
/// Translated from what the C functions "listarCadastros" and "pesquisarPorNome" printed.
/// </summary>
public class ChildSummaryDto
{
    /// <summary>Id of the child.</summary>
    public int Id { get; set; }

    /// <summary>Full name. (C: nome)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>CPF. (C: cpf)</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>Phone. The C list did not show it, but the C search did. (C: telefone)</summary>
    public string Phone { get; set; } = string.Empty;
}
