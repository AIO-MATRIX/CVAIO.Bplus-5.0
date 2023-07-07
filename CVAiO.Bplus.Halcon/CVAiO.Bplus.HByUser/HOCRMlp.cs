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
    // https://www.mvtec.com/doc/halcon/13/en/toc_ocr_neuralnets.html
    [Serializable]
    public class HOCRMlp : ToolBase
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

        private HOCRMlpRunParams runParams;
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
        public HOCRMlpRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new HOCRMlpRunParams();
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

        [Category("4. Output"), PropertyOrder(14), ReadOnly(true)]
        public string DataOCR { get => dataOCR; set => dataOCR = value; }

        #endregion

        public HOCRMlp()
        {
            toolName = "(H) Ocr Mlp";
            toolGroup = "Halcon Vision"; // Don't change tool Group
            name = "Ocr Mlp";
            ToolColor = System.Drawing.Color.GreenYellow;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public HOCRMlp(SerializationInfo info, StreamingContext context) : base(info, context)
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
                if (blob.Left < RunParams.SearchRegion.X || blob.Left + blob.Width > RunParams.SearchRegion.X + RunParams.SearchRegion.Width ||
                    blob.Top < RunParams.SearchRegion.Y || blob.Top + blob.Height > RunParams.SearchRegion.Y + RunParams.SearchRegion.Height) continue;
                Rectf rectF = new Rectf(blob.Left, blob.Top, blob.Width, blob.Height);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1), rectF);
            }
        }

        public void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage) || listChar.Count == 0 || listPosition.Count != listChar.Count) return;
            System.Drawing.Font f = new System.Drawing.Font("굴림체", 15, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            int minY = (int)listPosition.Min(x => x.Y);
            for (int i = 0; i < listChar.Count; i++)
            {
                display.DrawString(listChar[i].ToString(), f, SB, new Point2d(listPosition[i].X, minY - 15));
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
                if (RunParams.WordParsed.Contains('U') && RunParams.OcrHandleUpper == null) throw new Exception("OcrHandleUpper = Null");
                if (RunParams.WordParsed.Contains('L') && RunParams.OcrHandleLower == null) throw new Exception("OcrHandleLower = Null");
                if (RunParams.WordParsed.Contains('N') && RunParams.OcrHandleNumber == null) throw new Exception("OcrHandleNumber = Null");
                OutImage = InImage.Clone(true);
                outputImageInfo.Image = OutImage;
                dataOCR = "";
                HImage hImage = new HImage("byte", OutImage.Mat.Width, OutImage.Mat.Height, OutImage.Mat.Data);
                Blobs.OrderBy(x => x.Left);
                listChar.Clear();
                listPosition.Clear();
                List<ConnectedComponents.Blob> filteredBlobs = Blobs.FindAll(x =>
                                    x.Left > RunParams.SearchRegion.X && x.Left + x.Width < RunParams.SearchRegion.X + RunParams.SearchRegion.Width
                                    && x.Top > RunParams.SearchRegion.Y && x.Top + x.Height < RunParams.SearchRegion.Y + RunParams.SearchRegion.Height);
                if (filteredBlobs.Count == 0) throw new Exception("No Region found");
                int countChar = 0;
                int maxHeight = filteredBlobs.Max(x => x.Height);
                int avarageBottom = (int)filteredBlobs.Average(x => (x.Top + x.Height));
                foreach (ConnectedComponents.Blob blob in filteredBlobs)
                {
                    if (blob.Top + blob.Height < (int)(avarageBottom - 0.3 * maxHeight)) continue;
                    int characterTop = blob.Top;
                    int characterBottom = blob.Top + blob.Height;
                    int characterLeft = blob.Left;
                    int characterRight = blob.Left + blob.Width;
                    int offset = maxHeight - blob.Height;

                    Mat findRegion = OutImage.Mat.SubMat(characterTop - offset, characterBottom, blob.Left, blob.Left + blob.Width);
                    int foundUpper = offset;
                    for (int i = 0; i < offset; i++)
                    {
                        bool stopSearching = false;
                        for (int j = 0; j < findRegion.Width; j++)
                            if (findRegion.At<byte>(i, j) < 75) { stopSearching = true; break; }
                        if (stopSearching) { foundUpper = i; break; }
                    }
                    findRegion.Dispose();
                    characterTop -= (offset - foundUpper);

                    Mat character = OutImage.Mat.SubMat(characterTop - 1, characterBottom + 1, characterLeft - 1, characterRight + 1);
                    HRegion hRegion = new HRegion((HTuple)(characterTop - 1), (HTuple)(characterLeft - 1), (HTuple)(characterBottom + 1), (HTuple)(characterRight + 1));
                    HTuple classVal, confidence;
                    char foundChar;
                    //The result "\x1A" in classVal signifies that the region has been classified as rejection class.

                    //AiO.ShowImage(character, 1);
                    if (RunParams.WordParsed[countChar] == 'U')
                    {
                        HOperatorSet.DoOcrMultiClassMlp(hRegion, hImage, RunParams.OcrHandleUpper, out classVal, out confidence);
                        if (string.IsNullOrEmpty(classVal.S) || classVal.S.ToLower().Contains("1a") || confidence.D < RunParams.ConfidenceLimit)
                            throw new Exception(string.Format("Char type: U; Confidence: {0}; Limit {1}", confidence.D, RunParams.ConfidenceLimit));
                        // Special case: I (l)
                        if (classVal.S[0] == 'l')
                            foundChar = 'I';
                        else if (classVal.S[0] == '5')
                            foundChar = 'S';
                        else if (classVal.S[0] == '0')
                            foundChar = 'O';
                        else
                            foundChar = classVal.S.ToUpper()[0];
                        if ((int)(foundChar) < 65 || (int)(foundChar) > 90)
                            throw new Exception(string.Format("Char type: U; Found char: {0}; Confidence: {1}; Limit {2}", foundChar, confidence.D, RunParams.ConfidenceLimit));
                    }
                    else if (RunParams.WordParsed[countChar] == 'L')
                    {
                        HOperatorSet.DoOcrMultiClassMlp(hRegion, hImage, RunParams.OcrHandleLower, out classVal, out confidence);
                        if (string.IsNullOrEmpty(classVal.S) || classVal.S.ToLower().Contains("1a") || confidence.D < RunParams.ConfidenceLimit)
                            throw new Exception(string.Format("Char type: L; Confidence: {0}; Limit {1}", confidence.D, RunParams.ConfidenceLimit));
                        if (classVal.S[0] == 'I')
                            foundChar = 'l';
                        else if (classVal.S[0] == '9')
                            foundChar = 'g';
                        else if (classVal.S[0] == '5')
                            foundChar = 's';
                        else if (classVal.S[0] == '0')
                            foundChar = 'o';
                        else
                            foundChar = classVal.S.ToLower()[0];
                        if ((int)(foundChar) < 97 || (int)(foundChar) > 122)
                            throw new Exception(string.Format("Char type: U; Found char: {0}; Confidence: {1}; Limit {2}", foundChar, confidence.D, RunParams.ConfidenceLimit));
                    }
                    else if (RunParams.WordParsed[countChar] == 'N')
                    {
                        HOperatorSet.DoOcrMultiClassMlp(hRegion, hImage, RunParams.OcrHandleNumber, out classVal, out confidence);
                        if (string.IsNullOrEmpty(classVal.S) || classVal.S.ToLower().Contains("1a") || confidence.D < RunParams.ConfidenceLimit)
                            throw new Exception(string.Format("Char type: U; Found char: {0}; Confidence: {1}; Limit {2}", classVal.S[0], confidence.D, RunParams.ConfidenceLimit));
                        foundChar = classVal.S[0];
                    }
                    else
                        throw new Exception("Could not find character type");
                    dataOCR += foundChar;
                    listChar.Add(foundChar);
                    listPosition.Add(new Point2f(blob.Left, blob.Top));
                    character.Dispose();
                    countChar++;
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
            HOperatorSet.ClearOcrClassMlp(RunParams.OcrHandleUpper);
            HOperatorSet.ClearOcrClassMlp(RunParams.OcrHandleLower);
            HOperatorSet.ClearOcrClassMlp(RunParams.OcrHandleNumber);
        }
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class HOCRMlpRunParams : RunParams, ISerializable
    {
        #region Fields
        private InteractRectangle searchRegion;
        private string wordFormat;
        private string wordParsed;
        private string ocrClassifierUpper;
        private string ocrClassifierLower;
        private string ocrClassifierNumber;
        private double confidenceLimit;
        [NonSerialized]
        private HTuple ocrHandleUpper;
        [NonSerialized]
        private HTuple ocrHandleLower;
        [NonSerialized]
        private HTuple ocrHandleNumber;
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

        [Description("Word format: Uppercase{count}Lowercase{count}Number{Count}. Example: U{1}L{7}N{2}L{10}N{3}"), PropertyOrder(32)]
        public string WordFormat 
        {
            get => wordFormat;
            set
            {
                if (wordFormat == value) return;
                string allowedChar = "0123456789{}ULN";
                string wordParsing = "";
                try
                {
                    if (!value.All(x => allowedChar.Contains(x))) throw new Exception("Wrong Format (Example: U{1}L{7}N{2}L{10}N{3})");
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] == 'U' || value[i] == 'L' || value[i] == 'N')
                        {
                            char foundChar = value[i];
                            if (value[i + 1] != '{') throw new Exception("Parsing Error: " + value);
                            string numberString = "";
                            for (int j = i + 2; j < value.Length; j++)
                            {
                                if (value[j] == '}') { i = j; break; }
                                numberString += value[j];
                            }
                            for (int k = 0; k < int.Parse(numberString); k++)
                                wordParsing += foundChar;
                        }
                        else return;
                    }
                }
                catch(Exception ex) { frmMessageBox.Show(EMessageIcon.Error, ex.ToString(), 2500);  return; }
                wordParsed = wordParsing;
                wordFormat = value;

            }
        }

        [ReadOnly(true)]
        public string WordParsed
        {
            get => wordParsed;
            set
            {
                wordParsed = value;
            }
        }
        [Description("Ocr Classifier for Uppercase Letter - omc format"), Category("HOCRMPL"), PropertyOrder(35)]
        [Editor(typeof(OcrClassifierMlpSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OcrClassifierUpper
        {
            get => ocrClassifierUpper;
            set
            {
                if (ocrClassifierUpper == value || value == "") return;
                if (!File.Exists(value)) return;
                ocrClassifierUpper = value;
                if (ocrHandleUpper != null) HOperatorSet.ClearOcrClassMlp(ocrHandleUpper);
                ocrHandleUpper = null;
                NotifyPropertyChanged(nameof(OcrClassifierUpper));
            }
        }

        [Browsable(false)]
        public HTuple OcrHandleUpper
        {
            get
            {
                if (ocrHandleUpper == null)
                {
                    if (string.IsNullOrEmpty(ocrClassifierUpper) || !File.Exists(ocrClassifierUpper)) return null;
                    ocrHandleUpper = new HTuple();
                    HOperatorSet.ReadOcrClassMlp(ocrClassifierUpper, out ocrHandleUpper);
                }
                return ocrHandleUpper;
            }
            set => ocrHandleUpper = value;
        }

        [Description("Ocr Classifier for Lowercase Letter - omc format"), Category("HOCRMPL"), PropertyOrder(35)]
        [Editor(typeof(OcrClassifierMlpSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OcrClassifierLower
        {
            get => ocrClassifierLower;
            set
            {
                if (ocrClassifierLower == value || value == "") return;
                if (!File.Exists(value)) return;
                ocrClassifierLower = value;
                if (ocrHandleLower != null) HOperatorSet.ClearOcrClassMlp(ocrHandleLower);
                ocrHandleLower = null;
                NotifyPropertyChanged(nameof(OcrClassifierLower));
            }
        }

        [Browsable(false)]
        public HTuple OcrHandleLower
        {
            get
            {
                if (ocrHandleLower == null)
                {
                    if (string.IsNullOrEmpty(ocrClassifierLower) || !File.Exists(ocrClassifierLower)) return null;
                    ocrHandleLower = new HTuple();
                    HOperatorSet.ReadOcrClassMlp(ocrClassifierLower, out ocrHandleLower);
                }
                return ocrHandleLower;
            }
            set => ocrHandleLower = value;
        }

        [Description("Ocr Classifier for Number Letter - omc format"), Category("HOCRMPL"), PropertyOrder(35)]
        [Editor(typeof(OcrClassifierMlpSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OcrClassifierNumber
        {
            get => ocrClassifierNumber;
            set
            {
                if (ocrClassifierNumber == value || value == "") return;
                if (!File.Exists(value)) return;
                ocrClassifierNumber = value;
                if (ocrHandleNumber != null) HOperatorSet.ClearOcrClassMlp(ocrHandleNumber);
                ocrHandleNumber = null;
                NotifyPropertyChanged(nameof(OcrClassifierNumber));
            }
        }

        [Browsable(false)]
        public HTuple OcrHandleNumber
        {
            get
            {
                if (ocrHandleNumber == null)
                {
                    if (string.IsNullOrEmpty(ocrClassifierNumber) || !File.Exists(ocrClassifierNumber)) return null;
                    ocrHandleNumber = new HTuple();
                    HOperatorSet.ReadOcrClassMlp(ocrClassifierNumber, out ocrHandleNumber);
                }
                return ocrHandleNumber;
            }
            set => ocrHandleNumber = value;
        }

        [Description("Confidence Limit (0.5~1)"), PropertyOrder(36)]
        public double ConfidenceLimit
        {
            get => confidenceLimit;
            set { if (confidenceLimit == value || value < 0.5 || value > 1) return; confidenceLimit = value; NotifyPropertyChanged(nameof(ConfidenceLimit)); }
        }


        #endregion

        public HOCRMlpRunParams()
        {
            confidenceLimit = 0.5;
        }

        #region Do not change
        public HOCRMlpRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public HOCRMlpRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (HOCRMlpRunParams)runParams;
            }
        }
        #endregion
    }

    public class OcrClassifierMlpSelectionEditor : UITypeEditor
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
                fd.Filter = "OCR MPL Classifier (*.omc)|*.omc";
                fd.FilterIndex = 1;
                fd.Multiselect = true;
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return "";
                value = fd.FileName;
            }
            return value;
        }
    }
}
