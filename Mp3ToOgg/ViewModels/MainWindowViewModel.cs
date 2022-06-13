namespace Mp3ToOgg.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Mp3ToOgg.Models;
    using NAudio.MediaFoundation;
    using NAudio.Wave;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        // wav -> ogg の変換を行うエンコーダーは実行ファイルと同じ階層に手動で配置する。
        private string oggEncoder = "oggenc2.exe";

        private ObservableCollection<ExFileInfo> mp3Files;

        private bool canConvert;

        public MainWindowViewModel()
        {
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public bool CanConvert { get => canConvert; set => SetProperty(ref canConvert, value); }

        public ObservableCollection<ExFileInfo> Mp3Files
        {
            get => mp3Files; set
            {
                CanConvert = value.Count != 0;
                SetProperty(ref mp3Files, value);
            }
        }

        public DelegateCommand StartConvertCommand => new DelegateCommand(() =>
        {
            Mp3Files.ToList().ForEach(f =>
            {
                var wavFile = ConvertMp3ToWav(f.FileInfo);
                ConvertWavToOgg(wavFile);
                f.Converted = true;
            });
        });

        private FileInfo ConvertMp3ToWav(FileInfo mp3File)
        {
            MediaFoundationReader reader = new MediaFoundationReader(mp3File.FullName);

            WaveFormat format = new WaveFormat(48000, 16, 2);
            MediaType mediaType = new MediaType(format);

            var wavFileInfo = new FileInfo($"{mp3File.DirectoryName}\\{Path.GetFileNameWithoutExtension(mp3File.FullName)}.wav");

            using (MediaFoundationEncoder encoder = new MediaFoundationEncoder(mediaType))
            {
                encoder.Encode(wavFileInfo.FullName, reader);
            }

            return wavFileInfo;
        }

        private void ConvertWavToOgg(FileInfo wavFileInfo)
        {
            var p = Process.Start(oggEncoder, $"\"{wavFileInfo.FullName}\"");
        }
    }
}
