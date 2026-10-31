namespace Recriar.Application.Validators;

/// <summary>
/// Reads and checks ages typed by the user.
/// Translated from the C function "lerIdade".
/// </summary>
public static class AgeValidator
{
    // "const": valor fixo que nunca muda. No C eram os números 0 e 120 escritos direto no if.
    public const int MinAge = 0;
    public const int MaxAge = 120;

    /// <summary>
    /// Tries to convert text into an age between 0 and 120.
    /// Returns false when the text is empty, has anything besides digits, or is out of range.
    /// (C: lerIdade, which also asked again in a loop; asking again is the screen's job now)
    /// </summary>
    public static bool TryParseAge(string? text, out int age)
    {
        age = 0;

        // No C: if(!somenteNumeros(entrada)) { printf("Digite apenas numeros: "); continue; }
        if (!TextValidator.HasOnlyDigits(text))
        {
            return false;
        }

        // No C: atoi(entrada). O int.TryParse também falha com números gigantes demais, que no C davam resultado errado.
        if (!int.TryParse(text, out int parsedAge))
        {
            return false;
        }

        // No C: if(idade < 0 || idade > 120) { printf("Idade invalida: "); continue; }
        if (parsedAge < MinAge || parsedAge > MaxAge)
        {
            return false;
        }

        age = parsedAge;
        return true;
    }
}
