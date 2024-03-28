using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using AForge.Video;


namespace WPF_MachineService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCaptureDevice videoSource;
        private Bitmap latestFrameBitmap;
        private FilterInfoCollection? _filterInfoCollectionVideoDevices;
        private VideoCaptureDevice[]? _videoCaptureDeviceSources;

        public MainWindow()
        {
            //context = new ScanMachineContext();
            InitializeComponent();
            Loaded += MachineWindow_Loaded;
            Closing += MachineWindow_Closing;
            StartCamera();
        }

        private void MachineWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            _filterInfoCollectionVideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_filterInfoCollectionVideoDevices != null && _filterInfoCollectionVideoDevices.Count >= 0)
            {
                _videoCaptureDeviceSources = new VideoCaptureDevice[1];
                _videoCaptureDeviceSources[0] = new VideoCaptureDevice(_filterInfoCollectionVideoDevices[0].MonikerString);
                _videoCaptureDeviceSources[0].NewFrame += VideoSource_BitMapFrame;
                _videoCaptureDeviceSources[0].Start();
            }
            else
            {
                MessageBox.Show("Không đủ thiết bị video.");
            }

        }

        private async void MachineWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_videoCaptureDeviceSources != null)
                {
                    foreach (var videoSource in _videoCaptureDeviceSources)
                    {
                        if (videoSource != null && videoSource.IsRunning)
                        {
                            await Task.Run(() =>
                            {
                                videoSource.SignalToStop();
                                videoSource.WaitForStop();
                            });
                        }
                    }
                    _videoCaptureDeviceSources = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi dừng video source: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            videoSource.NewFrame += new NewFrameEventHandler(VideoSource_BitMapFrame);
            videoSource.Start();
        }

        private void VideoSource_BitMapFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap videoCapture = (Bitmap)eventArgs.Frame.Clone();
                latestFrameBitmap = videoCapture; // Cập nhật latestFrameBitmap với hình ảnh mới
                imgVideo.Dispatcher.Invoke(() => DisplayImageInImageView(videoCapture, imgVideo));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in VideoSource_BitMapFrame: {ex.Message}");
            }
        }

        private void DisplayImageInImageView(Bitmap frame, System.Windows.Controls.Image imageView)
        {
            System.Windows.Media.Imaging.BitmapImage bitmapImage = ToBitMapImage(frame);

            if (bitmapImage != null)
            {
                imageView.Source = bitmapImage;
            }
        }

        private System.Windows.Media.Imaging.BitmapImage ToBitMapImage(Bitmap bitmap)
        {
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        /*private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
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
        }*/

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
            string folderPath = @"C:\D_Tools\FPT-Learn\HK6\PRN221\ObjectDetection\WPF_MachineService\ScannedImages";

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