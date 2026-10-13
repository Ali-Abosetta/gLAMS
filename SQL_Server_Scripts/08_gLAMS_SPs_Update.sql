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
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE [Structure].[Folders]	SET
				[ParentID] = @ParentID,
				[Name] = @Name
		WHERE	[FolderID] = @FolderID
			AND [IsDeleted] = 0

		SET @RowsAffected = @@ROWCOUNT
		EXEC [Structure].[usp_GetFolderById] @FolderID = @FolderID;
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

-- update course
CREATE PROCEDURE [Structure].[usp_UpdateCourse]
	@CourseID UNIQUEIDENTIFIER,
	@FolderID UNIQUEIDENTIFIER,
	@Name NVARCHAR(150),
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE [Structure].[Courses]	SET
			[FolderID] = @FolderID,
			[Name] = @Name
		WHERE	[CourseID] = @CourseID
			AND [IsDeleted] = 0
	
		SET @RowsAffected = @@ROWCOUNT
		EXEC [Structure].[usp_GetCourseById] @CourseID = @CourseID;
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
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE [Structure].[Lessons]	SET
			[CourseID] = @CourseID,
			[Title] = @Title,
			[SortOrder] = @SortOrder
		WHERE	[LessonID] = @LessonID
			AND [IsDeleted] = 0

		SET @RowsAffected = @@ROWCOUNT
		EXEC [Structure].[usp_GetLessonById] @LessonID = @LessonID;
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
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE [Content].[FlashCards]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[FrontContent] = @FrontContent,
			[BackContent] = @BackContent
		WHERE	[FlashCardID] = @FlashCardID
			AND [IsDeleted] = 0
	
	SET @RowsAffected = @@ROWCOUNT
	EXEC [Content].[usp_GetFlashCardById] @FlashCardID = @FlashCardID;
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
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE [Content].[Notes]	SET
			[CourseID] = @CourseID,
			[LessonID] = @LessonID,
			[Title] = @Title,
			[NoteText] = @NoteText
		WHERE	[NoteID] = @NoteID
			AND [IsDeleted] = 0

		SET @RowsAffected = @@ROWCOUNT
		EXEC [Content].[usp_GetNoteById] @NoteId = @NoteID;
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
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE [Content].[References]	SET
				[CourseID] = @CourseID,
				[LessonID] = @LessonID,
				[Title] = @Title,
				[ReferenceType] = @ReferenceType,
				[ContentValue] = @ContentValue
		WHERE	[ReferenceID] = @ReferenceID
		
		SET @RowsAffected = @@ROWCOUNT

		EXEC [Content].[usp_GetReferenceById] @ReferenceID = @ReferenceID;
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
		-- 1. Update the base question
		EXEC [Content].[usp_UpdateQuestion]
			@QuestionID = @QuestionID,
			@CourseID = @CourseID,
			@LessonID = @LessonID,
			@QuestionType = 1, -- Hardcode Type 1 (MCQ)
			@QuestionText = @QuestionText,
			@RowsAffected = @RowsAffected OUTPUT;
		-- 2. If question exists and was updated, process the options
		IF (@RowsAffected > 0)
		BEGIN
			-- Parse the JSON into #json
			SELECT 
				OptionID,
				AnswerText,
				IsCorrect,
				SortOrder
			INTO #json
			FROM OPENJSON(@OptionsJson)
			WITH (
				OptionID UNIQUEIDENTIFIER '$.Id',
				AnswerText NVARCHAR(500) '$.AnswerText',
				IsCorrect BIT '$.IsCorrect',
				SortOrder INT '$.SortOrder'
			);
			-- 3. Update existing options in place (OptionID is preserved!)
			UPDATE opt
			SET 
				opt.AnswerText = j.AnswerText,
				opt.IsCorrect = j.IsCorrect,
				opt.SortOrder = j.SortOrder
			FROM [Content].[McqOptions] opt
			JOIN #json j ON opt.OptionID = j.OptionID
			WHERE opt.QuestionID = @QuestionID
			  AND opt.IsDeleted = 0;

			-- 4. Insert any newly added options (not found in table)
			INSERT INTO [Content].[McqOptions]
			(
				[OptionID],
				[QuestionID],
				[AnswerText],
				[IsCorrect],
				[SortOrder]
			)
			SELECT 
				j.OptionID,
				@QuestionID,
				j.AnswerText,
				j.IsCorrect,
				j.SortOrder
			FROM #json j
			WHERE NOT EXISTS (
				SELECT 1 
				FROM [Content].[McqOptions] opt
				WHERE opt.OptionID = j.OptionID
			);

			-- 5. Soft-delete options that were omitted from #json
			UPDATE opt
			SET opt.IsDeleted = 1
			FROM [Content].[McqOptions] opt
			WHERE opt.QuestionID = @QuestionID
			  AND opt.IsDeleted = 0
			  AND NOT EXISTS (
				  SELECT 1 
				  FROM #json j
				  WHERE j.OptionID = opt.OptionID
			  );
			-- 6. Return the updated question with its choices
			EXEC [Content].[usp_GetMcqQuestionById] @QuestionID = @QuestionID;
			COMMIT TRANSACTION;
		END
		ELSE
		BEGIN
			-- Base question not found or already deleted
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
		
		EXEC [Content].[usp_GetEssayQuestionById] @QuestionID = @QuestionID;
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
