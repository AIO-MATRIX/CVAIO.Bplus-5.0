using CVAiO.Bplus.Core;
using CVAiO.Bplus.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.ToolByUser
{
    [Serializable]
    public class InspectionAI : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        //[NonSerialized]
        //private Yolov7 yoloDetector;
        [NonSerialized]
        private List<YoloPrediction> yoloPredictions = new List<YoloPrediction>();

        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;
        private InspectionAIRunParams runParams;

        [NonSerialized]
        private bool inspResult;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public InspectionAIRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new InspectionAIRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        //[Browsable(false)]
        //public Yolov7 YoloDetector 
        //{
        //    get 
        //    {
        //        if (string.IsNullOrEmpty(RunParams.OnnxFile)) return null;
        //        if (yoloDetector == null && System.IO.File.Exists(RunParams.OnnxFile))
        //        {
        //            yoloDetector = new Yolov7(RunParams.OnnxFile, RunParams.UsingGPU);
        //            yoloDetector.SetupYoloDefaultLabels();
        //        } 
        //        return yoloDetector; 
        //    }
        //    set => yoloDetector = value; 
        //}

        [Browsable(false)]
        public List<YoloPrediction> YoloPredictions 
        { 
            get => yoloPredictions;
            set
            {
                yoloPredictions.Clear();
                yoloPredictions = value;
            }
        }
        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspResult { get => inspResult; set => inspResult = value; }

        #endregion

        public InspectionAI()
        {
            toolName = "Inspection AI ";
            toolGroup = "AI Deep Learning"; // Don't change tool Group
            name = "Inspection AI"; 
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public InspectionAI(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
        }
        public override void InitOutParams()
        {
            outParams.Add("OutImage", null);
            outParams.Add("InspResult", null);
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            outputImageInfo.drawingFunc += DrawOutputs;
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }
        public void DrawOutputs(DisplayView display)
        {
            if (display.Image == null) return;
            if (YoloPredictions == null || YoloPredictions.Count == 0) return;
            System.Drawing.Font font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            foreach (YoloPrediction yoloPrediction in YoloPredictions)
            {
                double score = Math.Round(yoloPrediction.Score, 2);
                display.DrawString(string.Format("{0} {1}", yoloPrediction.Label.Name, score), font, solidBrush, new Point2d(yoloPrediction.Rectangle.X + 10, yoloPrediction.Rectangle.Y + 10));
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), new Rectf(yoloPrediction.Rectangle.X, yoloPrediction.Rectangle.Y, yoloPrediction.Rectangle.Width, yoloPrediction.Rectangle.Height));
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
                if (string.IsNullOrEmpty(RunParams.OnnxFile) || !System.IO.File.Exists(RunParams.OnnxFile)) throw new Exception("Could not find Onnx File");
                if (string.IsNullOrEmpty(RunParams.ClassFile) || !System.IO.File.Exists(RunParams.ClassFile)) throw new Exception("Could not find Class File");
                //if (YoloDetector == null) throw new Exception("Yolo Detector is Null");
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);

                using var imageBitmap = BitmapConverter.ToBitmap(InImage.Mat);
                //YoloPredictions = YoloDetector.Predict(imageBitmap); // Chạy AI dự đoán

                // Kiểm tra và phán định OK.NG ở đấy
                inspResult = true; // kết quả phán định sẽ được gửi đến tool Algorithm

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
}
