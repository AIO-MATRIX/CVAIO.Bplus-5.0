using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CVAiO.Bplus.ToolByUser
{
    // Kế thừa của ToolBase là bắt buộc
    [Serializable]
    public class ObjectSegmentation : ToolBase
    {
        // Khai báo các trường sẽ sử dụng trong lớp
        #region Fields
        [NonSerialized]
        private Image inImage; // ảnh đầu vào
        [NonSerialized]
        private Image inBinary; // ảnh đầu vào ở dạng Binary
        [NonSerialized]
        private Image outImage; // ảnh đầu ra

        private ImageInfo inputImageInfo; // hinh ảnh sẽ được hiển thị ở ToolEdit
        private ImageInfo outputImageInfo; // hinh ảnh sẽ được hiển thị ở ToolEdit

        private ObjectSegmentationRunParams runParams; // danh sách các parameters sẽ được sử dụng của công cụ
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InBinary { get => inBinary; set => inBinary = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public ObjectSegmentationRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new ObjectSegmentationRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        #endregion

        public ObjectSegmentation()
        {
            toolName = "ObjectSegmentation";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "ObjectSegmentation";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public ObjectSegmentation(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            // Thêm bớt số lượng parameter đầu vào
            // Tên cúa string key phải nằm trong danh sách các input params [InputParam]
            // Giá trị khởi tạo ban đầu mặc định là null
            inParams.Add("InImage", null);
            inParams.Add("InBinary", null);
        }
        public override void InitOutParams()
        {
            // Thêm bớt số lượng parameter đầu ra
            // Tên cúa string key phải nằm trong danh sách các input params [InputParam]
            // Giá trị khởi tạo ban đầu mặc định là null
            outParams.Add("OutImage", null);
        }
        public override void InitImageList()
        {
            // khởi tạo giá trị của các param imageInfo dùng để hiển thị ở tool edit control
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            inputImageInfo.drawingFunc += DrawInputs;
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            outputImageInfo.drawingFunc += DrawOutputs;
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }

        public void DrawInputs(DisplayView display)
        {
            if (display.Image == null) return;
            //display.DrawLine(....)
        }

        public void DrawOutputs(DisplayView display)
        {
            if (display.Image == null) return;
            //display.DrawLine(....)
        }
        public override void InitOutProperty()
        {
            // khởi tạo giá trị cho các output parameters
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            // thực hiện các lệnh xử lý ảnh để lấy ra kết quả mong muốn
            #region default
            // không nên sửa đổi nếu không thực sự cần thiết
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            #endregion
            try
            {
                if (!AiO.IsPossibleImage(InImage)) throw new Exception("InputImage = Null");
                if (!AiO.IsPossibleImage(InBinary)) throw new Exception("InBinary = Null");
                Mat DistanceTransform = InBinary.Mat.Clone();
                //AiO.ShowImage(DistanceTransform, 1);
                Point[][] contours; HierarchyIndex[] hierarchyIndex;
                DistanceTransform.FindContours(out contours, out hierarchyIndex, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                DistanceTransform.DrawContours(contours, -1, new Scalar(255), -1, LineTypes.Link8, hierarchyIndex);
                AiO.ShowImage(DistanceTransform, 1);
                CVAiO2.DistanceTransform(DistanceTransform, DistanceTransform, DistanceTypes.L2, DistanceTransformMasks.Mask5);
                AiO.ShowImage(DistanceTransform, 1);
                double min, max;
                DistanceTransform.MinMaxLoc(out min, out max);
                CVAiO2.Threshold(DistanceTransform, DistanceTransform, RunParams.Threshold * max, 255, 0);
                AiO.ShowImage(DistanceTransform, 1);
                DistanceTransform.ConvertTo(DistanceTransform, MatType.CV_8U);
                Mat Marker = Mat.Zeros(DistanceTransform.Size(), MatType.CV_32SC1);
                DistanceTransform.FindContours(out contours, out hierarchyIndex, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                for (int i = 0; i < contours.Length; i++)
                    Marker.DrawContours(contours, i, new Scalar(50 + 2 * i), -1, LineTypes.Link8, hierarchyIndex);
                Marker.Circle(new Point(15, 15), 5, new Scalar(255), 10);
                Mat inImageColor = new Mat();
                CVAiO2.CvtColor(inImage.Mat, inImageColor, ColorConversionCodes.GRAY2RGB);
                CVAiO2.Watershed(inImageColor, Marker);
                AiO.ShowImage(inImageColor, 1);
                AiO.ShowImage(Marker, 1);
                inImageColor.Dispose();
                DistanceTransform.Dispose();
                Marker.ConvertTo(Marker, MatType.CV_8U);
                if (OutImage != null) OutImage.Dispose();
                OutImage = new Image(Marker.Clone());
                Marker.Dispose();
                outputImageInfo.Image = OutImage;
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
                RunStatus = new RunStatus(EToolResult.Error, ex.ToString());
            }
        }

        #region default
        // không nên sửa đổi nếu không thực sự cần thiết
        public override void Reset()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
        }

        public override void Dispose()
        {
            InitOutProperty();
            if (InImage != null)
            {
                InImage.Dispose();
            }
        }
        #endregion
    }
}
