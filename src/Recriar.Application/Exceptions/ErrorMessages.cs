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
    // "=>" sem parâmetros: propriedade calculada. Não pode ser "const" porque usa o número de outra classe.
    public static string TooManyFamilyMembers =>
        $"A ficha pode ter no máximo {ChildValidator.MaxFamilyMembers} familiares.";

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

    /// <summary>The file could not be written (folder without permission, file open in another program...).</summary>
    // Fica em "const" igual aos outros textos fixos; está aqui embaixo só para ficar perto das regras de exportação.
    public const string ExportFailed =
        "Não foi possível salvar a ficha. Feche a ficha se ela estiver aberta e tente de novo. Se continuar, entre em contato com o suporte.";

    /// <summary>The chosen kinship does not exist in the list.</summary>
    public static string InvalidRelationshipType(string field) =>
        $"O campo {field} é inválido. Escolha uma das opções da lista.";

    /// <summary>The kinship "Outro" was chosen but the real kinship was not written.</summary>
    public static string OtherRelationshipRequired(string field) =>
        $"O campo {field} é obrigatório quando o parentesco escolhido é \"Outro\".";
}
