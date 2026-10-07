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
    -- Create a list of only folders that have a valid, unbroken chain to the root
		WITH ActiveFolders AS (
		    -- 1. Anchor: Start with all ROOT folders that are NOT deleted
		    SELECT 
		        [FolderID],
		        [ParentID],
		        [Name],
		        [CreatedAt],
		        [UpdatedAt],
		        [IsDeleted]
		    FROM [Structure].[Folders]
		    WHERE [ParentID] IS NULL AND [IsDeleted] = 0
		    
		    UNION ALL
		    
		    -- 2. Recursive Step: Only grab children if their parent is currently in the ActiveFolders list
		    SELECT 
		        f.[FolderID],
		        f.[ParentID],
		        f.[Name],
		        f.[CreatedAt],
		        f.[UpdatedAt],
		        f.[IsDeleted]
		    FROM [Structure].[Folders] f
		    INNER JOIN ActiveFolders a ON f.[ParentID] = a.[FolderID]
		    WHERE f.[IsDeleted] = 0
		)
		
		-- 3. Return the perfectly filtered list
		SELECT 
		    [FolderID],
		    [ParentID],
		    [Name],
		    [CreatedAt],
		    [UpdatedAt],
		    [IsDeleted]
		FROM ActiveFolders
		ORDER BY [CreatedAt] ASC;

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

-- get root folders
CREATE PROCEDURE [Structure].[usp_GetRootFolders]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[FolderID],
		[ParentID],
		[Name],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
		FROM [Structure].[Folders]
	WHERE	
		[ParentID] IS NULL
		AND [IsDeleted] = 0
END
GO

-- get sub folders by parent Id
CREATE PROCEDURE [Structure].[usp_GetFoldersByParentId]
	@ParentId UNIQUEIDENTIFIER

AS
BEGIN
    -- Generate the perfect, unbroken active tree
    WITH ActiveFolders AS (
        SELECT 
            [FolderID],
			[ParentID], 
			[Name],
			[CreatedAt], 
			[UpdatedAt], 
			[IsDeleted]
        FROM [Structure].[Folders]
        WHERE [ParentID] IS NULL AND [IsDeleted] = 0
        
        UNION ALL
        
        SELECT 
            f.[FolderID], f.[ParentID], f.[Name], f.[CreatedAt], f.[UpdatedAt], f.[IsDeleted]
        FROM [Structure].[Folders] f
        INNER JOIN ActiveFolders a ON f.[ParentID] = a.[FolderID]
        WHERE f.[IsDeleted] = 0
    )
    
    -- Filter the active tree for ONLY the parent we care about!
    SELECT * 
    FROM ActiveFolders
    WHERE [ParentID] = @ParentID
    ORDER BY [CreatedAt] ASC;
END
GO

-- get folder by id
CREATE PROCEDURE [Structure].[usp_GetFolderById]

	@FolderID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[FolderID],
		[ParentID],
		[Name],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
		FROM [Structure].[Folders]
	WHERE	
		[FolderID] = @FolderID
		AND [IsDeleted] = 0
	
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

-- get courses by folder id
CREATE PROCEDURE [Structure].[usp_GetCoursesByFolderId]
	@FolderID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
		SELECT 
			[CourseID],
			[FolderID],
			[Name],
			[CreatedAt],
			[UpdatedAt],
			[IsDeleted]
		FROM [Structure].[Courses]
		WHERE [IsDeleted] = 0
			AND FolderID = @FolderID;
END
GO

-- get course by id
CREATE PROCEDURE [Structure].[usp_GetCourseById]
	@CourseID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[CourseID],
		[FolderID],
		[Name],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
	FROM [Structure].[Courses]
	WHERE	[CourseID] = @CourseID
		AND [IsDeleted] = 0
	
END
GO

-- get (search) courses by name
CREATE PROCEDURE [Structure].[usp_SearchCoursesByName]
	@Name NVARCHAR(150)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[CourseID],
		[FolderID],
		[Name],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
	FROM [Structure].[Courses]
	WHERE	[Name] LIKE '%' + @Name + '%'
		AND [IsDeleted] = 0
	ORDER BY 	
		CASE 
			WHEN [Name] = @Name THEN 1           --  Exact Match
			WHEN [Name] LIKE @Name + '%' THEN 2  --  Starts With
			ELSE 3                               --  Contains it somewhere inside
		END ASC,
		[Name] ASC; 
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

-- get lessons by course id
CREATE PROCEDURE [Structure].[usp_GetLessonsByCourseId]

	@CourseID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[LessonID],
		[CourseID],
		[Title],
		[SortOrder],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
	FROM [Structure].[Lessons]
	WHERE	[CourseID] = @CourseID
		AND [IsDeleted] = 0
END
GO

-- get Lesson by id
CREATE PROCEDURE [Structure].[usp_GetLessonById]
	@LessonID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[LessonID],
		[CourseID],
		[Title],
		[SortOrder],
		[CreatedAt],
		[UpdatedAt],
		[IsDeleted]
	FROM [Structure].[Lessons]
	WHERE	[LessonID] = @LessonID
		AND [IsDeleted] = 0;
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

-- get falsh cards in lesson
CREATE PROCEDURE [Content].[usp_GetFlashCardsByLessonId]
	@LessonID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

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
	WHERE	[LessonID] = @LessonID
		AND [IsDeleted] = 0;
END
GO

-- get flash cards on course level
CREATE PROCEDURE [Content].[usp_GetFlashCardsByCourseId]

	@CourseID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

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
	WHERE	[CourseID] = @CourseID
			AND [IsDeleted] = 0;
END
GO

-- get flash card by id
CREATE PROCEDURE [Content].[usp_GetFlashCardById]
	@FlashCardID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

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
	WHERE	[FlashCardID] = @FlashCardID
		AND [IsDeleted] = 0;
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
