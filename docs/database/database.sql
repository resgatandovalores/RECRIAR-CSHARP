-- RECRIAR: instalar os dois bancos NOVOS. Executar como administrador.

-- Primeira instalação do banco principal. Não apaga bancos existentes.
CREATE DATABASE recriar_prod CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE recriar_prod;
SET NAMES utf8mb4;

-- RECRIAR: estrutura das 11 tabelas. Executar em um banco novo selecionado.

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

-- Banco de consulta, no mesmo servidor do principal.
-- As views mostram os dados atuais do banco principal, sem guardar uma cópia.
-- TEMPTABLE impede INSERT, UPDATE e DELETE pelas views.
-- Execute como administrador. A conta que cria as views precisa continuar existindo.
CREATE DATABASE recriar_leitura CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE recriar_leitura;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `ActivityLogs` AS
SELECT `Id`, `Action`, `Description`, `CreatedAt` FROM `recriar_prod`.`ActivityLogs`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Children` AS
SELECT `Id`, `Name`, `Age`, `Cpf`, `Rg`, `Address`, `Neighborhood`, `City`, `School`, `MotherName`, `FatherName`, `Phone`, `RegistrationDate`, `HealthCenter`, `FileNumber`, `Allergy` FROM `recriar_prod`.`Children`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Programs` AS
SELECT `Id`, `Name`, `Description` FROM `recriar_prod`.`Programs`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `RelationshipTypes` AS
SELECT `Id`, `Name` FROM `recriar_prod`.`RelationshipTypes`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `EmergencyContacts` AS
SELECT `Id`, `Name`, `Phone`, `ChildId` FROM `recriar_prod`.`EmergencyContacts`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Guardians` AS
SELECT `Id`, `Name`, `Rg`, `Cpf`, `ChildId` FROM `recriar_prod`.`Guardians`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Activities` AS
SELECT `Id`, `ProgramId`, `Name` FROM `recriar_prod`.`Activities`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `FamilyMembers` AS
SELECT `Id`, `RelationshipTypeId`, `OtherRelationship`, `Name`, `Age`, `ChildId` FROM `recriar_prod`.`FamilyMembers`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `ActivitySessions` AS
SELECT `Id`, `ActivityId`, `StartsAt`, `EndsAt`, `Location` FROM `recriar_prod`.`ActivitySessions`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `SessionEnrollments` AS
SELECT `Id`, `ChildId`, `SessionId`, `RegisteredAt` FROM `recriar_prod`.`SessionEnrollments`;

CREATE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `AttendanceRecords` AS
SELECT `Id`, `SessionEnrollmentId`, `Attended`, `RecordedAt` FROM `recriar_prod`.`AttendanceRecords`;


-- Executar como administrador após criar os dois bancos.
-- Os papéis não contêm senhas nem criam contas de login.
CREATE ROLE IF NOT EXISTS 'recriar_gravacao', 'recriar_consulta';
GRANT SELECT, INSERT, UPDATE, DELETE ON recriar_prod.* TO 'recriar_gravacao';
GRANT SELECT ON recriar_leitura.* TO 'recriar_consulta';

-- Depois de criar duas contas LOCAIS no Workbench, por exemplo recriar_app e recriar_reader:
-- GRANT 'recriar_gravacao' TO 'recriar_app'@'localhost';
-- SET DEFAULT ROLE 'recriar_gravacao' TO 'recriar_app'@'localhost';
-- GRANT 'recriar_consulta' TO 'recriar_reader'@'localhost';
-- SET DEFAULT ROLE 'recriar_consulta' TO 'recriar_reader'@'localhost';
-- Use contas novas, sem permissões globais ou outros papéis que permitam gravação.

USE recriar_prod;
