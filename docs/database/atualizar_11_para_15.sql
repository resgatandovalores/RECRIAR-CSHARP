-- Atualização da versão com 11 tabelas para 15. Executar UMA VEZ como administrador.
-- Faça um backup antes. DDL no MySQL não é desfeito por ROLLBACK.
-- Não usar em instalação nova: database.sql já inclui estas mudanças.
USE recriar_prod;
SET NAMES utf8mb4;

-- Estrutura de apoio às oficinas.
CREATE TABLE `Units` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Address` varchar(150) NOT NULL,
    `Phone` varchar(20) NOT NULL,
    CONSTRAINT `PK_Units` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Educators` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Phone` varchar(20) NOT NULL,
    `ActivityId` int NOT NULL,
    CONSTRAINT `PK_Educators` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Educators_Activities` FOREIGN KEY (`ActivityId`) REFERENCES `Activities` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Volunteers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Phone` varchar(20) NOT NULL,
    `ProgramId` int NOT NULL,
    CONSTRAINT `PK_Volunteers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Volunteers_Programs` FOREIGN KEY (`ProgramId`) REFERENCES `Programs` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Materials` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NOT NULL,
    `Quantity` int NOT NULL,
    `ActivityId` int NOT NULL,
    CONSTRAINT `PK_Materials` PRIMARY KEY (`Id`),
    CONSTRAINT `CK_Materials_Quantity` CHECK (`Quantity` >= 0),
    CONSTRAINT `FK_Materials_Activities` FOREIGN KEY (`ActivityId`) REFERENCES `Activities` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `ActivitySessions`
    ADD COLUMN `UnitId` int NULL,
    ADD CONSTRAINT `FK_ActivitySessions_Units` FOREIGN KEY (`UnitId`) REFERENCES `Units` (`Id`) ON DELETE RESTRICT;

USE recriar_leitura;
CREATE OR REPLACE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `ActivitySessions` AS
SELECT `Id`, `ActivityId`, `StartsAt`, `EndsAt`, `Location`, `UnitId` FROM `recriar_prod`.`ActivitySessions`;

CREATE OR REPLACE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Units` AS
SELECT `Id`, `Name`, `Address`, `Phone` FROM `recriar_prod`.`Units`;

CREATE OR REPLACE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Educators` AS
SELECT `Id`, `Name`, `Phone`, `ActivityId` FROM `recriar_prod`.`Educators`;

CREATE OR REPLACE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Volunteers` AS
SELECT `Id`, `Name`, `Phone`, `ProgramId` FROM `recriar_prod`.`Volunteers`;

CREATE OR REPLACE ALGORITHM=TEMPTABLE SQL SECURITY DEFINER VIEW `Materials` AS
SELECT `Id`, `Name`, `Quantity`, `ActivityId` FROM `recriar_prod`.`Materials`;

-- Os papéis existentes já usam permissões no schema inteiro e abrangem as novas tabelas/views.
USE recriar_prod;
