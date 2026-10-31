namespace Recriar.Domain.Entities;

/// <summary>
/// Kinds of kinship a family member can have with the child (lookup table).
/// The list is filled by a seed in the database, so the screen can show it as options.
/// In the C code the kinship was free text ("parentesco").
/// </summary>
/// <remarks>
/// Initial values (the Ids must be fixed so the seed always produces the same rows):
/// 1 Mãe, 2 Pai, 3 Avó, 4 Avô, 5 Irmã, 6 Irmão, 7 Tia, 8 Tio, 9 Outro.
/// When "Outro" is chosen, the real kinship is written in FamilyMember.OtherRelationship.
/// </remarks>
public class RelationshipType
{
    /// <summary>
    /// Fixed Id of the option "Outro" in the seed. When it is chosen, FamilyMember.OtherRelationship is required.
    /// </summary>
    // O seed do banco precisa usar este mesmo número para o "Outro".
    public const int OtherId = 9;

    public int Id { get; set; }

    /// <summary>Name shown to the user. Example: "Avó". (C: parentesco)</summary>
    public string Name { get; set; } = string.Empty;
}


//eh uma tabela de árentesco com id e name 