namespace Recriar.Application.Dtos;

/// <summary>
/// Data of one family member exactly as typed on the screen.
/// Used inside <see cref="ChildDto"/>. Translated from the C struct "Familiar".
/// </summary>
public class FamilyMemberDto
{
    /// <summary>Id of the chosen kinship (RelationshipType). It comes from the list shown to the user.</summary>
    public int RelationshipTypeId { get; set; }

    /// <summary>Kinship written by the user. Only used when the chosen kinship is "Outro".</summary>
    public string OtherRelationship { get; set; } = string.Empty;

    /// <summary>Full name. (C: nome)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Age as text, because a TextBox gives text. (C: idade)</summary>
    public string Age { get; set; } = string.Empty;
}
