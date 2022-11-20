/// <author> Hrishi Raaj Singh Chauhan </author>
/// <summary>
/// It contains the public interface provided to Summary module to save the summary.
/// </summary>
namespace Dashboard.Server.Persistence
{
    // Interface for the summary
    public interface ISummaryPersistence
    {
        /// <summary>
        ///     Saves the summary of the session in a text file.
        /// </summary>
        /// <param name="message"> message string</param>
        /// <returns> return true if saved successfully </returns>
        public bool SaveSummary(string message);
    }
}