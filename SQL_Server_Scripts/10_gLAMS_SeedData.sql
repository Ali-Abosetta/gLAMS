USE [gLAMS]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	PRINT 'Starting Mass Data Seed...'

	DECLARE @i INT = 1;
	DECLARE @j INT;
	DECLARE @k INT;
	DECLARE @l INT;
	
	DECLARE @RootFolderID UNIQUEIDENTIFIER;
	DECLARE @SubFolderID UNIQUEIDENTIFIER;
	DECLARE @CourseID UNIQUEIDENTIFIER;
	DECLARE @LessonID UNIQUEIDENTIFIER;
	DECLARE @FlashCardID UNIQUEIDENTIFIER;
	
	DECLARE @NameString NVARCHAR(150);
	DECLARE @ContentString NVARCHAR(MAX);
	DECLARE @ContentString2 NVARCHAR(MAX);

	-- Loop 5 times to create 5 Root Folders
	WHILE @i <= 5
	BEGIN
		-- Wait 50 milliseconds so timestamps don't tie!
		WAITFOR DELAY '00:00:00.050';
		
		SET @RootFolderID = NEWID();
		SET @NameString = 'Root Folder ' + CAST(@i AS VARCHAR);
		EXEC [Structure].[usp_AddFolder] 
			@FolderID = @RootFolderID, 
			@ParentID = NULL, 
			@Name = @NameString;

		-- For each Root Folder, create 4 Sub-Folders (Total 20)
		SET @j = 1;
		WHILE @j <= 4
		BEGIN
			-- Wait 50 milliseconds so timestamps don't tie!
			WAITFOR DELAY '00:00:00.050';
			
			SET @SubFolderID = NEWID();
			SET @NameString = 'Sub-Folder ' + CAST(@i AS VARCHAR) + '.' + CAST(@j AS VARCHAR);
			EXEC [Structure].[usp_AddFolder] 
				@FolderID = @SubFolderID, 
				@ParentID = @RootFolderID, 
				@Name = @NameString;

			-- For each Sub-Folder, create 1 Course (Total 20 Courses)
			SET @CourseID = NEWID();
			SET @NameString = 'Course ' + CAST(@i AS VARCHAR) + '.' + CAST(@j AS VARCHAR);
			EXEC [Structure].[usp_AddCourse] 
				@CourseID = @CourseID, 
				@FolderID = @SubFolderID, 
				@Name = @NameString;

			-- For each Course, create 2 Lessons (Total 40 Lessons)
			SET @k = 1;
			WHILE @k <= 2
			BEGIN
			    SET @LessonID = NEWID();
			    SET @NameString = 'Lesson ' + CAST(@i AS VARCHAR) + '-' + CAST(@k AS VARCHAR);
			    EXEC [Structure].[usp_AddLesson] 
					@LessonID = @LessonID, 
					@CourseID = @CourseID, 
					@Title = @NameString;
    
			    -- For each Lesson, create 2 FlashCards (Total 80 FlashCards)
                SET @l = 1;
                WHILE @l <= 2
                BEGIN
			        SET @FlashCardID = NEWID();
			        SET @ContentString = 'Question ' + CAST(@l AS VARCHAR);
			        SET @ContentString2 = 'Answer ' + CAST(@l AS VARCHAR);
			        
			        EXEC [Content].[usp_AddFlashCard] 
						@FlashCardID = @FlashCardID, 
						@CourseID = @CourseID, 
						@LessonID = @LessonID, 
						@FrontContent = @ContentString, 
						@BackContent = @ContentString2;
                    
					SET @l = @l + 1;
                END
                SET @k = @k + 1;
			END

			SET @j = @j + 1;
		END

		SET @i = @i + 1;
	END
    
	PRINT 'Base data created (5 Root Folders, 20 Sub-Folders, 20 Courses, 40 Lessons, 80 Flashcards).'

	-- Randomly Mark some as deleted to simulate real-world data (Tombstones)
	PRINT 'Simulating Soft-Deletes...'
	
	UPDATE TOP(5) [Structure].[Folders] SET IsDeleted = 1 WHERE ParentID IS NOT NULL;
	UPDATE TOP(5) [Structure].[Courses] SET IsDeleted = 1;
	UPDATE TOP(10) [Structure].[Lessons] SET IsDeleted = 1;
	UPDATE TOP(20) [Content].[FlashCards] SET IsDeleted = 1;

	COMMIT TRANSACTION;
	PRINT 'Data successfully seeded and tombstones created!'

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
