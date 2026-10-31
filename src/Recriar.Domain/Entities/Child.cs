namespace Recriar.Domain.Entities;

/// <summary>
/// A child assisted by the NGO. Main entity of the system.
/// Translated from the C struct "Cadastro".
/// </summary>
public class Child
{
    public int Id { get; set; }

    // ---- Personal data ----

    /// <summary>Full name. (C: nome)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Age in years, from 0 to 120. (C: idade)</summary>
    public int Age { get; set; }

    /// <summary>CPF document number, digits only. (C: cpf)</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>RG document number, digits only. (C: rg)</summary>
    public string Rg { get; set; } = string.Empty;

    /// <summary>Street and number. (C: endereco)</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Neighborhood. (C: bairro)</summary>
    public string Neighborhood { get; set; } = string.Empty;

    /// <summary>City. (C: cidade)</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>School where the child studies. (C: escola)</summary>
    public string School { get; set; } = string.Empty;

    /// <summary>Mother's full name. (C: nomeMae)</summary>
    public string MotherName { get; set; } = string.Empty;

    /// <summary>Father's full name. (C: nomePai)</summary>
    public string FatherName { get; set; } = string.Empty;

    /// <summary>Phone number, digits only. (C: telefone)</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Date when the child was registered. (C: dataCadastro)</summary>
    public DateTime RegistrationDate { get; set; }

    // ---- Other information ----

    /// <summary>Health center that attends the child. (C: centroSaude)</summary>
    public string HealthCenter { get; set; } = string.Empty;

    /// <summary>Number of the child's file at the health center. (C: ficha)</summary>
    public string FileNumber { get; set; } = string.Empty;

    /// <summary>Allergies of the child. (C: alergia)</summary>
    public string Allergy { get; set; } = string.Empty;

    // ---- Relationships with other entities ----

    /// <summary>Legal guardian of the child. (C: nomeResponsavel, rgResponsavel, cpfResponsavel)</summary>
    public Guardian? Guardian { get; set; }

    /// <summary>Emergency contact. (C: contatoEmergencia, telefoneEmergencia)</summary>
    public EmergencyContact? EmergencyContact { get; set; }

    /// <summary>Family composition, up to 10 people. (C: familia[] and totalFamilia)</summary>
    public List<FamilyMember> FamilyMembers { get; set; } = [];
}
