using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Entities;
using Recriar.Infrastructure.Persistence.Repositories;

string value = Environment.GetEnvironmentVariable("RECRIAR_TEST_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Configure RECRIAR_TEST_CONNECTION_STRING para um servidor MySQL local de testes.");
var connection = new MySqlConnectionStringBuilder(value);
if (!(connection.Server == "127.0.0.1" || connection.Server == "localhost" || connection.Server == "::1"))
    throw new InvalidOperationException("Os testes aceitam somente servidor local.");
// The runner owns only this newly generated schema, never the caller's database.
string database = "recriar_test_" + Guid.NewGuid().ToString("N");
connection.Database = database;
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseMySql(connection.ConnectionString, new MySqlServerVersion(new Version(8, 0, 16))).Options;
AppDbContext Create() => new(options);
int passed = 0;
void Check(bool ok, string label)
{
    if (!ok) throw new Exception("FALHOU: " + label);
    Console.WriteLine("OK: " + label); passed++;
}
void Reject(Action action, string label)
{
    bool rejected = false;
    try { action(); } catch (DbUpdateException) { rejected = true; }
    Check(rejected, label);
}

using var context = Create();
try
{
    context.Database.Migrate();
    int tableCount = context.Database.SqlQueryRaw<int>(
        "SELECT COUNT(*) AS Value FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name <> '__EFMigrationsHistory'").Single();
    Check(tableCount == 11, "11 tabelas de negocio, alem do historico de migrations");
    var types = new RelationshipTypeRepository(context).GetAll();
    Check(types.Count == 9 && types[8].Id == RelationshipType.OtherId && types[8].Name == "Outro", "seed com nove parentescos e Outro no Id 9");

    var repo = new ChildRepository(context);
    var errors = new CapturingErrorLogger();
    var service = new ChildService(repo, new RelationshipTypeRepository(context), new DbActivityLogger(options), errors, new StubExporter());
    var dto = Sample();
    int id = service.Register(dto);
    Check(id > 0, "cadastro pelo servico retorna Id gerado pelo MySQL");
    using (var fresh = Create())
    {
        var saved = new ChildRepository(fresh).GetById(id)!;
        Check(saved.Guardian is not null && saved.EmergencyContact is not null && saved.FamilyMembers.Count == 2
            && saved.FamilyMembers.All(x => x.RelationshipType is not null), "leitura inclui todo o agregado e parentescos");
        Check(saved.Cpf.StartsWith('0') && saved.RegistrationDate.TimeOfDay == TimeSpan.Zero, "documentos preservam zeros e cadastro armazena data");
    }
    var edit = service.GetById(id);
    int removed = edit.FamilyMembers[0].Id;
    edit.FamilyMembers.RemoveAt(0);
    edit.FamilyMembers[0].Name = "Familiar Alterado";
    edit.FamilyMembers[0].RelationshipTypeId = 8;
    edit.FamilyMembers.Add(new FamilyMemberDto { Name = "Familiar Novo", Age = "20", RelationshipTypeId = 9, OtherRelationship = "Primo" });
    edit.GuardianName = "Responsavel Alterado";
    edit.EmergencyContactPhone = "0012345";
    service.Update(id, edit);
    using (var fresh = Create())
    {
        var saved = new ChildRepository(fresh).GetById(id)!;
        Check(!fresh.FamilyMembers.Any(x => x.Id == removed) && saved.FamilyMembers.Count == 2, "edicao exclui familiar removido e inclui familiar novo");
        Check(saved.Guardian!.Name == "Responsavel Alterado" && saved.EmergencyContact!.Phone == "0012345"
            && saved.FamilyMembers.Any(x => x.Name == "Familiar Alterado" && x.RelationshipTypeId == 8), "edicao persiste responsavel, contato e familiar existente");
    }
    Check(service.SearchByName("crianca").Any(x => x.Id == id), "busca por trecho ignora maiusculas");
    int secondId = service.Register(Sample());
    Check(secondId > id && repo.GetAll().Select(x => x.Id).SequenceEqual(repo.GetAll().Select(x => x.Id).Order()), "CPF repetido permitido e listagem ordenada por Id");
    Check(service.SearchByName("").Count == 2 && service.SearchByName("%_").Count == 0, "busca vazia lista todos e caracteres SQL sao literais");
    var invalid = Sample();
    invalid.FamilyMembers = Enumerable.Range(0, 11).Select(_ => new FamilyMemberDto { Name = "Familiar Teste", Age = "20", RelationshipTypeId = 1 }).ToList();
    bool invalidRejected = false;
    try { service.Register(invalid); } catch (BusinessException) { invalidRejected = true; }
    Check(invalidRejected && repo.GetAll().Count == 2, "servico rejeita mais de dez familiares sem gravar cadastro");

    using (var fresh = Create())
    {
        fresh.RelationshipTypes.Remove(fresh.RelationshipTypes.Single(x => x.Id == 8));
        Reject(() => fresh.SaveChanges(), "parentesco em uso protegido por RESTRICT");
    }
    using (var fresh = Create())
    {
        fresh.Guardians.Add(new Guardian { ChildId = id, Name = "Duplicado", Cpf = "0", Rg = "0" });
        Reject(() => fresh.SaveChanges(), "indice unico impede dois responsaveis na mesma crianca");
    }
    using (var fresh = Create())
    {
        fresh.FamilyMembers.Add(new FamilyMember { ChildId = int.MaxValue, RelationshipTypeId = 1, Name = "Sem Vinculo", Age = 20 });
        Reject(() => fresh.SaveChanges(), "chave estrangeira impede familiar sem crianca");
    }
    using (var fresh = Create())
    {
        var child = fresh.Children.Single(x => x.Id == id); child.Age = 121;
        Reject(() => fresh.SaveChanges(), "CHECK impede idade fora da faixa original de zero a 120");
    }

    var program = new WorkshopProgram { Name = "Programa Exemplo", Description = "Dados ficticios" };
    context.Programs.Add(program); context.SaveChanges();
    var activity = new WorkshopActivity { ProgramId = program.Id, Name = "Oficina Exemplo" };
    context.Activities.Add(activity); context.SaveChanges();
    var session = new ActivitySession { ActivityId = activity.Id, StartsAt = new DateTime(2026,11,1,9,0,0), EndsAt = new DateTime(2026,11,1,10,0,0), Location = "Sala Exemplo" };
    context.ActivitySessions.Add(session); context.SaveChanges();
    var enrollment = new SessionEnrollment { ChildId = id, SessionId = session.Id, RegisteredAt = DateTime.UtcNow };
    context.SessionEnrollments.Add(enrollment); context.SaveChanges();
    context.AttendanceRecords.Add(new AttendanceRecord { SessionEnrollmentId = enrollment.Id, Attended = true, RecordedAt = DateTime.UtcNow });
    context.SaveChanges();
    Check(context.AttendanceRecords.Count() == 1, "oficina, encontro, inscricao e presenca vinculados");
    using (var fresh = Create())
    {
        fresh.SessionEnrollments.Add(new SessionEnrollment { ChildId = id, SessionId = session.Id, RegisteredAt = DateTime.UtcNow });
        Reject(() => fresh.SaveChanges(), "inscricao duplicada no mesmo encontro bloqueada");
    }
    using (var fresh = Create())
    {
        fresh.AttendanceRecords.Add(new AttendanceRecord { SessionEnrollmentId = enrollment.Id, Attended = false, RecordedAt = DateTime.UtcNow });
        Reject(() => fresh.SaveChanges(), "uma presenca por inscricao");
    }
    using (var fresh = Create())
    {
        fresh.ActivitySessions.Add(new ActivitySession { ActivityId = activity.Id, StartsAt = DateTime.UtcNow, EndsAt = DateTime.UtcNow.AddHours(-1), Location = "Sala" });
        Reject(() => fresh.SaveChanges(), "encontro deve terminar depois do inicio");
    }
    var logger = new DbActivityLogger(options);
    Reject(() => logger.Log(new string('x', 101), "Teste de falha"), "falha de log em contexto independente");
    logger.Log("Teste", "Log seguinte funciona");
    Check(service.Register(Sample()) > 0, "falha de log nao contamina o proximo cadastro");
    var brokenLoggerService = new ChildService(repo, new RelationshipTypeRepository(context), new FailingLogger(), errors, new StubExporter());
    int afterLogFailure = brokenLoggerService.Register(Sample());
    Check(afterLogFailure > 0 && errors.Count > 0, "cadastro permanece salvo quando o log de atividade falha");
    Check(service.ExportFile(id) == "ficha-teste.html", "servico usa exportador e registra exportacao");
    service.Delete(id);
    using (var fresh = Create())
    {
        Check(!fresh.Children.Any(x => x.Id == id) && !fresh.Guardians.Any(x => x.ChildId == id)
            && !fresh.EmergencyContacts.Any(x => x.ChildId == id) && !fresh.FamilyMembers.Any(x => x.ChildId == id)
            && !fresh.SessionEnrollments.Any(x => x.ChildId == id) && !fresh.AttendanceRecords.Any(), "exclusao em cascata remove dependentes e presenca");
        Check(fresh.ActivityLogs.Any(x => x.Action == "Cadastro excluído") && fresh.ActivityLogs.Any(x => x.Action == "Ficha exportada"), "log permanece apos exclusao, com acoes em portugues");
    }
    Check(repo.GetById(id) is null && !repo.Delete(id), "cadastro inexistente retorna null e Delete retorna false");
    // Removing the schema is safe: this name was generated above and is never supplied by the caller.
    Console.WriteLine($"RESULTADO: {passed} verificacoes aprovadas em MySQL real.");
}
finally
{
    context.Database.EnsureDeleted();
}

