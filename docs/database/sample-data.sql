USE recriar_prod;
-- Dados fictícios para demonstração. Executar somente em ambiente de desenvolvimento.
START TRANSACTION;
INSERT INTO Children (Name, Age, Cpf, Rg, Address, Neighborhood, City, School,
 MotherName, FatherName, Phone, RegistrationDate, HealthCenter, FileNumber, Allergy)
VALUES ('Crianca Exemplo', 8, '00000000000', '00123', '', 'Centro', 'Campinas', '',
 'Mae Exemplo', 'Pai Exemplo', '0012345678', '2026-10-31', '', '', '');
SET @child_id = LAST_INSERT_ID();
INSERT INTO Guardians (Name, Rg, Cpf, ChildId)
VALUES ('Responsavel Exemplo', '00123', '00000000000', @child_id);
INSERT INTO EmergencyContacts (Name, Phone, ChildId)
VALUES ('Contato Exemplo', '0012345678', @child_id);
INSERT INTO FamilyMembers (RelationshipTypeId, OtherRelationship, Name, Age, ChildId)
VALUES (9, 'Primo', 'Familiar Exemplo', 20, @child_id);
INSERT INTO ActivityLogs (Action, Description, CreatedAt)
VALUES ('Cadastro criado', CONCAT('Criança ', @child_id, ' - Crianca Exemplo'), UTC_TIMESTAMP(6));
INSERT INTO Programs (Name, Description) VALUES ('Programa Exemplo', 'Programa ficticio para demonstracao');
SET @program_id = LAST_INSERT_ID();
INSERT INTO Activities (ProgramId, Name) VALUES (@program_id, 'Oficina Exemplo');
SET @activity_id = LAST_INSERT_ID();
INSERT INTO Units (Name, Address, Phone) VALUES ('Unidade Exemplo', 'Rua Exemplo', '0000000000');
SET @unit_id = LAST_INSERT_ID();
INSERT INTO Educators (Name, Phone, ActivityId) VALUES ('Educador Exemplo', '0000000000', @activity_id);
INSERT INTO Volunteers (Name, Phone, ProgramId) VALUES ('Voluntario Exemplo', '0000000000', @program_id);
INSERT INTO Materials (Name, Quantity, ActivityId) VALUES ('Papel', 20, @activity_id);
INSERT INTO ActivitySessions (ActivityId, StartsAt, EndsAt, Location, UnitId)
VALUES (@activity_id, '2026-11-01 09:00:00', '2026-11-01 10:00:00', 'Sala Exemplo', @unit_id);
SET @session_id = LAST_INSERT_ID();
INSERT INTO SessionEnrollments (ChildId, SessionId, RegisteredAt)
VALUES (@child_id, @session_id, UTC_TIMESTAMP(6));
SET @enrollment_id = LAST_INSERT_ID();
INSERT INTO AttendanceRecords (SessionEnrollmentId, Attended, RecordedAt)
VALUES (@enrollment_id, TRUE, UTC_TIMESTAMP(6));
COMMIT;
