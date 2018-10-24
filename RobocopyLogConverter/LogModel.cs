using System;
using System.Collections.Generic;

namespace RobocopyLogConverter
{
    public class LogModel
    {
        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public string Mask { get; set; }

        public List<string> ExcludedFiles { get; set; } = new List<string>();

        public List<string> ExcludedFolders { get; set; } = new List<string>();

        public string Options { get; set; }

        public int TotalFolders { get; set; }

        public int TotalFiles { get; set; }

        public long TotalBytes { get; set; }

        public int CopiedFolders { get; set; }

        public int CopiedFiles { get; set; }

        public long CopiedBytes { get; set; }

        public int IgnoredFolders { get; set; }

        public int IgnoredFiles { get; set; }

        public long IgnoredBytes { get; set; }

        public long UnconformityFolders { get; set; }

        public long UnconformityFiles { get; set; }

        public long UnconformityBytes { get; set; }

        public int FailedFolders { get; set; }

        public int FailedFiles { get; set; }

        public long FailedBytes { get; set; }

        public int ExtraFolders { get; set; }

        public int ExtraFiles { get; set; }

        public long ExtraBytes { get; set; }

        public int OutputMB { get; set; }
    }
}
