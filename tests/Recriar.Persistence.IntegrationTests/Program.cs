using MySqlConnector;
using Recriar.Application.Dtos;
using Recriar.Application.Exceptions;
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Repositories;
using Recriar.Infrastructure.Export;

string value = Environment.GetEnvironmentVariable("RECRIAR_TEST_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Configure RECRIAR_TEST_CONNECTION_STRING para um MySQL local de testes.");
MySqlConnectionStringBuilder settings = new MySqlConnectionStringBuilder(value);
if (settings.Server != "localhost" && settings.Server != "127.0.0.1" && settings.Server != "::1")
    throw new InvalidOperationException("Use somente um servidor local para os testes.");

string suffix = Guid.NewGuid().ToString("N");
string prod = "recriar_test_p_" + suffix;
string read = "recriar_test_r_" + suffix;
string writer = "rtw_" + suffix[..20];
string upgradeProd = prod + "_old";
string upgradeRead = read + "_old";
string readerUser = "rtr_" + suffix[..20];
string writerPassword = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24));
string readerPassword = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24));
settings.Database = "";
settings.AllowUserVariables = true; // Only the administrator runs demonstration SQL with session variables.
using MySqlConnection admin = new MySqlConnection(settings.ConnectionString);
admin.Open();
int passed = 0;
void Check(bool ok, string label)
{
    if (!ok) throw new Exception("FALHOU: " + label);
    Console.WriteLine("OK: " + label);
    passed++;
}
void Exec(string sql)
{
    using MySqlCommand command = new MySqlCommand(sql, admin);
    command.ExecuteNonQuery();
}
int Count(string table, string condition = "1=1")
{
    using MySqlCommand command = new MySqlCommand($"SELECT COUNT(*) FROM `{prod}`.`{table}` WHERE {condition}", admin);
    return Convert.ToInt32(command.ExecuteScalar());
}
void Reject(Action action, string label)
{
    bool rejected = false;
    try { action(); } catch (MySqlException) { rejected = true; }
    Check(rejected, label);
}
string WriteConnection()
{
    MySqlConnectionStringBuilder s = new MySqlConnectionStringBuilder(settings.ConnectionString);
    s.Database = prod; s.UserID = writer; s.Password = writerPassword;
    s.AllowUserVariables = false;
    return s.ConnectionString;
}
string ReadConnection()
{
    MySqlConnectionStringBuilder s = new MySqlConnectionStringBuilder(settings.ConnectionString);
    s.Database = read; s.UserID = readerUser; s.Password = readerPassword;
    s.AllowUserVariables = false;
    return s.ConnectionString;
}
string FindRoot()
{
    DirectoryInfo? folder = new DirectoryInfo(AppContext.BaseDirectory);
    while (folder != null)
    {
        if (File.Exists(Path.Combine(folder.FullName, "Recriar.sln"))) return folder.FullName;
        folder = folder.Parent;
    }
    throw new DirectoryNotFoundException("Pasta do projeto não encontrada.");
}

