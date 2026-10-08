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
	DECLARE @NoteID UNIQUEIDENTIFIER;
	DECLARE @ReferenceID UNIQUEIDENTIFIER;
	DECLARE @QuestionID UNIQUEIDENTIFIER;
	
	DECLARE @NameString NVARCHAR(150);
	DECLARE @ContentString NVARCHAR(MAX);
	DECLARE @ContentString2 NVARCHAR(MAX);
	DECLARE @OptionsJSON NVARCHAR(MAX);

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
    
			    -- For each Lesson, create 20 of every content item type
                SET @l = 1;
                WHILE @l <= 20
                BEGIN
			        -- 1. FlashCard
			        SET @FlashCardID = NEWID();
			        SET @ContentString = 'Question ' + CAST(@l AS VARCHAR);
			        SET @ContentString2 = 'Answer ' + CAST(@l AS VARCHAR);
			        EXEC [Content].[usp_AddFlashCard] 
						@FlashCardID = @FlashCardID, 
						@CourseID = @CourseID, 
						@LessonID = @LessonID, 
						@FrontContent = @ContentString, 
						@BackContent = @ContentString2;
                    
			        -- 2. Note
			        SET @NoteID = NEWID();
					SET @ContentString = 'Note ' + CAST(@l AS VARCHAR) + ' for ' + @NameString;
					SET @ContentString2 = 'This is the note body content number ' + CAST(@l AS VARCHAR);
			        EXEC [Content].[usp_AddNote]
						@NoteID = @NoteID,
						@CourseID = @CourseID,
						@LessonID = @LessonID,
						@Title = @ContentString,
						@NoteText = @ContentString2;

			        -- 3. Reference (URL Type)
			        SET @ReferenceID = NEWID();
					SET @ContentString = 'External Reference ' + CAST(@l AS VARCHAR);
					SET @ContentString2 = 'https://docs.microsoft.com/en-us/dotnet/csharp/';
			        EXEC [Content].[usp_AddReference]
						@ReferenceID = @ReferenceID,
						@CourseID = @CourseID,
						@LessonID = @LessonID,
						@Title = @ContentString,
						@ReferenceType = 2,
						@ContentValue = @ContentString2;

			        -- 4. MCQ Question
			        SET @QuestionID = NEWID();
			        SET @OptionsJSON = N'[
			            {"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Option A", "IsCorrect": true},
			            {"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Option B", "IsCorrect": false},
			            {"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Option C", "IsCorrect": false}
			        ]';
					SET @ContentString = 'Which of the following is correct? (MCQ ' + CAST(@l AS VARCHAR) + ')';
			        EXEC [Content].[usp_AddMcqQuestion]
			            @QuestionID = @QuestionID,
			            @CourseID = @CourseID,
			            @LessonID = @LessonID,
			            @QuestionText = @ContentString,
			            @OptionsJSON = @OptionsJSON;

			        -- 5. Essay Question
			        SET @QuestionID = NEWID();
					SET @ContentString = 'Explain the concepts discussed in this lesson. (Essay ' + CAST(@l AS VARCHAR) + ')';
					SET @ContentString2 = 'The concepts involve deep architectural knowledge and standard REST patterns.';
			        EXEC [Content].[usp_AddEssayQuestion]
			            @QuestionID = @QuestionID,
			            @CourseID = @CourseID,
			            @LessonID = @LessonID,
			            @QuestionText = @ContentString,
			            @ModelAnswer = @ContentString2;

					SET @l = @l + 1;
                END
                SET @k = @k + 1;
			END

			SET @j = @j + 1;
		END

		SET @i = @i + 1;
	END
    
	PRINT 'Base data created (5 Root Folders, 20 Sub-Folders, 20 Courses, 40 Lessons, 800 of each content type).'

	-- Randomly Mark some as deleted to simulate real-world data (Tombstones)
	PRINT 'Simulating Soft-Deletes...'
	
	UPDATE TOP(5) [Structure].[Folders] SET IsDeleted = 1 WHERE ParentID IS NOT NULL;
	UPDATE TOP(5) [Structure].[Courses] SET IsDeleted = 1;
	UPDATE TOP(10) [Structure].[Lessons] SET IsDeleted = 1;
	UPDATE TOP(200) [Content].[FlashCards] SET IsDeleted = 1;
	UPDATE TOP(200) [Content].[Notes] SET IsDeleted = 1;
	UPDATE TOP(200) [Content].[References] SET IsDeleted = 1;
	UPDATE TOP(200) [Content].[Questions] SET IsDeleted = 1;

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
