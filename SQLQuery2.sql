-- ������������ �� ������ ���� ������
USE ClientAddressesDB;
GO

-- �������� ������ � �������� ������� ������������ (���� ��� ����������)
-- ������� ������� �������, ������� ��������� �� ������
IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL
    DROP TABLE dbo.Addresses;
GO
IF OBJECT_ID('dbo.Cities', 'U') IS NOT NULL
    DROP TABLE dbo.Cities;
GO
IF OBJECT_ID('dbo.Regions', 'U') IS NOT NULL
    DROP TABLE dbo.Regions;
GO
-- ����� ������� �������, �� ������� ��������� (��� ������� �� ����� ������������)
IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
    DROP TABLE dbo.Persons;
GO
IF OBJECT_ID('dbo.Countries', 'U') IS NOT NULL
    DROP TABLE dbo.Countries;
GO

-- ������ �������� ������

-- 1. ������� ����� (Countries)
CREATE TABLE Countries (
    ID INT PRIMARY KEY IDENTITY(1,1),
    NameFull NVARCHAR(100) NOT NULL,
    NameShort NVARCHAR(50) NULL
);
GO

-- 2. ������� ��������/������ (Persons) - ������� �� �������� ������, �.�. �� ��� ����� ��������� Addresses
CREATE TABLE Persons (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Surname NVARCHAR(100) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Patronymic NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL
);
GO

-- 3. ������� �������� (Regions)
CREATE TABLE Regions (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    CountryID INT NOT NULL,
    CONSTRAINT FK_Regions_Countries FOREIGN KEY (CountryID) REFERENCES Countries(ID)
        ON DELETE CASCADE  -- ���� ������ ���������, ��������� ������� ����
        ON UPDATE CASCADE
);
GO

-- 4. ������� ������� (Cities)
CREATE TABLE Cities (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    RegionID INT NULL,      -- ����� ���� NULL (����� ������������ ��������)
    CountryID INT NOT NULL, -- ������ ������ ���� ������
    CONSTRAINT FK_Cities_Regions FOREIGN KEY (RegionID) REFERENCES Regions(ID)
        ON DELETE NO ACTION -- ���� ������ ������ (��������, �������� �� ������), ����� �� ������� ������������� �� ���� �����
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Cities_Countries FOREIGN KEY (CountryID) REFERENCES Countries(ID)
        ON DELETE NO ACTION -- ������ ����� �� ������� ������ ��� �������, �.�. ���� ���� ����� ������
        ON UPDATE NO ACTION
);
GO

-- 5. ������� ������� (Addresses)
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
        ON DELETE CASCADE   -- ���� ������ ���������, ��� ������ ����
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

-- ����� �������� ������ ����� �������� �������� ������ (������ �� ����������� ������)
-- ��������:
-- ���������� ������� �����
INSERT INTO Countries (NameFull, NameShort) VALUES
(N'���������� ���������', N'������'),
(N'���������� ��������', N'��������'),
(N'���������� ���������', N'���������');
GO
-- ���������� ������� ��������
INSERT INTO Persons (Surname, Name, Patronymic, PhoneNumber, Email) VALUES
(N'������', N'����', N'��������', N'+79001234567', N'ivanov.ii@example.com'),
(N'������', N'����', N'��������', N'+375291234567', N'petrov.pp@example.com'),
(N'��������', N'����', N'���������', N'+79017654321', N'sidorova.as@example.com');
GO
-- ���������� ������� �������� (ID ����� ����� 1, 2, 3 ��������������)
INSERT INTO Regions (Name, CountryID) VALUES
(N'���������� �������', 1),
(N'������������� �������', 1),
(N'������� �������', 2),
(N'���������� �������', 2),
(N'����������� �������', 3);
GO
-- ���������� ������� �������
INSERT INTO Cities (Name, RegionID, CountryID) VALUES
(N'������', (SELECT ID FROM Regions WHERE Name = N'���������� �������' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'������')), (SELECT ID FROM Countries WHERE NameShort = N'������')),
(N'�����-���������', (SELECT ID FROM Regions WHERE Name = N'������������� �������' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'������')), (SELECT ID FROM Countries WHERE NameShort = N'������')),
(N'�����', (SELECT ID FROM Regions WHERE Name = N'������� �������' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'��������')), (SELECT ID FROM Countries WHERE NameShort = N'��������')),
(N'������', (SELECT ID FROM Regions WHERE Name = N'���������� �������' AND CountryID = (SELECT ID FROM Countries WHERE NameShort = N'��������')), (SELECT ID FROM Countries WHERE NameShort = N'��������'));
GO
-- ���������� ������� �������
INSERT INTO Addresses (IndexAddress, PersonID, CountryID, RegionID, CityID, Street, Building, Office) VALUES
(N'101000',
    (SELECT ID FROM Persons WHERE Email = N'ivanov.ii@example.com'),
    (SELECT ID FROM Countries WHERE NameShort = N'������'),
    (SELECT ID FROM Regions WHERE Name = N'���������� �������'),
    (SELECT ID FROM Cities WHERE Name = N'������'),
    N'��. ��������', N'10', N'5�'),
(N'220005',
    (SELECT ID FROM Persons WHERE Email = N'petrov.pp@example.com'),
    (SELECT ID FROM Countries WHERE NameShort = N'��������'),
    (SELECT ID FROM Regions WHERE Name = N'������� �������'),
    (SELECT ID FROM Cities WHERE Name = N'�����'),
    N'��. �������������', N'15', NULL);
GO

-- �������� ������ (�����������)
SELECT * FROM Countries;
SELECT * FROM Regions;
SELECT * FROM Cities;
SELECT * FROM Persons;
SELECT * FROM Addresses;
GO