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
