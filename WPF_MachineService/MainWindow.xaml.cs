using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;


namespace WPF_MachineService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private VideoCaptureDevice videoSource;
        private Bitmap latestFrameBitmap;

        public MainWindow()
        {
            InitializeComponent();
            StartCamera();
        }

        private void StartCamera()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Không tìm thấy thiết bị camera.");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            videoSource.Start();
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                latestFrameBitmap = (Bitmap)eventArgs.Frame.Clone();
                BitmapImage bitmapImage = ConvertBitmapToBitmapImage(latestFrameBitmap);

                imgVideo.Dispatcher.Invoke(() =>
                {
                    imgVideo.Source = bitmapImage;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in videoSource_NewFrame: {ex.Message}");
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
            }
        }

        private void btTakePicture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Truy cập và chụp ảnh từ camera
                Bitmap capturedImage = CaptureImageFromCamera();

                if (capturedImage == null)
                {
                    MessageBox.Show("Lỗi khi chụp ảnh từ camera.");
                    return;
                }

                // Chuyển đổi Bitmap thành BitmapImage
                BitmapImage bitmapImage = ConvertBitmapToBitmapImage(capturedImage);

                // Hiển thị hình ảnh trên đối tượng Image
                imgVideo.Source = bitmapImage;

                // Lưu ảnh đã chụp dưới dạng file JPEG
                SaveImageToFolder(capturedImage);

                MessageBox.Show("Hình ảnh được chụp và lưu thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp hoặc lưu ảnh: {ex.Message}");
            }
        }


        private Bitmap CaptureImageFromCamera()
        {
            if (latestFrameBitmap != null)
            {
                return latestFrameBitmap;
            }
            return null;
        }

        private void SaveImageToFolder(Bitmap image)
        {
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScannedImages");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"capture_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            string imagePath = System.IO.Path.Combine(folderPath, fileName);

            // Chuyển đổi System.Drawing.Bitmap thành System.Drawing.Image
            System.Drawing.Image imageToSave = (System.Drawing.Image)image;

            // Lưu ảnh dưới dạng file JPEG
            imageToSave.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            StopCamera(); // Dừng camera khi ứng dụng đóng
        }
    }

}