try
{
    Exec($"CREATE DATABASE `{prod}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci");
    Exec($"USE `{prod}`");
    Exec(File.ReadAllText(Path.Combine(FindRoot(), "docs/database/schema.sql")));
    string views = File.ReadAllText(Path.Combine(FindRoot(), "docs/database/dois-bancos/02_criar_leitura.sql"))
        .Replace("recriar_prod", prod).Replace("recriar_leitura", read);
    Exec(views);
    using (MySqlCommand create = new MySqlCommand($"CREATE USER '{writer}'@'localhost' IDENTIFIED BY @password", admin))
    {
        create.Parameters.AddWithValue("@password", writerPassword); create.ExecuteNonQuery();
    }
    using (MySqlCommand create = new MySqlCommand($"CREATE USER '{readerUser}'@'localhost' IDENTIFIED BY @password", admin))
    {
        create.Parameters.AddWithValue("@password", readerPassword); create.ExecuteNonQuery();
    }
    Exec($"GRANT SELECT, INSERT, UPDATE, DELETE ON `{prod}`.* TO '{writer}'@'localhost'");
    Exec($"GRANT SELECT ON `{read}`.* TO '{readerUser}'@'localhost'");

    AppDbContext context = new AppDbContext(WriteConnection(), ReadConnection());
    ChildRepository repo = new ChildRepository(context);
    CapturingErrorLogger errors = new CapturingErrorLogger();
    ChildService service = new ChildService(repo, new RelationshipTypeRepository(context), new DbActivityLogger(context), errors, new StubExporter());

    using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @db AND TABLE_TYPE = 'BASE TABLE'", admin))
    {
        command.Parameters.AddWithValue("@db", prod);
        Check(Convert.ToInt32(command.ExecuteScalar()) == 15, "producao tem 15 tabelas");
    }
    using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM information_schema.views WHERE table_schema = @db AND IS_UPDATABLE = 'NO'", admin))
    {
        command.Parameters.AddWithValue("@db", read);
        Check(Convert.ToInt32(command.ExecuteScalar()) == 15, "leitura tem 15 views sem permissao estrutural de atualizacao");
    }
    var types = service.GetRelationshipTypes();
    Check(types.Count == 9 && types[8].Id == 9 && types[8].Name == "Outro", "leitura dos nove parentescos com Outro no Id 9");
    int id = service.Register(Sample());
    Check(id > 0 && Count("Children") == 1, "ExecuteNonQuery salva e ExecuteScalar retorna o Id");
    Child saved = repo.GetById(id)!;
    Check(saved.Guardian != null && saved.EmergencyContact != null && saved.FamilyMembers.Count == 2 && saved.FamilyMembers.All(m => m.RelationshipType != null), "ExecuteReader traz o cadastro completo pelo banco de leitura");
    Check(saved.Cpf.StartsWith('0') && saved.RegistrationDate.TimeOfDay == TimeSpan.Zero, "documentos mantem zeros e cadastro usa data sem horario");
    Check(saved.Guardian!.Id > 0 && saved.EmergencyContact!.Id > 0 && saved.FamilyMembers.All(m => m.Id > 0), "Ids dos registros relacionados preenchidos");

    ChildDto edit = service.GetById(id);
    int removed = edit.FamilyMembers[0].Id;
    int kept = edit.FamilyMembers[1].Id;
    edit.FamilyMembers.RemoveAt(0);
    edit.FamilyMembers[0].Name = "Familiar Alterado";
    edit.FamilyMembers[0].RelationshipTypeId = 8;
    edit.FamilyMembers.Add(new FamilyMemberDto { Name = "Familiar Novo", Age = "20", RelationshipTypeId = 9, OtherRelationship = "Primo" });
    edit.GuardianName = "Responsavel Alterado";
    edit.EmergencyContactPhone = "0012345";
    service.Update(id, edit);
    saved = repo.GetById(id)!;
    Check(Count("FamilyMembers", "Id=" + removed) == 0 && saved.FamilyMembers.Count == 2, "edicao remove familiar antigo e inclui o novo");
    Check(saved.FamilyMembers.Any(m => m.Id == kept && m.Name == "Familiar Alterado" && m.RelationshipTypeId == 8), "edicao preserva o Id do familiar mantido");
    Check(saved.Guardian!.Name == "Responsavel Alterado" && saved.EmergencyContact!.Phone == "0012345", "edicao atualiza responsavel e contato");
    Check(service.SearchByName("crianca").Count == 1, "pesquisa por trecho ignora maiusculas");
    Check(service.SearchByName("%_").Count == 0 && service.SearchByName("' OR 1=1 --").Count == 0, "parametros tratam texto SQL e curingas como texto comum");
    int id2 = service.Register(Sample());
    Check(id2 > id && repo.GetAll()[0].Id == id && repo.GetAll()[1].Id == id2, "CPF repetido permitido e lista ordenada");
    Check(service.SearchByName("").Count == 2, "pesquisa vazia lista todos");
    ChildDto invalid = Sample();
    invalid.FamilyMembers = Enumerable.Range(0,11).Select(_ => new FamilyMemberDto { Name="Familiar Exemplo", Age="20", RelationshipTypeId=1 }).ToList();
    bool rejected = false;
    try { service.Register(invalid); } catch (BusinessException) { rejected = true; }
    Check(rejected && Count("Children") == 2, "servico bloqueia mais de dez familiares");

    Child invalidChild = new Child { Name="Teste", Age=8, Cpf="0", Rg="0", Neighborhood="Centro", City="Campinas", MotherName="Mae", FatherName="Pai", Phone="0", RegistrationDate=DateTime.Today,
        Guardian=new Guardian { Name="Teste",Cpf="0",Rg="0" }, EmergencyContact=new EmergencyContact { Name="Teste",Phone="0" } };
    invalidChild.FamilyMembers.Add(new FamilyMember { Name="Valido",Age=20,RelationshipTypeId=1 });
    invalidChild.FamilyMembers.Add(new FamilyMember { Name="Invalido",Age=20,RelationshipTypeId=int.MaxValue });
    Reject(() => repo.Add(invalidChild), "falha em familiar interrompe o cadastro");
    Check(Count("Children") == 2 && Count("Guardians") == 2 && invalidChild.Id == 0, "transacao desfaz crianca, responsavel e contato e nao devolve Id falso");
    saved = repo.GetById(id)!;
    string before = saved.Name;
    saved.Name = "Nome Que Nao Deve Gravar";
    saved.FamilyMembers[0].Age = 121;
    Reject(() => repo.Update(saved), "falha no familiar interrompe a edicao");
    Check(repo.GetById(id)!.Name == before, "rollback desfaz tambem os campos da crianca");
    Reject(() => Exec($"DELETE FROM `{prod}`.RelationshipTypes WHERE Id=8"), "parentesco em uso protegido por RESTRICT");
    Reject(() => Exec($"INSERT INTO `{prod}`.Guardians (Name,Cpf,Rg,ChildId) VALUES ('Teste','0','0',{id})"), "indice unico bloqueia segundo responsavel");
    Reject(() => Exec($"INSERT INTO `{prod}`.FamilyMembers (Name,Age,ChildId,RelationshipTypeId) VALUES ('Teste',20,2147483647,1)"), "FK bloqueia familiar sem crianca");
    Reject(() => Exec($"UPDATE `{prod}`.Children SET Age=121 WHERE Id={id}"), "CHECK bloqueia idade fora da faixa");

    using (MySqlConnection ro = context.CreateReadConnection())
    {
        ro.Open();
        using MySqlCommand command = new MySqlCommand("UPDATE Children SET Name='Bloqueado' WHERE Id=@id",ro);
        command.Parameters.AddWithValue("@id",id);
        Reject(() => command.ExecuteNonQuery(), "conta de leitura nao consegue alterar dados");
        command.CommandText=$"SELECT * FROM `{prod}`.Children";
        Reject(() => command.ExecuteScalar(), "conta de leitura nao acessa diretamente a producao");
    }
    Reject(() => Exec($"UPDATE `{read}`.Children SET Name='Bloqueado' WHERE Id={id}"), "views recusam alteracao mesmo pela conta administradora");

    Exec($"INSERT INTO `{prod}`.Programs (Name,Description) VALUES ('Programa','Exemplo')");
    Exec($"INSERT INTO `{prod}`.Activities (Name,ProgramId) VALUES ('Oficina',1)");
    Exec($"INSERT INTO `{prod}`.ActivitySessions (ActivityId,StartsAt,EndsAt,Location) VALUES (1,'2026-11-06 09:00','2026-11-06 10:00','Sala')");
    Exec($"INSERT INTO `{prod}`.SessionEnrollments (ChildId,SessionId,RegisteredAt) VALUES ({id},1,UTC_TIMESTAMP())");
    Exec($"INSERT INTO `{prod}`.AttendanceRecords (SessionEnrollmentId,Attended,RecordedAt) VALUES (1,1,UTC_TIMESTAMP())");
    Check(Count("AttendanceRecords") == 1, "cinco tabelas de oficinas relacionadas");
    Reject(() => Exec($"INSERT INTO `{prod}`.SessionEnrollments (ChildId,SessionId,RegisteredAt) VALUES ({id},1,UTC_TIMESTAMP())"), "inscricao duplicada bloqueada");
    Reject(() => Exec($"INSERT INTO `{prod}`.AttendanceRecords (SessionEnrollmentId,Attended,RecordedAt) VALUES (1,0,UTC_TIMESTAMP())"), "segunda presenca da mesma inscricao bloqueada");
    Reject(() => Exec($"INSERT INTO `{prod}`.ActivitySessions (ActivityId,StartsAt,EndsAt,Location) VALUES (1,'2026-11-06 10:00','2026-11-06 09:00','Sala')"), "fim do encontro deve ocorrer depois do inicio");

    Exec($"INSERT INTO `{prod}`.Units (Name,Address,Phone) VALUES ('Unidade Teste','Rua Teste','00123')");
    Exec($"INSERT INTO `{prod}`.Educators (Name,Phone,ActivityId) VALUES ('Educador Teste','00123',1)");
    Exec($"INSERT INTO `{prod}`.Volunteers (Name,Phone,ProgramId) VALUES ('Voluntario Teste','00123',1)");
    Exec($"INSERT INTO `{prod}`.Materials (Name,Quantity,ActivityId) VALUES ('Papel',20,1)");
    Exec($"UPDATE `{prod}`.ActivitySessions SET UnitId=1 WHERE Id=1");
    Check(Count("Units")==1 && Count("Educators")==1 && Count("Volunteers")==1 && Count("Materials")==1,
        "quatro entidades novas salvas com seus vinculos");
    Reject(() => Exec($"INSERT INTO `{prod}`.Educators (Name,Phone,ActivityId) VALUES ('Teste','0',2147483647)"), "educador sem atividade bloqueado");
    Reject(() => Exec($"INSERT INTO `{prod}`.Volunteers (Name,Phone,ProgramId) VALUES ('Teste','0',2147483647)"), "voluntario sem programa bloqueado");
    Reject(() => Exec($"INSERT INTO `{prod}`.Materials (Name,Quantity,ActivityId) VALUES ('Teste',1,2147483647)"), "material sem atividade bloqueado");
    Reject(() => Exec($"UPDATE `{prod}`.ActivitySessions SET UnitId=2147483647 WHERE Id=1"), "encontro com unidade inexistente bloqueado");
    Reject(() => Exec($"UPDATE `{prod}`.Materials SET Quantity=-1 WHERE Id=1"), "quantidade negativa de material bloqueada");
    Reject(() => Exec($"DELETE FROM `{prod}`.Units WHERE Id=1"), "unidade com encontro protegida contra exclusao");
    using (MySqlConnection ro = context.CreateReadConnection())
    {
        ro.Open();
        foreach (string table in new[] { "Units", "Educators", "Volunteers", "Materials" })
        {
            using MySqlCommand command = new MySqlCommand($"SELECT COUNT(*) FROM `{table}`", ro);
            Check(Convert.ToInt32(command.ExecuteScalar())==1, "conta de leitura consulta " + table);
            command.CommandText=$"DELETE FROM `{table}` WHERE Id=1";
            Reject(() => command.ExecuteNonQuery(), "conta de leitura nao altera " + table);
        }
    }

    DbActivityLogger logger = new DbActivityLogger(context);
    Reject(() => logger.Log(new string('x',101), "Teste"), "falha de log por texto acima do tamanho");
    logger.Log("Teste","Proxima operacao funciona");
    Check(service.Register(Sample()) > 0, "erro de log nao deixa comandos pendentes");
    ChildService failedLog = new ChildService(repo,new RelationshipTypeRepository(context),new FailingLogger(),errors,new StubExporter());
    Check(failedLog.Register(Sample()) > 0 && errors.Count > 0, "cadastro permanece salvo quando o log falha");
    Check(service.ExportFile(id) == "ficha-teste.html", "servico chama exportador de teste e registra operacao");
    {
        AppDbContext reopened = new AppDbContext(WriteConnection(), ReadConnection());
        Child? loadedAgain = new ChildRepository(reopened).GetById(id);
        Check(loadedAgain != null && loadedAgain.FamilyMembers.Any(m => m.Id == kept), "nova instancia do programa le os dados e os IDs persistidos");
    }
    string exportFolder = Path.Combine(Path.GetTempPath(), "recriar_export_test_" + suffix);
    string exportPath = Path.Combine(exportFolder, $"ficha_{id}.html");
    try
    {
        ChildService actualExport = new ChildService(repo, new RelationshipTypeRepository(context),
            new DbActivityLogger(context), errors, new HtmlChildFileExporter(exportFolder));
        string path = actualExport.ExportFile(id);
        Check(path == exportPath && File.Exists(path), "exportador real gera arquivo HTML a partir do MySQL");
        string html = File.ReadAllText(path);
        Check(html.Contains("Crianca Exemplo") && html.Contains("Responsavel Alterado")
            && html.Contains("Familiar Alterado") && html.Contains("Primo"), "HTML real contem crianca, responsavel e familiares com parentescos");
    }
    finally
    {
        // Only remove the exact fictitious file and folder created by this test.
        if (File.Exists(exportPath)) File.Delete(exportPath);
        if (Directory.Exists(exportFolder)) Directory.Delete(exportFolder);
    }
    service.Delete(id);
    Check(Count("Children","Id="+id)==0 && Count("Guardians","ChildId="+id)==0 && Count("EmergencyContacts","ChildId="+id)==0 && Count("FamilyMembers","ChildId="+id)==0 && Count("SessionEnrollments","ChildId="+id)==0 && Count("AttendanceRecords")==0,"exclusao em cascata remove dependentes e presenca");
    Check(repo.GetById(id)==null && !repo.Delete(id), "leitura mostra exclusao imediatamente e Delete inexistente retorna false");
    Check(Count("ActivityLogs", "Action='Cadastro excluído'") > 0 && Count("ActivityLogs", "Action='Ficha exportada'") > 0, "historico permanece apos exclusao");
    int beforeSample = Count("Children");
    string sampleSql = File.ReadAllText(Path.Combine(FindRoot(), "docs/database/sample-data.sql")).Replace("recriar_prod",prod);
    Exec(sampleSql);
    Check(Count("Children") == beforeSample + 1, "arquivo de dados ficticios executa com os relacionamentos");
    string querySql = File.ReadAllText(Path.Combine(FindRoot(), "docs/database/queries.sql"))
        .Replace("recriar_prod",prod).Replace("recriar_leitura",read);
    Exec(querySql);
    Check(Count("Children") == beforeSample + 1, "consultas de demonstracao executam e ROLLBACK preserva os cadastros");
    string originalSchema = File.ReadAllText(Path.Combine(FindRoot(), "docs/database/schema.sql"))
        .Split("-- Estrutura de apoio às oficinas.")[0];
    Exec($"CREATE DATABASE `{upgradeProd}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci; USE `{upgradeProd}`");
    Exec(originalSchema);
    Exec($"CREATE DATABASE `{upgradeRead}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci");
    string oldViews = File.ReadAllText(Path.Combine(FindRoot(), "docs/database/dois-bancos/02_criar_leitura.sql"));
    foreach (string statement in oldViews.Split(';'))
    {
        if (!statement.Contains("VIEW `")) continue;
        if (new[] { "Units", "Educators", "Volunteers", "Materials" }.Any(t => statement.Contains("VIEW `" + t + "`"))) continue;
        Exec($"USE `{upgradeRead}`");
        Exec(statement.Replace(", `UnitId`", "").Replace("recriar_prod", upgradeProd));
    }
    // Use the administrator's actual settings for this separate upgrade fixture.
    MySqlConnectionStringBuilder upgradeSettings = new MySqlConnectionStringBuilder(settings.ConnectionString);
    upgradeSettings.Database=upgradeProd;
    MySqlConnectionStringBuilder upgradeReadSettings = new MySqlConnectionStringBuilder(settings.ConnectionString);
    upgradeReadSettings.Database=upgradeRead;
    AppDbContext oldContext = new AppDbContext(upgradeSettings.ConnectionString, upgradeReadSettings.ConnectionString);
    ChildRepository oldRepo = new ChildRepository(oldContext);
    ChildService oldService = new ChildService(oldRepo, new RelationshipTypeRepository(oldContext), new DbActivityLogger(oldContext), errors, new StubExporter());
    int oldId=oldService.Register(Sample());
    string upgradeSql=File.ReadAllText(Path.Combine(FindRoot(), "docs/database/atualizar_11_para_15.sql"))
        .Replace("recriar_prod", upgradeProd).Replace("recriar_leitura", upgradeRead);
    Exec(upgradeSql);
    Check(oldRepo.GetById(oldId)?.FamilyMembers.Count==2, "atualizacao de 11 para 15 preserva cadastro e familiares");
    using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=@db AND TABLE_TYPE='BASE TABLE'",admin))
    {
        command.Parameters.AddWithValue("@db",upgradeProd);
        Check(Convert.ToInt32(command.ExecuteScalar())==15,"atualizacao cria exatamente 15 tabelas");
        command.CommandText="SELECT COUNT(*) FROM information_schema.views WHERE table_schema=@db AND IS_UPDATABLE='NO'";
        command.Parameters["@db"].Value=upgradeRead;
        Check(Convert.ToInt32(command.ExecuteScalar())==15,"atualizacao cria 15 views de leitura");
    }
    Console.WriteLine($"RESULTADO: {passed} verificacoes aprovadas em MySQL com ADO.NET.");
}
finally
{
    // These schemas and accounts were created above with random names for this run only.
    Exec($"DROP DATABASE IF EXISTS `{upgradeRead}`");
    Exec($"DROP DATABASE IF EXISTS `{upgradeProd}`");
    Exec($"DROP DATABASE IF EXISTS `{read}`");
    Exec($"DROP DATABASE IF EXISTS `{prod}`");
    Exec($"DROP USER IF EXISTS '{writer}'@'localhost', '{readerUser}'@'localhost'");
}

static ChildDto Sample() => new()
{
    Name="Crianca Exemplo",Age="8",Cpf="00000000000",Rg="00123",Address="",Neighborhood="Centro",City="Campinas",School="",
    MotherName="Mae Exemplo",FatherName="Pai Exemplo",Phone="0012345678",RegistrationDate="31/10/2026",
    GuardianName="Responsavel Exemplo",GuardianCpf="00000000000",GuardianRg="00123",EmergencyContactName="Contato Exemplo",EmergencyContactPhone="0012345678",
    HealthCenter="",FileNumber="",Allergy="",FamilyMembers=[new(){Name="Familiar Exemplo",Age="20",RelationshipTypeId=1},new(){Name="Outro Familiar",Age="25",RelationshipTypeId=9,OtherRelationship="Primo"}]
};
sealed class CapturingErrorLogger : IErrorLogger
{
    public int Count { get; private set; }
    public void LogError(Exception exception,string context) { Count++; }
}
sealed class StubExporter : IChildFileExporter
{
    public string Export(Child child,IReadOnlyDictionary<int,string> names) => "ficha-teste.html";
}
sealed class FailingLogger : IActivityLogger
{
    public void Log(string action,string description) => throw new IOException("Falha simulada");
}
