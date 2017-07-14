using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App17
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DisplayRequest _displayRequest = new DisplayRequest();
        MediaCapture _mediaCapture;
        StorageFolder _captureFolder;

        public MainPage()
        {
            this.InitializeComponent();
            InitMediaCapture();
        }

        async private void InitMediaCapture()
        {
            _mediaCapture = new MediaCapture();
            var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);
            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
            await _mediaCapture.InitializeAsync(settings);
            _displayRequest.RequestActive();
            PreviewControl.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
            
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;

            await Task.Delay(500);
            CaptureImage();
        }
        async private void CaptureImage()
        {
            var storeFile = await _captureFolder.CreateFileAsync("PreviewFrame.jpg", CreationCollisionOption.GenerateUniqueName);
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
            await _mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, storeFile);
            await _mediaCapture.StopPreviewAsync();
        }
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }
    }
}
