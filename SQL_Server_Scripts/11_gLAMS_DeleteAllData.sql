USE [gLAMS]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	PRINT 'Starting Database Teardown (Hard Deleting all data)...';
	
	-- Disable all triggers to bypass INSTEAD OF DELETE soft-delete logic
	DISABLE TRIGGER ALL ON [Content].[McqOptions];
	DISABLE TRIGGER ALL ON [Content].[EssayAnswers];
	DISABLE TRIGGER ALL ON [Content].[FlashCards];
	DISABLE TRIGGER ALL ON [Content].[Notes];
	DISABLE TRIGGER ALL ON [Content].[References];
	DISABLE TRIGGER ALL ON [Content].[Questions];
	DISABLE TRIGGER ALL ON [Structure].[Lessons];
	DISABLE TRIGGER ALL ON [Structure].[Courses];
	DISABLE TRIGGER ALL ON [Structure].[Folders];

	-- 1. Delete all Content (Child tables of Questions and Lessons)
	DELETE FROM [Content].[McqOptions];
	DELETE FROM [Content].[EssayAnswers];
	DELETE FROM [Content].[FlashCards];
	DELETE FROM [Content].[Notes];
	DELETE FROM [Content].[References];

	-- 2. Delete the base Questions table
	DELETE FROM [Content].[Questions];

	-- 3. Delete Structure tables (Child to Parent order)
	DELETE FROM [Structure].[Lessons];
	DELETE FROM [Structure].[Courses];

	-- 4. Delete Folders 
	-- We must delete sub-folders first because of the self-referencing Foreign Key!
	DELETE FROM [Structure].[Folders] WHERE ParentID IS NOT NULL;
	DELETE FROM [Structure].[Folders] WHERE ParentID IS NULL;

	-- Re-enable all triggers
	ENABLE TRIGGER ALL ON [Content].[McqOptions];
	ENABLE TRIGGER ALL ON [Content].[EssayAnswers];
	ENABLE TRIGGER ALL ON [Content].[FlashCards];
	ENABLE TRIGGER ALL ON [Content].[Notes];
	ENABLE TRIGGER ALL ON [Content].[References];
	ENABLE TRIGGER ALL ON [Content].[Questions];
	ENABLE TRIGGER ALL ON [Structure].[Lessons];
	ENABLE TRIGGER ALL ON [Structure].[Courses];
	ENABLE TRIGGER ALL ON [Structure].[Folders];

	COMMIT TRANSACTION;
	PRINT 'All data has been successfully hard deleted!'

END TRY
BEGIN CATCH
	IF (@@TRANCOUNT > 0)
	BEGIN
		ROLLBACK TRANSACTION;
	END
	PRINT 'Error Occurred: ' + ERROR_MESSAGE();
	;THROW;
END CATCH
GO
