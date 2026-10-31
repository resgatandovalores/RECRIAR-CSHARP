using System.Globalization;

namespace Recriar.Application.Validators;

/// <summary>
/// Reads and checks dates typed by the user.
/// Translated from the C function "validarData".
/// </summary>
public static class DateValidator
{
    // Formatos aceitos: dia/mês/ano com barra ou com ponto. "d" e "M" aceitam com ou sem zero (1 ou 01).
    // No C: "tem que ter 2 barras OU 2 pontos".
    private static readonly string[] AcceptedFormats = ["d/M/yyyy", "d.M.yyyy"];

    /// <summary>
    /// Tries to convert text such as "31/10/2026" or "31.10.2026" into a date.
    /// Returns false when the text is empty, has another format or is a date that does not exist (31/02/2026).
    /// (C: validarData, which only counted slashes and digits and did not check if the date really existed)
    /// </summary>
    // "out DateTime date": além do true/false, o método devolve a data já convertida por esse parâmetro.
    // Esse formato "TryParse" é o padrão do C# para converter texto sem estourar erro.
    public static bool TryParseDate(string? text, out DateTime date)
    {
        date = default;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        // InvariantCulture: usa as regras fixas de data, iguais em qualquer computador (não depende do idioma do Windows).
        return DateTime.TryParseExact(
            text,
            AcceptedFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }
}
