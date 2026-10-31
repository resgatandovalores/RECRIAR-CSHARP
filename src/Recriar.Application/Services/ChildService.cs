using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Application.Helpers;
using Recriar.Application.Interfaces;
using Recriar.Application.Mappers;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;

namespace Recriar.Application.Services;

/// <summary>
/// The actions the screens call: register, edit, delete, list and search children.
/// Translated from the C functions cadastrarPessoa, editarCadastro, excluirCadastro, listarCadastros and pesquisarPorNome.
/// </summary>
/// <remarks>
/// Every action follows the same path: clean the data (Trim + capital letters), check every field,
/// convert to entities, save through the repository and record the activity.
/// Any unexpected error is saved in the error log and turned into the generic "call support" message.
/// </remarks>
public class ChildService : IChildService
{
    // "readonly": só pode receber valor no construtor. Depois disso, nunca muda.
    private readonly IChildRepository _children;
    private readonly IRelationshipTypeRepository _relationshipTypes;
    private readonly IActivityLogger _activityLogger;
    private readonly IErrorLogger _errorLogger;
    private readonly IChildFileExporter _fileExporter;

    /// <summary>
    /// Creates the service. It receives the contracts, not the real classes, so it does not know
    /// (and does not care) whether the data is in MySQL, in memory or anywhere else.
    /// </summary>
    // Isso se chama "injeção de dependência": quem cria o serviço entrega as peças que ele usa.
    public ChildService(
        IChildRepository children,
        IRelationshipTypeRepository relationshipTypes,
        IActivityLogger activityLogger,
        IErrorLogger errorLogger,
        IChildFileExporter fileExporter)
    {
        _children = children;
        _relationshipTypes = relationshipTypes;
        _activityLogger = activityLogger;
        _errorLogger = errorLogger;
        _fileExporter = fileExporter;
    }

    /// <inheritdoc />
    public int Register(ChildDto child)
    {
        return Run("Cadastro de criança", () =>
        {
            ChildDto data = PrepareAndValidate(child);

            Child entity = ChildMapper.ToNewEntity(data);
            _children.Add(entity);

            LogActivity("Cadastro criado", entity);
            return entity.Id;
        });
    }

    /// <inheritdoc />
    public void Update(int id, ChildDto child)
    {
        Run("Edição de criança", () =>
        {
            // No C: procura pelo ID e, se não achar, "Cadastro nao encontrado!".
            Child entity = _children.GetById(id) ?? throw new BusinessException(ErrorMessages.ChildNotFound);

            ChildDto data = PrepareAndValidate(child);

            ChildMapper.ApplyTo(entity, data);
            _children.Update(entity);

            LogActivity("Cadastro editado", entity);
        });
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        Run("Exclusão de criança", () =>
        {
            // Busca antes para ter o nome no log de atividades.
            Child entity = _children.GetById(id) ?? throw new BusinessException(ErrorMessages.ChildNotFound);

            if (!_children.Delete(id))
            {
                throw new BusinessException(ErrorMessages.ChildNotFound);
            }

            LogActivity("Cadastro excluído", entity);
        });
    }

    /// <inheritdoc />
    public List<ChildSummaryDto> GetAll()
    {
        return Run("Listagem de crianças", () =>
            _children.GetAll().Select(ChildMapper.ToSummary).ToList());
    }

    /// <inheritdoc />
    public List<ChildSummaryDto> SearchByName(string name)
    {
        return Run("Pesquisa por nome", () =>
        {
            string text = name?.Trim() ?? string.Empty;

            // No C, strstr com texto vazio encontra todos. Mantemos: busca vazia lista todo mundo.
            List<Child> found = text.Length == 0 ? _children.GetAll() : _children.SearchByName(text);

            return found.Select(ChildMapper.ToSummary).ToList();
        });
    }

