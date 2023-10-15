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
using System.Reflection;

namespace CVAiO.Bplus.Algorithm
{
    [Serializable]
    public class AlgoTapeInspection : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        private ImageInfo inputImageInfo;
        private AlgoTapeInspectionRunParams runParams;

        [NonSerialized]
        private Execution calc;
        [NonSerialized]
        private bool algoJudgement;

        [NonSerialized]
        private List<HistogramResult> histograms;
        [NonSerialized]
        private List<InteractPoint> vinylPoints;
        [NonSerialized]
        private List<InteractPoint> tapePoints;
        [NonSerialized]
        private List<InteractLine> lines = new List<InteractLine>();
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<HistogramResult> Histograms { get => histograms; set => histograms = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<InteractPoint> VinylPoints { get => vinylPoints; set => vinylPoints = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<InteractPoint> TapePoints { get => tapePoints; set => tapePoints = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoTapeInspectionRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoTapeInspectionRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false), Description("Vision Algorithm Execute"), Category("4. Input"), PropertyOrder(11)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        #endregion

        public AlgoTapeInspection()
        {
            toolName = "AlgoTapeInspection";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoTapeInspection";
            ToolColor = System.Drawing.Color.Orange;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public AlgoTapeInspection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("Histograms", null);
            inParams.Add("VinylPoints", null);
            inParams.Add("TapePoints", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            inputImageInfo.drawingFunc += DrawInput1;
            imageList.Add(inputImageInfo);
        }
        public void DrawInput1(DisplayView display)
        {
            if (display.Image == null) return;
            if (!AiO.IsPossibleImage(InImage) || Histograms == null || Histograms.Count == 0) return;
            float lineTickness = 0.5f;
            float fontSize = 6 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("arial", fontSize, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, lineTickness);
            System.Drawing.Pen BluePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1);
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Pen GreenPen = new System.Drawing.Pen(System.Drawing.Color.Green, 1);
            int[] thresholdArr = new int[3] { RunParams.TopThreshold, RunParams.MidThreshold, RunParams.BotThreshold };
            for (int i = 0; i < Histograms.Count; i++)
            {
                Rect schROI = Histograms[i].HistogramRegion.Rect;
                Point rightbottom = schROI.Location + new Point(schROI.Size.Width - 1, schROI.Size.Height - 1);
                System.Drawing.Color color;
                if ((i == 0 && Histograms[i].Mean > RunParams.TopThreshold) || i == 1 && Histograms[i].Mean > RunParams.MidThreshold ||
                    i == 2 && Histograms[i].Mean > RunParams.BotThreshold)
                    color = System.Drawing.Color.Red;
                else color = System.Drawing.Color.Blue;
                display.DrawRectangle(new System.Drawing.Pen(color, 1), AiO.ImageToFixture2D(schROI.Location, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
                string result = string.Format("-{0}- Mean: {1:f0} (Limit: {2:f0})", i, Histograms[i].Mean, thresholdArr[i]);
                display.DrawString(result, font, new System.Drawing.SolidBrush(color), AiO.ImageToFixture2D(schROI.Location, InImage.TransformMat));
            }


            foreach (InteractPoint tapePoint in TapePoints)
            {
                display.DrawCross(RedPen, AiO.ImageToFixture2D(tapePoint.Point2d, inImage.TransformMat), 20, 20, 0);
            }
            foreach (InteractPoint vinylPoint in VinylPoints)
            {
                display.DrawCross(BluePen, AiO.ImageToFixture2D(vinylPoint.Point2d, inImage.TransformMat), 20, 20, 0);
            }
            if (lines != null && lines.Count > 0)
            {
                foreach (InteractLine interactLine in lines)
                {
                    Point2d startPoint = AiO.ImageToFixture2D(interactLine.SP, display.Image.TransformMat);
                    Point2d endPoint = AiO.ImageToFixture2D(interactLine.EP, display.Image.TransformMat);
                    if (interactLine.GetLineLength() < RunParams.MinLength || interactLine.GetLineLength() > RunParams.MaxLength)
                        display.DrawLine(RedPen, startPoint, endPoint);
                    else
                        display.DrawLine(GreenPen, startPoint, endPoint);
                }

                Point2d centerPoint = AiO.ImageToFixture2D(lines[0].SP, display.Image.TransformMat);
                display.DrawString(string.Format("Min Length: {0:f0} (Lower Threshold: {1:f1})", lines.Min(x => x.GetLineLength()), RunParams.MinLength), font, solidBrush, centerPoint + new Point2d(0, 50));
                display.DrawString(string.Format("Max length: {0:f0} (Upper Threshold: {1:f1})", lines.Max(x => x.GetLineLength()), RunParams.MaxLength), font, solidBrush, centerPoint + new Point2d(0, 100));
            }
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            if (!AiO.IsPossibleImage(inImage)) throw new Exception("InputImage = Null");
            try
            {
                algoJudgement = true; // kết quả phán định quá trình kiểm tra

                // Kiểm tra kết quả histogram vùng ROI Top
                if (Histograms[0].Mean > RunParams.TopThreshold) algoJudgement = false;

                // Kiểm tra kết quả histogram vùng ROI Mid
                if (Histograms[1].Mean > RunParams.TopThreshold) algoJudgement = false;

                // Kiểm tra kết quả histogram vùng ROI Bot
                if (Histograms[2].Mean > RunParams.TopThreshold) algoJudgement = false;

                lines.Clear();
                int count = -1;
                foreach (InteractPoint tapePoint in tapePoints)
                {
                    count++;
                    if (count == 3 || count == 4 || count == 5 || count == 6 || count == 10 || count == 11 || count == 12 || count == 16 || count == 17 || count == 18) continue;
                    InteractPoint searchVinylPoint = vinylPoints.FirstOrDefault(x => Math.Abs(x.X - tapePoint.X) < 5);
                    if (searchVinylPoint != null)
                        lines.Add(new InteractLine(tapePoint.Point2d, searchVinylPoint.Point2d));
                    else
                        algoJudgement = false;
                }
                // Kiểm tra độ dài nhỏ nhất, lớn nhất của các đoạn thẳng đo được
                if (lines == null || lines.Count == 0 ||
                    lines.Min(x => x.GetLineLength()) < RunParams.MinLength ||
                    lines.Max(x => x.GetLineLength()) > RunParams.MaxLength)
                    algoJudgement = false;

                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                OnRan();
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
    public class AlgoTapeInspectionRunParams : RunParams, ISerializable
    {
        #region Fields
        private int topThreshold;
        private int midThreshold;
        private int botThreshold;
        private int minLength;
        private int maxLength;

        [Description("Giá trị ngưỡng cho vùng ROI phía trên (0~255)"), PropertyOrder(50)]
        [Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int TopThreshold
        {
            get => topThreshold;
            set
            {
                if (value == topThreshold || value < 0 || value > 255) return;
                topThreshold = value;
                NotifyPropertyChanged(nameof(TopThreshold));
            }
        }

        [Description("Giá trị ngưỡng cho vùng ROI giữa (0~255)"), PropertyOrder(51)]
        [Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int MidThreshold
        {
            get => midThreshold;
            set
            {
                if (value == midThreshold || value < 0 || value > 255) return;
                midThreshold = value;
                NotifyPropertyChanged(nameof(MidThreshold));
            }
        }

        [Description("Giá trị ngưỡng cho vùng ROI phía dưới (0~255)"), PropertyOrder(52)]
        [Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int BotThreshold
        {
            get => botThreshold;
            set
            {
                if (value == botThreshold || value < 0 || value > 255) return;
                botThreshold = value;
                NotifyPropertyChanged(nameof(BotThreshold));
            }
        }

        [Description("Khoảng cách nhỏ nhất từ mép tape đến vinyl (px)"), PropertyOrder(53)]
        [Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int MinLength
        {
            get => minLength;
            set
            {
                if (value == minLength || value < 0 || value > MaxLength) return;
                minLength = value;
                NotifyPropertyChanged(nameof(MinLength));
            }
        }

        [Description("Khoảng cách lớn nhất từ mép tape đến vinyl (px)"), PropertyOrder(54)]
        [Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int MaxLength
        {
            get => maxLength;
            set
            {
                if (value == maxLength || value < minLength) return;
                maxLength = value;
                NotifyPropertyChanged(nameof(MaxLength));
            }
        }
        #endregion

        #region Properties
        #endregion
        public AlgoTapeInspectionRunParams()
        {
        }

        #region Do not change
        public AlgoTapeInspectionRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoTapeInspectionRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoTapeInspectionRunParams)runParams;
            }
        }
        #endregion
    }
}
