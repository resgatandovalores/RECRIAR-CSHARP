using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Infrastructure.Export;
using Recriar.Infrastructure.Logging;
using Recriar.WinForms.Forms;

namespace Recriar.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        IErrorLogger errorLogger = new FileErrorLogger();
        System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        System.Windows.Forms.Application.ThreadException += (_, e) => ShowUnexpectedError(e.Exception, "Tela", errorLogger);

        try
        {
            IChildService? service = CreateService(errorLogger);
            if (service is null)
            {
                MessageBox.Show("A integração com o banco de dados ainda precisa ser configurada.",
                    "Configuração do RECRIAR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var mainForm = new MainForm(service, errorLogger);
            System.Windows.Forms.Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            ShowUnexpectedError(ex, "Abertura do programa", errorLogger);
        }
    }

    private static IChildService? CreateService(IErrorLogger errorLogger)
    {
        // Once persistence is available, instantiate its three implementations here
        // and pass them to CreateChildService along with the error logger.
        return null;
    }

    private static ChildService CreateChildService(
        IChildRepository children,
        IRelationshipTypeRepository relationshipTypes,
        IActivityLogger activityLogger,
        IErrorLogger errorLogger) =>
        new(children, relationshipTypes, activityLogger, errorLogger, new HtmlChildFileExporter());

    private static void ShowUnexpectedError(Exception exception, string context, IErrorLogger errorLogger)
    {
        errorLogger.LogError(exception, context);
        MessageBox.Show(ErrorHandler.GetMessage(exception), "Atenção",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
