-- Переключение на нужную базу данных
USE ClientAddressesDB;
GO

-- Удаление таблиц в обратном порядке зависимостей (если они существуют)
-- Сначала удаляем таблицы, которые ссылаются на другие
IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL
    DROP TABLE dbo.Addresses;
GO
IF OBJECT_ID('dbo.Cities', 'U') IS NOT NULL
    DROP TABLE dbo.Cities;
GO
IF OBJECT_ID('dbo.Regions', 'U') IS NOT NULL
    DROP TABLE dbo.Regions;
GO
-- Затем удаляем таблицы, на которые ссылались (или которые не имели зависимостей)
IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
    DROP TABLE dbo.Persons;
GO
IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL
    DROP TABLE dbo.Countries;
GO

-- Теперь создание таблиц

-- 1. Таблица Стран (Countries)
CREATE TABLE Countries (
    ID INT PRIMARY KEY IDENTITY(1,1),
    NameFull NVARCHAR(100) NOT NULL,
    NameShort NVARCHAR(50) NULL
);
GO

-- 2. Таблица Клиентов/Персон (Persons) - выносим ее создание раньше, т.к. на нее будет ссылаться Addresses
CREATE TABLE Persons (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Surname NVARCHAR(100) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Patronymic NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL
);
GO

-- 3. Таблица Регионов (Regions)
CREATE TABLE Regions (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    CountryID INT NOT NULL,
    CONSTRAINT FK_Regions_Countries FOREIGN KEY (CountryID) REFERENCES Countries(ID)
        ON DELETE CASCADE  -- Если страна удаляется, связанные регионы тоже
        ON UPDATE CASCADE
);
GO

-- 4. Таблица Городов (Cities)
CREATE TABLE Cities (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    RegionID INT NULL,      -- Может быть NULL (город федерального значения)
    CountryID INT NOT NULL, -- Всегда должна быть страна
    CONSTRAINT FK_Cities_Regions FOREIGN KEY (RegionID) REFERENCES Regions(ID)
        ON DELETE NO ACTION -- Если регион удален (например, каскадно от страны), город не удаляем автоматически по этой связи
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Cities_Countries FOREIGN KEY (CountryID) REFERENCES Countries(ID)
        ON DELETE NO ACTION -- Прямую связь со страной делаем без каскада, т.к. есть путь через регион
        ON UPDATE NO ACTION
);
GO

-- 5. Таблица Адресов (Addresses)
CREATE TABLE Addresses (
    ID INT PRIMARY KEY IDENTITY(1,1),
    IndexAddress NVARCHAR(10) NULL,
    PersonID INT NOT NULL,
    CountryID INT NOT NULL,
    RegionID INT NULL,
    CityID INT NULL,
    Street NVARCHAR(255) NOT NULL,
    Building NVARCHAR(50) NOT NULL,
    Office NVARCHAR(50) NULL,
    CONSTRAINT FK_Addresses_Persons FOREIGN KEY (PersonID) REFERENCES Persons(ID)
        ON DELETE CASCADE   -- Если клиент удаляется, его адреса тоже
        ON UPDATE CASCADE,
    CONSTRAINT FK_Addresses_Countries FOREIGN KEY (CountryID) REFERENCES Countries(ID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Addresses_Regions FOREIGN KEY (RegionID) REFERENCES Regions(ID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Addresses_Cities FOREIGN KEY (CityID) REFERENCES Cities(ID)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

-- После создания таблиц можно добавить тестовые данные (скрипт из предыдущего ответа)
-- Например:
-- Заполнение таблицы Стран
INSERT INTO Countries (NameFull, NameShort) VALUES
(N'Российская Федерация', N'Россия'),
(N'Республика Беларусь', N'Беларусь'),
(N'Республика Казахстан', N'Казахстан');
GO
-- Заполнение таблицы Клиентов
INSERT INTO Persons (Surname, Name, Patronymic, PhoneNumber, Email) VALUES
(N'Иванов', N'Иван', N'Иванович', N'+79001234567', N'ivanov.ii@example.com'),
(N'Петров', N'Петр', N'Петрович', N'+375291234567', N'petrov.pp@example.com'),
(N'Сидорова', N'Анна', N'Сергеевна', N'+79017654321', N'sidorova.as@example.com');
GO
-- Заполнение таблицы Регионов (ID стран будут 1, 2, 3 соответственно)
INSERT INTO Regions (Name, CountryID) VALUES
(N'Московская область', 1),
(N'Ленинградская область', 1),
(N'Минская область', 2),
(N'Гомельская область', 2),
(N'Алматинская область', 3);
GO
-- Заполнение таблицы Городов
INSERT INTO Cities (Name, RegionID, CountryID) VALUES
(N'Москва', (SELECT ID FROM Regions WHERE Name = N'Московская область' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'Россия')), (SELECT ID FROM Countries WHERE NameShort = N'Россия')),
(N'Санкт-Петербург', (SELECT ID FROM Regions WHERE Name = N'Ленинградская область' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'Россия')), (SELECT ID FROM Countries WHERE NameShort = N'Россия')),
(N'Минск', (SELECT ID FROM Regions WHERE Name = N'Минская область' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'Беларусь')), (SELECT ID FROM Countries WHERE NameShort = N'Беларусь')),
(N'Гомель', (SELECT ID FROM Regions WHERE Name = N'Гомельская область' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'Беларусь')), (SELECT ID FROM Countries WHERE NameShort = N'Беларусь'));
GO
-- Заполнение таблицы Адресов
INSERT INTO Addresses (IndexAddress, PersonID, CountryID, RegionID, CityID, Street, Building, Office) VALUES
(N'101000',
    (SELECT ID FROM Persons WHERE Email = N'ivanov.ii@example.com'),
    (SELECT ID FROM Countries WHERE NameShort = N'Россия'),
    (SELECT ID FROM Regions WHERE Name = N'Московская область'),
    (SELECT ID FROM Cities WHERE Name = N'Москва'),
    N'ул. Тверская', N'10', N'5А'),
(N'220005',
    (SELECT ID FROM Persons WHERE Email = N'petrov.pp@example.com'),
    (SELECT ID FROM Countries WHERE NameShort = N'Беларусь'),
    (SELECT ID FROM Regions WHERE Name = N'Минская область'),
    (SELECT ID FROM Cities WHERE Name = N'Минск'),
    N'пр. Независимости', N'15', NULL);
GO

-- Проверка данных (опционально)
SELECT * FROM Countries;
SELECT * FROM Regions;
SELECT * FROM Cities;
SELECT * FROM Persons;
SELECT * FROM Addresses;
GO