namespace Recriar.Application.Helpers;

/// <summary>
/// Changes how text is written (lower case, capital letters) without checking if it is valid.
/// Translated from the C functions "converterMinusculo" and "primeiraMaiuscula".
/// </summary>
public static class TextFormatter
{
    /// <summary>
    /// Returns the text in lower case. Null becomes an empty text. (C: converterMinusculo)
    /// </summary>
    // No C, a função alterava o próprio texto recebido. Em C# a string não pode ser alterada:
    // o método devolve uma string nova, e quem chama guarda o resultado.
    public static string ToLowerCase(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // ToLowerInvariant: regras fixas, iguais em qualquer Windows.
        // Também converte letras com acento: "JOÃO" vira "joão" (no C, o "Ã" não era convertido).
        return text.ToLowerInvariant();
    }

    /// <summary>
    /// Returns the text with every word starting in capital letter and the rest in lower case.
    /// Example: "mARIA da silva" becomes "Maria Da Silva". Null becomes an empty text. (C: primeiraMaiuscula)
    /// </summary>
    public static string CapitalizeWords(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // No C: converterMinusculo(texto) primeiro, para deixar tudo minúsculo.
        // ToCharArray: copia o texto em um vetor de letras, que pode ser alterado (como o char texto[] do C).
        char[] characters = ToLowerCase(text).ToCharArray();

        for (int i = 0; i < characters.Length; i++)
        {
            // No C: texto[0] é maiúsculo, e texto[i] é maiúsculo quando texto[i - 1] == ' '.
            bool isFirstLetter = i == 0;
            bool followsSpace = i > 0 && characters[i - 1] == ' ';

            if (isFirstLetter || followsSpace)
            {
                characters[i] = char.ToUpperInvariant(characters[i]);
            }
        }

        // new string(vetor): monta o texto final a partir do vetor de letras.
        return new string(characters);
    }
}
