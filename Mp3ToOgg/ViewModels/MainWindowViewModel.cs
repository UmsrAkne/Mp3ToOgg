namespace Mp3ToOgg.ViewModels
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using NAudio.MediaFoundation;
    using NAudio.Wave;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        // wav -> ogg の変換を行うエンコーダーは実行ファイルと同じ階層に手動で配置する。
        private string oggEncoder = "oggenc2.exe";

        private List<FileInfo> mp3Files;

        public MainWindowViewModel()
        {
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public List<FileInfo> Mp3Files { get => mp3Files; set => SetProperty(ref mp3Files, value); }

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
            Process.Start(oggEncoder, wavFileInfo.FullName);
        }
    }
}
