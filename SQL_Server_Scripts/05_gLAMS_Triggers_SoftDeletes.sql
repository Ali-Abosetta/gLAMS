USE [gLAMS]
GO
-- Instead of delete trigger to soft delete:
-- folder
CREATE TRIGGER [Structure].[trg_Folder_SoftDelete]
ON [Structure].[Folders]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE f
	SET f.IsDeleted = 1
	FROM [Structure].[Folders] f
	JOIN deleted d ON f.FolderID = d.FolderID;
END
GO

-- course
CREATE TRIGGER [Structure].[trg_Course_SoftDelete]
ON [Structure].[Courses]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE c
	SET c.IsDeleted = 1
	FROM [Structure].[Courses] c
	JOIN deleted d ON c.CourseID = d.CourseID;
END
GO

-- lesson
CREATE TRIGGER [Structure].[trg_Lesson_SoftDelete]
ON [Structure].[Lessons]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE l
	SET l.IsDeleted = 1
	FROM [Structure].[Lessons] l
	JOIN deleted d ON l.LessonID = d.LessonID;
END
GO

-- question
CREATE TRIGGER [Content].[trg_Question_SoftDelete]
ON [Content].[Questions]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE q
	SET q.IsDeleted = 1
	FROM [Content].[Questions] q
	JOIN deleted d ON q.QuestionID = d.QuestionID;
END
GO

-- McqOption
CREATE TRIGGER [Content].[trg_SoftDelete_McqOption]
ON [Content].[McqOptions]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
	UPDATE opt
    SET opt.IsDeleted = 1
    FROM [Content].[McqOptions] opt
    JOIN deleted d ON opt.OptionID = d.OptionID;
END
GO

-- Essay Answers
CREATE TRIGGER [Content].[trg_SoftDelete_EssayAnswer]
ON [Content].[EssayAnswers]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
	UPDATE ans
    SET ans.IsDeleted = 1
    FROM [Content].[EssayAnswers] ans
    JOIN deleted d ON ans.QuestionID = d.QuestionID;
END
GO

-- flash card
CREATE TRIGGER [Content].[trg_FlashCard_SoftDelete]
ON [Content].[FlashCards]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE f
	SET f.IsDeleted = 1
	FROM [Content].[FlashCards] f
	JOIN deleted d ON f.FlashCardID = d.FlashCardID;
END
GO

-- note
CREATE TRIGGER [Content].[trg_Note_SoftDelete]
ON [Content].[Notes]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE n
	SET n.IsDeleted = 1
	FROM [Content].[Notes] n
	JOIN deleted d ON n.NoteID = d.NoteID;
END
GO

-- reference
CREATE TRIGGER [Content].[trg_Reference_SoftDelete]
ON [Content].[References]
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE r
	SET r.IsDeleted = 1
	FROM [Content].[References] r
	JOIN deleted d ON r.ReferenceID = d.ReferenceID;
END
GO
