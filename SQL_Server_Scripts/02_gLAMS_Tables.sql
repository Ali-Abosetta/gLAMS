USE [gLAMS]
GO

-- creat table 'Folders'
CREATE TABLE [Structure].[Folders]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		FolderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		ParentID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES [Structure].[Folders](FolderID),
		Name NVARCHAR(150) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'Courses'
CREATE TABLE [Structure].[Courses]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		FolderID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Folders](FolderID),
		Name NVARCHAR(150) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'Lessons'
CREATE TABLE [Structure].[Lessons]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		LessonID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Courses](CourseID),
		Title NVARCHAR(200) NOT NULL,
		SortOrder INT NOT NULL, -- this will be MAX(SortOrder) + 1 WHERE CourseID = @CourseID in the SP
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'Questions'
CREATE TABLE [Content].[Questions]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		QuestionID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Courses](CourseID),
		LessonID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES [Structure].[Lessons](LessonID),
		QuestionType TINYINT NOT NULL, -- 1 MCQ, 2 Essay
		QuestionText NVARCHAR(MAX) NOT NULL, 
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)

		CONSTRAINT CHK_QuestionType CHECK (QuestionType IN (1, 2))
	)
GO

-- create table 'McqOptions'
CREATE TABLE [Content].[McqOptions]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		OptionID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		QuestionID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Content].[Questions](QuestionID),
		AnswerText NVARCHAR(500) NOT NULL,
		IsCorrect BIT NOT NULL,
		SortOrder INT NOT NULL, -- this will be MAX(SortOrder) + 1 WHERE QuestionID = @QuestionID in the SP
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'EssayAnswers'
CREATE TABLE [Content].[EssayAnswers]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		QuestionID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED
			FOREIGN KEY REFERENCES [Content].[Questions](QuestionID),
		ModelAnswer NVARCHAR(MAX) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'FlashCards'
CREATE TABLE [Content].[FlashCards]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		FlashCardID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Courses](CourseID),
		LessonID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES [Structure].[Lessons](LessonID),
		FrontContent NVARCHAR(MAX) NOT NULL,
		BackContent NVARCHAR(MAX) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table Notes
CREATE TABLE [Content].[Notes]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		NoteID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Courses](CourseID),
		LessonID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES [Structure].[Lessons](LessonID),
		Title NVARCHAR(200) NOT NULL,
		NoteText NVARCHAR(MAX) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)
	)
GO

-- create table 'References'
CREATE TABLE [Content].[References]
	(
		SortID BIGINT IDENTITY(1,1) NOT NULL UNIQUE CLUSTERED,
		ReferenceID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
		CourseID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Structure].[Courses](CourseID),
		LessonID UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES [Structure].[Lessons](LessonID),
		Title NVARCHAR(200) NOT NULL,
		ReferenceType TINYINT NOT NULL, -- 1 = File, 2 = URL, 3 = Text
		ContentValue NVARCHAR(MAX) NOT NULL,
		CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		UpdatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
		IsDeleted BIT NOT NULL DEFAULT(0)

		CONSTRAINT CHK_ReferenceType CHECK (ReferenceType IN (1, 2, 3))
	)
GO


-- Idexes:
-- Unique names for Sub-Folders (where ParentID is NOT NULL)
CREATE UNIQUE NONCLUSTERED INDEX UQ_Folders_ParentID_Name
ON [Structure].[Folders](ParentID, Name)
WHERE IsDeleted = 0 AND ParentID IS NOT NULL;

-- Unique names for Root Folders (where ParentID IS NULL)
CREATE UNIQUE NONCLUSTERED INDEX UQ_Folders_Root_Name
ON [Structure].[Folders](Name)
WHERE IsDeleted = 0 AND ParentID IS NULL;

-- Unique names for Courses inside a Folder
CREATE UNIQUE NONCLUSTERED INDEX UQ_Courses_FolderID_Name
ON [Structure].[Courses](FolderID, Name)
WHERE IsDeleted = 0;

-- Add Nonclustered Indexes for all Foreign Keys
CREATE NONCLUSTERED INDEX IX_Folders_ParentID ON [Structure].[Folders](ParentID);
CREATE NONCLUSTERED INDEX IX_Courses_FolderID ON [Structure].[Courses](FolderID);
CREATE NONCLUSTERED INDEX IX_Lessons_CourseID ON [Structure].[Lessons](CourseID);

CREATE NONCLUSTERED INDEX IX_Questions_CourseID ON [Content].[Questions](CourseID);
CREATE NONCLUSTERED INDEX IX_Questions_LessonID ON [Content].[Questions](LessonID);

CREATE NONCLUSTERED INDEX IX_McqOptions_QuestionID ON [Content].[McqOptions](QuestionID);

CREATE NONCLUSTERED INDEX IX_FlashCards_CourseID ON [Content].[FlashCards](CourseID);
CREATE NONCLUSTERED INDEX IX_FlashCards_LessonID ON [Content].[FlashCards](LessonID);

CREATE NONCLUSTERED INDEX IX_Notes_CourseID ON [Content].[Notes](CourseID);
CREATE NONCLUSTERED INDEX IX_Notes_LessonID ON [Content].[Notes](LessonID);

CREATE NONCLUSTERED INDEX IX_References_CourseID ON [Content].[References](CourseID);
CREATE NONCLUSTERED INDEX IX_References_LessonID ON [Content].[References](LessonID);
GO

-- Sync Engine Indexes (Performance optimization for GET procedures)
CREATE NONCLUSTERED INDEX IX_Folders_Sync ON [Structure].[Folders](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_Courses_Sync ON [Structure].[Courses](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_Lessons_Sync ON [Structure].[Lessons](IsDeleted, UpdatedAt);

CREATE NONCLUSTERED INDEX IX_Questions_Sync ON [Content].[Questions](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_McqOptions_Sync ON [Content].[McqOptions](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_EssayAnswers_Sync ON [Content].[EssayAnswers](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_FlashCards_Sync ON [Content].[FlashCards](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_Notes_Sync ON [Content].[Notes](IsDeleted, UpdatedAt);
CREATE NONCLUSTERED INDEX IX_References_Sync ON [Content].[References](IsDeleted, UpdatedAt);
GO