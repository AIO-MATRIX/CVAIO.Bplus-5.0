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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


//https://multipix.com/halcon-offers-unique-matching-techniques-2d-images/
/*Correlation - based matching uses a normalized cross correlation of gray values to evaluate the correspondence between a model and a search image.
 * It is significantly faster than classical gray-value-based matching and can handle linear variations in illumination as well as rotation relative to the template pattern. 
 * Correlation-based matching is preferred to shape–based matching (see below) for instances when objects may appear with slightly changing shapes, lots of texture, or may be 
 * out of focus(contours vanish in blurred images) in an image.Correlation - based matching does not handle variations in scaling of the object in an image.*/

/*Shape - based matching searches for objects based on edge contours.   
 * Unlike gray-value and correlation-based matching, shape-based matching can determine the position and orientation of objects despite variations in scale (uniform or anisotropic),
 * color, or if the object is occluded and / or cluttered.*/

namespace CVAiO.Bplus.HByUser
{
   /* https://www.mvtec.com/doc/halcon/12/en/toc_matching_shapebased.html */
[Serializable]
    public class HTemplateMatch : ToolBase
    {
        #region Fields
        [NonSerialized]
        private protected Image inImage;
        [NonSerialized]
        private protected Image outImage;

        private protected ImageInfo inputImageInfo;
        private protected ImageInfo outputImageInfo;
        private HTemplateMatchRunParams runParams;

        [NonSerialized]
        private MatchingResult matchingResult;

