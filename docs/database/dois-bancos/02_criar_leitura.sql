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
