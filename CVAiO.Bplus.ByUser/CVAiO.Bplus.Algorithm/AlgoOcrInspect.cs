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
    public class AlgoOcrInspect : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        [NonSerialized]
        private string inOcr1;
        [NonSerialized]
        private string inOcr2;

        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private AlgoOcrInspectRunParams runParams;

        [NonSerialized]
        private Execution calc;
        [NonSerialized]
        private bool algoJudgement;

        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoOcrInspectRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoOcrInspectRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false), Description("Vision Algorithm Execute"), Category("4. Input"), PropertyOrder(11)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        [InputParam, ReadOnly(true), Category("4. Input")]
        public string InOcr1 { get => inOcr1; set => inOcr1 = value; }

        [InputParam, ReadOnly(true), Category("4. Input")]
        public string InOcr2 { get => inOcr2; set => inOcr2 = value; }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        #endregion

        public AlgoOcrInspect()
        {
            toolName = "AlgoOcrInspect";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoOcrInspect";
            ToolColor = System.Drawing.Color.Orange;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public AlgoOcrInspect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("InOcr1", null);
            inParams.Add("InOcr2", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
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
            System.Drawing.Font f = new System.Drawing.Font("굴림체", 15, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            display.DrawString(InOcr1, f, SB, new Point2d(700, 100));
            display.DrawString(InOcr2, f, SB, new Point2d(700, 370));
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
                algoJudgement = true;
                OutImage = new Image(InImage.Mat.SubMat((int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Y + (int)RunParams.SearchRegion.Height,
                                                     (int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.X + (int)RunParams.SearchRegion.Width).Clone());
                //if (inOcr1 != RunParams.Ocr1 || inOcr2 != RunParams.Ocr2 || inOcr3 != RunParams.Ocr3)
                //    algoJudgement = false; 
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
    public class AlgoOcrInspectRunParams : RunParams, ISerializable
    {
        #region Fields
        private InteractRectangle searchRegion;
        private string ocr1;
        private string ocr2;
        private string ocr3;
        #endregion

        #region Properties
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("SearchRegion"), PropertyOrder(30)]
        public InteractRectangle SearchRegion
        {
            get
            {
                if (searchRegion == null) searchRegion = new InteractRectangle(0, 0, 200, 200);
                return searchRegion;
            }
            set => searchRegion = value;
        }

        public string Ocr1
        {
            get => ocr1;
            set
            {
                if (ocr1 == value) return;
                ocr1 = value;
                NotifyPropertyChanged(nameof(Ocr1));
            }
        }
        public string Ocr2 
        {
            get => ocr2;
            set
            {
                if (ocr2 == value) return;
                ocr2 = value;
                NotifyPropertyChanged(nameof(Ocr2));
            }
        }
        public string Ocr3 
        {
            get => ocr3;
            set
            {
                if (ocr3 == value) return;
                ocr3 = value;
                NotifyPropertyChanged(nameof(Ocr3));
            }
        }

        #endregion
        public AlgoOcrInspectRunParams()
        {
            ocr1 = "";
            ocr2 = "";
            ocr3 = "";
        }

        #region Do not change
        public AlgoOcrInspectRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoOcrInspectRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoOcrInspectRunParams)runParams;
            }
        }
        #endregion
    }
}
