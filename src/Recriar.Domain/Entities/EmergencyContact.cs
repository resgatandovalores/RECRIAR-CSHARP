namespace Recriar.Domain.Entities;

/// <summary>
/// Person to be called in case of emergency.
/// Translated from the C fields contatoEmergencia and telefoneEmergencia.
/// </summary>
public class EmergencyContact
{
    public int Id { get; set; }

    /// <summary>Full name of the contact. (C: contatoEmergencia)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Phone number, digits only. (C: telefoneEmergencia)</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Id of the child this contact belongs to.</summary>
    public int ChildId { get; set; }
}
