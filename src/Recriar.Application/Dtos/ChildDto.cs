namespace Recriar.Application.Dtos;

/// <summary>
/// All the data of a child exactly as typed on the screen (everything is text).
/// The same class is used to register, to edit and to open a child on the edit screen.
/// The service checks and converts it into the entities of the Domain.
/// </summary>
// DTO (Data Transfer Object): "pacote de dados" que viaja da tela até o serviço, sem regra nenhuma.
// Tudo é texto porque o TextBox do Windows Forms entrega texto; quem converte e valida é o serviço.
public class ChildDto
{
    /// <summary>Id of the child. Zero when the child was not saved yet (new registration).</summary>
    public int Id { get; set; }

    // ---- Personal data ----

    /// <summary>Full name. (C: nome)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Age as text, from 0 to 120. (C: idade)</summary>
    public string Age { get; set; } = string.Empty;

    /// <summary>CPF, digits only. (C: cpf)</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>RG, digits only. (C: rg)</summary>
    public string Rg { get; set; } = string.Empty;

    /// <summary>Street and number. (C: endereco)</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Neighborhood. (C: bairro)</summary>
    public string Neighborhood { get; set; } = string.Empty;

    /// <summary>City. (C: cidade)</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>School. (C: escola)</summary>
    public string School { get; set; } = string.Empty;

    /// <summary>Mother's full name. (C: nomeMae)</summary>
    public string MotherName { get; set; } = string.Empty;

    /// <summary>Father's full name. (C: nomePai)</summary>
    public string FatherName { get; set; } = string.Empty;

    /// <summary>Phone, digits only. (C: telefone)</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Registration date as text, for example "31/10/2026" or "31.10.2026". (C: dataCadastro)</summary>
    public string RegistrationDate { get; set; } = string.Empty;

    // ---- Guardian ----

    /// <summary>Guardian's full name. (C: nomeResponsavel)</summary>
    public string GuardianName { get; set; } = string.Empty;

    /// <summary>Guardian's RG, digits only. (C: rgResponsavel)</summary>
    public string GuardianRg { get; set; } = string.Empty;

    /// <summary>Guardian's CPF, digits only. (C: cpfResponsavel)</summary>
    public string GuardianCpf { get; set; } = string.Empty;

    // ---- Emergency contact ----

    /// <summary>Emergency contact's full name. (C: contatoEmergencia)</summary>
    public string EmergencyContactName { get; set; } = string.Empty;

    /// <summary>Emergency contact's phone, digits only. (C: telefoneEmergencia)</summary>
    public string EmergencyContactPhone { get; set; } = string.Empty;

    // ---- Other information ----

    /// <summary>Health center. (C: centroSaude)</summary>
    public string HealthCenter { get; set; } = string.Empty;

    /// <summary>File number at the health center. (C: ficha)</summary>
    public string FileNumber { get; set; } = string.Empty;

    /// <summary>Allergies. (C: alergia)</summary>
    public string Allergy { get; set; } = string.Empty;

    // ---- Family composition ----

    /// <summary>Family members, up to 10. (C: familia[] and totalFamilia)</summary>
    public List<FamilyMemberDto> FamilyMembers { get; set; } = [];
}
