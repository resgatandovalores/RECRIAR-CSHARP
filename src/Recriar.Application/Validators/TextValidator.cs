namespace Recriar.Application.Validators;

/// <summary>
/// Checks the content of text typed by the user.
/// Translated from the C functions "somenteNumeros" and "somenteLetras".
/// </summary>
// "static": a classe não precisa ser criada com "new". Chama-se direto: TextValidator.HasOnlyDigits(...)
public static class TextValidator
{
    /// <summary>
    /// True when the text has at least one character and all of them are digits from 0 to 9.
    /// Used for CPF, RG and phone numbers. (C: somenteNumeros)
    /// </summary>
    // "string?": o ? indica que o texto recebido pode ser null (ninguém digitou nada).
    public static bool HasOnlyDigits(string? text)
    {
        // No C: if(strlen(texto) == 0) return 0;
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        // No C: for(i = 0; i < strlen(texto); i++) { if(!isdigit(texto[i])) return 0; }
        foreach (char character in text)
        {
            // IsAsciiDigit só aceita 0 a 9. O IsDigit comum aceitaria também números de outros alfabetos.
            if (!char.IsAsciiDigit(character))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// True when the text has at least one letter and contains only letters, spaces and hyphens.
    /// Accented letters (ã, é, ç...) are accepted. Used for names, city, neighborhood... (C: somenteLetras)
    /// </summary>
    public static bool HasOnlyLetters(string? text)
    {
        // Diferente do C: um texto só com espaços também é recusado (no C, "   " passava).
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        foreach (char character in text)
        {
            // No C: texto[i] != ' ' && texto[i] != '-' && !isalpha(texto[i])
            // IsLetter reconhece letras com acento, o isalpha do C não reconhecia.
            if (character != ' ' && character != '-' && !char.IsLetter(character))
            {
                return false;
            }
        }

        return true;
    }
}
