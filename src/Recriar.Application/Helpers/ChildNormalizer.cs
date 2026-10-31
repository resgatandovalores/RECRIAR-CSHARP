using Recriar.Application.Dtos;

namespace Recriar.Application.Helpers;

/// <summary>
/// Cleans the data typed on the screen before it is checked and saved:
/// removes spaces at the start and the end of every text (Trim) and writes names with capital letters.
/// </summary>
public static class ChildNormalizer
{
    /// <summary>
    /// Returns a NEW, cleaned copy. The object received from the screen is not changed,
    /// so the screen keeps showing exactly what the user typed if there is an error.
    /// </summary>
    /// <remarks>
    /// Formatted with CapitalizeWords (as primeiraMaiuscula in the C code): names of people,
    /// neighborhood, city, school and the kinship written in "Outro".
    /// Only trimmed (as in the C code): address, documents, phones, date, health center, file number and allergy.
    /// </remarks>
    public static ChildDto Normalize(ChildDto source)
    {
        return new ChildDto
        {
            Id = source.Id,
            Name = Capitalize(source.Name),
            Age = Clean(source.Age),
            Cpf = Clean(source.Cpf),
            Rg = Clean(source.Rg),
            Address = Clean(source.Address),
            Neighborhood = Capitalize(source.Neighborhood),
            City = Capitalize(source.City),
            School = Capitalize(source.School),
            MotherName = Capitalize(source.MotherName),
            FatherName = Capitalize(source.FatherName),
            Phone = Clean(source.Phone),
            RegistrationDate = Clean(source.RegistrationDate),
            GuardianName = Capitalize(source.GuardianName),
            GuardianRg = Clean(source.GuardianRg),
            GuardianCpf = Clean(source.GuardianCpf),
            EmergencyContactName = Capitalize(source.EmergencyContactName),
            EmergencyContactPhone = Clean(source.EmergencyContactPhone),
            HealthCenter = Clean(source.HealthCenter),
            FileNumber = Clean(source.FileNumber),
            Allergy = Clean(source.Allergy),

            // "?? []": se a tela mandar a lista nula, vira lista vazia.
            // ".Select(...)": aplica a função em cada familiar e ".ToList()" junta tudo numa lista nova.
            FamilyMembers = (source.FamilyMembers ?? []).Select(NormalizeMember).ToList(),
        };
    }

    private static FamilyMemberDto NormalizeMember(FamilyMemberDto source)
    {
        return new FamilyMemberDto
        {
            Id = source.Id,
            RelationshipTypeId = source.RelationshipTypeId,
            OtherRelationship = Capitalize(source.OtherRelationship),
            Name = Capitalize(source.Name),
            Age = Clean(source.Age),
        };
    }

    /// <summary>Removes spaces at the start and the end. Null becomes an empty text.</summary>
    // "?.": só chama Trim se o texto não for nulo. "?? string.Empty": se for nulo, usa texto vazio.
    private static string Clean(string? text) => text?.Trim() ?? string.Empty;

    /// <summary>Trim and then capital letter at the start of each word.</summary>
    private static string Capitalize(string? text) => TextFormatter.CapitalizeWords(Clean(text));
}