static ChildDto Sample() => new()
{
    Name = "Crianca Exemplo", Age = "8", Cpf = "00000000000", Rg = "00123",
    Address = "", Neighborhood = "Centro", City = "Campinas", School = "",
    MotherName = "Mae Exemplo", FatherName = "Pai Exemplo", Phone = "0012345678",
    RegistrationDate = "31/10/2026", GuardianName = "Responsavel Exemplo", GuardianCpf = "00000000000",
    GuardianRg = "00123", EmergencyContactName = "Contato Exemplo", EmergencyContactPhone = "0012345678",
    HealthCenter = "", FileNumber = "", Allergy = "",
    FamilyMembers = [new() { Name = "Familiar Exemplo", Age = "20", RelationshipTypeId = 1 },
        new() { Name = "Outro Familiar", Age = "25", RelationshipTypeId = 9, OtherRelationship = "Primo" }]
};

sealed class CapturingErrorLogger : IErrorLogger
{
    public int Count { get; private set; }
    public void LogError(Exception exception, string context) => Count++;
}
sealed class StubExporter : IChildFileExporter
{
    public string Export(Child child, IReadOnlyDictionary<int,string> relationshipNames) => "ficha-teste.html";
}
sealed class FailingLogger : IActivityLogger
{
    public void Log(string action, string description) => throw new IOException("Falha simulada");
}
