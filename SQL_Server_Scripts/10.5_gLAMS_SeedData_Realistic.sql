USE [gLAMS]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	PRINT 'Starting Realistic Data Seed...'

	DECLARE @RootFolderID UNIQUEIDENTIFIER = NEWID();
	DECLARE @SubFolderID1 UNIQUEIDENTIFIER = NEWID();
	DECLARE @SubFolderID2 UNIQUEIDENTIFIER = NEWID();
	DECLARE @CourseID1 UNIQUEIDENTIFIER = NEWID();
	DECLARE @CourseID2 UNIQUEIDENTIFIER = NEWID();
	DECLARE @LessonID1 UNIQUEIDENTIFIER = NEWID();
	DECLARE @LessonID2 UNIQUEIDENTIFIER = NEWID();
	DECLARE @LessonID3 UNIQUEIDENTIFIER = NEWID();
    
    DECLARE @ContentID UNIQUEIDENTIFIER;
	DECLARE @OptionsJSON NVARCHAR(MAX);

	-- 1. Root Folder
	EXEC [Structure].[usp_AddFolder] @FolderID = @RootFolderID, @ParentID = NULL, @Name = 'Computer Science Curriculum';

	-- 2. Sub-Folders
	EXEC [Structure].[usp_AddFolder] @FolderID = @SubFolderID1, @ParentID = @RootFolderID, @Name = 'Backend Development';
	EXEC [Structure].[usp_AddFolder] @FolderID = @SubFolderID2, @ParentID = @RootFolderID, @Name = 'Database Architecture';

	-- 3. Courses
	EXEC [Structure].[usp_AddCourse] @CourseID = @CourseID1, @FolderID = @SubFolderID1, @Name = 'C# Masterclass: From Zero to Hero';
	EXEC [Structure].[usp_AddCourse] @CourseID = @CourseID2, @FolderID = @SubFolderID2, @Name = 'SQL Server Performance Tuning';

	-- 4. Lessons for C# Course
	EXEC [Structure].[usp_AddLesson] @LessonID = @LessonID1, @CourseID = @CourseID1, @Title = 'Introduction to Value Types and Reference Types';
	EXEC [Structure].[usp_AddLesson] @LessonID = @LessonID2, @CourseID = @CourseID1, @Title = 'Understanding Object-Oriented Polymorphism';

	-- 5. Lessons for SQL Course
	EXEC [Structure].[usp_AddLesson] @LessonID = @LessonID3, @CourseID = @CourseID2, @Title = 'Clustered vs Non-Clustered Indexes';

	-- ==========================================
	-- CONTENT FOR: C# - Value vs Reference Types
	-- ==========================================
	
	-- Note
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddNote] @NoteID = @ContentID, @CourseID = @CourseID1, @LessonID = @LessonID1, 
		@Title = 'Heap vs Stack Memory Allocation', 
		@NoteText = 'Value types are allocated on the stack and immediately destroyed when they fall out of scope. Reference types are allocated on the heap, and only their memory address pointer is stored on the stack. The Garbage Collector will eventually clean up heap memory when there are no more pointers referencing it.';

	-- FlashCard
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddFlashCard] @FlashCardID = @ContentID, @CourseID = @CourseID1, @LessonID = @LessonID1, 
		@FrontContent = 'Is a C# struct a value type or a reference type?', 
		@BackContent = 'A struct is always a Value Type. It is stored on the stack.';

	-- Reference
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddReference] @ReferenceID = @ContentID, @CourseID = @CourseID1, @LessonID = @LessonID1, 
		@Title = 'Microsoft Docs: Built-in reference types', 
		@ReferenceType = 2, 
		@ContentValue = 'https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types';

	-- MCQ
    SET @ContentID = NEWID();
	SET @OptionsJSON = N'[
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Integer (int)", "IsCorrect": false},
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "String", "IsCorrect": true},
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Boolean (bool)", "IsCorrect": false}
	]';
	EXEC [Content].[usp_AddMcqQuestion] @QuestionID = @ContentID, @CourseID = @CourseID1, @LessonID = @LessonID1, 
		@QuestionText = 'Which of the following is a Reference Type in C#?', 
		@OptionsJSON = @OptionsJSON;

	-- Essay
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddEssayQuestion] @QuestionID = @ContentID, @CourseID = @CourseID1, @LessonID = @LessonID1, 
		@QuestionText = 'Explain the process of "Boxing" and "Unboxing" in C#.', 
		@ModelAnswer = 'Boxing is the process of converting a value type to the type object or to any interface type implemented by this value type. Unboxing extracts the value type from the object. Boxing is computationally expensive because it requires allocating a new object on the heap.';

	-- ==========================================
	-- CONTENT FOR: SQL - Clustered Indexes
	-- ==========================================

	-- Note
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddNote] @NoteID = @ContentID, @CourseID = @CourseID2, @LessonID = @LessonID3, 
		@Title = 'Why GUIDs are bad for Clustered Indexes', 
		@NoteText = 'A Clustered Index dictates the physical sorting of rows on disk. Inserting a random GUID causes massive page fragmentation because the engine must constantly shift rows around to insert the new GUID in physical order. Always use Sequential GUIDs (NEWSEQUENTIALID) or an IDENTITY INT for clustered keys.';

	-- FlashCard
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddFlashCard] @FlashCardID = @ContentID, @CourseID = @CourseID2, @LessonID = @LessonID3, 
		@FrontContent = 'How many Clustered Indexes can a single SQL table have?', 
		@BackContent = 'Only ONE. Because the data can only be sorted physically on disk in one order.';

	-- Reference
    SET @ContentID = NEWID();
	EXEC [Content].[usp_AddReference] @ReferenceID = @ContentID, @CourseID = @CourseID2, @LessonID = @LessonID3, 
		@Title = 'Brent Ozar - Clustered Index Design', 
		@ReferenceType = 2, 
		@ContentValue = 'https://www.brentozar.com/archive/2013/05/why-clustered-indexes-are-important/';

	-- MCQ
    SET @ContentID = NEWID();
	SET @OptionsJSON = N'[
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Non-Clustered Index", "IsCorrect": false},
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Clustered Index", "IsCorrect": true},
		{"OptionID": "' + CAST(NEWID() AS VARCHAR(36)) + '", "AnswerText": "Columnstore Index", "IsCorrect": false}
	]';
	EXEC [Content].[usp_AddMcqQuestion] @QuestionID = @ContentID, @CourseID = @CourseID2, @LessonID = @LessonID3, 
		@QuestionText = 'Which type of index determines the physical sort order of data pages?', 
		@OptionsJSON = @OptionsJSON;


	COMMIT TRANSACTION;
	PRINT 'Realistic Data successfully seeded!'

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
