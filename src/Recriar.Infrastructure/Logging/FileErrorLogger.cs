using Recriar.Application.Interfaces;

namespace Recriar.Infrastructure.Logging;

/// <summary>
/// Saves the technical detail of unexpected errors in text files, one file per day.
/// Works even when the database is offline, because it does not use the database.
/// Files are written to the "logs" folder next to the program (errors-yyyy-MM-dd.log).
/// </summary>
public class FileErrorLogger : IErrorLogger
{
    // "lock": só uma gravação por vez no arquivo, para duas ações ao mesmo tempo não se misturarem.
    private static readonly object FileLock = new();

    private readonly string _folder;

    /// <param name="folder">Folder where the files are saved. When omitted, uses a "logs" folder next to the program.</param>
    public FileErrorLogger(string? folder = null)
    {
        _folder = folder ?? Path.Combine(AppContext.BaseDirectory, "logs");
    }

    /// <inheritdoc />
    public void LogError(Exception exception, string context)
    {
        // O log nunca pode causar um erro novo: qualquer falha aqui é engolida de propósito.
        try
        {
            Directory.CreateDirectory(_folder);

            DateTime now = DateTime.Now;
            string path = Path.Combine(_folder, $"errors-{now:yyyy-MM-dd}.log");

            // exception.ToString() traz o tipo do erro, a mensagem, o caminho no código (stack trace) e os erros internos.
            string text =
                $"[{now:yyyy-MM-dd HH:mm:ss}] {context}{Environment.NewLine}" +
                $"{exception}{Environment.NewLine}" +
                $"{new string('-', 60)}{Environment.NewLine}";

            lock (FileLock)
            {
                File.AppendAllText(path, text);
            }
        }
        catch
        {
            // Sem log não há o que fazer; o erro original continua sendo tratado normalmente.
        }
    }
}
