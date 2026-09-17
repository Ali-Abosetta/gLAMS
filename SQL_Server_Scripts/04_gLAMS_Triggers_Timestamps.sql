USE [gLAMS]
GO

-- Triggers:
-- creating after Update trg for [Structure].[Folders] table
CREATE TRIGGER [trg_Folders_UpdateTimestamp]
ON [Structure].[Folders]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE f
	SET UpdatedAt = GETUTCDATE()
	FROM [Structure].[Folders] f 
	JOIN inserted i
	ON f.FolderID = i.FolderID 

END
GO

-- creating after Update trg for [Structure].[Courses] table
CREATE TRIGGER [trg_Courses_UpdateTimestamp]
ON [Structure].[Courses]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE c
	SET UpdatedAt = GETUTCDATE()
	FROM [Structure].[Courses] c 
	JOIN inserted i
	ON c.CourseID = i.CourseID 

END
GO

-- creating after Update trg for [Structure].[Lessons] table
CREATE TRIGGER [trg_Lessons_UpdateTimestamp]
ON [Structure].[Lessons]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE l
	SET UpdatedAt = GETUTCDATE()
	FROM [Structure].[Lessons] l 
	JOIN inserted i
	ON l.LessonID = i.LessonID 

END
GO

-- creating after Update trg for [Content].[Questions] table
CREATE TRIGGER [trg_Questions_UpdateTimestamp]
ON [Content].[Questions]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE q
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[Questions] q
	JOIN inserted i
	ON q.QuestionID = i.QuestionID

END
GO

-- creating after Update trg for [Content].[McqOptions] table
CREATE TRIGGER [trg_McqOptions_UpdateTimestamp]
ON [Content].[McqOptions]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE mcq
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[McqOptions] mcq
	JOIN inserted i
	ON mcq.OptionID = i.OptionID

END
GO

-- creating after Update trg for [Content].[EssayAnswers] table
CREATE TRIGGER [trg_EssayAnswers_UpdateTimestamp]
ON [Content].[EssayAnswers]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE ea
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[EssayAnswers] ea
	JOIN inserted i
	ON ea.QuestionID = i.QuestionID 

END
GO

-- creating after Update trg for [Content].[FlashCards] table
CREATE TRIGGER [trg_FlashCards_UpdateTimestamp]
ON [Content].[FlashCards]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE fc
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[FlashCards] fc
	JOIN inserted i
	ON fc.FlashCardID = i.FlashCardID

END
GO

-- creating after Update trg for [Content].[Notes] table
CREATE TRIGGER [trg_Notes_UpdateTimestamp]
ON [Content].[Notes]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE n
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[Notes] n
	JOIN inserted i
	ON n.NoteID = i.NoteID 

END
GO

-- creating after Update trg for [Content].[References] table
CREATE TRIGGER [trg_References_UpdateTimestamp]
ON [Content].[References]
AFTER UPDATE
AS
BEGIN
		
	SET NOCOUNT ON;

	UPDATE r
	SET UpdatedAt = GETUTCDATE()
	FROM [Content].[References] r
	JOIN inserted i
	ON r.ReferenceID = i.ReferenceID 

END
GO
