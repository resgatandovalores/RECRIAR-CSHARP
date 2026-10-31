namespace Recriar.Application.Exceptions;

/// <summary>
/// Turns any error into a message that is safe to show to the user.
/// The screens call it inside "catch", so the user never sees a technical error.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Returns the text to show in a message box.
    /// Known errors (BusinessException) show their own messages, one per line.
    /// Any other error shows the generic message that asks the user to call support.
    /// </summary>
    public static string GetMessage(Exception exception)
    {
        // "is BusinessException business": confere se o erro é do nosso tipo e, se for, já o guarda em "business".
        if (exception is BusinessException business)
        {
            return string.Join(Environment.NewLine, business.Errors);
        }

        // Erro não catalogado (banco fora do ar, falha inesperada...): mensagem genérica, sem detalhe técnico.
        return ErrorMessages.Unexpected;
    }
}