        [NonSerialized]
        private PatternData selectedPattern;
        [NonSerialized]
        private int selectedPatternIndex = 0;
        [NonSerialized]
        private protected Rect matchingRegion;
        [NonSerialized]
        private List<Point3f> calibPoint; // Use for Auto Calibration
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(10)]
        public HTemplateMatchRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new HTemplateMatchRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [Description("Run Parameters"), Category("4. Output"), PropertyOrder(11), ReadOnly(true)]
        public int SelectedPatternIndex
        {
            get => selectedPatternIndex;
            set
            {
                if (selectedPatternIndex == value || value < 0) return;
                selectedPatternIndex = value;
                NotifyPropertyChanged(nameof(SelectedPatternIndex));
            }
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point
        {
            get
            {
                if (MatchingResult == null) return null;
                InteractPoint result = new InteractPoint(MatchingResult.CP.X, MatchingResult.CP.Y);
                result.ThetaRad = MatchingResult.CP.Z;
                return result;
            }
            set { }

        }

        [OutputParam, Browsable(false)]
        public float Score { get => (float)MatchingResult.Score; set => MatchingResult.Score = value; }

        [OutputParam, Browsable(false)]
        public PatternData SelectedPattern { get => selectedPattern; set => selectedPattern = value; }

        [OutputParam, TypeConverterAttribute(typeof(ExpandableObjectConverter)), ReadOnly(true)]
        [Category("4. Output"), PropertyOrder(12)]
        public MatchingResult MatchingResult
        {
            get
            {
                if (matchingResult == null) matchingResult = new MatchingResult(0, new Point3f(), new Point2f[4]);
                return matchingResult;
            }
            set => matchingResult = value;
        }

        [OutputParam, Browsable(false)]
        public List<Point3f> CalibPoint
        {
            get
            {
                List<Point3f> point3fs = new List<Point3f>();
                point3fs.Add(new Point3f((float)MatchingResult.CP.X, (float)MatchingResult.CP.Y, (float)MatchingResult.CP.Z));
                calibPoint = point3fs;
                return calibPoint;
            }
            set => calibPoint = value;
        }

        #endregion

        public HTemplateMatch()
        {
            toolName = "(H) Template Match";
            toolGroup = "Halcon Vision"; // Don't change tool Group
            name = "Template Match";
            ToolColor = System.Drawing.Color.GreenYellow;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public HTemplateMatch(SerializationInfo info, StreamingContext context) : base(info, context)
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
            outParams.Add("Point", null);
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            outputImageInfo.drawingFunc += DrawOutputs;
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }
        public virtual void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage)) return;
            System.Drawing.Font f = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB;
            Point2d P0 = new Point2d(0, 0);
            Point2d P1, P2;
            string tmp = null;
            Rect schROI = RunParams.SearchRegion.Rect;
            Point rightbottom = schROI.Location + new Point(schROI.Size.Width - 1, schROI.Size.Height - 1);
            display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Pink, 1), AiO.ImageToFixture2D(schROI.Location, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
            if (MatchingResult != null)
            {
                P0 = new Point2d(MatchingResult.CP.X, MatchingResult.CP.Y);
                P1 = P0 + AiO.Rotate(new Point2d(display.Width / 4, 0), MatchingResult.CP.Z);
                P2 = P0 + AiO.Rotate(new Point2d(0, display.Width / 4), MatchingResult.CP.Z);
                display.DrawArrow(new System.Drawing.Pen(System.Drawing.Color.OrangeRed, 1), P0, P1, 10);
                display.DrawArrow(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), P0, P2, 10);
                display.DrawPolyLines(new System.Drawing.Pen(System.Drawing.Color.YellowGreen, 2), MatchingResult.Box);
                SB = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
                tmp = string.Format("P : ({0:F3}, {1:F3})\n\rT : {2:F3}deg\n\rScore : {3:F3}", MatchingResult.CP.X, MatchingResult.CP.Y, MatchingResult.CP.Z * 180f / Math.PI, Score);
                display.DrawString(tmp, f, SB, P0 + new Point2d(10, 10));
                tmp = string.Format("P : ({0:F3}, {1:F3}) T : {2:F3}deg Score : {3:F3}", MatchingResult.CP.X, MatchingResult.CP.Y, MatchingResult.CP.Z * 180f / Math.PI, Score);
                display.DrawString(tmp, f, SB, new System.Drawing.PointF(0, 5));
                display.DrawCross(System.Drawing.Pens.Green, new Point2d(MatchingResult.CP.X, MatchingResult.CP.Y), 100, 100, MatchingResult.CP.Z);
            }
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (outImage != null) outImage.Dispose();
            outImage = null;
            matchingResult = null;
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = inImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            try
            {
                if (!AiO.IsPossibleImage(inImage) || inImage.Mat.Channels() == 3) throw new Exception("InputImage = Null or InputImage Channel is three(Color)");
                if (outImage != null) outImage.Dispose();
                outImage = inImage.Clone(true);
                outputImageInfo.Image = OutImage;
                bool runStatus = false;
                bool foundMark = false;
                // Do image processing
                switch (RunParams.Mode)
                {
                    case EMultiMarkMode.None:
                        // Single Mark Mode: Only First Mark
                        if (RunParams.PatternDatas[0].PatternImage == null || RunParams.PatternDatas[0].PatternImage.Height == 0 || RunParams.PatternDatas[0].PatternImage.Width == 0)
                        throw new Exception ("Main Pattern is NULL or Pattern Height/Width = 0");
                        matchingRegion = new Rect((int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Width - RunParams.PatternDatas[0].PatternImage.Width, (int)RunParams.SearchRegion.Height - RunParams.PatternDatas[0].PatternImage.Height);
                        runStatus = FindTemplate(inImage, RunParams.PatternDatas[0], RunParams.SearchRegion, out matchingResult); 
                        if (!runStatus) throw new Exception("Processing NG");
                        if (this.matchingResult.Score < RunParams.ScoreLimit) throw new Exception("Score Limit NG");
                        SelectedPatternIndex = 0;
                        outputImageInfo.Image = OutImage;
                        RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                        break;

                    case EMultiMarkMode.FirstMark:
                        foundMark = false;
                        for (int i = 0; i < 3; i++)
                        {
                            if (RunParams.PatternDatas[i].PatternImage == null || RunParams.PatternDatas[i].PatternImage.Height == 0 || RunParams.PatternDatas[i].PatternImage.Width == 0)
                                continue;
                            matchingRegion = new Rect((int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Width - RunParams.PatternDatas[i].PatternImage.Width, (int)RunParams.SearchRegion.Height - RunParams.PatternDatas[i].PatternImage.Height);
                            runStatus = FindTemplate(inImage, RunParams.PatternDatas[i], RunParams.SearchRegion, out matchingResult);
                            if (!runStatus) continue;
                            if (this.matchingResult.Score > RunParams.ScoreLimit)
                            {
                                SelectedPatternIndex = i;
                                foundMark = true;
                                break;
                            }
                        }
                        if (!foundMark) throw new Exception("Could not find any mark over limit score");
                        outputImageInfo.Image = OutImage;
                        RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                        break;
                    case EMultiMarkMode.BestMark:
                        List<MatchingResult> matchResults = new List<MatchingResult>();
                        foundMark = false;
                        for (int i = 0; i < 3; i++)
                        {
                            if (RunParams.PatternDatas[i].PatternImage == null || RunParams.PatternDatas[i].PatternImage.Height == 0 || RunParams.PatternDatas[i].PatternImage.Width == 0)
                            {
                                matchResults.Add(new MatchingResult(0, new Point3f(), new Point2f[4])); continue;
                            }
                            MatchingResult matchResult = new MatchingResult(0, new Point3f(), new Point2f[4]);
                            matchingRegion = new Rect((int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Width - RunParams.PatternDatas[i].PatternImage.Width, (int)RunParams.SearchRegion.Height - RunParams.PatternDatas[i].PatternImage.Height);
                            runStatus = FindTemplate(inImage, RunParams.PatternDatas[i], RunParams.SearchRegion, out matchingResult);
                            if (!runStatus)
                            {
                                matchResults.Add(new MatchingResult(0, new Point3f(), new Point2f[4])); continue;
                            }
                            if (matchResult.Score > RunParams.ScoreLimit)
                            {
                                matchResults.Add(matchResult);
                                foundMark = true;
                            }
                            else
                                matchResults.Add(new MatchingResult(0, new Point3f(), new Point2f[4]));
                        }

                        if (!foundMark) throw new Exception("Could not find any mark over limit score");

                        this.MatchingResult = matchResults.FirstOrDefault(x => x != null && x.Score == matchResults.Max(y => y.Score));
                        this.SelectedPatternIndex = matchResults.FindIndex(x => x.Score == this.MatchingResult.Score);
                        RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                        break;
                    default:
                        break;
                }

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

        private protected bool FindTemplate(Image sourceImage, PatternData patternData, InteractRectangle searchingRegion, out MatchingResult matchResult)
        {
            Rect adaptiveSearchingRegion = searchingRegion.Rect;
            Mat searchingImage = sourceImage.Mat.SubMat(adaptiveSearchingRegion).Clone();
            double adaptiveScale = 1;
            if (patternData.PatternImage.Width > 300 || patternData.PatternImage.Height > 300)
                adaptiveScale = Math.Max((double)patternData.PatternImage.Width / 300, (double)patternData.PatternImage.Height / 300);
            Mat searchingImageScale = new Mat();
            CVAiO2.Resize(searchingImage, searchingImageScale, AiO.ResizeSize(searchingImage.Size(), 1 / adaptiveScale), 0, 0, InterpolationFlags.Linear);
            matchResult = new MatchingResult(0, new Point3f(), new Point2f[4]);
            HImage hImage = new HImage("byte", searchingImageScale.Width, searchingImageScale.Height, searchingImageScale.Data);
            HTuple hRow, hColumn, hAngle, hScore;
            if (RunParams.MatchMode == EMatchMode.Shape_Based)
                patternData.ShapeModel.FindShapeModel(hImage, RunParams.AngleNeg * Math.PI / 180, (RunParams.AnglePos - RunParams.AngleNeg) * Math.PI / 180, 0.5, 1, 0, "interpolation", 1, 0, out hRow, out hColumn, out hAngle, out hScore);
            else
            {
                int numLevels = 1;
                if (Math.Max(patternData.PatternImage.Width, patternData.PatternImage.Height) > 150) numLevels = 2;
                patternData.NccModel.FindNccModel(hImage, RunParams.AngleNeg * Math.PI / 180, (RunParams.AnglePos - RunParams.AngleNeg) * Math.PI / 180, 0.5, 1, 0, "true", numLevels, out hRow, out hColumn, out hAngle, out hScore);
            }
                
            searchingImageScale.Dispose();
            searchingImage.Dispose();
            Point3f resultcp;
            try
            {
                resultcp = new Point3f((float)(hColumn.D * adaptiveScale + searchingRegion.X), (float)(hRow.D * adaptiveScale + searchingRegion.Y), (float)-hAngle.D);
                matchResult.Score = (float)((255 - hScore.D) / 255);
            }
            catch { throw new Exception("Could not find mark"); }
            matchResult.Box = Origin2Box(resultcp, patternData.PatternImage.Width, patternData.PatternImage.Height);
            float offsetX = RunParams.Origin.X - patternData.PatternCP.X;
            float offsetY = RunParams.Origin.Y - patternData.PatternCP.Y;
            matchResult.CP = resultcp + new Point3f((float)(offsetX * Math.Cos(resultcp.Z) - offsetY * Math.Sin(resultcp.Z)), (float)(offsetX * Math.Sin(resultcp.Z) + offsetY * Math.Cos(resultcp.Z)), 0);
            return true;
        }

        private Point2f[] Origin2Box(Point3f OriginP, double W, double H)
        {
            Point2f[] Box = new Point2f[4];
            Box[0] = new Point2f((float)OriginP.X, (float)OriginP.Y);
            Box[0] = Box[0] - new Point2f((float)Math.Cos(OriginP.Z), (float)Math.Sin(OriginP.Z)) * (W / 2) - new Point2f((float)Math.Cos(OriginP.Z + Math.PI / 2), (float)Math.Sin(OriginP.Z + Math.PI / 2)) * (H / 2);
            Box[1] = Box[0] + new Point2f((float)Math.Cos(OriginP.Z), (float)Math.Sin(OriginP.Z)) * W;
            Box[3] = Box[0] + new Point2f((float)Math.Cos(OriginP.Z + Math.PI / 2), (float)Math.Sin(OriginP.Z + Math.PI / 2)) * H;
            Box[2] = Box[1] + Box[3] - Box[0];
            return Box;
        }
    }


    public enum EMultiMarkMode
    {
        None,
        FirstMark,
        BestMark,
    }

    public enum EMatchMode
    {
        Shape_Based,
        Correlation_Based
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class HTemplateMatchRunParams : RunParams, ISerializable
    {
        #region Fields
        private int angleNeg;
        private int anglePos;
        private EMultiMarkMode mode;
        private List<PatternData> patternDatas = new List<PatternData>();
        private double scoreLimit = 0.7;

        private InteractRectangle searchRegion;
        private InteractRectangle trainRegion;
        private InteractCoordinate origin;
        private EMatchMode matchMode;
        #endregion

        #region Properties

        [Description("Mode"), PropertyOrder(31)]
        public EMultiMarkMode Mode
        {
            get => mode;
            set
            {
                if (mode == value) return;
                mode = value;
                NotifyPropertyChanged(nameof(Mode));
            }
        }

        [Description("ScoreLimit 0.5 ~ 1.0"), PropertyOrder(32)]
        public double ScoreLimit { get => scoreLimit; set { if (scoreLimit == value || value < 0.5 || value > 1) return; scoreLimit = value; NotifyPropertyChanged(nameof(ScoreLimit)); } }

        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("SearchRegion"), PropertyOrder(33)]
        public InteractRectangle SearchRegion
        {
            get
            {
                if (searchRegion == null) searchRegion = new InteractRectangle(0, 0, 200, 200);
                return searchRegion;
            }
            set => searchRegion = value;
        }
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("SearchRegion"), PropertyOrder(34)]
        public InteractRectangle TrainRegion
        {
            get
            {
                if (trainRegion == null) trainRegion = new InteractRectangle(100, 100, 200, 200);
                return trainRegion;
            }
            set => trainRegion = value;
        }
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("Origin"), PropertyOrder(35)]
        public InteractCoordinate Origin
        {
            get
            {
                if (origin == null) origin = new InteractCoordinate(new System.Drawing.PointF(100, 100), 100, 100);
                return origin;
            }
            set => origin = value;
        }

        [Description("PatternDatas"), PropertyOrder(36), ReadOnly(true)]
        public List<PatternData> PatternDatas
        {
            get
            {
                if (patternDatas == null) patternDatas = new List<PatternData>();
                return patternDatas;
            }
            set => patternDatas = value;
        }

        [Description("https://www.mvtec.com/doc/halcon/12/en/toc_matching.html"), PropertyOrder(37)]
        public EMatchMode MatchMode
        {
            get => matchMode;
            set
            {
                if (matchMode == value) return;
                matchMode = value;
                if (matchMode == EMatchMode.Shape_Based)
                    foreach (PatternData patternData in PatternDatas)
                    {
                        if (patternData.PatternImage == null) continue;
                        patternData.ShapeModel.ClearShapeModel();
                        patternData.ShapeModel = null;
                        patternData.NccModel.ClearNccModel();
                        patternData.NccModel = null;
                        double adaptiveScale = 1;
                        if (patternData.PatternImage.Width > 300 || patternData.PatternImage.Height > 300)
                            adaptiveScale = Math.Max((double)patternData.PatternImage.Width / 300, (double)patternData.PatternImage.Height / 300);
                        Mat templateScale = new Mat();
                        CVAiO2.Resize(patternData.PatternImage.Mat, templateScale, AiO.ResizeSize(patternData.PatternImage.Mat.Size(), 1 / adaptiveScale), 0, 0, InterpolationFlags.Linear);
                        HImage hTemplateImage = new HImage("byte", templateScale.Width, templateScale.Height, templateScale.Data);
                        HTuple angleStep = "auto";
                        patternData.ShapeModel = new HShapeModel(hTemplateImage, (HTuple)3, AngleNeg * Math.PI / 180, (AnglePos - AngleNeg) * Math.PI / 180, angleStep, (HTuple)"auto", "use_polarity", (HTuple)"auto_contrast", (HTuple)"auto");
                    }
                else
                    foreach (PatternData patternData in PatternDatas)
                    {
                        if (patternData.PatternImage == null) continue;
                        patternData.ShapeModel.ClearShapeModel();
                        patternData.ShapeModel = null;
                        patternData.NccModel.ClearNccModel();
                        patternData.NccModel = null;
                        double adaptiveScale = 1;
                        if (patternData.PatternImage.Width > 300 || patternData.PatternImage.Height > 300)
                            adaptiveScale = Math.Max((double)patternData.PatternImage.Width / 300, (double)patternData.PatternImage.Height / 300);
                        Mat templateScale = new Mat();
                        CVAiO2.Resize(patternData.PatternImage.Mat, templateScale, AiO.ResizeSize(patternData.PatternImage.Mat.Size(), 1 / adaptiveScale), 0, 0, InterpolationFlags.Linear);
                        HImage hTemplateImage = new HImage("byte", templateScale.Width, templateScale.Height, templateScale.Data);
                        HTuple angleStep = "auto";
                        patternData.NccModel = new HNCCModel(hTemplateImage, (HTuple)3, AngleNeg * Math.PI / 180, (AnglePos - AngleNeg) * Math.PI / 180, "auto", "use_polarity");
                    }
            }
        }

        [Description("Start Searching Angle"), PropertyOrder(38)]
        public int AngleNeg { get => angleNeg; set { if (angleNeg == value || value > anglePos) return; angleNeg = value;  NotifyPropertyChanged(nameof(AngleNeg)); } }

        [Description("End Searching Angle"), PropertyOrder(39)]
        public int AnglePos { get => anglePos; set { if (anglePos == value || value < angleNeg) return; anglePos = value; NotifyPropertyChanged(nameof(AnglePos)); } }


        #endregion

        public HTemplateMatchRunParams()
        {
            angleNeg = -20;
            anglePos = 20;
            patternDatas.Add(new PatternData());
            patternDatas.Add(new PatternData());
            patternDatas.Add(new PatternData());
        }
        #region Do not change
        public HTemplateMatchRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public HTemplateMatchRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (HTemplateMatchRunParams)runParams;
            }
        }
        #endregion
    }
    [Serializable]
    public class PatternData : ISerializable, IDisposable
    {
        private Image patternImage;
        private Point3f patternCP;
        private InteractMask mask;
        private Mat maskedPattern;
        private double borderColor;
        private HShapeModel shapeModel;
        private HNCCModel nccModel;
        public Image PatternImage { get => patternImage; set => patternImage = value; }
        public Point3f PatternCP { get => patternCP; set => patternCP = value; }
        public InteractMask Mask { get => mask; set => mask = value; }
        public Mat MaskedPattern { get => maskedPattern; set => maskedPattern = value; }
        public double BorderColor { get => borderColor; set => borderColor = value; }
        public HShapeModel ShapeModel 
        {
            get
            {
                if (shapeModel == null) shapeModel = new HShapeModel();
                return shapeModel;
            }
            set => shapeModel = value; 
        }

        public HNCCModel NccModel
        {
            get
            {
                if (nccModel == null) nccModel = new HNCCModel();
                return nccModel;
            }
            set => nccModel = value;
        }

        public PatternData()
        { }

        public PatternData(Image pattern, Point3f center, InteractMask mask = null)
        {
            if (pattern == null)
                return;
            Image temp = new Image(new Mat(pattern.Mat.Size(), MatType.CV_8UC1));
            if (pattern.Mat.Type() == MatType.CV_8UC3)
                CVAiO2.CvtColor(pattern.Mat, temp.Mat, ColorConversionCodes.RGB2GRAY);
            else if (pattern.Mat.Type() == MatType.CV_8UC1)
                temp = pattern.Clone(true);
            else
                return;

            if (patternImage != null)
                patternImage.Dispose();
            patternImage = pattern.Clone(true);
            patternCP = center;
            maskedPattern = patternImage.Mat.Clone();
            borderColor = ((double)patternImage.Mat.Mean() + 255 / 2) % 255;
            if (mask == null || mask.MaskMat.Width != PatternImage.Width || mask.MaskMat.Height != PatternImage.Height)
                this.mask = new InteractMask(pattern.Width, pattern.Height);
            else
                this.mask = mask.Clone();
        }

        public PatternData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }

        public void MakeMaskedPattern()
        {
            if (PatternImage.Mat != null && Mask.MaskMat != null)
            {
                if (MaskedPattern != null)
                    MaskedPattern.Dispose();
                MaskedPattern = new Mat();
                PatternImage.Mat.CopyTo(MaskedPattern, Mask.MaskMat);
                double mean = (double)MaskedPattern.Mean(Mask.MaskMat);
                CVAiO2.ScaleAdd(new Scalar(255) - Mask.MaskMat, mean / 255.0, MaskedPattern, MaskedPattern);
            }
        }
        public void ResetMaskedPattern()
        {
            if (patternImage.Mat != null && Mask.MaskMat != null)
            {
                if (maskedPattern != null)
                    maskedPattern.Dispose();
                maskedPattern = patternImage.Mat.Clone();
            }
        }

        public void Dispose()
        {
            if (patternImage != null) patternImage.Dispose();
            if (mask != null) mask.Dispose();
        }
    }
}
