--
-- Um mehr Test Datensätze in der Datenbank zu generieren, muss das
-- Programm KöTaf.RandomGenerator verwendet werden!
--

-- 
-- Table: Account
--
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'MM',N'123456',0,N'bild2.png',1);
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'MK',N'123456',0,N'mk.png',1);
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'BL',N'123456',0,N'bl.png',1);
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'KG',N'123456',0,N'kg.png',1);
INSERT INTO [UserAccounts] ([Username],[Password],[IsAdmin],[ImageName],[IsActive]) VALUES (N'GS',N'1',0,N'bild1.png',1);

-- 
-- Table: Persons
--
INSERT INTO [Persons] ([FirstName],[LastName],[Street],[Nationality],[DateOfBirth],[CountryOfBirth],[IsActive],[Group],[ZipCode],[ValidityStart],[ValidityEnd],[Email],[MobileNo],[Phone],[MaritalFirstName],[MaritalLastName],[MaritalBirthday],[MaritalNationality],[CreationDate],[LastPurchase],[LastModified],[Comment],[City],[TableNo],[Title_TitleID],[FamilyState_FamilyStateID]) VALUES (N'Maximilian',N'Mustermann',N'Musterstraße',N'Albanisch',{ts '1987-04-13 00:00:00.000'},N'Albanien',1,1,80687,{ts '2000-04-13 00:00:00.000'},{ts '2013-04-18 00:00:00.000'},N'mustermann@muster.de',N'923858427',N'35624387',NULL,NULL,NULL,NULL,{ts '2012-04-18 00:00:00.000'},{ts '2013-04-17 00:00:00.000'},{ts '2013-04-17 00:00:00.000'},N'Irgendein Kommentar',N'Musterstadt',2,1,1);
INSERT INTO [Persons] ([FirstName],[LastName],[Street],[Nationality],[DateOfBirth],[CountryOfBirth],[IsActive],[Group],[ZipCode],[ValidityStart],[ValidityEnd],[Email],[MobileNo],[Phone],[MaritalFirstName],[MaritalLastName],[MaritalBirthday],[MaritalNationality],[CreationDate],[LastPurchase],[LastModified],[Comment],[City],[TableNo],[Title_TitleID],[FamilyState_FamilyStateID]) VALUES (N'Kirsten',N'Gedönikas',N'Gedönsstraße 23',N'Serbisch',{ts '1990-06-15 00:00:00.000'},NULL,1,2,54322,{ts '2000-04-12 00:00:00.000'},{ts '2013-04-19 00:00:00.000'},NULL,N'4378388',N'98892398',NULL,NULL,NULL,NULL,{ts '1753-04-19 00:00:00.000'},NULL,{ts '2012-02-13 00:00:00.000'},N'eshfdidhfgiudfn',N'Musterhausen',1,2,1);

--
-- Table: Teams
--
INSERT INTO [Teams]([Title_TitleID],[TeamFunction_TeamFunctionID],[FirstName],[LastName],[IsActive],[Street],[ZipCode],[City],[MobileNo],[PhoneNo],[Email],[IsFormLetterAllowed],[DateOfBirth]) 
VALUES(1,1,'Karsten','Müller',1,'Müllerstraße 23',22123,'München','0891234332','01765344433','karstenM@gmx.de',1,'1980-01-15');
INSERT INTO [Teams]([Title_TitleID],[TeamFunction_TeamFunctionID],[FirstName],[LastName],[IsActive],[Street],[ZipCode],[City],[MobileNo],[PhoneNo],[Email],[IsFormLetterAllowed],[DateOfBirth]) 
VALUES(2,3,'Sabine','Kuschinski',1,'Alphametastraße 321a',12334,'Dresden','','','sm@googlemail.com',0,'1970-08-15');
INSERT INTO [Teams]([Title_TitleID],[TeamFunction_TeamFunctionID],[FirstName],[LastName],[IsActive],[Street],[ZipCode],[City],[MobileNo],[PhoneNo],[Email],[IsFormLetterAllowed],[DateOfBirth]) 
VALUES(1,2,'Hubert','Zucker',1,'Hurzknoten 23a',22123,'München','0891234332','01765344433','zuckerliHubi@t-online.de',1,'1965-02-17');