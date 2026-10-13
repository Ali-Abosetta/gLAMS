USE [gLAMS]
GO

-- Delete Stored Procedures:

-- delete folder:
CREATE PROCEDURE [Structure].[usp_DeleteFolder]
	@FolderID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Structure].[Folders]
	WHERE	[FolderID] = @FolderID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete course:
CREATE PROCEDURE [Structure].[usp_DeleteCourse]
	@CourseID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Structure].[Courses]
	WHERE	[CourseID] = @CourseID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete lesson:
CREATE PROCEDURE [Structure].[usp_DeleteLesson]
	@LessonID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Structure].[Lessons]
	WHERE	[LessonID] = @LessonID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete question:
CREATE PROCEDURE [Content].[usp_DeleteQuestion]
	@QuestionID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Content].[Questions]
	WHERE	[QuestionID] = @QuestionID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete mcq question
CREATE PROCEDURE [Content].[usp_DeleteMcqQuestion]
    @QuestionID UNIQUEIDENTIFIER,
    @RowsAffected INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [Content].[McqOptions]
        WHERE [QuestionID] = @QuestionID
          AND [IsDeleted] = 0;

        EXEC [Content].[usp_DeleteQuestion] 
            @QuestionID = @QuestionID,
            @RowsAffected = @RowsAffected OUTPUT; 

        IF (@RowsAffected > 0)
        BEGIN
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN 
            ROLLBACK TRANSACTION;
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

-- delete mcq option
CREATE PROCEDURE [Content].[usp_DeleteMcqOption]
	@OptionID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
 
	DELETE FROM [Content].[McqOptions] 
    WHERE [OptionID] = @OptionID
      AND [IsDeleted] = 0;

	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete essay answer
CREATE PROCEDURE [Content].[usp_DeleteEssayAnswer]
    @QuestionID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [Content].[EssayAnswers] 
    WHERE [QuestionID] = @QuestionID
      AND [IsDeleted] = 0;

	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete essay question
CREATE PROCEDURE [Content].[usp_DeleteEssayQuestion]
    @QuestionID UNIQUEIDENTIFIER,
    @RowsAffected INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [Content].[EssayAnswers]
        WHERE [QuestionID] = @QuestionID
          AND [IsDeleted] = 0;

        EXEC [Content].[usp_DeleteQuestion] 
            @QuestionID = @QuestionID,
            @RowsAffected = @RowsAffected OUTPUT; 

        IF (@RowsAffected > 0)
        BEGIN
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN 
            ROLLBACK TRANSACTION;
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

-- delete flash card:
CREATE PROCEDURE [Content].[usp_DeleteFlashCard]
	@FlashCardID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Content].[FlashCards]
	WHERE	[FlashCardID] = @FlashCardID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete note:
CREATE PROCEDURE [Content].[usp_DeleteNote]
	@NoteID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Content].[Notes]
	WHERE	[NoteID] = @NoteID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO

-- delete reference
CREATE PROCEDURE [Content].[usp_DeleteReference]
	@ReferenceID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Content].[References]
	WHERE	[ReferenceID] = @ReferenceID
	  AND	[IsDeleted] = 0;
	
	SET @RowsAffected = @@ROWCOUNT;
END
GO
