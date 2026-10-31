using Recriar.Application.Dtos;
using Recriar.Domain.Entities;

namespace Recriar.Application.Interfaces;

/// <summary>
/// Everything the screens can ask the system to do with children.
/// The screens (Windows Forms) only talk to this contract; the rules and the validations live behind it.
/// </summary>
/// <remarks>
/// How errors work: a method that fails throws a <see cref="Exceptions.BusinessException"/> with the messages
/// from the error catalog. Any unexpected error (database offline, bugs) is saved in the error log and also
/// reaches the screen as a BusinessException with the generic "call support" message.
/// So the screen only needs: try { ... } catch (Exception ex) { MessageBox.Show(ErrorHandler.GetMessage(ex)); }
/// </remarks>
public interface IChildService
{
    /// <summary>
    /// Registers a new child. (C: cadastrarPessoa)
    /// All texts are trimmed (spaces at the start and the end are removed) and names are written with capital
    /// letters (CapitalizeWords) before being checked and saved.
    /// Every wrong field is reported together, in one BusinessException.
    /// The activity "Cadastro criado" is recorded in the activity log.
    /// </summary>
    /// <returns>The Id (file number) generated for the new child.</returns>
    /// <exception cref="Exceptions.BusinessException">Some field is invalid, or there are more than 10 family members.</exception>
    int Register(ChildDto child);

    /// <summary>
    /// Replaces ALL the data of an existing child with the received data. (C: editarCadastro, now for every field)
    /// Uses the same checks and formatting as <see cref="Register"/>. The value of child.Id is ignored; the id parameter is used.
    /// The activity "Cadastro editado" is recorded in the activity log.
    /// </summary>
    /// <exception cref="Exceptions.BusinessException">The child does not exist, or some field is invalid.</exception>
    void Update(int id, ChildDto child);

    /// <summary>
    /// Deletes a child and everything that belongs to it. (C: excluirCadastro)
    /// The activity "Cadastro excluído" is recorded in the activity log.
    /// </summary>
    /// <exception cref="Exceptions.BusinessException">The child does not exist.</exception>
    void Delete(int id);

    /// <summary>Lists every child, ordered by Id. (C: listarCadastros)</summary>
    List<ChildSummaryDto> GetAll();

    /// <summary>
    /// Searches children by part of the name, ignoring upper and lower case. (C: pesquisarPorNome)
    /// An empty text returns everyone, as the C code did.
    /// </summary>
    List<ChildSummaryDto> SearchByName(string name);

    /// <summary>
    /// Returns the complete data of a child, ready to fill the edit screen.
    /// </summary>
    /// <exception cref="Exceptions.BusinessException">The child does not exist.</exception>
    ChildDto GetById(int id);

    /// <summary>
    /// Returns the kinship options (Mãe, Pai, Avó...) to fill a ComboBox.
    /// Use Id as the value and Name as the text. The chosen Id goes to FamilyMemberDto.RelationshipTypeId.
    /// </summary>
    List<RelationshipType> GetRelationshipTypes();
}
