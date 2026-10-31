namespace Recriar.Application.Validators;

/// <summary>
/// Names of the fields as the user sees them, used inside the error messages
/// (for example "O campo Nome da mãe é obrigatório.").
/// Kept in one place so the same field always has the same name in every message.
/// </summary>
public static class FieldNames
{
    /// <summary>Child's name.</summary>
    public const string Name = "Nome";

    /// <summary>Child's age.</summary>
    public const string Age = "Idade";

    /// <summary>Child's CPF.</summary>
    public const string Cpf = "CPF";

    /// <summary>Child's RG.</summary>
    public const string Rg = "RG";

    /// <summary>Address.</summary>
    public const string Address = "Endereço";

    /// <summary>Neighborhood.</summary>
    public const string Neighborhood = "Bairro";

    /// <summary>City.</summary>
    public const string City = "Cidade";

    /// <summary>School.</summary>
    public const string School = "Escola";

    /// <summary>Mother's name.</summary>
    public const string MotherName = "Nome da mãe";

    /// <summary>Father's name.</summary>
    public const string FatherName = "Nome do pai";

    /// <summary>Child's phone.</summary>
    public const string Phone = "Telefone";

    /// <summary>Registration date.</summary>
    public const string RegistrationDate = "Data de cadastro";

    /// <summary>Guardian's name.</summary>
    public const string GuardianName = "Nome do responsável";

    /// <summary>Guardian's RG.</summary>
    public const string GuardianRg = "RG do responsável";

    /// <summary>Guardian's CPF.</summary>
    public const string GuardianCpf = "CPF do responsável";

    /// <summary>Emergency contact's name.</summary>
    public const string EmergencyContactName = "Nome do contato de emergência";

    /// <summary>Emergency contact's phone.</summary>
    public const string EmergencyContactPhone = "Telefone de emergência";

    /// <summary>Health center.</summary>
    public const string HealthCenter = "Centro de saúde";

    /// <summary>File number.</summary>
    public const string FileNumber = "Número da ficha";

    /// <summary>Allergies.</summary>
    public const string Allergy = "Alergia";

    // Campos do familiar levam o número dele na lista (1, 2, 3...), para a pessoa saber qual corrigir.

    /// <summary>Kinship of the family member at position <paramref name="number"/> (starting at 1).</summary>
    public static string FamilyMemberRelationship(int number) => $"Parentesco do familiar {number}";

    /// <summary>Kinship written when "Outro" is chosen, for the family member at position <paramref name="number"/>.</summary>
    public static string FamilyMemberOtherRelationship(int number) => $"Outro parentesco do familiar {number}";

    /// <summary>Name of the family member at position <paramref name="number"/>.</summary>
    public static string FamilyMemberName(int number) => $"Nome do familiar {number}";

    /// <summary>Age of the family member at position <paramref name="number"/>.</summary>
    public static string FamilyMemberAge(int number) => $"Idade do familiar {number}";
}
