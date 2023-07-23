using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Mp3ToOgg.ViewModels;
using Mp3ToOgg.Views;

namespace Mp3ToOgg.Models
{
    public class DropBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // ファイルをドラッグしてきて、コントロール上に乗せた際の処理
            AssociatedObject.PreviewDragOver += AssociatedObject_PreviewDragOver;

            // ファイルをドロップした際の処理
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDragOver -= AssociatedObject_PreviewDragOver;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            // ファイルパスの一覧の配列
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null)
            {
                return;
            }

            var exFiles = files
                .Select(p => new ExFileInfo(new FileInfo(p)))
                .Where(f => f.FileInfo.Extension == ".mp3" || f.FileInfo.Extension == ".wav");

            var w = sender as MainWindow;

            if (w?.DataContext is MainWindowViewModel vm)
            {
                vm.Mp3Files = new ObservableCollection<ExFileInfo>(exFiles);
            }
        }

        private void AssociatedObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }
    }
}