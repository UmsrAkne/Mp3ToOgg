using System.IO;
using Prism.Mvvm;

namespace Mp3ToOgg.Models
{
    public class ExFileInfo : BindableBase
    {
        private FileInfo fileInfo;
        private bool converted;

        public ExFileInfo(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public string Name => FileInfo.Name;

        public FileInfo FileInfo { get => fileInfo; private set => SetProperty(ref fileInfo, value); }

        public bool Converted { get => converted; set => SetProperty(ref converted, value); }
    }
}
