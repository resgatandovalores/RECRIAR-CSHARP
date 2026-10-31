namespace Recriar.Domain.Entities;

/// <summary>
/// A person who belongs to the family of a child (mother, grandmother, sibling...).
/// Translated from the C struct "Familiar".
/// </summary>
// "public": outros projetos podem usar esta classe.
public class FamilyMember
{
    // Número único do familiar. O banco gera sozinho (no C, a posição no array fazia isso).
    public int Id { get; set; }

    /// <summary>Id of the kinship chosen from the list (RelationshipType). (C: parentesco)</summary>
    public int RelationshipTypeId { get; set; }

    /// <summary>The kinship itself, for example "Avó".</summary>
    public RelationshipType? RelationshipType { get; set; }

    /// <summary>Free text used only when the kinship is "Outro". Null in the other cases.</summary>
    // "string?": o ? permite null, ou seja, o campo pode ficar sem valor.
    public string? OtherRelationship { get; set; }

    /// <summary>Full name of the family member. (C: nome)</summary>
    // "{ get; set; }": propriedade com leitura (get) e escrita (set).
    // "= string.Empty": valor inicial vazio, pois com Nullable ativado a string não pode ser null.
    public string Name { get; set; } = string.Empty;

    /// <summary>Age in years. (C: idade)</summary>
    public int Age { get; set; }

    /// <summary>Id of the child this family member belongs to.</summary>
    public int ChildId { get; set; }

    /// <summary>The child this family member belongs to.</summary>
    public Child? Child { get; set; }
}
