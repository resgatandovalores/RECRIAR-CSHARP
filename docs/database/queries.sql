USE recriar_leitura;
-- Consultas equivalentes às usadas pelo serviço. @... são variáveis desta sessão.
-- No C#, o EF Core transforma os valores recebidos em parâmetros SQL.

-- 1. Listagem da tela principal, ordenada por Id.
SELECT Id, Name, Cpf, Phone FROM Children ORDER BY Id;

-- 2. Pesquisa por trecho do nome. Escapa caracteres curinga digitados.
SET @name = 'exemplo';
SET @pattern = CONCAT('%', REPLACE(REPLACE(REPLACE(@name, '!', '!!'), '%', '!%'), '_', '!_'), '%');
SELECT Id, Name, Cpf, Phone FROM Children WHERE Name LIKE @pattern ESCAPE '!' ORDER BY Id;

-- 3. Dados para edição e exportação da ficha.
SET @selected_child_id = (SELECT MIN(Id) FROM Children);
SELECT c.*, g.Id AS GuardianId, g.Name AS GuardianName, g.Cpf AS GuardianCpf, g.Rg AS GuardianRg,
 e.Id AS EmergencyContactId, e.Name AS EmergencyContactName, e.Phone AS EmergencyContactPhone
FROM Children c LEFT JOIN Guardians g ON g.ChildId = c.Id
LEFT JOIN EmergencyContacts e ON e.ChildId = c.Id WHERE c.Id = @selected_child_id;

-- 4. Familiares e nomes dos parentescos da criança selecionada.
SELECT f.Id, f.Name, f.Age, f.RelationshipTypeId, r.Name AS RelationshipName, f.OtherRelationship
FROM FamilyMembers f JOIN RelationshipTypes r ON r.Id = f.RelationshipTypeId
WHERE f.ChildId = @selected_child_id ORDER BY f.Id;

-- 5. Opções do campo de parentesco.
SELECT Id, Name FROM RelationshipTypes ORDER BY Id;

-- 6. Histórico das operações. Horários em UTC.
SELECT Id, Action, Description, CreatedAt FROM ActivityLogs ORDER BY CreatedAt DESC, Id DESC;

-- 7. Extensão de oficinas: presenças e inscrições ainda não avaliadas.
SELECT p.Name AS Program, a.Name AS Activity, s.StartsAt, c.Name AS Child,
 CASE WHEN ar.Id IS NULL THEN 'Não registrada' WHEN ar.Attended = 1 THEN 'Presente' ELSE 'Ausente' END AS Attendance
FROM SessionEnrollments se
JOIN Children c ON c.Id = se.ChildId
JOIN ActivitySessions s ON s.Id = se.SessionId
JOIN Activities a ON a.Id = s.ActivityId
JOIN Programs p ON p.Id = a.ProgramId
LEFT JOIN AttendanceRecords ar ON ar.SessionEnrollmentId = se.Id
ORDER BY s.StartsAt, c.Id;

-- 8. Demonstração de edição e exclusão sem alterar os dados da entrega.
-- Esta parte deve ser executada com a conta de gravação.
USE recriar_prod;
START TRANSACTION;
UPDATE Children SET Age = 9 WHERE Id = @selected_child_id;
SELECT Id, Name, Age FROM Children WHERE Id = @selected_child_id;
DELETE FROM Children WHERE Id = @selected_child_id;
SELECT COUNT(*) AS ExpectedZero FROM FamilyMembers WHERE ChildId = @selected_child_id;
ROLLBACK;
