using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.OpenCV;
using System.Diagnostics;
using Basler.Pylon;

namespace CVAiO.Bplus.Device.ByUser
{
    // Camera type:Basler;
    // Khai báo lớp camera được tạo bởi user, sử dụng interface ICamDevice, IDisposable
    // Bắt buộc phải implement tất cả các thành phần của ICamDevice
    public class Camera_ByUser : ICamDevice, IDisposable
    {
        // Khai báo các trường sẽ được sử dụng trong lớp Camera_ByUser
        #region Fields
        private int CamIndex;
        private Basler.Pylon.Camera Cam;
        private Mat Image;
        private bool IsGrabSuccess = false;
        private bool IsGrabFail = false;
        private object mLock = new object();
        private static List<ICameraInfo> Caminfolist;
        private static List<ICamera> Camlist;
        private Stopwatch stopWatch = new Stopwatch();
        #endregion

        #region Properties
        #endregion

        // Hàm khơi tạo
        public Camera_ByUser()
        {
            InitializeCamera();
            Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "500");
        }

        // Hàm khởi tạo đóng vài trò thu thập danh sách các camera cùng loại được kết nối
        // Các camera được kết nối đến sẽ được list vào danh sách Caminfolist
        private void InitializeCamera()
        {
            if (Camlist == null)
            {
                Camlist = new List<ICamera>();
            }
            else
            {
                if (Caminfolist == CameraFinder.Enumerate())
                {
                    return;
                }
                foreach (ICamera cam in Camlist)
                {
                    cam.Close();
                    cam.Dispose();
                }
                Camlist.Clear();
            }
            Caminfolist = CameraFinder.Enumerate();
        }
  
        // ICamDevice Interface: đóng lại kết nối hiện có của camera
        public void CameraClose()
        {
            if (Cam != null)
            {
                if (Cam.StreamGrabber != null)
                {
                    Cam.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                    Cam.StreamGrabber.Stop();
                }
                Cam.Close();
            }
        }

        // ICamDevice Interface: mở kết nối với camera đã được liệt kê trong danh sách Caminfolist
        public void CameraOpen(int camNumber)
        {
            if (camNumber < 0 || camNumber >= Caminfolist.Count)
            {
                return;
            }
            CamIndex = camNumber;
            int index = Camlist.FindIndex((ICamera ss) => Caminfolist[CamIndex]["SerialNumber"] == ss.CameraInfo["SerialNumber"]);
            if (index < 0)
            {
                Cam = new Basler.Pylon.Camera(Caminfolist[CamIndex]);
                Camlist.Add(Cam);
            }
            else
            {
                Cam = Camlist[index] as Basler.Pylon.Camera;
            }
            if (true)
            {
                Cam.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                Cam.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            }
            lock (mLock)
            {
                if (!Cam.IsOpen)
                {
                    Cam.Open();
                }
                if (!Cam.IsConnected)
                {
                    Cam.Close();
                    Thread.Sleep(500);
                    Cam.Open();
                }
            }
        }

        // ICamDevice Interface: lấy thông tin của camera
        public string GetCameraSeiralNo()
        {
            return GetCameraSeiralNo(CamIndex);
        }

        // ICamDevice Interface: lấy thông tin của camera
        public string GetCameraSeiralNo(int camNumber)
        {
            if (Caminfolist == null)
            {
                return null;
            }
            if (Caminfolist.Count <= camNumber || camNumber < 0)
            {
                return null;
            }
            return Caminfolist[camNumber]["SerialNumber"];
        }

        // ICamDevice Interface: Kết nối vào camera và trả ra hình ảnh dưới định dạng Mat của OpenCV
        public Mat GrabImage()
        {
            lock (mLock)
            {
                try
                {
                    IsGrabSuccess = false;
                    IsGrabFail = false;
                    if (Cam == null)
                    {
                        return null;
                    }
                    if (Image != null)
                    {
                        ((DisposableObject)Image).Dispose();
                    }
                    DateTime startTime = DateTime.Now;
                    if (Cam.StreamGrabber.IsGrabbing)
                    {
                        Cam.StreamGrabber.Stop();
                        LogWriter.Instance.LogError("Why Grabbing Basler?");
                    }
                    if (!Cam.IsOpen)
                    {
                        Cam.Open();
                    }
                    Cam.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                    Cam.StreamGrabber.Start(1L, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                    stopWatch.Reset();
                    stopWatch.Start();
                    while (true)
                    {
                        if (IsGrabSuccess)
                        {
                            return Image;
                        }
                        if (IsGrabFail)
                        {
                            return null;
                        }
                        if (stopWatch.ElapsedMilliseconds > 2000)
                        {
                            break;
                        }
                        Thread.Sleep(10);
                    }
                    LogWriter.Instance.LogError(string.Format("GrabImage: {0}", "Basler Camera Grab TimeOut"));
                    return null;
                }
                catch (Exception ex)
                {
                    LogWriter.Instance.LogException(string.Format("GrabImage: {0}", ex.ToString()));
                    return null;
                }
                finally
                {
                    Cam.StreamGrabber.Stop();
                    Cam.Close();
                }
            }
        }

        // Basler Camera: Event khi việc thu thập hình ảnh thành công
        private void OnImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                if (grabResult.GrabSucceeded)
                {
                    if (Image != null)
                    {
                        ((DisposableObject)Image).Dispose();
                    }
                    Image = new Mat(grabResult.Height, grabResult.Width, MatType.CV_8UC1, (Array)(grabResult.PixelData as byte[]), 0L);
                    IsGrabSuccess = true;
                }
                else
                {
                    IsGrabFail = true;
                    LogWriter.Instance.LogError("Basler Camera Grab Fail");
                }
            }
            catch (Exception exception)
            {
                LogWriter.Instance.LogError(exception.ToString());
            }
            finally
            {
                e.DisposeGrabResultIfClone();
            }
        }

        // ICamDevice Interface: hàm hủy, đóng kết nối của camera
        protected virtual void Dispose(bool disposing)
        {
            if (Cam != null)
            {
                if (Image != null)
                {
                    ((DisposableObject)Image).Dispose();
                }
                if (Cam.IsOpen)
                {
                    Cam.Close();
                }
                Cam.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public int GetConnectedCamCount()
        {
            return Caminfolist.Count;
        }
    }
}
