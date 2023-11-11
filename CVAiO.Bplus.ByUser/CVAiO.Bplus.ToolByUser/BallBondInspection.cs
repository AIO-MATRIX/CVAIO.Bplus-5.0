using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CVAiO.Bplus.ToolByUser
{
    // Tham khảo video setting tại:
    // https://youtu.be/LmsKSqrACpI?si=OMhcxlnJ_VK_PslY
    // Kết quả tại:
    // https://youtu.be/oJFWKhVK54Y?si=UAFP5S_2Hv6K573y
    public struct InspectionDetail
    {
        public bool status;
        public InteractEllipse bondFound;
        public string message;
    }
    [Serializable]
    public class BallBondInspection : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;
        [NonSerialized]
        private List<ConnectedComponents.Blob> blobs;
        [NonSerialized]
        private InspectionDetail[] inspectionDetails;

        private BallBondInspectionRunParams runParams;
        [NonSerialized]
        private bool inspectionResult;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false)]
        public List<ConnectedComponents.Blob> Blobs
        {
            get => blobs;
            set => blobs = value;
        }
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public BallBondInspectionRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new BallBondInspectionRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }
        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspectionResult { get => inspectionResult; set => inspectionResult = value; }

        #endregion

        public BallBondInspection()
        {
            toolName = "Ball Bond Inspection";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Ball Bond Inspection";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public BallBondInspection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("Blobs", null);
        }
        public override void InitOutParams()
        {
            outParams.Add("OutImage", null);
            outParams.Add("InspectionResult", null);
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            inputImageInfo.drawingFunc += DrawInputs;
            outputImageInfo.drawingFunc += DrawOutputs;
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }
        public virtual void DrawInputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(InImage)) return;
            float fontSize = 6 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("arial", fontSize, System.Drawing.FontStyle.Bold);
            if (Blobs == null || Blobs.Count == 0) return;
            foreach (ConnectedComponents.Blob blob in Blobs)
            {
                Point lefttop = new Point(blob.Left, blob.Top);
                Point rightbottom = lefttop + new Point(blob.Width - 1, blob.Height - 1);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 4), AiO.ImageToFixture2D(lefttop, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
            }
        }
        public virtual void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage)) return;
            float fontSize = 6 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("arial", fontSize, System.Drawing.FontStyle.Bold);
            if (Blobs == null || Blobs.Count == 0) return;
            int count = -1;
            foreach (ConnectedComponents.Blob blob in Blobs)
            {
                count++;
                System.Drawing.Color color = !inspectionDetails[count].status ? color = System.Drawing.Color.Red : color = System.Drawing.Color.Blue;
                Point lefttop = new Point(blob.Left, blob.Top);
                Point rightbottom = lefttop + new Point(blob.Width - 1, blob.Height - 1);
                display.DrawRectangle(new System.Drawing.Pen(color, 4), AiO.ImageToFixture2D(lefttop, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
                if (!inspectionDetails[count].status)
                {
                    display.DrawString(inspectionDetails[count].message, font, new System.Drawing.SolidBrush(color), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
                }
                if (inspectionDetails[count].bondFound != null)
                {
                    display.DrawEllipse(new System.Drawing.Pen(color, 4), inspectionDetails[count].bondFound.Center, inspectionDetails[count].bondFound.Height, inspectionDetails[count].bondFound.Width);
                    string result = string.Format("d = {0}-px", 2 * inspectionDetails[count].bondFound.Width);
                    display.DrawString(result, font, new System.Drawing.SolidBrush(color), AiO.ImageToFixture2D(inspectionDetails[count].bondFound.Center + new Point2d(50, 0), InImage.TransformMat));
                }
            }
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            try
            {
                if (!AiO.IsPossibleImage(InImage)) throw new Exception("InputImage = Null");
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                if (Blobs.Count != 2) throw new Exception("Blobs count NG");
                inspectionResult = true;
                inspectionDetails = new InspectionDetail[Blobs.Count];
                int count = -1;
                foreach (ConnectedComponents.Blob blob in Blobs)
                {
                    count++;
                    // Tìm đường tròn fit vào bên trong của contour
                    Mat pad = inImage.Mat.SubMat(blob.Top + 2, blob.Top + blob.Height - 4, blob.Left + 2, blob.Left + blob.Width - 4).Clone();
                    CVAiO2.Threshold(pad, pad, 135, 255, ThresholdTypes.BinaryInv);
                    //AiO.ShowImage(pad, 1); //Kiểm tra hình sau khi Threshold
                    Mat[] contours;
                    CVAiO2.FindContours(pad, out contours, new Mat(), RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                    int index = contours.ToList().FindIndex(x => x.Height == contours.ToList().Max(x => x.Height));
                    Mat mask = new Mat(pad.Height, pad.Width, MatType.CV_8UC1, new Scalar(0));
                    CVAiO2.DrawContours(mask, contours, index, new Scalar(255), CVAiO2.FILLED);
                    Mat distanceTransform = mask.DistanceTransform(DistanceTypes.L2, DistanceTransformMasks.Precise);
                    double minVal, maxVal;
                    Point minLoc, maxLoc;
                    CVAiO2.MinMaxLoc(distanceTransform, out minVal, out maxVal, out minLoc, out maxLoc);

                    // kiểm tra đường ball bond tìm được
                    //CVAiO2.Circle(pad, maxLoc, (int)maxVal, new Scalar(0));
                    //AiO.ShowImage(pad, 1);

                    // Lỗi 1: không tìm thấy đường tròn của vị trí hàn
                    inspectionDetails[count].status = true;
                    if (maxVal < 30)
                    {
                        inspectionDetails[count].status = false;
                        inspectionDetails[count].message = "Ball bond is Missing";
                        inspectionResult = false;
                        continue;
                    }

                    // Lỗi 2: Vị trí hàn lệch về phía biên của pad
                    if (Math.Abs(maxLoc.X - blob.Width / 2) > RunParams.BondCenter || Math.Abs(maxLoc.Y - blob.Height / 2) > RunParams.BondCenter)
                    {
                        inspectionDetails[count].status = false;
                        inspectionDetails[count].message = "Ball Bond too close to Border";
                        inspectionResult = false;
                    }

                    // Lỗi 3: kích thước hàn quá lớn hoặc quá nhỏ
                    if (2 * maxVal < RunParams.BondDMin || 2 * maxVal > RunParams.BondDMax)
                    {
                        inspectionDetails[count].status = false;
                        inspectionDetails[count].message = "Ball Bond too big or too small";
                        inspectionResult = false;
                    }
                    inspectionDetails[count].bondFound = new InteractEllipse(new Point(maxLoc.X + blob.Left + 2, maxLoc.Y + blob.Top + 2), (int)maxVal, (int)maxVal);
                }

                // Kết hợp kết quả kiểm tra ở cả 2 vị trí thành kết quả kiểm tra cuối cùng
                inspectionResult = inspectionDetails[0].status & inspectionDetails[1].status;
                outputImageInfo.Image = OutImage;
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
                inspectionResult = false;
                RunStatus = new RunStatus(EToolResult.Error, ex.ToString());
            }
        }
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
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class BallBondInspectionRunParams : RunParams, ISerializable
    {
        #region Fields
        private float bondDMax;
        private float bondDMin;
        private float bondCenter;
        #endregion

        #region Properties

        [Description("Đường kính lớn nhất của vị trí bond"), PropertyOrder(30)]
        public float BondDMax
        {
            get => bondDMax;
            set
            {
                if (bondDMax == value || value < bondDMin) return;
                bondDMax = value;
                NotifyPropertyChanged(nameof(BondDMax));
            }
        }

        [Description("Đường kính nhỏ nhất của vị trí bond"), PropertyOrder(31)]
        public float BondDMin
        {
            get => bondDMin;
            set
            {
                if (bondDMin == value || value > bondDMax || value < 0) return;
                bondDMin = value;
                NotifyPropertyChanged(nameof(BondDMin));
            }
        }

        [Description("Độ lệch tâm cho phép của vị trí Bond so với Pad"), PropertyOrder(32)]
        public float BondCenter
        {
            get => bondCenter;
            set
            {
                if (bondCenter == value || value < 0) return;
                bondCenter = value;
                NotifyPropertyChanged(nameof(BondCenter));
            }
        }

        #endregion
        public BallBondInspectionRunParams()
        {
            bondDMax = 85;
            bondDMin = 50;
            bondCenter = 12;
        }

        #region Do not change
        public BallBondInspectionRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public BallBondInspectionRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (BallBondInspectionRunParams)runParams;
            }
        }
        #endregion
    }
}
