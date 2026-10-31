namespace Recriar.Application.Interfaces;

/// <summary>
/// Contract for recording the technical detail of unexpected errors (database offline, bugs...),
/// so the support team can find out what really happened.
/// The user only sees the generic message; the detail goes here.
/// </summary>
public interface IErrorLogger
{
    /// <summary>
    /// Records an unexpected error with its full technical detail.
    /// IMPORTANT: this method must NEVER throw an error. If the log itself fails,
    /// it has to stay silent, otherwise it would hide the original error.
    /// </summary>
    /// <param name="exception">The error that happened.</param>
    /// <param name="context">Where it happened. Example: "Cadastro de criança".</param>
    void LogError(Exception exception, string context);
}
