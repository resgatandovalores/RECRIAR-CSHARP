namespace Recriar.Domain.Entities;

/// <summary>
/// A person who belongs to the family of a child (mother, grandmother, sibling...).
/// Translated from the C struct "Familiar".
/// </summary>
/// 
/// outro projto pd pegar essa class
public class FamilyMember
{
    public int Id { get; set; }

    /// <summary>Kinship with the child. Example: "Avó", "Irmão". (C: parentesco)</summary>
    //campo de leitura get e escrita set
    public string Relationship { get; set; } = string.Empty; //valor inicial com u mtexto vazio, o nullable enabled exige
    //uma string nunca fique null

    /// <summary>Full name of the family member. (C: nome)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Age in years. (C: idade)</summary>
    public int Age { get; set; }

    /// <summary>Id of the child this family member belongs to.</summary>
    public int ChildId { get; set; }

    //numero unico d cada familia no banco, eo banco gera sozinho
    //no c o array fazia sozinho
}
