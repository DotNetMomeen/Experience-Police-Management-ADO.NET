
CREATE DATABASE ExperiencedPoliceDB
GO

USE ExperiencedPoliceDB
GO

CREATE TABLE Designation(
    DesignationId INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    DesignationTitle VARCHAR (30) NOT NULL
)
GO

INSERT INTO Designation (DesignationTitle) VALUES
('Constable'),
('Nayek'),
('Assistant Sub-Inspector'),
('Sergeant'),
('Sub-Inspector'),
('Captain')
GO

CREATE TABLE Police (
    PoliceId INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    PoliceCode	VARCHAR (10)  NOT NULL,
    PoliceName	VARCHAR (30)  NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    IsPermanent	BIT NOT NULL,
    Gender	CHAR (10)      NOT NULL,
    ImagePath VARCHAR (MAX) NULL,
    DesignationId INT REFERENCES Designation(DesignationId)
)
GO

CREATE TABLE Experience (
    ExperienceId INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    PoliceStationName  VARCHAR (25) NOT NULL,
    YearsWorked  INT NOT NULL,
	PoliceId INT REFERENCES Police(PoliceId)
)
GO

CREATE PROCEDURE PoliceExperienceAddAndEdit
@ExperienceId INT,
@PoliceStationName VARCHAR(25),
@YearsWorked INT,
@PoliceId INT
AS
if @ExperienceId=0
BEGIN

    IF @ExperienceId = 0
    BEGIN
        -- Insert new experience
        INSERT INTO Experience (PoliceStationName, YearsWorked, PoliceId)
        VALUES (@PoliceStationName, @YearsWorked, @PoliceId);
    END
    ELSE
    BEGIN
        -- Update existing experience
        UPDATE Experience
        SET PoliceStationName = @PoliceStationName,
            YearsWorked = @YearsWorked,
            PoliceId = @PoliceId
        WHERE ExperienceId = @ExperienceId
    END
END
GO


CREATE PROC PoliceAddOrEdit
@PoliceId INT,
@PoliceCode VARCHAR(10),
@PoliceName VARCHAR(30),
@DateOfBirth DATETIME,
@IsPermanent BIT,
@DesignationId INT,
@ImagePath VARCHAR(250),
@Gender CHAR(6)
AS
BEGIN
--Insert---
IF @PoliceId=0
BEGIN
INSERT INTO Police(PoliceCode,PoliceName,DateOfBirth,IsPermanent,DesignationId,ImagePath,Gender) VALUES
(@PoliceCode,@PoliceName,@DateOfBirth,@IsPermanent,@DesignationId,@ImagePath,@Gender)
SELECT SCOPE_IDENTITY();
END
ELSE
BEGIN
--Update
UPDATE Police SET 
PoliceCode=@PoliceCode,PoliceName=@PoliceName,DateOfBirth=@DateOfBirth,IsPermanent=@IsPermanent,DesignationId=@DesignationId,ImagePath=@ImagePath,Gender=@Gender WHERE PoliceId=@PoliceId
SELECT @PoliceId
END
END


CREATE PROC PoliceExperienceDelete
@PoliceId INT
AS
BEGIN
---Details---
DELETE FROM Experience WHERE PoliceId =@PoliceId 
---Master---
DELETE FROM Police WHERE PoliceId =@PoliceId 
END


CREATE PROC ExperienceDelete
@ExperienceId int
AS
BEGIN
DELETE FROM Experience WHERE ExperienceId=@ExperienceId
END


CREATE PROC ViewAllPolice
AS
BEGIN
SELECT p.PoliceId,p.PoliceCode,p.PoliceName,p.DateOfBirth,p.Gender,p.IsPermanent ,
(SELECT SUM(YearsWorked) FROM Experience WHERE PoliceId=p.PoliceId) AS TotalExperience,
d.DesignationTitle,p.ImagePath
FROM Police p JOIN Designation d
ON p.DesignationId=d.DesignationId
END




CREATE PROC ViewPoliceByPoliceId
@PoliceId int
AS
BEGIN
---Master---
SELECT *  FROM Police WHERE PoliceId=@PoliceId
---Details---
SELECT * FROM   Experience WHERE PoliceId=@PoliceId
END