using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CVAiO.Bplus.ToolByUser
{
    [Serializable]
    public class BlobFill : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image binaryImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        [NonSerialized]
        private List<Rect> contourRect;
        [NonSerialized]
        private List<double> contourArea;
        [NonSerialized]
        private int contourSelected;

        private BlobFillRunParams runParams;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image BinaryImage { get => binaryImage; set => binaryImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public BlobFillRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new BlobFillRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [Browsable(false)]
        public List<Rect> ContourRect { get { if (contourRect == null) contourRect = new List<Rect>(); return contourRect; } set => contourRect = value; }

        [Browsable(false)]
        public List<double> ContourArea { get { if (contourArea == null) contourArea = new List<double>(); return contourArea; } set => contourArea = value; }

        [Browsable(false)]
        public int ContourSelected { get => contourSelected; set { if (contourSelected == value || value < 0 || value > ContourRect.Count) return; contourSelected = value; } }

        #endregion

        public BlobFill()
        {
            toolName = "Blob Fill";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Blob Fill";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public BlobFill(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("BinaryImage", null);
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
            if (display.Image == null) return;
            if (ContourRect == null || ContourRect.Count == 0) return;
            display.GraphicsFuncCollection.Clear();
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Font f = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan);
            Random rnd = new Random();
            for (int i = 0; i < ContourRect.Count; i++)
            {
                Rectf rectF = new Rectf(ContourRect[i].Left, ContourRect[i].Top, ContourRect[i].Width, ContourRect[i].Height);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1), rectF);
            }
            if (contourSelected > 0 && contourSelected < ContourRect.Count)
            {
                Rectf rectF = new Rectf(ContourRect[contourSelected].Left, ContourRect[contourSelected].Top, ContourRect[contourSelected].Width, ContourRect[contourSelected].Height);
                display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 10), rectF);
                display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Red, 10), new Point2d(ContourRect[contourSelected].Location.X + ContourRect[contourSelected].Width /2, ContourRect[contourSelected].Location.Y + ContourRect[contourSelected].Height / 2), 15, 15);
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
                if (!AiO.IsPossibleImage(inImage) || !AiO.IsPossibleImage(BinaryImage)) throw new Exception("InputImage / BinaryImage = Null");
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);

                if (RunParams.SearchRegion.Width == 0 || RunParams.SearchRegion.Height == 0) return;
                float regionX = (int)RunParams.SearchRegion.Location.X;
                float regionY = (int)RunParams.SearchRegion.Location.Y;
                float regionWidth = (int)RunParams.SearchRegion.Width;
                float regionHeight = (int)RunParams.SearchRegion.Height;
                Rect roiRect = new Rect(AiO.FixtureToImage(new Point(regionX, regionY), InImage.TransformMat),
                                                                    new Size(regionWidth, regionHeight));
                Rect rect = AiO.GetRect(InImage.Mat, roiRect);
                Mat inBinary = BinaryImage.Mat[rect].Clone();
                CVAiO2.InRange(inBinary, new Scalar(10), new Scalar(250), inBinary);
                //AiO.ShowImage(inBinary, 1);
                Point[][] contours; HierarchyIndex[] hierarchyIndex;
                inBinary.FindContours(out contours, out hierarchyIndex, RetrievalModes.List, ContourApproximationModes.ApproxNone);
               // Mat binaryFound = Mat.Zeros(inBinary.Size(), inBinary.GetType());
                if (contours.Length > 0)
                {
                    ContourRect.Clear();
                    ContourArea.Clear();
                    for (int i = 0; i < contours.Length; i++)
                    {
                        ContourArea.Add(CVAiO2.ContourArea(contours[i]));
                        ContourRect.Add(CVAiO2.BoundingRect(contours[i]));
                        if (ContourArea[i] > RunParams.LowerLimit && ContourArea[i] < RunParams.UpperLimit)
                            CVAiO2.FillPoly(OutImage.Mat, new Point[][] { contours[i] }, new Scalar(RunParams.FillColor));
                    }
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
    public class BlobFillRunParams : RunParams, ISerializable
    {
        #region Fields
        private int fillColor = 125;
        private InteractRectangle searchRegion;
        private int upperLimit;
        private int lowerLimit;
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

        [Description("Fill Color 0~255"), Category("Blob Fill"), PropertyOrder(31)]
        public int FillColor { get => fillColor; set { if (fillColor == value || value < 0 || value > 255) return; fillColor = value; NotifyPropertyChanged(nameof(FillColor)); } }

        [Description("Upper Limit of Contour Search, Must be bigger than Lower Limit"), Category("Blob Fill"), PropertyOrder(32)]
        public int UpperLimit { get => upperLimit; set { if (upperLimit == value || value < lowerLimit) return; upperLimit = value; NotifyPropertyChanged(nameof(UpperLimit)); } }

        [Description("Lower Limit of Contour Search, Must be smaller than Upper Limit"), Category("Blob Fill"), PropertyOrder(33)]
        public int LowerLimit { get => lowerLimit; set { if (lowerLimit == value || value < 0 || value > upperLimit) return; lowerLimit = value; NotifyPropertyChanged(nameof(LowerLimit)); } }
        #endregion

        public BlobFillRunParams()
        {
            fillColor = 125;
            upperLimit = 100000;
            lowerLimit = 0;
        }

        #region Do not change
        public BlobFillRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public BlobFillRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (BlobFillRunParams)runParams;
            }
        }
        #endregion
    }
}
