using System;
using System.Collections.Generic;
using System.IO;

namespace RobocopyLogConverter
{
    public class LogConverter
    {
        #region Constants

        const int KHeaderLineNumber = 3;
        const string KHeader = "   ROBOCOPY   ::";

        const int KStartDateTimeLineNumber = 6;
        const string KStartDateTime = "  Début : ";

        const int KSourceLineNumber = 7;
        const string KSource = "   Source : ";

        const int KTargetLineNumber = 8;
        const string KTarget = "     Dest : ";

        const int KMaskLineNumber = 10;
        const string KMask = "    Fichiers : ";

        const string KExcludedFiles = "Fich. exclus : ";
        const string KExcludedFolders = "Rép. exclus : .";

        const string KOptions = "  Options : ";

        const string KFolder = "     Rép :";
        const string KFile = "Fichiers :";
        const string KBytes = "  Octets :";
        const string KDuration = "   Heures:";
        const string KOutput = "   Débit :";
        const string KEndStartTime = "   Fin :";

        #endregion


        FileStream _fileStream;
        StreamReader _reader;
        int _currentLineNumber = 0;
        LogModel _log;


        public IEnumerable<LogModel> ConvertFolderToJson(string folder)
        {
            if (Directory.Exists(folder) == false)
                throw new FileNotFoundException($"Folder {folder} was not found.");

            var results = new List<LogModel>();

            foreach (var file in Directory.GetFiles(folder, "*.txt"))
            {
                try
                {
                    results.Add(ConvertFileToJson(file));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }

            return results;
        }


        public LogModel ConvertFileToJson(string file)
        {
            if (File.Exists(file) == false)
                throw new FileNotFoundException($"File {file} was not found.");

            _log = new LogModel();

            CloseFileIfNeeded();

            OpenFile(file);

            if (CheckHeader() == false)
                throw new Exception("Robocopy header was not found.");

            ParseStartDateTime();
            ParseSource();
            ParseTarget();
            ReadLine();
            ParseMask();
            ReadLine();

            ParseHeader();
            ParseFileSection();
            ParseFooter();

            CloseFileIfNeeded();

            return _log;
        }




        private void ParseHeader()
        {
            string line = ReadLine();

            while (line != null)
            {
                if (line.StartsWith(KExcludedFiles))
                    ParseExcludedFiles(line);

                else if (line.StartsWith(KExcludedFolders))
                    ParseExcludedFolders(line);

                else if (line.StartsWith(KOptions))
                    ParseOptions(line);

                else if (line.StartsWith("-----------------------------"))
                    break; // End of header

                line = ReadLine();
            }
        }


        private void ParseFooter()
        {
            string line = ReadLine();

            while (line != null)
            {
                if (line.StartsWith(KFolder))
                    ParseStatisticsFolders(line);

                else if (line.StartsWith(KFile))
                    ParseStatisticsFiles(line);

                else if (line.StartsWith(KBytes))
                    ParseStatisticsBytes(line);

                else if (line.StartsWith(KDuration))
                    ParseStatisticsDurations(line);

                else if (line.StartsWith(KEndStartTime))
                    ParseEndDateTime(line);

                line = ReadLine();
            }

        }


        private void ParseStatisticsFiles(string line)
        {
            int[] stats = ParseStatistics(line);

            _log.TotalFiles = stats[0];
            _log.CopiedFiles = stats[1];
            _log.IgnoredFiles = stats[2];
            _log.UnconformityFiles = stats[3];
            _log.FailedFiles = stats[4];
            _log.ExtraFiles = stats[5];
        }


        private void ParseStatisticsFolders(string line)
        {
            int[] stats = ParseStatistics(line);

            _log.TotalFolders = stats[0];
            _log.CopiedFolders = stats[1];
            _log.IgnoredFolders = stats[2];
            _log.UnconformityFolders = stats[3];
            _log.FailedFolders = stats[4];
            _log.ExtraFolders = stats[5];
        }


        private void ParseStatisticsBytes(string line)
        {
        }


        private void ParseStatisticsDurations(string line)
        {
        }


        private int[] ParseStatistics(string line)
        {
            int[] stats = new int[6];

            while (line.IndexOf("  ") >= 0)
                line = line.Replace("  ", " ");

            string[] statArray = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i <= 6; i++)
            {
                stats[i - 1] = Int32.Parse(statArray[i].Trim());
            }

            return stats;
        }


        private void ParseFileSection()
        {
            string line = ReadLine();

            while (line != null)
            {
                if (line.StartsWith("-----------------------------"))
                    break; // End of file section

                line = ReadLine();
            }
        }


        private bool CheckHeader()
        {
            string line = ReadLine(3);

            return (line.StartsWith(KHeader));
        }


        private void ParseStartDateTime()
        {
            string line = ReadLine(KStartDateTimeLineNumber);
            if (line.StartsWith(KStartDateTime))
            {
                _log.Start = DateTime.Parse(line.Substring(KStartDateTime.Length));
            }
        }


        private void ParseEndDateTime(string line)
        {
            _log.End = DateTime.Parse(line.Substring(KEndStartTime.Length));
        }


        private void ParseSource()
        {
            string line = ReadLine(KSourceLineNumber);
            if (line.StartsWith(KSource))
            {
                _log.Source = line.Substring(KSource.Length);
            }
        }


        private void ParseTarget()
        {
            string line = ReadLine(KTargetLineNumber);
            if (line.StartsWith(KTarget))
            {
                _log.Target = line.Substring(KTarget.Length);
            }
        }


        private void ParseMask()
        {
            string line = ReadLine(KMaskLineNumber);
            if (line.StartsWith(KMask))
            {
                _log.Mask = line.Substring(KMask.Length);
            }
        }


        private void ParseOptions(string line)
        {
            _log.Options = line.Substring(KOptions.Length);
        }


        private void ParseExcludedFiles(string line)
        {
            _log.ExcludedFiles.Add(line.Substring(KExcludedFiles.Length));

            do
            {
                line = ReadLine().Trim();
                if (line != "")
                    _log.ExcludedFiles.Add(line);
            }
            while (string.IsNullOrEmpty(line) == false);
        }


        private void ParseExcludedFolders(string line)
        {
            _log.ExcludedFolders.Add(line.Substring(KExcludedFolders.Length));

            do
            {
                line = ReadLine().Trim();
                if (line != "")
                    _log.ExcludedFolders.Add(line);
            }
            while (string.IsNullOrEmpty(line) == false);
        }


        #region File management

        private void OpenFile(string file)
        {
            _fileStream = new FileStream(file, FileMode.Open);
            _reader = new StreamReader(_fileStream);

            _currentLineNumber = 0;
        }


        private void CloseFileIfNeeded()
        {
            if (_fileStream != null)
                _fileStream.Dispose();

            if (_reader != null)
                _reader.Dispose();
        }


        private string ReadLine()
        {
            _currentLineNumber++;

            return _reader.ReadLine();
        }


        private string ReadLine(int lineNumber)
        {
            string line = "";

            while (_currentLineNumber < lineNumber && line != null)
            {
                line = ReadLine();
            }

            return line;
        }

        #endregion
    }
}
