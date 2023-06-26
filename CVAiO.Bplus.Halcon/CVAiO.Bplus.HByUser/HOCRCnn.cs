using CVAiO.Bplus.Core;
using CVAiO.Bplus.Halcon;
using CVAiO.Bplus.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CVAiO.Bplus.HByUser
{
    // https://www.mvtec.com/doc/halcon/13/en/toc_ocr_convolutionalneuralnets.html
    [Serializable]
    public class HOCRCnn : ToolBase
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

        private HOCRCnnRunParams runParams;
        [NonSerialized]
        private string dataOCR;
        [NonSerialized]
        private List<char> listChar = new List<char>();
        [NonSerialized]
        private List<Point2f> listPosition = new List<Point2f>();
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(11)]
        public HOCRCnnRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new HOCRCnnRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [Browsable(false), InputParam]
        public List<ConnectedComponents.Blob> Blobs
        {
            get
            {
                return blobs;
            }
            set
            {
                blobs = value;
            }
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [Category("4. Output"), PropertyOrder(12), Browsable(false)]
        public List<char> ListChar { get => listChar; set => listChar = value; }

        [Category("4. Output"), PropertyOrder(13), Browsable(false)]
        public List<Point2f> ListPosition { get => listPosition; set => listPosition = value; }

        [OutputParam, Category("4. Output"), PropertyOrder(14) , ReadOnly(true)]
        public string DataOCR { get => dataOCR; set => dataOCR = value; }

        #endregion

        public HOCRCnn()
        {
            toolName = "(H) Ocr Cnn";
            toolGroup = "Halcon Vision"; // Don't change tool Group
            name = "Ocr Cnn";
            ToolColor = System.Drawing.Color.GreenYellow;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public HOCRCnn(SerializationInfo info, StreamingContext context) : base(info, context)
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
            outParams.Add("DataOCR", null); 
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

        public void DrawInputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(InImage) || Blobs == null || Blobs.Count == 0) return;
            display.GraphicsFuncCollection.Clear();
            Random rnd = new Random();
            foreach (ConnectedComponents.Blob blob in Blobs)
            {
                Rectf rectF = new Rectf(blob.Left, blob.Top, blob.Width, blob.Height);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1), rectF);
            }
        }

        public void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage) || listChar.Count == 0 || listPosition.Count != listChar.Count ) return;
            System.Drawing.Font f = new System.Drawing.Font("굴림체", 15, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            for (int i = 0; i < listChar.Count; i++)
            {
                display.DrawString(listChar[i].ToString(), f, SB, new Point2d(listPosition[i].X, listPosition[i].Y));
            }
        }

        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            dataOCR = "";
            GetOutParams();
        }
        #endregion
        //Note that the pretrained OCR classifiers were trained with symbols that are printed dark on light.
        // Hình ảnh của chữ đầu vào phải là chữ đen trên nền trắng
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
                if (RunParams.OcrHandle == null) throw new Exception("OcrHandle = Null"); 
                OutImage = InImage.Clone(true);
                outputImageInfo.Image = OutImage;
                dataOCR = "";
                HImage hImage = new HImage("byte", InImage.Width, InImage.Height, InImage.Mat.Data);
                listChar.Clear();
                listPosition.Clear();
                foreach (ConnectedComponents.Blob blob in Blobs)
                {
                    HRegion character = new HRegion((double)blob.Top - 2, blob.Left - 2, (double)blob.Top + (double)blob.Height + 2, blob.Left + blob.Width + 2);
                    HTuple classVal, confidence;
                    Halcon.HOperatorSet.DoOcrSingleClassCnn(character, hImage, RunParams.OcrHandle, 1, out classVal, out confidence);
                    //The result "\x1A" in classVal signifies that the region has been classified as rejection class.
                    if (!string.IsNullOrEmpty(classVal.S) && confidence.D > RunParams.ConfidenceLimit)
                    {
                        dataOCR += classVal.S;
                        listChar.Add(classVal.S[0]);
                        listPosition.Add(new Point2f(blob.Left, blob.Top));
                    }    
                }
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
            //Clears the OCR classifier given by OCRHandle that was created and frees all memory required for the classifier.
            //After calling, the classifier can no longer be used. The handle OCRHandle becomes invalid.
            Halcon.HOperatorSet.ClearOcrClassCnn(RunParams.OcrHandle);
        }
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class HOCRCnnRunParams : RunParams, ISerializable
    {
        #region Fields
        private string ocrClassifier;
        private double confidenceLimit;
        [NonSerialized]
        private HTuple ocrHandle;
        #endregion

        #region Properties
        [Description("Ocr Classifier - occ format"), Category("HOCRCnn"), PropertyOrder(30)]
        [Editor(typeof(OcrClassifierCnnSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OcrClassifier
        {
            get => ocrClassifier;
            set
            {
                if (ocrClassifier == value || value == "") return;
                if (!File.Exists(value)) return;
                ocrClassifier = value;
                ocrHandle = null;
                NotifyPropertyChanged(nameof(OcrClassifier));
            }
        }

        [Browsable(false)]
        public HTuple OcrHandle
        {
            get
            {
                if (ocrHandle == null)
                {
                    if (string.IsNullOrEmpty(ocrClassifier) || !File.Exists(ocrClassifier)) return null;
                    ocrHandle = new HTuple();
                    Halcon.HOperatorSet.ReadOcrClassCnn(ocrClassifier, out ocrHandle);
                }
                return ocrHandle;
            }
            set => ocrHandle = value;
        }

        [Description("Confidence Limit (0.5~1)"), PropertyOrder(31)]
        public double ConfidenceLimit 
        {
            get => confidenceLimit;
            set { if (confidenceLimit == value || value < 0.5 || value > 1) return; confidenceLimit = value; NotifyPropertyChanged(nameof(ConfidenceLimit)); }
        }


        #endregion

        public HOCRCnnRunParams()
        {
            confidenceLimit = 0.5;
        }

        #region Do not change
        public HOCRCnnRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public HOCRCnnRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (HOCRCnnRunParams)runParams;
            }
        }
        #endregion
    }

    public class OcrClassifierCnnSelectionEditor : UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (editorService != null)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.InitialDirectory = @"D:\";
                fd.Filter = "OCR CNN Classifier (*.occ)|*.occ";
                fd.FilterIndex = 1;
                fd.Multiselect = true;
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return "";
                value = fd.FileName;
            }
            return value;
        }
    }
}
