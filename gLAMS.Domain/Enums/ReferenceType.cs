namespace gLAMS.Domain.Enums
{
    /// <summary>
    /// Defines the supported types of external or supplementary references.
    /// Mapped as a TINYINT in the database.
    /// </summary>
    public enum ReferenceType : byte
    {
        /// <summary>
        /// A physical file path or identifier.
        /// </summary>
        File = 1,

        /// <summary>
        /// A web hyperlink.
        /// </summary>
        Url = 2,

        /// <summary>
        /// Plain text reference information (e.g., book ISBN or citation).
        /// </summary>
        Text = 3
    }
}