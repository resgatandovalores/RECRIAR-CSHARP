using Recriar.Domain.Entities;

namespace Recriar.Application.Interfaces;

/// <summary>
/// Contract for saving the file (ficha) of one child, ready to open and print.
/// Translated from the C function "salvarTXTIndividual". Today the file is HTML; another format
/// (PDF, TXT) only needs a new class that implements this same contract.
/// </summary>
public interface IChildFileExporter
{
    /// <summary>
    /// Writes the file of the child, replacing an older file of the same child if it exists.
    /// </summary>
    /// <param name="child">The child with guardian, emergency contact and family members.</param>
    /// <param name="relationshipNames">Kinship names by Id (1 = "Mãe"...), to write the family members.</param>
    /// <returns>Full path of the saved file, so the screen can open it.</returns>
    string Export(Child child, IReadOnlyDictionary<int, string> relationshipNames);
}
