USE [gLAMS]
GO

-- Update Stored Prcedures:
-- update folder 
CREATE PROCEDURE [Structure].[usp_UpdateFolder]
	@FolderID UNIQUEIDENTIFIER,
	@ParentID UNIQUEIDENTIFIER = NULL,
	@Name NVARCHAR(150),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Structure].[Folders]	SET
			[ParentID] = @ParentID,
			[Name] = @Name
	WHERE	[FolderID] = @FolderID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update course
CREATE PROCEDURE [Structure].[usp_UpdateCourse]
	@CourseID UNIQUEIDENTIFIER,
	@FolderID UNIQUEIDENTIFIER,
	@Name NVARCHAR(150),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Structure].[Courses]	SET
			[FolderID] = @FolderID,
			[Name] = @Name
	WHERE	[CourseID] = @CourseID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update lessons
CREATE PROCEDURE [Structure].[usp_UpdateLesson]
	@LessonID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@Title NVARCHAR(200),
	@SortOrder INT,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Structure].[Lessons]	SET
			[CourseID] = @CourseID,
			[Title] = @Title,
			[SortOrder] = @SortOrder
	WHERE	[LessonID] = @LessonID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update flash cards
CREATE PROCEDURE [Content].[usp_UpdateFlashCard]
	@FlashCardID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@FrontContent NVARCHAR(MAX),
	@BackContent NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Content].[FlashCards]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[FrontContent] = @FrontContent,
			[BackContent] = @BackContent
	WHERE	[FlashCardID] = @FlashCardID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update note
CREATE PROCEDURE [Content].[usp_UpdateNote]
	@NoteID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@Title NVARCHAR(200),
	@NoteText NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Content].[Notes]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[Title] = @Title,
			[NoteText] = @NoteText
	WHERE	[NoteID] = @NoteID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update references
CREATE PROCEDURE [Content].[usp_UpdateReference]
	@ReferenceID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@Title NVARCHAR(200),
	@ReferenceType TINYINT,
	@ContentValue NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Content].[References]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[Title] = @Title,
			[ReferenceType] = @ReferenceType,
			[ContentValue] = @ContentValue
	WHERE	[ReferenceID] = @ReferenceID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update question
CREATE PROCEDURE [Content].[usp_UpdateQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionType TINYINT,
	@QuestionText NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ExistingType TINYINT;
	SELECT @ExistingType = QuestionType 
	FROM [Content].[Questions] 
	WHERE QuestionID = @QuestionID;

	IF (@ExistingType IS NOT NULL AND @ExistingType <> @QuestionType)
	BEGIN
		;THROW 51001, 'Data Integrity Error: You cannot change the QuestionType of an existing question.', 1;
	END

	IF (@QuestionType < 1 OR @QuestionType > 3) 
	BEGIN
		;THROW 51000, 'QuestionType must be between 1 and 3.', 1;
	END

	UPDATE [Content].[Questions]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[QuestionType] = @QuestionType,
			[QuestionText] = @QuestionText
	WHERE	[QuestionID] = @QuestionID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update mcq option
CREATE PROCEDURE [Content].[usp_UpdateMcqOption]
	@OptionID UNIQUEIDENTIFIER,
	@QuestionID UNIQUEIDENTIFIER,
	@AnswerText NVARCHAR(500),
	@IsCorrect BIT,
	@SortOrder INT,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Content].[McqOptions]	SET
			[QuestionID] = @QuestionID,
			[AnswerText] = @AnswerText,
			[IsCorrect] = @IsCorrect,
			[SortOrder] = @SortOrder
	WHERE	[OptionID] = @OptionID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update mcq question
CREATE PROCEDURE [Content].[usp_UpdateMcqQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionText NVARCHAR(MAX),
	@OptionsJson NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [Content].[usp_UpdateQuestion]
			@QuestionID = @QuestionID,
			@CourseID = @CourseID,
			@LessonID = @LessonID,
			@QuestionType = 1, -- Hardcode Type 1 (MCQ)
			@QuestionText = @QuestionText,
			@RowsAffected = @RowsAffected OUTPUT;

		-- If base update succeeded, replace the options
		IF (@RowsAffected > 0)
		BEGIN
			DELETE FROM [Content].[McqOptions]
			WHERE QuestionID = @QuestionID;

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
				@QuestionID, 
				AnswerText,
				IsCorrect,
				ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) 
			FROM OPENJSON(@OptionsJson)
			WITH (
				OptionID UNIQUEIDENTIFIER '$.OptionID',
				AnswerText NVARCHAR(500) '$.AnswerText',
				IsCorrect BIT '$.IsCorrect'
			);

			COMMIT TRANSACTION;
		END
		ELSE
		BEGIN
			-- Base question not found
			ROLLBACK TRANSACTION;
			RETURN;
		END
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

-- update essay answer
CREATE PROCEDURE [Content].[usp_UpdateEssayAnswer]
	@QuestionID UNIQUEIDENTIFIER,
	@ModelAnswer NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Content].[EssayAnswers]	SET
			[ModelAnswer] = @ModelAnswer
	WHERE	[QuestionID] = @QuestionID
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

-- update essay question
CREATE PROCEDURE [Content].[usp_UpdateEssayQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@CourseID UNIQUEIDENTIFIER,
	@LessonID UNIQUEIDENTIFIER = NULL,
	@QuestionText NVARCHAR(MAX),
	@ModelAnswer NVARCHAR(MAX),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [Content].[usp_UpdateQuestion]
			@QuestionID = @QuestionID,
			@CourseID = @CourseID,
			@LessonID = @LessonID,
			@QuestionType = 2,
			@QuestionText = @QuestionText,
			@RowsAffected = @RowsAffected OUTPUT

		IF (@RowsAffected > 0)
		BEGIN
			EXEC [Content].[usp_UpdateEssayAnswer]
				@QuestionID = @QuestionID,
				@ModelAnswer = @ModelAnswer,
				@RowsAffected = @RowsAffected OUTPUT
		END

		ELSE
		BEGIN
			ROLLBACK TRANSACTION;
			RETURN;
		END
		
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