    /// <inheritdoc />
    public ChildDto GetById(int id)
    {
        return Run("Consulta de criança", () =>
        {
            Child entity = _children.GetById(id) ?? throw new BusinessException(ErrorMessages.ChildNotFound);
            return ChildMapper.ToDto(entity);
        });
    }

    /// <inheritdoc />
    public string ExportFile(int id)
    {
        return Run("Exportação de ficha", () =>
        {
            Child entity = _children.GetById(id) ?? throw new BusinessException(ErrorMessages.ChildNotFound);

            // Nomes dos parentescos por Id. Só consulta o banco se a criança tiver familiares.
            // ToDictionary(chave, valor): monta um "dicionário" para achar o nome pelo Id rapidamente.
            IReadOnlyDictionary<int, string> relationshipNames = entity.FamilyMembers.Count > 0
                ? _relationshipTypes.GetAll().ToDictionary(r => r.Id, r => r.Name)
                : new Dictionary<int, string>();

            string path;
            try
            {
                path = _fileExporter.Export(entity, relationshipNames);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // "when": este catch só pega erros de arquivo (pasta sem permissão, arquivo aberto em outro programa).
                // Para esses, há uma mensagem própria no catálogo; o detalhe técnico vai para o log.
                _errorLogger.LogError(ex, "Exportação de ficha");
                throw new BusinessException(ErrorMessages.ExportFailed);
            }

            LogActivity("Ficha exportada", entity);
            return path;
        });
    }

    /// <inheritdoc />
    public List<RelationshipType> GetRelationshipTypes()
    {
        return Run("Consulta de parentescos", () => _relationshipTypes.GetAll());
    }

    // ==================== Passos internos ====================

    /// <summary>Cleans the data and checks every field. Throws one BusinessException with all the problems.</summary>
    private ChildDto PrepareAndValidate(ChildDto? child)
    {
        // Sem dados nenhum é erro de programação na tela (deveria sempre mandar um ChildDto).
        ArgumentNullException.ThrowIfNull(child);

        ChildDto data = ChildNormalizer.Normalize(child);

        // Só consulta os parentescos no banco quando há familiares para conferir.
        IReadOnlyCollection<int> relationshipIds = data.FamilyMembers.Count > 0
            ? _relationshipTypes.GetAll().Select(r => r.Id).ToHashSet()
            : [];

        List<string> errors = ChildValidator.Validate(data, relationshipIds);

        if (errors.Count > 0)
        {
            throw new BusinessException(errors);
        }

        return data;
    }

    /// <summary>
    /// Records the activity. If the activity log fails, the action is NOT undone (the data is already saved):
    /// the failure goes to the error log and the user still sees success.
    /// </summary>
    private void LogActivity(string action, Child child)
    {
        try
        {
            _activityLogger.Log(action, $"Criança {child.Id} - {child.Name}");
        }
        catch (Exception ex)
        {
            _errorLogger.LogError(ex, $"Log de atividades ({action})");
        }
    }

    /// <summary>
    /// Runs an action with the standard error handling:
    /// known errors (BusinessException) go to the screen as they are;
    /// any other error is saved in the error log and becomes the generic message.
    /// </summary>
    // "Func<T>": uma função guardada numa variável, que devolve um valor do tipo T.
    // "() => { ... }" na chamada é a função sendo escrita ali mesmo (expressão lambda).
    private T Run<T>(string context, Func<T> action)
    {
        try
        {
            return action();
        }
        catch (BusinessException)
        {
            // "throw;" sozinho relança o mesmo erro, sem perder de onde ele veio.
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.LogError(ex, context);
            throw new BusinessException(ErrorMessages.Unexpected);
        }
    }

    /// <summary>Same as Run&lt;T&gt;, for actions that do not return a value.</summary>
    // "Action": uma função guardada numa variável, que não devolve nada (void).
    private void Run(string context, Action action)
    {
        Run(context, () =>
        {
            action();
            return true;
        });
    }
}
