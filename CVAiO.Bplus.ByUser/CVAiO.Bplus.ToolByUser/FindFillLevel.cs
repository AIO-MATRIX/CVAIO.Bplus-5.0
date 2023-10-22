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
    // https://youtu.be/wEJXDEYiuhU?si=iUL5HNtlBDY3S9k9
    // Kết quả tại:
    // https://youtu.be/jk2_6ARA7yA?si=9rz71ViXEvbw1CEd

    [Serializable]
    public class FindFillLevel : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private FindFillLevelRunParams runParams;
        [NonSerialized]
        private List<int> edgePos = new List<int>();
        [NonSerialized]
        private List<MatchingResult> matchedResults;
        [NonSerialized]
        private bool inspectionResult;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, TypeConverterAttribute(typeof(ExpandableObjectConverter)), Browsable(false)]
        [Category("4. Intput"), PropertyOrder(15)]
        public List<MatchingResult> MatchedResults
        {
            get => matchedResults;

            set => matchedResults = value;
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public FindFillLevelRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new FindFillLevelRunParams();
                return runParams;
            }
            set => runParams = value;
        }


        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspectionResult { get => inspectionResult; set => inspectionResult = value; }

        #endregion

        public FindFillLevel()
        {
            toolName = "Find Fill Level";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Find Fill Level";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public FindFillLevel(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("MatchedResults", null);
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
        }
        public virtual void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage) || MatchedResults == null || MatchedResults.Count == 0) return;
            float lineTickness = 1f;
            float fontSize = 12 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("굴림체", fontSize, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, lineTickness);
            for (int i = 0; i < MatchedResults.Count; i++)
            {
                Rect schROI = new Rect((int)MatchedResults[i].CP.X + RunParams.X, (int)MatchedResults[i].CP.Y + RunParams.Y, RunParams.W, RunParams.H);
                Point rightbottom = schROI.Location + new Point(schROI.Width, schROI.Height);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Aquamarine, 2), AiO.ImageToFixture2D(schROI.Location, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
                System.Drawing.Pen pen = schROI.Y + edgePos[i] > RunParams.InspectionLevel.Y && schROI.Y + edgePos[i] < RunParams.InspectionLevel.Y + RunParams.InspectionLevel.Height ?
                                         new System.Drawing.Pen(System.Drawing.Color.Blue, lineTickness) : new System.Drawing.Pen(System.Drawing.Color.Red, lineTickness);
                display.DrawCross(pen, new Point2d(schROI.X + schROI.Width / 2, schROI.Y + edgePos[i]), 20, 20, 0);
                if (schROI.Y + edgePos[i] > RunParams.InspectionLevel.Y && schROI.Y + edgePos[i] < RunParams.InspectionLevel.Y + RunParams.InspectionLevel.Height)
                    display.DrawString("OK", font, new System.Drawing.SolidBrush(System.Drawing.Color.Blue), new Point2d(schROI.X + schROI.Width / 2 - 5, schROI.Y + edgePos[i] + 10));
                else
                    display.DrawString("NG", font, new System.Drawing.SolidBrush(System.Drawing.Color.Red), new Point2d(schROI.X + schROI.Width / 2 - 5, schROI.Y + edgePos[i] + 10));
            }
            Rect inspectionLevel = RunParams.InspectionLevel.Rect;
            display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.BlueViolet, 2), AiO.ImageToFixture2D(inspectionLevel.Location, InImage.TransformMat),
                AiO.ImageToFixture2D(inspectionLevel.Location + new Point(inspectionLevel.Size.Width - 1, inspectionLevel.Size.Height - 1), InImage.TransformMat));
            //display.DrawString("Upper Limit", font, new System.Drawing.SolidBrush(System.Drawing.Color.Blue), new Point2d(0, inspectionLevel.Location.Y - 10));
            //display.DrawString("Lower Limit", font, new System.Drawing.SolidBrush(System.Drawing.Color.Blue), new Point2d(0, inspectionLevel.Location.Y + inspectionLevel.Height - 10));
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
                edgePos.Clear();
                inspectionResult = true;
                foreach (MatchingResult matchingResult in MatchedResults)
                {
                    Rect schROI = new Rect((int)matchingResult.CP.X + RunParams.X, (int)matchingResult.CP.Y + RunParams.Y, RunParams.W, RunParams.H);
                    Mat roiImage = InImage.Mat[AiO.GetRect(InImage.Mat, new Rect(AiO.FixtureToImage(new Point((int)schROI.Location.X, (int)schROI.Location.Y), InImage.TransformMat),
                                                                        new Size((int)schROI.Width, (int)schROI.Height)))];
                    //AiO.ShowImage(roiImage, 1);
                    Mat<Byte> roiMat = new Mat<Byte>(roiImage);
                    MatIndexer<byte> roiIndexer = roiMat.GetIndexer();

                    Mat<float> projectionMat = new Mat<float>(roiMat.Height, 1);
                    MatIndexer<float> projectionIndexer = projectionMat.GetIndexer();
                    for (int y = 0; y < roiMat.Height; y++)
                    {
                        float sum = 0;
                        for (int x = 0; x < roiMat.Width; x++)
                        {
                            sum += roiIndexer[y, x];
                        }
                        projectionIndexer[y, 0] = sum / roiMat.Width;
                    }

                    Mat<float> filter2DMat = new Mat<float>();
                    Mat prewittFilter = new Mat(3, 1, MatType.CV_32F, new float[3] { -1, 0, 1 });
                    CVAiO2.Filter2D(projectionMat, filter2DMat, -1, prewittFilter);
                    MatIndexer<float> Deriv1stMatIndexer = filter2DMat.GetIndexer();
                    int pointEdge = 0;
                    float maxValue = 0;
                    for (int y = filter2DMat.Height - 1; y >= 0; y--)
                    {
                        float value = Deriv1stMatIndexer[y, 0];
                        if (value > RunParams.MinContrastThreshold)
                        {
                            if (value > maxValue)
                            {
                                maxValue = value;
                                pointEdge = y;
                            }
                        }
                        if (maxValue > 0 && value < maxValue) break;
                    }
                    edgePos.Add(pointEdge);
                    if (schROI.Y + pointEdge < RunParams.InspectionLevel.Y || schROI.Y + pointEdge > RunParams.InspectionLevel.Y + RunParams.InspectionLevel.Height) inspectionResult = false;
                }
                outputImageInfo.Image = OutImage;
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
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
    public class FindFillLevelRunParams : RunParams, ISerializable
    {
        #region Fields
        private int x;
        private int y;
        private int w;
        private int h;
        private float minContrastThreshold;
        private InteractRectangle inspectionLevel;
        #endregion

        #region Properties

        [Description("Vị trí X của vùng ROI tính trong hệ tọa độ đầu vào"), PropertyOrder(30)]
        public int X { get => x; set { if (x == value) return; x = value; NotifyPropertyChanged(nameof(X)); } }

        [Description("Vị trí Y của vùng ROI tính trong hệ tọa độ đầu vào"), PropertyOrder(31)]
        public int Y { get => y; set { if (y == value) return; y = value; NotifyPropertyChanged(nameof(Y)); } }

        [Description("Độ rộng của vùng ROI tính trong hệ tọa độ đầu vào"), PropertyOrder(32)]
        public int W { get => w; set { if (w == value || value < 1) return; w = value; NotifyPropertyChanged(nameof(W)); } }

        [Description("Độ dài của vùng ROI tính trong hệ tọa độ đầu vào"), PropertyOrder(33)]
        public int H { get => h; set { if (h == value || value < 1) return; h = value; NotifyPropertyChanged(nameof(H)); } }

        [Description("Độ tương phản trong việc phát hiện biên Edge (0~255)"), PropertyOrder(34)]
        public float MinContrastThreshold
        {
            get { return minContrastThreshold; }
            set { if (minContrastThreshold == value || value < 0 || value > 255) return; minContrastThreshold = value; NotifyPropertyChanged(nameof(MinContrastThreshold)); }
        }

        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("Vùng kiểm tra độ điền đầy chất lỏng trong lọ"), PropertyOrder(35)]
        public InteractRectangle InspectionLevel
        {
            get
            {
                if (inspectionLevel == null) inspectionLevel = new InteractRectangle(0, 0, 200, 200);
                return inspectionLevel;
            }
            set => inspectionLevel = value;
        }

        #endregion
        public FindFillLevelRunParams()
        {
            minContrastThreshold = 7;
            w = 100;
            h = 100;
        }

        #region Do not change
        public FindFillLevelRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public FindFillLevelRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (FindFillLevelRunParams)runParams;
            }
        }
        #endregion
    }
}
