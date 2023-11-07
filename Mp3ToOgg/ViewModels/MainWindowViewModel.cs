using System.Collections.Generic;
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

namespace Mp3ToOgg.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        // wav -> ogg の変換を行うエンコーダーは実行ファイルと同じ階層に手動で配置する。
        private const string OggEncoder = "oggenc2.exe";

        private string title = "mp3 to ogg converter";

        private ObservableCollection<ExFileInfo> mp3Files;

        private bool canConvert;

        private string message = string.Empty;
        private int convertedCounter;
        private bool deleteIntermediateFile = true;
        private List<FileInfo> intermediateFileInfos = new List<FileInfo>();

        public string Title { get => title; set => SetProperty(ref title, value); }

        public bool CanConvert { get => canConvert; set => SetProperty(ref canConvert, value); }

        public int ConvertedCounter { get => convertedCounter; set => SetProperty(ref convertedCounter, value); }

        public bool DeleteIntermediateFile
        {
            get => deleteIntermediateFile;
            set => SetProperty(ref deleteIntermediateFile, value);
        }

        public ObservableCollection<ExFileInfo> Mp3Files
        {
            get => mp3Files;
            set
            {
                CanConvert = value.Count != 0;
                SetProperty(ref mp3Files, value);
            }
        }

        public string Message { get => message; private set => SetProperty(ref message, value); }

        public DelegateCommand StartConvertCommand => new DelegateCommand(() =>
        {
            if (File.Exists(OggEncoder))
            {
                Message = string.Empty;

                Mp3Files.ToList().ForEach(f =>
                {
                    var _ = ConvertToOggAsync(f);
                });
            }
            else
            {
                Message = $"{new FileInfo(OggEncoder).FullName} が見つかりません。";
            }
        });

        public DelegateCommand StartConvertToWavCommand => new DelegateCommand(() =>
        {
            if (File.Exists(OggEncoder))
            {
                Message = string.Empty;

                Mp3Files.ToList().ForEach(f =>
                {
                    var _ = ConvertToWavAsync(f);
                });
            }
            else
            {
                Message = $"{new FileInfo(OggEncoder).FullName} が見つかりません。";
            }
        });

        private FileInfo ConvertMp3ToWav(FileInfo mp3File)
        {
            MediaFoundationReader reader = new MediaFoundationReader(mp3File.FullName);

            WaveFormat format = new WaveFormat(48000, 16, 2);
            MediaType mediaType = new MediaType(format);

            var wavFileInfo =
                new FileInfo($"{mp3File.DirectoryName}\\{Path.GetFileNameWithoutExtension(mp3File.FullName)}.wav");

            using (MediaFoundationEncoder encoder = new MediaFoundationEncoder(mediaType))
            {
                encoder.Encode(wavFileInfo.FullName, reader);
            }

            return wavFileInfo;
        }

        private void ConvertWavToOgg(FileInfo wavFileInfo)
        {
            var pr = new Process();
            pr.EnableRaisingEvents = true;
            pr.Exited += (sender, e) =>
            {
                ConvertedCounter++;
                if (ConvertedCounter == Mp3Files.Count && DeleteIntermediateFile)
                {
                    intermediateFileInfos.ForEach(f => f.Delete());
                    intermediateFileInfos = new List<FileInfo>();
                }

                pr.Dispose();
            };

            pr.StartInfo = new ProcessStartInfo
            {
                FileName = OggEncoder,
                Arguments = $"\"{wavFileInfo.FullName}\"",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            pr.Start();
        }

        private async Task ConvertToWavAsync(ExFileInfo f)
        {
            await Task.Run(() =>
            {
                ConvertMp3ToWav(f.FileInfo);
                f.Converted = true;
                ConvertedCounter++;
            });
        }

        /// <summary>
        ///     入力された mp3, ogg ファイルを ogg　に変換します。
        /// </summary>
        /// <param name="f">.mp3, .ogg の ExFileInfo</param>
        private async Task ConvertToOggAsync(ExFileInfo f)
        {
            await Task.Run(() =>
            {
                if (f.FileInfo.Extension == ".mp3")
                {
                    ConvertMp3ToWav(f.FileInfo);
                    f.SetExtension(".wav");
                    intermediateFileInfos.Add(new FileInfo(f.FileInfo.FullName));
                }

                ConvertWavToOgg(f.FileInfo);
                f.SetExtension(".ogg");

                f.Converted = true;
            });
        }
    }
}