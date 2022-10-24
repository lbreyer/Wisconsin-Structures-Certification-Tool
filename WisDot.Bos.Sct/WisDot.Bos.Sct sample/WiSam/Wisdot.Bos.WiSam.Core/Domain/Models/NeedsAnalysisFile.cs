using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class NeedsAnalysisFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public DateTime LastUpdate { get; set; }
        public WisamType.NeedsAnalysisFileTypes NeedsAnalysisFileType { get; set; }

        public NeedsAnalysisFile()
        { }

        public NeedsAnalysisFile(string fileName, string filePath, string fileExtension, DateTime lastUpdate, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType)
        {
            FileName = fileName;
            FilePath = filePath;
            FileExtension = fileExtension;
            LastUpdate = lastUpdate;
            NeedsAnalysisFileType = needsAnalysisFileType;
        }
    }
}
