namespace Recriar.Application.Validators;

/// <summary>
/// Maximum number of characters of each text field.
/// The sizes come from the C struct (char nome[100], char bairro[50], char cpf[20]...).
/// The database configuration can use these same numbers, so screen, rules and database always agree.
/// </summary>
public static class FieldLimits
{
    /// <summary>Names of people: child, mother, father, guardian, emergency contact, family member. (C: [100])</summary>
    public const int PersonName = 100;

    /// <summary>CPF and RG. (C: [20])</summary>
    public const int Document = 20;

    /// <summary>Phone numbers. (C: [20])</summary>
    public const int Phone = 20;

    /// <summary>Street and number. (C: endereco[100])</summary>
    public const int Address = 100;

    /// <summary>Neighborhood. (C: bairro[50])</summary>
    public const int Neighborhood = 50;

    /// <summary>City. (C: cidade[50])</summary>
    public const int City = 50;

    /// <summary>School. (C: escola[100])</summary>
    public const int School = 100;

    /// <summary>Health center. (C: centroSaude[100])</summary>
    public const int HealthCenter = 100;

    /// <summary>File number at the health center. (C: ficha[50])</summary>
    public const int FileNumber = 50;

    /// <summary>Allergies. (C: alergia[100])</summary>
    public const int Allergy = 100;

    /// <summary>Kinship: RelationshipType.Name and FamilyMember.OtherRelationship. (C: parentesco[50])</summary>
    public const int Relationship = 50;
}
