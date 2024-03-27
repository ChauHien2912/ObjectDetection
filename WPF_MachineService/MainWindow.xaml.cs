using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


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
                Bitmap videoCapture = (Bitmap)eventArgs.Frame.Clone();
                BitmapImage bitmapImage = ConvertBitmapToBitmapImage(videoCapture);

                // Sử dụng Dispatcher của imgVideo để cập nhật UI elements từ UI thread của imgVideo
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
                // Truy cập camera sử dụng thư viện phù hợp (ví dụ: Emgu CV, AForge.NET)
                // Thay thế bằng logic truy cập và chụp ảnh từ camera của bạn
                BitmapImage capturedImage = CaptureImageFromCamera(); // Thay đổi kiểu dữ liệu từ Image sang BitmapImage

                if (capturedImage == null)
                {
                    MessageBox.Show("Lỗi khi chụp ảnh từ camera.");
                    return;
                }

                // Lưu ảnh đã chụp dưới dạng file JPEG
                SaveImageToFolder(capturedImage);

                MessageBox.Show("Hình ảnh được chụp và lưu thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp hoặc lưu ảnh: {ex.Message}");
            }
        }

        private BitmapImage CaptureImageFromCamera()
        {
            if (latestFrameBitmap != null)
            {
                return ConvertBitmapToBitmapImage(latestFrameBitmap);
            }
            return null;
        }

        private void SaveImageToFolder(BitmapImage image)
        {
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScannedImages");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"capture_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            string imagePath = System.IO.Path.Combine(folderPath, fileName);

            // Tạo một FileStream để lưu ảnh
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                // Khởi tạo một encoder để lưu ảnh dưới dạng JPEG
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image)); // Thêm hình ảnh vào encoder

                // Ghi dữ liệu hình ảnh vào file
                encoder.Save(fileStream);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            StopCamera(); // Dừng camera khi ứng dụng đóng
        }
    }
}