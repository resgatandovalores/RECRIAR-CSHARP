using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Domain.Entities;

namespace Recriar.Application.Validators;

/// <summary>
/// Checks every field of a child registration and returns ALL the problems found at once.
/// Uses the same rules as the C code (somenteLetras, somenteNumeros, lerIdade, validarData, MAX_FAMILIA),
/// plus the maximum sizes of the C struct.
/// </summary>
public static class ChildValidator
{
    /// <summary>Maximum number of family members per child. (C: #define MAX_FAMILIA 10)</summary>
    public const int MaxFamilyMembers = 10;

    /// <summary>
    /// Returns the list of error messages. An empty list means the registration is valid.
    /// The data must already be normalized (texts trimmed and formatted by ChildNormalizer).
    /// </summary>
    /// <param name="child">The registration typed on the screen, already normalized.</param>
    /// <param name="relationshipTypeIds">Ids of the kinship options that exist in the database.</param>
    public static List<string> Validate(ChildDto child, IReadOnlyCollection<int> relationshipTypeIds)
    {
        // Em vez de parar no primeiro erro (como o C fazia com o "do while"), juntamos todos numa lista.
        var errors = new List<string>();

        // ---- Dados da criança ----
        CheckLetters(errors, child.Name, FieldNames.Name, FieldLimits.PersonName);
        CheckAge(errors, child.Age, FieldNames.Age);
        CheckDigits(errors, child.Cpf, FieldNames.Cpf, FieldLimits.Document);
        CheckDigits(errors, child.Rg, FieldNames.Rg, FieldLimits.Document);
        CheckOptional(errors, child.Address, FieldNames.Address, FieldLimits.Address);
        CheckLetters(errors, child.Neighborhood, FieldNames.Neighborhood, FieldLimits.Neighborhood);
        CheckLetters(errors, child.City, FieldNames.City, FieldLimits.City);
        CheckOptional(errors, child.School, FieldNames.School, FieldLimits.School);
        CheckLetters(errors, child.MotherName, FieldNames.MotherName, FieldLimits.PersonName);
        CheckLetters(errors, child.FatherName, FieldNames.FatherName, FieldLimits.PersonName);
        CheckDigits(errors, child.Phone, FieldNames.Phone, FieldLimits.Phone);
        CheckDate(errors, child.RegistrationDate);

        // ---- Responsável ----
        CheckLetters(errors, child.GuardianName, FieldNames.GuardianName, FieldLimits.PersonName);
        CheckDigits(errors, child.GuardianRg, FieldNames.GuardianRg, FieldLimits.Document);
        CheckDigits(errors, child.GuardianCpf, FieldNames.GuardianCpf, FieldLimits.Document);

        // ---- Contato de emergência ----
        CheckLetters(errors, child.EmergencyContactName, FieldNames.EmergencyContactName, FieldLimits.PersonName);
        CheckDigits(errors, child.EmergencyContactPhone, FieldNames.EmergencyContactPhone, FieldLimits.Phone);

        // ---- Outras informações (livres no C, só o tamanho é conferido) ----
        CheckOptional(errors, child.HealthCenter, FieldNames.HealthCenter, FieldLimits.HealthCenter);
        CheckOptional(errors, child.FileNumber, FieldNames.FileNumber, FieldLimits.FileNumber);
        CheckOptional(errors, child.Allergy, FieldNames.Allergy, FieldLimits.Allergy);

        // ---- Composição familiar ----
        List<FamilyMemberDto> members = child.FamilyMembers ?? [];

        // No C: while(... && novo.totalFamilia < MAX_FAMILIA)
        if (members.Count > MaxFamilyMembers)
        {
            errors.Add(ErrorMessages.TooManyFamilyMembers);
        }

        for (int i = 0; i < members.Count; i++)
        {
            CheckFamilyMember(errors, members[i], number: i + 1, relationshipTypeIds);
        }

        return errors;
    }

    private static void CheckFamilyMember(
        List<string> errors, FamilyMemberDto member, int number, IReadOnlyCollection<int> relationshipTypeIds)
    {
        if (!relationshipTypeIds.Contains(member.RelationshipTypeId))
        {
            errors.Add(ErrorMessages.InvalidRelationshipType(FieldNames.FamilyMemberRelationship(number)));
        }
        else if (member.RelationshipTypeId == RelationshipType.OtherId)
        {
            // "Outro": o parentesco real é digitado, com as mesmas regras do parentesco livre do C.
            string field = FieldNames.FamilyMemberOtherRelationship(number);

            if (member.OtherRelationship.Length == 0)
            {
                errors.Add(ErrorMessages.OtherRelationshipRequired(field));
            }
            else
            {
                CheckLetters(errors, member.OtherRelationship, field, FieldLimits.Relationship);
            }
        }

        CheckLetters(errors, member.Name, FieldNames.FamilyMemberName(number), FieldLimits.PersonName);
        CheckAge(errors, member.Age, FieldNames.FamilyMemberAge(number));
    }

    // ---- Regras de cada tipo de campo ----
    // Cada uma acrescenta no máximo UMA mensagem por campo, na ordem: obrigatório, tamanho, conteúdo.

    /// <summary>Required, only letters, spaces and hyphens. (C: somenteLetras)</summary>
    private static void CheckLetters(List<string> errors, string value, string field, int maxLength)
    {
        if (value.Length == 0)
        {
            errors.Add(ErrorMessages.FieldRequired(field));
        }
        else if (value.Length > maxLength)
        {
            errors.Add(ErrorMessages.FieldTooLong(field, maxLength));
        }
        else if (!TextValidator.HasOnlyLetters(value))
        {
            errors.Add(ErrorMessages.OnlyLetters(field));
        }
    }

    /// <summary>Required, only digits. (C: somenteNumeros)</summary>
    private static void CheckDigits(List<string> errors, string value, string field, int maxLength)
    {
        if (value.Length == 0)
        {
            errors.Add(ErrorMessages.FieldRequired(field));
        }
        else if (value.Length > maxLength)
        {
            errors.Add(ErrorMessages.FieldTooLong(field, maxLength));
        }
        else if (!TextValidator.HasOnlyDigits(value))
        {
            errors.Add(ErrorMessages.OnlyDigits(field));
        }
    }

    /// <summary>Free text that can be empty; only the size is checked. (C: fields read with fgets and no check)</summary>
    private static void CheckOptional(List<string> errors, string value, string field, int maxLength)
    {
        if (value.Length > maxLength)
        {
            errors.Add(ErrorMessages.FieldTooLong(field, maxLength));
        }
    }

    /// <summary>Required number from 0 to 120. (C: lerIdade)</summary>
    private static void CheckAge(List<string> errors, string value, string field)
    {
        if (value.Length == 0)
        {
            errors.Add(ErrorMessages.FieldRequired(field));
        }
        else if (!AgeValidator.TryParseAge(value, out _))
        {
            // "out _": descarta o valor convertido, aqui só interessa saber se é válido.
            errors.Add(ErrorMessages.InvalidAge(field));
        }
    }

    /// <summary>Required date in dd/mm/yyyy or dd.mm.yyyy. (C: validarData)</summary>
    private static void CheckDate(List<string> errors, string value)
    {
        if (value.Length == 0)
        {
            errors.Add(ErrorMessages.FieldRequired(FieldNames.RegistrationDate));
        }
        else if (!DateValidator.TryParseDate(value, out _))
        {
            errors.Add(ErrorMessages.InvalidRegistrationDate);
        }
    }
}
