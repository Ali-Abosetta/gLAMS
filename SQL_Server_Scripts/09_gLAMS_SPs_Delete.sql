USE [gLAMS]
GO

-- Delete Stored Prcedures:
-- delete folder:
CREATE PROCEDURE [Structure].[usp_DeleteFolder]

	@FolderID UNIQUEIDENTIFIER,
	@RowsAffected INT = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Structure].[Folders]
	WHERE	[FolderID] = @FolderID
	
	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
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
    WHERE OptionID = @OptionID;

	SET @RowsAffected = @@ROWCOUNT
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
    WHERE QuestionID = @QuestionID;

	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
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
	
	SET @RowsAffected = @@ROWCOUNT
END
GO

