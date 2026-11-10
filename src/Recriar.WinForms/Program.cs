using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Infrastructure.Export;
using Recriar.Infrastructure.Logging;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Repositories;
using Recriar.Domain.Entities;
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
            IChildService service = CreateService(errorLogger);
            using var mainForm = new MainForm(service, errorLogger);
            System.Windows.Forms.Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            ShowUnexpectedError(ex, "Abertura do programa", errorLogger);
        }
    }

    private static IChildService CreateService(IErrorLogger errorLogger)
    {
        AppDbContext context = AppDbContextFactory.Create();

        // Check the write connection before opening the main window.
        using (var connection = context.CreateWriteConnection())
        {
            connection.Open();
        }

        IChildRepository children = new ChildRepository(context);
        IRelationshipTypeRepository relationshipTypes = new RelationshipTypeRepository(context);
        IActivityLogger activityLogger = new DbActivityLogger(context);

        bool hasOther = false;
        foreach (RelationshipType type in relationshipTypes.GetAll())
        {
            if (type.Id == RelationshipType.OtherId && type.Name == "Outro")
            {
                hasOther = true;
            }
        }
        if (!hasOther)
        {
            throw new InvalidOperationException("Confira a carga inicial de parentescos: Outro deve ter Id 9.");
        }

        return CreateChildService(children, relationshipTypes, activityLogger, errorLogger);
    }

    private static ChildService CreateChildService(
        IChildRepository children,
        IRelationshipTypeRepository relationshipTypes,
        IActivityLogger activityLogger,
        IErrorLogger errorLogger)
    {
        return new ChildService(children, relationshipTypes, activityLogger, errorLogger,
            new HtmlChildFileExporter());
    }

    private static void ShowUnexpectedError(Exception exception, string context, IErrorLogger errorLogger)
    {
        errorLogger.LogError(exception, context);
        string message = ErrorHandler.GetMessage(exception);
        if (context == "Abertura do programa")
        {
            message = "Não foi possível iniciar o RECRIAR com o MySQL.\n\n" +
                "Confira se o servidor está ligado, se os dois bancos foram instalados e se as " +
                "variáveis RECRIAR_CONNECTION_STRING e RECRIAR_READ_CONNECTION_STRING " +
                "estão configuradas nesta sessão.\n\n" +
                "Veja as instruções em docs/database/README.md.";
        }
        MessageBox.Show(message, "Atenção",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
