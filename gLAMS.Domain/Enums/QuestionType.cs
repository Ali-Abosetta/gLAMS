namespace gLAMS.Domain.Enums
{
    /// <summary>
    /// Defines the supported types of questions in the LMS.
    /// Mapped as a TINYINT in the database.
    /// </summary>
    public enum QuestionType : byte
    {
        /// <summary>
        /// Multiple Choice Question. Requires associated McqOption records.
        /// </summary>
        MCQ = 1,

        /// <summary>
        /// Essay question. Requires an associated EssayAnswer record for the model answer.
        /// </summary>
        Essay = 2
    }
}