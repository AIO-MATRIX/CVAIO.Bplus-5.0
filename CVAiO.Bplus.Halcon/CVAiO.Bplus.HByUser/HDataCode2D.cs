using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using CVAiO.Bplus.Halcon;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CVAiO.Bplus.HByUser
{
    [Serializable]
    public class HDataCode2D : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;

        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private HDataCode2DRunParams runParams;
        [NonSerialized]
        private string dataCode;
        [NonSerialized]
        private InteractPolyline interactPolyline;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public HDataCode2DRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new HDataCode2DRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, ReadOnly(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Data Code"), Category("4. Output")]
        public string DataCode { get => dataCode; set => dataCode = value; }

        #endregion

        public HDataCode2D()
        {
            toolName = "(H) DataCode2D";
            toolGroup = "Halcon Vision"; // Don't change tool Group
            name = "DataCode2D";
            ToolColor = System.Drawing.Color.GreenYellow;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public HDataCode2D(SerializationInfo info, StreamingContext context) : base(info, context) 
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
            if (!AiO.IsPossibleImage(OutImage) || interactPolyline == null || interactPolyline.Count == 0) return;

            float lineTickness = 0.5f;
            System.Drawing.Pen GreenPen = new System.Drawing.Pen(System.Drawing.Color.LimeGreen, lineTickness);
            for (int i = 0; i < interactPolyline.Count; i++)
            {
                display.DrawLine(GreenPen, new Point2d(interactPolyline[i].X, interactPolyline[i].Y), new Point2d(interactPolyline[(i + 1) % interactPolyline.Count].X, interactPolyline[(i + 1) % interactPolyline.Count].Y));
            }
            System.Drawing.Font f = new System.Drawing.Font("굴림체", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
            display.DrawString(DataCode, f, SB, new Point2d(interactPolyline[0].X, interactPolyline[0].Y));
        }

        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            interactPolyline = null;
            dataCode = "";
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
                outputImageInfo.Image = OutImage;
                HImage hImage = new HImage("byte", InImage.Width, InImage.Height, InImage.Mat.Data);
                HTuple resultHandles = 0;
                HTuple decodedDataStrings;
                Halcon.HXLDCont hXLDCont = RunParams.DataCode2D.FindDataCode2d(hImage, new HTuple(), new HTuple(), out resultHandles, out decodedDataStrings);
                if (resultHandles.Length == 0 || resultHandles.ToIArr()[0] != 0) throw new Exception("Fail to find Data Code");
                HTuple rowContour = new HTuple(), columnContour = new HTuple();
                hXLDCont.GetContourXld(out rowContour, out columnContour);
                List<InteractPoint> interactPoints = new List<InteractPoint>();
                for (int i = 0; i < rowContour.Length; i++)
                    interactPoints.Add(new InteractPoint() { X = (int)columnContour.DArr[i], Y = (int)rowContour.DArr[i] }); 
                 interactPolyline = new InteractPolyline(interactPoints);
                dataCode = decodedDataStrings.ToString();
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
            RunParams.DataCode2D.ClearDataCode2dModel();
            if (InImage != null)
            {
                InImage.Dispose();
            }
        }
    }

    public enum ESymbolType
    {
        Data_Matrix_ECC_200,
        QR_Code,
        Micro_QR_Code,
        PDF417,
        Aztec_Code,
        GS1_DataMatrix,
        GS1_QR_Code,
        GS1_Aztec_Code
    }
    public enum EDefaultParameters
    {
        standard_recognition,
        enhanced_recognition,
        maximum_recognition
    }
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class HDataCode2DRunParams : RunParams, ISerializable
    {
        #region Fields
        private ESymbolType symbolType;
        private EDefaultParameters defaultParameters;
        [NonSerialized]
        private Halcon.HDataCode2D dataCode2D;
        [NonSerialized]
        private Dictionary<string, string> parameters;
        #endregion

        #region Properties

        public ESymbolType SymbolType
        {
            get => symbolType;
            set
            {
                if (symbolType == value) return;
                symbolType = value;
                if (dataCode2D != null) dataCode2D.ClearDataCode2dModel();
                dataCode2D = new Halcon.HDataCode2D();
                dataCode2D.CreateDataCode2dModel(symbolType.ToString().Replace('_', ' '), new HTuple(), new HTuple());
                dataCode2D.SetDataCode2dParam("default_parameters", defaultParameters.ToString());
                UpdateParameters();
                NotifyPropertyChanged(nameof(SymbolType));
            }
        }
        public EDefaultParameters DefaultParameters
        {
            get => defaultParameters;
            set
            {
                if (defaultParameters == value) return;
                defaultParameters = value;
                dataCode2D.SetDataCode2dParam("default_parameters", defaultParameters.ToString());
                UpdateParameters();
                NotifyPropertyChanged(nameof(DefaultParameters));
            }
        }

        [Browsable(false)]
        public Halcon.HDataCode2D DataCode2D
        {
            get
            {
                if (dataCode2D == null)
                {
                    dataCode2D = new Halcon.HDataCode2D();
                    dataCode2D.CreateDataCode2dModel(symbolType.ToString().Replace('_', ' '), new HTuple(), new HTuple());
                    dataCode2D.SetDataCode2dParam("default_parameters", defaultParameters.ToString());
                    UpdateParameters();
                }
                return dataCode2D;
            }
            set => dataCode2D = value;
        }

        public Dictionary<string, string> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new Dictionary<string, string>();
                    #region Adding Parameters
                    parameters.Add("abort", "");
                    parameters.Add("additional_levels", "");
                    parameters.Add("candidate_selection", "");
                    parameters.Add("contrast_min", "");
                    parameters.Add("contrast_tolerance", "");
                    parameters.Add("decoding_scheme", "");
                    parameters.Add("default_parameters", "");
                    parameters.Add("discard_undecoded_candidates", "");
                    parameters.Add("finder_pattern_tolerance", "");
                    parameters.Add("format", "");
                    parameters.Add("mirrored", "");
                    parameters.Add("model_type", "");
                    parameters.Add("module_aspect", "");
                    parameters.Add("module_aspect_max", "");
                    parameters.Add("module_aspect_min", "");
                    parameters.Add("module_gap", "");
                    parameters.Add("module_gap_max", "");
                    parameters.Add("module_gap_min", "");
                    parameters.Add("module_grid", "");
                    parameters.Add("module_size", "");
                    parameters.Add("module_size_max", "");
                    parameters.Add("module_size_min", "");
                    parameters.Add("module_width", "");
                    parameters.Add("module_width_max", "");
                    parameters.Add("module_width_min", "");
                    parameters.Add("persistence", "");
                    parameters.Add("polarity", "");
                    parameters.Add("position_pattern_min", "");
                    parameters.Add("quality_isoiec15415_aperture_size", "");
                    parameters.Add("slant_max", "");
                    parameters.Add("small_modules_robustness", "");
                    parameters.Add("strict_model", "");
                    parameters.Add("strict_quiet_zone", "");
                    parameters.Add("string_encoding", "");
                    parameters.Add("symbol_cols", "");
                    parameters.Add("symbol_cols_max", "");
                    parameters.Add("symbol_cols_min", "");
                    parameters.Add("symbol_rows", "");
                    parameters.Add("symbol_rows_max", "");
                    parameters.Add("symbol_rows_min", "");
                    parameters.Add("symbol_shape", "");
                    parameters.Add("symbol_size", "");
                    parameters.Add("symbol_size_max", "");
                    parameters.Add("symbol_size_min", "");
                    parameters.Add("timeout", "");
                    parameters.Add("trained", "");
                    parameters.Add("version", "");
                    parameters.Add("version_max", "");
                    parameters.Add("version_min", "");
                    #endregion
                }
                return parameters;
            }
            set => parameters = value;
        }

        #endregion
        public HDataCode2DRunParams()
        {
        }

        private void UpdateParameters()
        {
            for (int i = 0; i < Parameters.Keys.Count; i++)
            {
                try
                {
                    string key = Parameters.Keys.ToList()[i];
                    string paramValue = dataCode2D.GetDataCode2dParam(key).ToString();
                    Parameters[key] = paramValue;
                }
                catch
                {
                    string key = Parameters.Keys.ToList()[i];
                    Parameters[key] = "";
                }

            }
        }

        #region Do not change
        public HDataCode2DRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public HDataCode2DRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (HDataCode2DRunParams)runParams;
            }
        }
        #endregion
    }

}
