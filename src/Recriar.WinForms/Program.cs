namespace Recriar.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        // Nome completo, pois "Application" sozinho aponta para o projeto Recriar.Application.
        System.Windows.Forms.Application.Run(new Form1());
    }    
}