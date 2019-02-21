using System;
using System.Collections.Generic;
using System.Text;

namespace FolkTickets.Helpers
{
    /// <summary>
    /// Settings file
    /// </summary>
    public class SettingsFile
    {
        /// <summary>
        /// Website URI
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Use SSL
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Consumer API Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Consumer API Secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Default website language
        /// </summary>
        public string Language { get; set; }
    }
}
