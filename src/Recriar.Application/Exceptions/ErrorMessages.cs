using Recriar.Application.Validators;

namespace Recriar.Application.Exceptions;

/// <summary>
/// Catalog of every error message the system can show to the user.
/// This is the ONLY place where error texts are written; the rest of the code just calls it.
/// Translated from the messages printed by the C code ("Digite apenas numeros!", "Data invalida!"...).
/// </summary>
// Para criar um erro novo: acrescente uma constante (texto fixo) ou um método (texto com campos variáveis) aqui.
public static class ErrorMessages
{
    // ---- Erros sem variação: texto fixo ("const") ----

    /// <summary>Any error that is not catalogued. Shown instead of the technical error.</summary>
    public const string Unexpected =
        "Ocorreu um erro inesperado. Entre em contato com o suporte.";

    /// <summary>The id does not exist. (C: "Cadastro nao encontrado!")</summary>
    public const string ChildNotFound =
        "Cadastro não encontrado.";

    /// <summary>The registration date is wrong. (C: "Data invalida!")</summary>
    public const string InvalidRegistrationDate =
        "Data de cadastro inválida. Use o formato dd/mm/aaaa ou dd.mm.aaaa e uma data que exista.";

    /// <summary>More family members than allowed. (C: MAX_FAMILIA)</summary>
    public const string TooManyFamilyMembers =
        "A ficha pode ter no máximo 10 familiares.";

    /// <summary>The chosen kinship does not exist in the list.</summary>
    public const string InvalidRelationshipType =
        "Parentesco inválido. Escolha uma das opções da lista.";

    /// <summary>The kinship "Outro" was chosen but not described.</summary>
    public const string OtherRelationshipRequired =
        "Informe qual é o parentesco quando escolher \"Outro\".";

    // ---- Erros com variação: método que recebe o nome do campo ----
    // O nome do campo (ex.: "Nome", "CPF") é passado por quem chama, para a mensagem dizer qual campo está errado.

    /// <summary>The field was left empty.</summary>
    public static string FieldRequired(string field) =>
        $"O campo {field} é obrigatório.";

    /// <summary>The field has something besides letters, spaces and hyphens. (C: "Digite apenas letras!")</summary>
    public static string OnlyLetters(string field) =>
        $"O campo {field} deve conter apenas letras, espaços e hífen.";

    /// <summary>The field has something besides digits. (C: "Digite apenas numeros!")</summary>
    public static string OnlyDigits(string field) =>
        $"O campo {field} deve conter apenas números.";

    /// <summary>The age is not a number between the minimum and the maximum. (C: "Idade invalida")</summary>
    public static string InvalidAge(string field) =>
        $"O campo {field} é inválido. Informe um número de {AgeValidator.MinAge} a {AgeValidator.MaxAge}.";

    /// <summary>The text is longer than the database accepts. (In C the text was silently cut.)</summary>
    public static string FieldTooLong(string field, int maxLength) =>
        $"O campo {field} aceita no máximo {maxLength} caracteres.";
}
