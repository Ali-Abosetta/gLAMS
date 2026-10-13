USE [gLAMS]
GO

CREATE VIEW [Content].[vw_McqQuestions]
AS
SELECT 
    q.[QuestionID],
    q.[CourseID],
    q.[LessonID],
    q.[QuestionText],
    q.[CreatedAt],
    q.[UpdatedAt],
    q.[IsDeleted],
    (
        SELECT 
            [OptionID] AS [Id], 
            [AnswerText], 
            [IsCorrect], 
            [SortOrder],
            [CreatedAt],
            [UpdatedAt],
            [IsDeleted]
        FROM [Content].[McqOptions] o
        WHERE o.[QuestionID] = q.[QuestionID] 
          AND o.[IsDeleted] = 0
        ORDER BY o.[SortOrder]
        FOR JSON PATH
    ) AS [OptionsJson]
FROM [Content].[Questions] q
WHERE q.[QuestionType] = 1 
  AND q.[IsDeleted] = 0;
GO


CREATE VIEW [Content].[vw_EssayQuestions]
AS
SELECT 
    q.[QuestionID],
    q.[CourseID],
    q.[LessonID],
    q.[QuestionType],
    q.[QuestionText],
    q.[CreatedAt],
    q.[UpdatedAt],
    q.[IsDeleted],
    ea.[ModelAnswer]
FROM [Content].[Questions] q
JOIN [Content].[EssayAnswers] ea ON q.[QuestionID] = ea.[QuestionID]
WHERE q.[QuestionType] = 2 
  AND q.[IsDeleted] = 0 
  AND ea.[IsDeleted] = 0;
GO