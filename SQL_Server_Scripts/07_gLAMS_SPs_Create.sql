USE [gLAMS]
GO

-- Stored Procedures:

-- Inseart Stored Procedures:
-- sp add folder
CREATE PROCEDURE [Structure].[usp_AddFolder]

	@FolderID UNIQUEIDENTIFIER,
	@ParentID UNIQUEIDENTIFIER = NULL,
	@Name NVARCHAR(150)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [Structure].[Folders]
	(
		[FolderID],
		[ParentID],
		[Name]
	) VALUES
	(
		@FolderID,
		@ParentID,
		@Name
	)

	EXEC [Structure].[usp_GetFolderById] @FolderID = @FolderID;
END
GO

-- sp add course
CREATE PROCEDURE [Structure].[usp_AddCourse]

	@CourseID UNIQUEIDENTIFIER,
	@FolderID UNIQUEIDENTIFIER,
	@Name NVARCHAR(150)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [Structure].[Courses]
	(
		[CourseID],
		[FolderID],
		[Name]
	) VALUES
	(
		@CourseID,
		@FolderID,
		@Name
	)
END
GO

-- sp add lesson
CREATE PROCEDURE [Structure].[usp_AddLesson]

	@LessonID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@Title NVARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @NextSort INT;

-- Use UPDLOCK and HOLDLOCK to serialize concurrent inserts
	SELECT @NextSort = ISNULL(MAX(SortOrder), 0) + 1 
	FROM [Structure].[Lessons] WITH (UPDLOCK, HOLDLOCK)
	WHERE CourseID = @CourseID;

	INSERT INTO [Structure].[Lessons]
	(
		[LessonID],
		[CourseID],
		[Title],
		[SortOrder]
	) VALUES
	(
		@LessonID,
		@CourseID,
		@Title,
		@NextSort
	)
END
GO

-- sp add new qustion (the base table)
CREATE PROCEDURE [Content].[usp_AddQuestion]

	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionType TINYINT,
	@QuestionText NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@QuestionType < 1 OR @QuestionType > 3)
	BEGIN
		;THROW 51000, 'QuestionType must be between 1 and 3.', 1;
	END

	INSERT INTO [Content].[Questions]
	(
		[QuestionID],
		[CourseID],
		[LessonID],
		[QuestionType],
		[QuestionText]
	) VALUES
	(
		@QuestionID,
		@CourseID,
		@LessonID,
		@QuestionType,
		@QuestionText
	)
END
GO

-- sp add new mcq option
CREATE PROCEDURE [Content].[usp_AddMcqOption]

	@OptionID UNIQUEIDENTIFIER,
	@QuestionID UNIQUEIDENTIFIER,
	@AnswerText NVARCHAR(500),
	@IsCorrect BIT

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @NextSort INT;

	-- Use UPDLOCK and HOLDLOCK to serialize concurrent inserts
	SELECT @NextSort = ISNULL(MAX(SortOrder), 0) + 1 
	FROM [Content].[McqOptions] WITH (UPDLOCK, HOLDLOCK)
	WHERE QuestionID = @QuestionID;

	INSERT INTO [Content].[McqOptions]
	(
		[OptionID],
		[QuestionID],
		[AnswerText],
		[IsCorrect],
		[SortOrder]
	) VALUES
	(
		@OptionID,
		@QuestionID,
		@AnswerText,
		@IsCorrect,
		@NextSort
	)
END
GO

-- sp add new mcq question
CREATE PROCEDURE [Content].[usp_AddMcqQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionText NVARCHAR(MAX),
	@OptionsJSON NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [Content].[usp_AddQuestion]
			@QuestionID = @QuestionID,
			@CourseID = @CourseID,
			@LessonID = @LessonID,
			@QuestionType = 1, -- the question type int
			@QuestionText = @QuestionText			

		INSERT INTO [Content].[McqOptions]
		(
			[OptionID],
			[QuestionID],
			[AnswerText],
			[IsCorrect],
			[SortOrder]
		)
		SELECT 
			OptionID,
			@QuestionID, -- We reuse the parameter from the top
			AnswerText,
			IsCorrect,
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) -- Generates 1, 2, 3, 4 automatically
		FROM OPENJSON(@OptionsJSON)
		WITH (
			OptionID UNIQUEIDENTIFIER '$.OptionID',
			AnswerText NVARCHAR(500) '$.AnswerText',
			IsCorrect BIT '$.IsCorrect'
			);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
		BEGIN 
			ROLLBACK TRANSACTION;
		END
		;THROW;
	END CATCH
END
GO

-- sp add new essay question
CREATE PROCEDURE [Content].[usp_AddEssayQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionText NVARCHAR(MAX),
	@ModelAnswer NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [Content].[usp_AddQuestion] 
			@QuestionID = @QuestionID,
			@CourseID = @CourseID,
			@LessonID = @LessonID,
			@QuestionType = 2, -- the question type int
			@QuestionText = @QuestionText

		INSERT INTO [Content].[EssayAnswers]
		(
			[QuestionID],
			[ModelAnswer]
		) VALUES
		(
			@QuestionID,
			@ModelAnswer
		)

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
		BEGIN 
			ROLLBACK TRANSACTION;
		END
		;THROW;
	END CATCH
END
GO

-- sp add falsh card
CREATE PROCEDURE [Content].[usp_AddFlashCard]

	@FlashCardID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@FrontContent NVARCHAR(MAX),
	@BackContent NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [Content].[FlashCards]
	(
		[FlashCardID],
		[CourseID],
		[LessonID],
		[FrontContent],
		[BackContent]
	) VALUES
	(
		@FlashCardID,
		@CourseID,
		@LessonID,
		@FrontContent,
		@BackContent
	)
END
GO

-- sp add a Note
CREATE PROCEDURE [Content].[usp_AddNote]

	@NoteID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@Title NVARCHAR(200),
	@NoteText NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [Content].[Notes]
	(
		[NoteID],
		[CourseID],
		[LessonID],
		[Title],
		[NoteText]
	) VALUES
	(
		@NoteID,
		@CourseID,
		@LessonID,
		@Title,
		@NoteText
	)
END
GO

-- sp add a refrence
CREATE PROCEDURE [Content].[usp_AddReference]

	@ReferenceID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@Title NVARCHAR(200),
	@ReferenceType TINYINT,
	@ContentValue NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [Content].[References]
	(
		[ReferenceID],
		[CourseID],
		[LessonID],
		[Title],
		[ReferenceType],
		[ContentValue]
	) VALUES
	(
		@ReferenceID,
		@CourseID,
		@LessonID,
		@Title,
		@ReferenceType,
		@ContentValue
	)
END
GO

