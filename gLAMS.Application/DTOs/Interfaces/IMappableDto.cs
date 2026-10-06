using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gLAMS.Application.DTOs.Interfaces
{
    /// <summary>
    /// A strict contract interface that forces incoming DTOs to implement a method 
    /// for mapping their properties into a pure Domain Entity.
    /// </summary>
    /// <typeparam name="TEntity">The Domain Entity class this DTO maps to.</typeparam>
    public interface IMappableDto<TEntity>
    {
        /// <summary>
        /// Converts the DTO payload into its corresponding Domain Entity.
        /// </summary>
        /// <returns>A fully mapped instance of the domain entity.</returns>
        TEntity ToEntity();
    }
}

