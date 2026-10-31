namespace Recriar.Application.Exceptions;

/// <summary>
/// Thrown when something the system already knows about goes wrong (wrong data typed, child not found...).
/// Carries one or more messages taken from <see cref="ErrorMessages"/>.
/// </summary>
// "Exception" é a classe base de todo erro do C#. Ao herdar dela (": Exception"), esta classe vira um tipo de erro nosso.
public class BusinessException : Exception
{
    /// <summary>Every message of the error. A registration can have many wrong fields at once.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>Creates an error with a single message.</summary>
    public BusinessException(string message) : base(message)
    {
        Errors = [message];
    }

    /// <summary>Creates an error with several messages (for example, one for each wrong field).</summary>
    public BusinessException(IReadOnlyList<string> messages)
        : base(string.Join(Environment.NewLine, messages))
    {
        Errors = messages;
    }
}
