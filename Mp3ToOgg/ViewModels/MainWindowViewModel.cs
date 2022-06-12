namespace Mp3ToOgg.ViewModels
{
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

        public MainWindowViewModel()
        {
            var wavFile = ConvertMp3ToWav(new FileInfo("a.mp3"));
            Process.Start(oggEncoder, wavFile.FullName);
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public FileInfo ConvertMp3ToWav(FileInfo mp3File)
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
    }
}
