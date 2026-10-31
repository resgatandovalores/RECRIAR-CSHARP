namespace Recriar.Application.Interfaces;

/// <summary>
/// Contract for recording what people did in the system (activity log / audit).
/// Implemented by the database project, saving each record in a table.
/// The service calls it when a child is registered, edited or deleted.
/// </summary>
public interface IActivityLogger
{
    /// <summary>
    /// Records one activity. The implementation must also save the date and time of the record.
    /// </summary>
    /// <param name="action">Short name of what happened. Example: "Cadastro criado".</param>
    /// <param name="description">Details. Example: "Criança 12 - Maria Silva".</param>
    void Log(string action, string description);
}
