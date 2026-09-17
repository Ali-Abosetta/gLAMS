USE [gLAMS]
GO

-- Stored Procedures:

-- get Stored Procedures:

-- get folders
CREATE PROCEDURE [Structure].[usp_GetFolders]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LastSyncTime IS NULL)
	BEGIN
		-- Initial Load: Fetch only active records
		SELECT 
			[FolderID],
			[ParentID],
			[Name],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Folders]
		WHERE [IsDeleted] = 0;
	END
	ELSE
	BEGIN
		-- Delta Sync: Fetch ALL records (including soft-deleted) that changed recently
		SELECT 
			[FolderID],
			[ParentID],
			[Name],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Folders]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get courses
CREATE PROCEDURE [Structure].[usp_GetCourses]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[CourseID],
			[FolderID],
			[Name],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Courses]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[CourseID],
			[FolderID],
			[Name],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Courses]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get lessons
CREATE PROCEDURE [Structure].[usp_GetLessons]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[LessonID],
			[CourseID],
			[Title],
			[SortOrder],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Lessons]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN 
		SELECT 
			[LessonID],
			[CourseID],
			[Title],
			[SortOrder],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Lessons]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get flash cards
CREATE PROCEDURE [Content].[usp_GetFlashCards]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[FlashCardID],
			[CourseID],
			[LessonID],
			[FrontContent],
			[BackContent],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[FlashCards]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[FlashCardID],
			[CourseID],
			[LessonID],
			[FrontContent],
			[BackContent],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[FlashCards]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get notes
CREATE PROCEDURE [Content].[usp_GetNotes]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[NoteID],
			[CourseID],
			[LessonID],
			[Title],
			[NoteText],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[Notes]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[NoteID],
			[CourseID],
			[LessonID],
			[Title],
			[NoteText],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[Notes]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get refrences
CREATE PROCEDURE [Content].[usp_GetReferences]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[ReferenceID],
			[CourseID],
			[LessonID],
			[Title],
			[ReferenceType],
			[ContentValue],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[References]
		WHERE [IsDeleted] = 0;
	END

	ELSE 
	BEGIN
		SELECT 
			[ReferenceID],
			[CourseID],
			[LessonID],
			[Title],
			[ReferenceType],
			[ContentValue],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[References]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get questions
CREATE PROCEDURE [Content].[usp_GetQuestions]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[QuestionID],
			[CourseID],
			[LessonID],
			[QuestionType],
			[QuestionText],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[Questions]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[QuestionID],
			[CourseID],
			[LessonID],
			[QuestionType],
			[QuestionText],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[Questions]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get mcq options
CREATE PROCEDURE [Content].[usp_GetMcqOptions]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[OptionID],
			[QuestionID],
			[AnswerText],
			[IsCorrect],
			[SortOrder],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[McqOptions]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[OptionID],
			[QuestionID],
			[AnswerText],
			[IsCorrect],
			[SortOrder],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[McqOptions]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

-- get essay answers
CREATE PROCEDURE [Content].[usp_GetEssayAnswers]
	@LastSyncTime DATETIME2 = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF (@LastSyncTime IS NULL)
	BEGIN
		SELECT 
			[QuestionID],
			[ModelAnswer],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[EssayAnswers]
		WHERE [IsDeleted] = 0;
	END

	ELSE
	BEGIN
		SELECT 
			[QuestionID],
			[ModelAnswer],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Content].[EssayAnswers]
		WHERE [UpdatedAt] > @LastSyncTime;
	END
END
GO

CREATE PROCEDURE [Content].[usp_GetActiveMcqsByCourse]
	@CourseID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		[QuestionID],
		[CourseID],
		[LessonID],
		[QuestionText],
		[QuestionUpdatedAt],
		[OptionID],
		[AnswerText],
		[IsCorrect],
		[SortOrder]
	FROM [Content].[vw_ActiveMcqQuestions]
	WHERE [CourseID] = @CourseID;
END
GO

CREATE PROCEDURE [Content].[usp_GetActiveEssaysByCourse]
	@CourseID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		[QuestionID],
		[CourseID],
		[LessonID],
		[QuestionText],
		[UpdatedAt],
		[ModelAnswer]
	FROM [Content].[vw_ActiveEssayQuestions]
	WHERE [CourseID] = @CourseID;
END
GO
