/// <author>Hrishi Raaj Singh Chauhan</author>
/// <created>31/10/2022</created>
/// <summary>
///     It contains the ResponseEntity Class, which is required as the return type
///     of save functions implemeted 
/// </summary>

using System.Collections.Generic;

namespace Dashboard.Server.Persistence
{
    /// <summary>
    ///     It is the return type for save functions, as for this module
    ///     It saves the data with variable/different name of files, hence it should be returned.
    /// </summary>
    public class ResponseEntity
    {
        // name of the file 
        public string FileName;

        // True if file saved successfully
        public bool IsSaved;

    }
}