namespace Mp3ToOgg.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
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

        private string message = string.Empty;

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

        public string Message { get => message; set => SetProperty(ref message, value); }

        public DelegateCommand StartConvertCommand => new DelegateCommand(() =>
        {
            if (File.Exists(oggEncoder))
            {
                Message = string.Empty;

                Mp3Files.ToList().ForEach(f =>
                {
                    var t = ConvertAsync(f);
                });
            }
            else
            {
                Message = $"{new FileInfo(oggEncoder).FullName} が見つかりません。";
            }
        });

        public DelegateCommand StartConvertToWavCommand => new DelegateCommand(() =>
        {
            if (File.Exists(oggEncoder))
            {
                Message = string.Empty;

                Mp3Files.ToList().ForEach(f =>
                {
                    var t = ConvertToWavAsync(f);
                });
            }
            else
            {
                Message = $"{new FileInfo(oggEncoder).FullName} が見つかりません。";
            }
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
            var pi = new ProcessStartInfo();
            pi.FileName = oggEncoder;
            pi.Arguments = $"\"{wavFileInfo.FullName}\"";
            pi.UseShellExecute = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(pi);
        }

        private async Task ConvertAsync(ExFileInfo f)
        {
            await Task.Run(() =>
            {
                var wavFile = ConvertMp3ToWav(f.FileInfo);
                ConvertWavToOgg(wavFile);
                f.Converted = true;
            });
        }

        private async Task ConvertToWavAsync(ExFileInfo f)
        {
            await Task.Run(() =>
            {
                var wavFile = ConvertMp3ToWav(f.FileInfo);
                f.Converted = true;
            });
        }
    }
}
