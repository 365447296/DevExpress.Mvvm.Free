using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Mvvm.UI.Interactivity;

namespace DevExpress.Mvvm.UI {
    [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
    [TargetType(typeof(System.Windows.Controls.UserControl)), TargetType(typeof(Window))]
    public class OpenFileDialogService : FileDialogServiceBase, IOpenFileDialogService {
        protected interface IOpenFileDialog : IFileDialog {
            bool Multiselect { get; set; }
            bool ReadOnlyChecked { get; set; }
            bool ShowReadOnly { get; set; }
        }

        protected class OpenFileDialogAdapter : FileDialogAdapter<OpenFileDialog>, IOpenFileDialog {
            public OpenFileDialogAdapter(OpenFileDialog fileDialog) : base(fileDialog) { }

            bool IOpenFileDialog.Multiselect {
                get { return fileDialog.Multiselect; }
                set { fileDialog.Multiselect = value; }
            }

            bool IOpenFileDialog.ReadOnlyChecked {
                get { return fileDialog.ReadOnlyChecked; }
                set { fileDialog.ReadOnlyChecked = value; }
            }
            bool IOpenFileDialog.ShowReadOnly {
                get { return fileDialog.ShowReadOnly; }
                set { fileDialog.ShowReadOnly = value; }
            }
        }

        public static readonly DependencyProperty MultiselectProperty =
            DependencyProperty.Register("Multiselect", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
        public static readonly DependencyProperty ReadOnlyCheckedProperty =
            DependencyProperty.Register("ReadOnlyChecked", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
        public static readonly DependencyProperty ShowReadOnlyProperty =
            DependencyProperty.Register("ShowReadOnly", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(OpenFileDialogService), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty FilterIndexProperty =
            DependencyProperty.Register("FilterIndex", typeof(int), typeof(OpenFileDialogService), new PropertyMetadata(1));
        public bool Multiselect {
            get { return (bool)GetValue(MultiselectProperty); }
            set { SetValue(MultiselectProperty, value); }
        }
        public bool ReadOnlyChecked {
            get { return (bool)GetValue(ReadOnlyCheckedProperty); }
            set { SetValue(ReadOnlyCheckedProperty, value); }
        }
        public bool ShowReadOnly {
            get { return (bool)GetValue(ShowReadOnlyProperty); }
            set { SetValue(ShowReadOnlyProperty, value); }
        }
        public string Filter {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
        public int FilterIndex {
            get { return (int)GetValue(FilterIndexProperty); }
            set { SetValue(FilterIndexProperty, value); }
        }

        IOpenFileDialog OpenFileDialog { get { return (IOpenFileDialog)GetFileDialog(); } }
        public OpenFileDialogService() {
            CheckFileExists = true;
        }
        protected override object CreateFileDialog() {
            return new OpenFileDialog();
        }
        protected override IFileDialog CreateFileDialogAdapter() {
            return new OpenFileDialogAdapter((OpenFileDialog)CreateFileDialog());
        }
        protected override void InitFileDialog() {
            OpenFileDialog.Multiselect = Multiselect;
            OpenFileDialog.ReadOnlyChecked = ReadOnlyChecked;
            OpenFileDialog.ShowReadOnly = ShowReadOnly;
            OpenFileDialog.Filter = Filter;
            OpenFileDialog.FilterIndex = FilterIndex;
        }
        protected override List<FileInfoWrapper> GetFileInfos() {
            List<FileInfoWrapper> res = new List<FileInfoWrapper>();
            foreach(string fileName in OpenFileDialog.FileNames)
                res.Add(FileInfoWrapper.Create(fileName));
            return res;
        }
        IFileInfo IOpenFileDialogService.File { get { return GetFiles().FirstOrDefault(); } }
        IEnumerable<IFileInfo> IOpenFileDialogService.Files { get { return GetFiles(); } }
        bool IOpenFileDialogService.ShowDialog(Action<CancelEventArgs> fileOK, string directoryName) {
            if(directoryName != null)
                InitialDirectory = directoryName;
            var res = Show(fileOK);
            FilterIndex = OpenFileDialog.FilterIndex;
            return res;
        }
    }
}