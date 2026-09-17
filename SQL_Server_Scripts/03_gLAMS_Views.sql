CREATE VIEW [Content].[vw_ActiveMcqQuestions]
AS
SELECT 
	q.[QuestionID],
	q.[CourseID],
	q.[LessonID],
	q.[QuestionText],
	q.[UpdatedAt] AS QuestionUpdatedAt,
	o.[OptionID],
	o.[AnswerText],
	o.[IsCorrect],
	o.[SortOrder]
FROM [Content].[Questions] q
INNER JOIN [Content].[McqOptions] o ON q.[QuestionID] = o.[QuestionID]
WHERE q.[QuestionType] = 1 
  AND q.[IsDeleted] = 0 
  AND o.[IsDeleted] = 0;
GO

CREATE VIEW [Content].[vw_ActiveEssayQuestions]
AS
SELECT 
	q.[QuestionID],
	q.[CourseID],
	q.[LessonID],
	q.[QuestionText],
	q.[UpdatedAt],
	ea.[ModelAnswer]
FROM [Content].[Questions] q
INNER JOIN [Content].[EssayAnswers] ea ON q.[QuestionID] = ea.[QuestionID]
WHERE q.[QuestionType] = 2 
  AND q.[IsDeleted] = 0 
  AND ea.[IsDeleted] = 0;
GO