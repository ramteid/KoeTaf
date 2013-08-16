--
-- Base Data Script
--
-- Last Modification: 2013-07-21 / Florian Wasielewski
--
-- 
-- Table: Title
--
INSERT INTO [Titles]([Name]) VALUES('Herr');
INSERT INTO [Titles]([Name]) VALUES('Frau');

-- 
-- Table: FamilyState
-- URL: http://de.wikipedia.org/wiki/Familienstand
--
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Ledig','LD',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Verheiratet','VH',2);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Getrennt lebend','GL',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Geschieden','GS',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Verwitwet','VW',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Lebenspartnerschaft','LP',2);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Lebenspartnerschaft aufgehoben','LA',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Lebenspartner verstorben','LV',1);
INSERT INTO [FamilyStates]([Name],[ShortName],[NumberOfPerson]) VALUES('Familienstand unbekannt','FU',1);

--
-- Table: TeamFunctions
--
INSERT INTO [TeamFunctions]([Name]) VALUES('Fahrer');
INSERT INTO [TeamFunctions]([Name]) VALUES('Büro');
INSERT INTO [TeamFunctions]([Name]) VALUES('Ausgabe');

--
-- Table: FundingTypes
--
INSERT INTO [FundingTypes]([Name]) VALUES('Lebensmittel');
INSERT INTO [FundingTypes]([Name]) VALUES('Finanziell');

-- 
-- Table: RevenueType
--
INSERT INTO [RevenueTypes]([Name]) VALUES('Waisenrente');
INSERT INTO [RevenueTypes]([Name]) VALUES('Rente');
INSERT INTO [RevenueTypes]([Name]) VALUES('Wohngeld');
INSERT INTO [RevenueTypes]([Name]) VALUES('ALG 1/2');
INSERT INTO [RevenueTypes]([Name]) VALUES('Sonstiges');

-- 
-- Table: Account
--
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'ADMIN',N'ADMIN',1,NULL,1);
