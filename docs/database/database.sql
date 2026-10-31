-- RECRIAR - MySQL 8.0.16 ou superior
-- Instalação em schema NOVO, com 11 tabelas de negócio e histórico técnico do EF.
-- Se recriar_dev já existir, pare e use migrations; não execute novamente neste banco.
-- No cliente mysql, não use --force: um erro deve interromper a execução.
CREATE DATABASE recriar_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE recriar_dev;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `ActivityLogs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Action` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_ActivityLogs` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Children` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Age` int NOT NULL,
    `Cpf` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Rg` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Address` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Neighborhood` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `City` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `School` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `MotherName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `FatherName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `RegistrationDate` date NOT NULL,
    `HealthCenter` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `FileNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Allergy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    CONSTRAINT `PK_Children` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Children_Age` CHECK (`Age` BETWEEN 0 AND 120)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Programs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    CONSTRAINT `PK_Programs` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `RelationshipTypes` (
    `Id` int NOT NULL,
    `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    CONSTRAINT `PK_RelationshipTypes` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `EmergencyContacts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `ChildId` int NOT NULL,
    CONSTRAINT `PK_EmergencyContacts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EmergencyContacts_Children_ChildId` FOREIGN KEY (`ChildId`) REFERENCES `Children` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Guardians` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Rg` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Cpf` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `ChildId` int NOT NULL,
    CONSTRAINT `PK_Guardians` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Guardians_Children_ChildId` FOREIGN KEY (`ChildId`) REFERENCES `Children` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Activities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProgramId` int NOT NULL,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    CONSTRAINT `PK_Activities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Activities_Programs_ProgramId` FOREIGN KEY (`ProgramId`) REFERENCES `Programs` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `FamilyMembers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RelationshipTypeId` int NOT NULL,
    `OtherRelationship` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Age` int NOT NULL,
    `ChildId` int NOT NULL,
    CONSTRAINT `PK_FamilyMembers` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_FamilyMembers_Age` CHECK (`Age` BETWEEN 0 AND 120),
    CONSTRAINT `FK_FamilyMembers_Children_ChildId` FOREIGN KEY (`ChildId`) REFERENCES `Children` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_FamilyMembers_RelationshipTypes_RelationshipTypeId` FOREIGN KEY (`RelationshipTypeId`) REFERENCES `RelationshipTypes` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `ActivitySessions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ActivityId` int NOT NULL,
    `StartsAt` datetime(6) NOT NULL,
    `EndsAt` datetime(6) NOT NULL,
    `Location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    CONSTRAINT `PK_ActivitySessions` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_ActivitySessions_Period` CHECK (`EndsAt` > `StartsAt`),
    CONSTRAINT `FK_ActivitySessions_Activities_ActivityId` FOREIGN KEY (`ActivityId`) REFERENCES `Activities` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `SessionEnrollments` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ChildId` int NOT NULL,
    `SessionId` int NOT NULL,
    `RegisteredAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_SessionEnrollments` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SessionEnrollments_ActivitySessions_SessionId` FOREIGN KEY (`SessionId`) REFERENCES `ActivitySessions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_SessionEnrollments_Children_ChildId` FOREIGN KEY (`ChildId`) REFERENCES `Children` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `AttendanceRecords` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SessionEnrollmentId` int NOT NULL,
    `Attended` tinyint(1) NOT NULL,
    `RecordedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_AttendanceRecords` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AttendanceRecords_SessionEnrollments_SessionEnrollmentId` FOREIGN KEY (`SessionEnrollmentId`) REFERENCES `SessionEnrollments` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT INTO `RelationshipTypes` (`Id`, `Name`)
VALUES (1, 'Mãe'),
(2, 'Pai'),
(3, 'Avó'),
(4, 'Avô'),
(5, 'Irmã'),
(6, 'Irmão'),
(7, 'Tia'),
(8, 'Tio'),
(9, 'Outro');

CREATE INDEX `IX_Activities_ProgramId` ON `Activities` (`ProgramId`);

CREATE INDEX `IX_ActivityLogs_CreatedAt` ON `ActivityLogs` (`CreatedAt`);

CREATE INDEX `IX_ActivitySessions_ActivityId` ON `ActivitySessions` (`ActivityId`);

CREATE INDEX `IX_ActivitySessions_StartsAt` ON `ActivitySessions` (`StartsAt`);

CREATE UNIQUE INDEX `IX_AttendanceRecords_SessionEnrollmentId` ON `AttendanceRecords` (`SessionEnrollmentId`);

CREATE INDEX `IX_Children_Name` ON `Children` (`Name`);

CREATE UNIQUE INDEX `IX_EmergencyContacts_ChildId` ON `EmergencyContacts` (`ChildId`);

CREATE INDEX `IX_FamilyMembers_ChildId` ON `FamilyMembers` (`ChildId`);

CREATE INDEX `IX_FamilyMembers_RelationshipTypeId` ON `FamilyMembers` (`RelationshipTypeId`);

CREATE UNIQUE INDEX `IX_Guardians_ChildId` ON `Guardians` (`ChildId`);

CREATE INDEX `IX_SessionEnrollments_ChildId` ON `SessionEnrollments` (`ChildId`);

CREATE UNIQUE INDEX `IX_SessionEnrollments_SessionId_ChildId` ON `SessionEnrollments` (`SessionId`, `ChildId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261031201442_InitialDatabase', '9.0.22');

COMMIT;


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
INSERT INTO ActivitySessions (ActivityId, StartsAt, EndsAt, Location)
VALUES (@activity_id, '2026-11-01 09:00:00', '2026-11-01 10:00:00', 'Sala Exemplo');
SET @session_id = LAST_INSERT_ID();
INSERT INTO SessionEnrollments (ChildId, SessionId, RegisteredAt)
VALUES (@child_id, @session_id, UTC_TIMESTAMP(6));
SET @enrollment_id = LAST_INSERT_ID();
INSERT INTO AttendanceRecords (SessionEnrollmentId, Attended, RecordedAt)
VALUES (@enrollment_id, TRUE, UTC_TIMESTAMP(6));
COMMIT;

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
START TRANSACTION;
UPDATE Children SET Age = 9 WHERE Id = @selected_child_id;
SELECT Id, Name, Age FROM Children WHERE Id = @selected_child_id;
DELETE FROM Children WHERE Id = @selected_child_id;
SELECT COUNT(*) AS ExpectedZero FROM FamilyMembers WHERE ChildId = @selected_child_id;
ROLLBACK;
