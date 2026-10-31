using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;

namespace Recriar.Infrastructure.Export;

/// <summary>
/// Saves the file (ficha) of a child as an HTML page, which opens in any browser and prints well.
/// Files are saved in the "fichas" folder next to the program, as ficha_ID.html (C: cadastro_ID.txt).
/// </summary>
public class HtmlChildFileExporter : IChildFileExporter
{
    // Protege só os caracteres perigosos no HTML (< > & e aspas) e deixa as letras com acento como estão.
    // "UnicodeRanges.All": todas as letras são permitidas, porque o arquivo já é salvo em UTF-8.
    private static readonly HtmlEncoder Encoder = HtmlEncoder.Create(UnicodeRanges.All);

    private readonly string _folder;

    /// <param name="folder">Folder where the files are saved. When omitted, uses a "fichas" folder next to the program.</param>
    public HtmlChildFileExporter(string? folder = null)
    {
        _folder = folder ?? Path.Combine(AppContext.BaseDirectory, "fichas");
    }

    /// <inheritdoc />
    public string Export(Child child, IReadOnlyDictionary<int, string> relationshipNames)
    {
        Directory.CreateDirectory(_folder);

        // No C: sprintf(nomeArquivo, "cadastro_%d.txt", pessoa.id);
        string path = Path.Combine(_folder, $"ficha_{child.Id}.html");

        // No C: fopen(..., "w") apagava o arquivo antigo e escrevia de novo. WriteAllText faz o mesmo.
        // UTF-8 para os acentos aparecerem certos.
        File.WriteAllText(path, BuildHtml(child, relationshipNames), Encoding.UTF8);

        return path;
    }

    /// <summary>Builds the whole page. Every typed value goes through <see cref="Encode"/>.</summary>
    // StringBuilder: monta um texto grande pedaço por pedaço sem criar uma string nova a cada "+".
    private static string BuildHtml(Child child, IReadOnlyDictionary<int, string> relationshipNames)
    {
        var html = new StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"pt-BR\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"utf-8\">");
        html.AppendLine($"<title>Ficha {child.Id} - {Encode(child.Name)}</title>");
        html.AppendLine("<style>");
        html.AppendLine("  body { font-family: Arial, sans-serif; margin: 32px; color: #222; }");
        html.AppendLine("  h1 { font-size: 22px; border-bottom: 2px solid #333; padding-bottom: 6px; }");
        html.AppendLine("  h2 { font-size: 16px; background: #eee; padding: 6px 8px; margin-top: 24px; }");
        html.AppendLine("  table { border-collapse: collapse; width: 100%; }");
        html.AppendLine("  th, td { border: 1px solid #bbb; padding: 6px 8px; text-align: left; vertical-align: top; }");
        html.AppendLine("  th { width: 30%; background: #f7f7f7; }");
        html.AppendLine("  .family th { width: auto; }");
        html.AppendLine("  .footer { margin-top: 32px; font-size: 12px; color: #666; }");
        // @media print: regras só para a impressão (sem margem extra, cores mantidas).
        html.AppendLine("  @media print { body { margin: 0; } h2 { -webkit-print-color-adjust: exact; print-color-adjust: exact; } }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("<h1>RECRIAR - Ficha de Cadastro</h1>");

        // No C: "========== CADASTRO =========="
        StartSection(html, "Dados da criança");
        Row(html, "Número da ficha", child.Id.ToString());
        Row(html, "Nome", child.Name);
        Row(html, "Idade", child.Age.ToString());
        Row(html, "CPF", child.Cpf);
        Row(html, "RG", child.Rg);
        Row(html, "Endereço", child.Address);
        Row(html, "Bairro", child.Neighborhood);
        Row(html, "Cidade", child.City);
        Row(html, "Escola", child.School);
        Row(html, "Nome da mãe", child.MotherName);
        Row(html, "Nome do pai", child.FatherName);
        Row(html, "Telefone", child.Phone);
        Row(html, "Data de cadastro", child.RegistrationDate.ToString("dd/MM/yyyy"));
        EndSection(html);

        // No C: "===== RESPONSAVEL ====="
        StartSection(html, "Responsável");
        Row(html, "Nome", child.Guardian?.Name);
        Row(html, "RG", child.Guardian?.Rg);
        Row(html, "CPF", child.Guardian?.Cpf);
        EndSection(html);

        // No C: "===== CONTATO EMERGENCIA ====="
        StartSection(html, "Contato de emergência");
        Row(html, "Nome", child.EmergencyContact?.Name);
        Row(html, "Telefone", child.EmergencyContact?.Phone);
        EndSection(html);

        // No C: "===== COMPOSICAO FAMILIAR =====" com um bloco por familiar. Aqui vira uma tabela.
        html.AppendLine("<h2>Composição familiar</h2>");
        if (child.FamilyMembers.Count == 0)
        {
            html.AppendLine("<p>Nenhum familiar cadastrado.</p>");
        }
        else
        {
            html.AppendLine("<table class=\"family\">");
            html.AppendLine("<tr><th>Parentesco</th><th>Nome</th><th>Idade</th></tr>");
            foreach (FamilyMember member in child.FamilyMembers)
            {
                html.AppendLine(
                    $"<tr><td>{Encode(RelationshipText(member, relationshipNames))}</td>" +
                    $"<td>{Encode(member.Name)}</td>" +
                    $"<td>{member.Age}</td></tr>");
            }
            html.AppendLine("</table>");
        }

        // No C: "===== OUTRAS INFORMACOES ====="
        StartSection(html, "Outras informações");
        Row(html, "Centro de saúde", child.HealthCenter);
        Row(html, "Número da ficha (saúde)", child.FileNumber);
        Row(html, "Alergia", child.Allergy);
        EndSection(html);

        html.AppendLine($"<p class=\"footer\">Ficha gerada em {DateTime.Now:dd/MM/yyyy HH:mm}.</p>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    /// <summary>
    /// Kinship shown in the file: the text written by the user when it is "Outro",
    /// otherwise the name of the option.
    /// </summary>
    private static string RelationshipText(FamilyMember member, IReadOnlyDictionary<int, string> relationshipNames)
    {
        if (member.RelationshipTypeId == RelationshipType.OtherId && !string.IsNullOrWhiteSpace(member.OtherRelationship))
        {
            return member.OtherRelationship;
        }

        // TryGetValue: procura no dicionário sem dar erro se a chave não existir.
        if (relationshipNames.TryGetValue(member.RelationshipTypeId, out string? name))
        {
            return name;
        }

        return member.RelationshipType?.Name ?? string.Empty;
    }

    private static void StartSection(StringBuilder html, string title)
    {
        html.AppendLine($"<h2>{Encode(title)}</h2>");
        html.AppendLine("<table>");
    }

    private static void EndSection(StringBuilder html) => html.AppendLine("</table>");

    /// <summary>One line "label | value". Empty values are shown as a dash.</summary>
    private static void Row(StringBuilder html, string label, string? value)
    {
        string shown = string.IsNullOrWhiteSpace(value) ? "-" : value;
        html.AppendLine($"<tr><th>{Encode(label)}</th><td>{Encode(shown)}</td></tr>");
    }

    /// <summary>
    /// Makes typed text safe inside HTML: characters such as &lt; and &gt; are written as text,
    /// so a value like "&lt;script&gt;" is shown, never run as code.
    /// </summary>
    private static string Encode(string? text) => Encoder.Encode(text ?? string.Empty);
}
