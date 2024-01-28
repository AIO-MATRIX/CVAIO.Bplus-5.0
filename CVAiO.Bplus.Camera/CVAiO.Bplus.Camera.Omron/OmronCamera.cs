//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Sentech.StApiDotNET;
//using Sentech.GenApiDotNET;
//using CVAiO.Bplus.Core;
//using CVAiO.Bplus.OpenCV;
//using System.Diagnostics;
//using System.Threading;
//using System.Globalization;

//namespace CVAiO.Bplus.Camera.Omron
//{
//    public class OmronCamera : ICamDevice, IDisposable
//    {
//        private struct OmronGrabber
//        {
//            public CStDevice Device;
//            public CStDataStream DataStream;
//            public IntPtr Handle;
//        }

//        private static List<string> CamInfoList;
//        private static List<OmronGrabber> CameraList;
//        private static CStApiAutoInit CstApi = new CStApiAutoInit();
//        private static CStSystem CstSystem = new CStSystem();

//        #region Fields
//        private int CamIndex;
//        private OmronGrabber Camera;
//        private Mat Image;
//        private bool IsGrabSuccess = false;
//        private bool IsGrabFail = false;
//        private object mLock = new object();
//        private Stopwatch stopWatch = new Stopwatch();
//        #endregion

//        #region Properties
//        #endregion

//        public OmronCamera()
//        {
//            InitializeCamera();
//        }

//        private void InitializeCamera()
//        {
//            if (CameraList == null) CameraList = new List<OmronGrabber>(); 
//            else
//            {
//                if (CamInfoList == GetDeviceIDs()) return;
//                foreach (OmronGrabber cam in CameraList)
//                {
//                    cam.Device.AcquisitionStop();
//                    cam.DataStream.StopAcquisition();
//                    cam.DataStream.Dispose();
//                    cam.Device.Dispose();
//                }
//                CameraList.Clear();
//            }
//            CamInfoList = GetDeviceIDs();
//        }

//        private List<string> GetDeviceIDs()
//        {
//            List<string> deviceIDs = new List<string>();
//            CstSystem.UpdateInterfaceList();
//            uint interfaceCount = CstSystem.InterfaceCount;
//            for (uint i = 0; i < interfaceCount; i++)
//            {
//                uint deviceCount = CstSystem.GetIStInterface(i).DeviceCount;
//                for (uint j = 0; j < deviceCount; j++)
//                    deviceIDs.Add(CstSystem.GetIStInterface(i).GetIStDeviceInfo(j).ID);
//            }
//            return deviceIDs;
//        }

//        public void CameraClose()
//        {
//            if (Camera.Device != null) Camera.Device.AcquisitionStop();
//            if (Camera.DataStream != null) Camera.DataStream.StopAcquisition();
//        }

//        // ICamDevice Interface: mở kết nối với camera đã được liệt kê trong danh sách Caminfolist
//        public void CameraOpen(int camNumber)
//        {
//            if (camNumber < 0 || camNumber >= CamInfoList.Count) return; 
//            int index = CameraList.FindIndex((OmronGrabber camera) => CamInfoList[camNumber] == camera.Device.GetIStDeviceInfo().ID);
//            if (index < 0)
//            {
//                if (Camera.Device != null)
//                {
//                    Camera.Device.Dispose();
//                }
//                CstSystem.UpdateInterfaceList();
//                uint interfaceCount = CstSystem.InterfaceCount;
//                for (uint i = 0; i < interfaceCount; ++i)
//                {
//                    IStInterface iInterface = CstSystem.GetIStInterface(i);
//                    if (iInterface.DeviceCount == 0) continue;
//                    try
//                    {
//                        Camera.Device = iInterface.CreateStDevice(CamInfoList[camNumber]);
//                        INodeMap nodeMapRemote = Camera.Device.GetRemoteIStPort().GetINodeMap();
//                        SetEnumeration(nodeMapRemote, "AcquisitionMode", "SingleFrame");
//                        Camera.DataStream = Camera.Device.CreateStDataStream();
//                        Camera.DataStream.StreamBufferCount = 1;
//                        Camera.DataStream.StartAcquisition();
//                        break;
//                    }
//                    catch (Exception ex)
//                    {
//                        LogWriter.Instance.LogException(string.Format("CameraOpen: {0}", ex.ToString()));
//                    }
//                }
//                CameraList.Add(Camera);
//            }
//            else
//            {
//                Camera = CameraList[index];
//            }
//        }

