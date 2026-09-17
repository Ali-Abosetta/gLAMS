namespace gLAMS.Domain.Interfaces
{
    /// <summary>
    /// Defines a contract for entities that can be manually ordered in the UI.
    /// </summary>
    public interface ISortable
    {
        /// <summary>
        /// Gets or sets the numerical order of the entity in a list.
        /// </summary>
        int SortOrder { get; set; }
    }
}