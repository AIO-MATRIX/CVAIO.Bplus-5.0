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
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro;

namespace CVAiO.Bplus.CogByUser
{
    [Serializable]
    public class CogTemplateMatch : ToolBase
    {
        #region Fields
        [NonSerialized]
        private protected Image inImage;
        [NonSerialized]
        private protected Image outImage;

        private protected ImageInfo inputImageInfo;
        private protected ImageInfo outputImageInfo;
        private CogTemplateMatchRunParams runParams;

        [NonSerialized]
        private MatchingResult matchingResult;

        [NonSerialized]
        private CogPMAlignPattern selectedPattern;
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
        public CogTemplateMatchRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new CogTemplateMatchRunParams();
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
        public CogPMAlignPattern SelectedPattern { get => selectedPattern; set => selectedPattern = value; }

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

        public CogTemplateMatch()
        {
            toolName = "(C) Template Match";
            toolGroup = "Cognex Vision"; // Don't change tool Group
            name = "Template Match";
            ToolColor = System.Drawing.Color.GreenYellow;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public CogTemplateMatch(SerializationInfo info, StreamingContext context) : base(info, context)
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
            System.Drawing.Font f = new System.Drawing.Font("굴림체", 10, System.Drawing.FontStyle.Bold);
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
            if (!AiO.IsPossibleImage(inImage) || inImage.Mat.Channels() == 3) throw new Exception("InputImage = Null or InputImage Channel is three(Color)");
            DateTime lastProcessTimeStart = DateTime.Now;

            try
            {
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
                        if (RunParams.PatternDatas[0] == null || !RunParams.PatternDatas[0].Trained)
                            throw new Exception("Main Pattern is NULL or Pattern not Trained");
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
                            if (RunParams.PatternDatas[i] == null || !RunParams.PatternDatas[i].Trained)
                                continue;
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
                            if (RunParams.PatternDatas[i] == null || !RunParams.PatternDatas[i].Trained)
                            {
                                matchResults.Add(new MatchingResult(0, new Point3f(), new Point2f[4])); continue;
                            }
                            MatchingResult matchResult = new MatchingResult(0, new Point3f(), new Point2f[4]);
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

        private protected bool FindTemplate(Image sourceImage, CogPMAlignPattern patternData, InteractRectangle searchingRegion, out MatchingResult matchResult)
        {
            RunParams.CogPMAlignTool.InputImage = new CogImage8Grey(sourceImage.Mat.ToBitmap());
            RunParams.CogPMAlignTool.Pattern = patternData;
            RunParams.CogPMAlignTool.SearchRegion = new CogRectangle() { X = searchingRegion.X, Y = searchingRegion.Y, Width = searchingRegion.Width, Height = searchingRegion.Height };
            matchResult = new MatchingResult(0, new Point3f(), new Point2f[4]);
            RunParams.CogPMAlignTool.Run();
            if (RunParams.CogPMAlignTool.RunStatus.Result != CogToolResultConstants.Accept || RunParams.CogPMAlignTool.Results.Count == 0) return false;
            matchingResult.Score = (float)RunParams.CogPMAlignTool.Results[0].Score;
            matchingResult.CP = new Point3f((float)RunParams.CogPMAlignTool.Results[0].GetPose().TranslationX, (float)RunParams.CogPMAlignTool.Results[0].GetPose().TranslationY, (float)RunParams.CogPMAlignTool.Results[0].GetPose().Rotation);
            double offsetX = (patternData.TrainRegion as CogRectangle).CenterX - RunParams.Origin.X;
            double offsetY = (patternData.TrainRegion as CogRectangle).CenterY - RunParams.Origin.Y;
            double x = matchingResult.CP.X + offsetX * Math.Cos(matchingResult.CP.Z) - offsetY * Math.Sin(matchingResult.CP.Z);
            double y = matchingResult.CP.Y + offsetX * Math.Sin(matchingResult.CP.Z) + offsetY * Math.Cos(matchingResult.CP.Z);
            matchingResult.Box = OrigintoBox(new Point3f((float)x, (float)y,matchingResult.CP.Z), (patternData.TrainRegion as CogRectangle).Width, (patternData.TrainRegion as CogRectangle).Height);
            return true;
        }
        private Point2f[] OrigintoBox(Point3f OriginP, double W, double H)
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

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class CogTemplateMatchRunParams : RunParams, ISerializable
    {
        #region Fields

        private int angleNeg;
        private int anglePos;
        private double scaleLow;
        private double scaleHigh;


        private EMultiMarkMode mode;
        private List<CogPMAlignPattern> patternDatas = new List<CogPMAlignPattern>();
        private double scoreLimit = 0.7;

        private InteractRectangle searchRegion;
        private InteractRectangle trainRegion;
        private InteractCoordinate origin;

        [NonSerialized]
        private CogPMAlignTool cogPMAlignTool;

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
        public double ScoreLimit
        {
            get => scoreLimit;
            set
            {
                if (scoreLimit == value || value < 0.5 || value > 1) return;
                scoreLimit = value;
                cogPMAlignTool.RunParams.AcceptThreshold = value;
                NotifyPropertyChanged(nameof(ScoreLimit));
            }
        }

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
        public List<CogPMAlignPattern> PatternDatas
        {
            get
            {
                if (patternDatas == null) patternDatas = new List<CogPMAlignPattern>();
                return patternDatas;
            }
            set => patternDatas = value;
        }

        [Description("Start Searching Angle"), PropertyOrder(37)]
        public int AngleNeg
        {
            get => angleNeg;
            set
            {
                if (angleNeg == value || value > anglePos) return;
                angleNeg = value;
                CogPMAlignTool.RunParams.ZoneAngle.Low = value;
                NotifyPropertyChanged(nameof(AngleNeg));
            }
        }

        [Description("End Searching Angle"), PropertyOrder(38)]
        public int AnglePos
        {
            get => anglePos;
            set
            {
                if (anglePos == value || value < angleNeg) return;
                anglePos = value;
                CogPMAlignTool.RunParams.ZoneAngle.High = value;
                NotifyPropertyChanged(nameof(AnglePos));
            }
        }

        [Description("Low scale limit of Found Mark"), PropertyOrder(39)]
        public double ScaleLow
        {
            get => scaleLow;
            set
            {
                if (scaleLow == value || value > scaleHigh || value < 0.5) return;
                scaleLow = value;
                CogPMAlignTool.RunParams.ZoneScale.Low = value;
                NotifyPropertyChanged(nameof(ScaleLow));
            }
        }

        [Description("High scale limit of Found Mark"), PropertyOrder(40)]
        public double ScaleHigh
        {
            get => scaleHigh;
            set
            {
                if (scaleHigh == value || value < scaleLow || value > 1.5) return;
                scaleHigh = value;
                CogPMAlignTool.RunParams.ZoneScale.High = value;
                NotifyPropertyChanged(nameof(ScaleHigh));
            }
        }


        [Browsable(false)]
        public CogPMAlignTool CogPMAlignTool
        {
            get
            {
                if (cogPMAlignTool == null)
                {
                    cogPMAlignTool = new CogPMAlignTool();
                    cogPMAlignTool.RunParams.AcceptThreshold = scoreLimit;
                    cogPMAlignTool.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
                    cogPMAlignTool.RunParams.ZoneAngle.Low = angleNeg * Math.PI / 180;
                    cogPMAlignTool.RunParams.ZoneAngle.High = anglePos * Math.PI / 180;
                    cogPMAlignTool.RunParams.ZoneScale.Configuration = CogPMAlignZoneConstants.LowHigh;
                    cogPMAlignTool.RunParams.ZoneScale.Low = scaleLow;
                    cogPMAlignTool.RunParams.ZoneScale.High = scaleHigh;
                    cogPMAlignTool.RunParams.SearchRegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
                    cogPMAlignTool.RunParams.RunAlgorithm = CogPMAlignRunAlgorithmConstants.PatMax;
                    cogPMAlignTool.RunParams.RunMode = CogPMAlignRunModeConstants.SearchImage;
                    cogPMAlignTool.RunParams.ApproximateNumberToFind = 1;
                    cogPMAlignTool.RunParams.TimeoutEnabled = true;
                    cogPMAlignTool.RunParams.ScoreUsingClutter = false;
                    cogPMAlignTool.SearchRegion = new CogRectangle() { X = SearchRegion.X, Y = SearchRegion.Y, Width = SearchRegion.Width, Height = SearchRegion.Height };
                }
                return cogPMAlignTool;
            }
            set => cogPMAlignTool = value;
        }

        #endregion

        public CogTemplateMatchRunParams()
        {
            angleNeg = -20;
            anglePos = 20;
            scaleLow = 0.8;
            scaleHigh = 1.2;
            patternDatas.Add(new CogPMAlignPattern());
            patternDatas.Add(new CogPMAlignPattern());
            patternDatas.Add(new CogPMAlignPattern());
        }
        #region Do not change
        public CogTemplateMatchRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public CogTemplateMatchRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (CogTemplateMatchRunParams)runParams;
            }
        }
        #endregion
    }
}