//        // ICamDevice Interface: lấy thông tin của camera
//        public string GetCameraSeiralNo()
//        {
//            return GetCameraSeiralNo(CamIndex);
//        }

//        // ICamDevice Interface: lấy thông tin của camera
//        public string GetCameraSeiralNo(int camNumber)
//        {
//            if (CamInfoList == null || CamInfoList.Count <= camNumber || camNumber < 0) return null;
//            return CamInfoList[camNumber];
//        }

//        // ICamDevice Interface: Kết nối vào camera và trả ra hình ảnh dưới định dạng Mat của OpenCV
//        public Mat GrabImage()
//        {
//            lock (mLock)
//            {
//                try
//                {
//                    if (Camera.Device == null) return null;
//                    if (Image != null) ((DisposableObject)Image).Dispose();
//                    Camera.Handle =  Camera.DataStream.RegisterCallbackMethod(OnImageGrabbed);
//                    Camera.Device.AcquisitionStart();
//                    stopWatch.Reset();
//                    stopWatch.Start();
//                    IsGrabSuccess = false;
//                    IsGrabFail = false;
//                    while (true)
//                    {
//                        if (IsGrabSuccess)  return Image;  
//                        if (IsGrabFail || stopWatch.ElapsedMilliseconds > 2000) return null; 
//                        Thread.Sleep(10);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.Instance.LogException(string.Format("GrabImage: {0}", ex.ToString()));
//                    return null;
//                }
//            }
//        }

//        void SetEnumeration(INodeMap nodeMap, string enumerationName, string valueName)
//        {
//            IEnum enumNode = nodeMap.GetNode<IEnum>(enumerationName);
//            enumNode.StringValue = valueName;
//        }

//        private void OnImageGrabbed(IStCallbackParamBase paramBase, object[] param)
//        {
//            if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
//            {
//                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;
//                if (callbackParam != null)
//                {
//                    try
//                    {
//                        IStDataStream dataStream = callbackParam.GetIStDataStream();
//                        using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(0))
//                        {
//                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
//                            {
//                                IStImage stImage = streamBuffer.GetIStImage();
//                                if (Image != null) ((DisposableObject)Image).Dispose();
//                                Image = new Mat((int)stImage.ImageHeight, (int)stImage.ImageWidth, MatType.CV_8UC1, (Array)(stImage.GetByteArray() as byte[]), 0L);
//                                IsGrabSuccess = true;
//                            }
//                            else
//                            {
//                                IsGrabFail = true;
//                                LogWriter.Instance.LogError("Camera Grab Fail");
//                            }
//                        }
//                    }
//                    catch (Exception exception)
//                    {
//                        IsGrabFail = true;
//                        LogWriter.Instance.LogError(exception.ToString());
//                    }
//                    finally
//                    {
//                        Camera.DataStream.DeregisterCallbackMethod(Camera.Handle);
//                    }
//                }
//            }
//        }


//        // ICamDevice Interface: hàm hủy, đóng kết nối của camera
//        protected virtual void Dispose(bool disposing)
//        {
//            if (Image != null) ((DisposableObject)Image).Dispose();
//            if (Camera.DataStream != null) Camera.DataStream.Dispose();
//            if (Camera.Device != null) Camera.Device.Dispose();
//        }

//        public void Dispose()
//        {
//            Dispose(disposing: true);
//        }

//        public int GetConnectedCamCount()
//        {
//            return CamInfoList.Count;
//        }
//    }
//}
