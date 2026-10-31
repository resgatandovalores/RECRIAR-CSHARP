using Recriar.Application.Dtos;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;

namespace Recriar.Application.Mappers;

/// <summary>
/// Converts between what the screen uses (DTOs, everything as text) and what the database stores (entities).
/// The DTO must already be normalized and validated before being converted into an entity.
/// </summary>
// "Mapper" = quem copia os campos de um tipo de objeto para outro.
public static class ChildMapper
{
    // Formato usado para mostrar a data na tela de edição.
    private const string DateFormat = "dd/MM/yyyy";

    // ==================== Tela -> banco ====================

    /// <summary>Creates a brand new Child (for registration) from a validated DTO.</summary>
    public static Child ToNewEntity(ChildDto dto)
    {
        var child = new Child();
        ApplyTo(child, dto);
        return child;
    }

    /// <summary>
    /// Copies every field of the validated DTO into an existing Child (for editing).
    /// Family members with a known Id are updated, the ones with Id 0 are added
    /// and the ones that are no longer in the DTO are removed from the list.
    /// </summary>
    public static void ApplyTo(Child child, ChildDto dto)
    {
        child.Name = dto.Name;
        child.Age = ToAge(dto.Age);
        child.Cpf = dto.Cpf;
        child.Rg = dto.Rg;
        child.Address = dto.Address;
        child.Neighborhood = dto.Neighborhood;
        child.City = dto.City;
        child.School = dto.School;
        child.MotherName = dto.MotherName;
        child.FatherName = dto.FatherName;
        child.Phone = dto.Phone;
        child.RegistrationDate = ToDate(dto.RegistrationDate);
        child.HealthCenter = dto.HealthCenter;
        child.FileNumber = dto.FileNumber;
        child.Allergy = dto.Allergy;

        // "??=": se ainda não existir responsável (cadastro novo), cria um. Se existir, reaproveita o mesmo.
        child.Guardian ??= new Guardian();
        child.Guardian.Name = dto.GuardianName;
        child.Guardian.Rg = dto.GuardianRg;
        child.Guardian.Cpf = dto.GuardianCpf;

        child.EmergencyContact ??= new EmergencyContact();
        child.EmergencyContact.Name = dto.EmergencyContactName;
        child.EmergencyContact.Phone = dto.EmergencyContactPhone;

        ApplyFamilyMembers(child, dto.FamilyMembers);
    }

    private static void ApplyFamilyMembers(Child child, List<FamilyMemberDto> members)
    {
        // Ids dos familiares que continuam na ficha. Os que não estiverem aqui saem da lista.
        var keptIds = new HashSet<int>();

        foreach (FamilyMemberDto memberDto in members)
        {
            // Procura o familiar que já existe com esse Id. "FirstOrDefault" devolve null se não achar.
            FamilyMember? member = memberDto.Id > 0
                ? child.FamilyMembers.FirstOrDefault(m => m.Id == memberDto.Id)
                : null;

            if (member is null)
            {
                // Familiar novo (Id 0, ou um Id que não pertence a esta criança).
                member = new FamilyMember();
                child.FamilyMembers.Add(member);
            }
            else
            {
                keptIds.Add(member.Id);
            }

            member.RelationshipTypeId = memberDto.RelationshipTypeId;

            // O texto livre só é guardado quando o parentesco é "Outro"; nos outros casos fica nulo.
            member.OtherRelationship = memberDto.RelationshipTypeId == RelationshipType.OtherId
                ? memberDto.OtherRelationship
                : null;

            member.Name = memberDto.Name;
            member.Age = ToAge(memberDto.Age);
        }

        // Remove os familiares antigos que a tela não mandou mais. Os novos têm Id 0 e não são removidos.
        child.FamilyMembers.RemoveAll(m => m.Id > 0 && !keptIds.Contains(m.Id));
    }

    // ==================== Banco -> tela ====================

    /// <summary>Converts a Child into the DTO used to fill the edit screen.</summary>
    public static ChildDto ToDto(Child child)
    {
        return new ChildDto
        {
            Id = child.Id,
            Name = child.Name,
            Age = child.Age.ToString(),
            Cpf = child.Cpf,
            Rg = child.Rg,
            Address = child.Address,
            Neighborhood = child.Neighborhood,
            City = child.City,
            School = child.School,
            MotherName = child.MotherName,
            FatherName = child.FatherName,
            Phone = child.Phone,
            RegistrationDate = child.RegistrationDate.ToString(DateFormat),

            // "?.": se não houver responsável, o resultado é nulo, e "??" troca por texto vazio.
            GuardianName = child.Guardian?.Name ?? string.Empty,
            GuardianRg = child.Guardian?.Rg ?? string.Empty,
            GuardianCpf = child.Guardian?.Cpf ?? string.Empty,
            EmergencyContactName = child.EmergencyContact?.Name ?? string.Empty,
            EmergencyContactPhone = child.EmergencyContact?.Phone ?? string.Empty,

            HealthCenter = child.HealthCenter,
            FileNumber = child.FileNumber,
            Allergy = child.Allergy,
            FamilyMembers = child.FamilyMembers.Select(ToDto).ToList(),
        };
    }

    /// <summary>Converts a FamilyMember into its DTO.</summary>
    public static FamilyMemberDto ToDto(FamilyMember member)
    {
        return new FamilyMemberDto
        {
            Id = member.Id,
            RelationshipTypeId = member.RelationshipTypeId,
            OtherRelationship = member.OtherRelationship ?? string.Empty,
            Name = member.Name,
            Age = member.Age.ToString(),
        };
    }

    /// <summary>Converts a Child into the short version used in lists and search results.</summary>
    public static ChildSummaryDto ToSummary(Child child)
    {
        return new ChildSummaryDto
        {
            Id = child.Id,
            Name = child.Name,
            Cpf = child.Cpf,
            Phone = child.Phone,
        };
    }

    // ==================== Conversões de texto ====================
    // O DTO já foi validado, então a conversão sempre funciona. Se não funcionar, é erro de programação
    // (alguém chamou o mapper sem validar antes), e o erro é lançado para aparecer no log.

    private static int ToAge(string text) =>
        AgeValidator.TryParseAge(text, out int age)
            ? age
            : throw new InvalidOperationException($"Idade não validada antes da conversão: '{text}'.");

    private static DateTime ToDate(string text) =>
        DateValidator.TryParseDate(text, out DateTime date)
            ? date
            : throw new InvalidOperationException($"Data não validada antes da conversão: '{text}'.");
}
