using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro;

namespace CVAiO.Bplus.CogByUser
{
    [Serializable]
    public class CogPMAlign : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private CogPMAlignRunParams runParams;
        private CogPMAlignPattern pattern;
        [NonSerialized]
        private MatchingResult matchingResult;
        [NonSerialized]
        private CogPMAlignTool cogPMAlignTool;
        [NonSerialized]
        private List<Point3f> calibPoint; // Use for Auto Calibration
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(10)]
        public CogPMAlignRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new CogPMAlignRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Pattern"), Category("3. Tool Params"), PropertyOrder(11)]
        public CogPMAlignPattern Pattern 
        {
            get
            {
                if (pattern == null) pattern = new CogPMAlignPattern();
                return pattern;
            }
            set => pattern = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point
        {
            get
            {
                if (CogPMAlignTool.Results == null || CogPMAlignTool.Results.Count == 0) return null;
                InteractPoint result = new InteractPoint(CogPMAlignTool.Results[0].GetPose().TranslationX, CogPMAlignTool.Results[0].GetPose().TranslationY); 
                result.ThetaRad = CogPMAlignTool.Results[0].GetPose().Rotation;
                return result;
            }
            set { }
        }

        [OutputParam, Browsable(false)]
        public float Score 
        {
            get
            {
                if (CogPMAlignTool.Results == null || CogPMAlignTool.Results.Count == 0) return 0;
                return (float)(CogPMAlignTool.Results[0].Score);
            }
            set { throw new Exception("Can not set"); }
        }

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

        [Browsable(false)]
        public CogPMAlignTool CogPMAlignTool 
        { 
            get 
            { 
                if (cogPMAlignTool == null)
                {
                    cogPMAlignTool = new CogPMAlignTool();
                    cogPMAlignTool.RunParams = this.RunParams;
                    cogPMAlignTool.Pattern = this.Pattern;
                }
                return cogPMAlignTool; 
            }
            set => cogPMAlignTool = value;
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

        public CogPMAlign()
        {
            toolName = "(C) PMAlign";
            toolGroup = "Cognex Vision"; // Don't change tool Group
            name = "(C) PMAlign";
            RunParams.Changed += RunParams_Changed;
            RunParams.StartPose.Changed += RunParams_Changed;
            RunParams.ZoneScaleX.Changed += RunParams_Changed;
            RunParams.ZoneScaleY.Changed += RunParams_Changed;
            RunParams.ZoneScale.Changed += RunParams_Changed;
            RunParams.ZoneAngle.Changed += RunParams_Changed;
            Pattern.Changed += RunParams_Changed;
            Pattern.Composite.Changed += RunParams_Changed;
            Pattern.Origin.Changed += RunParams_Changed;
        }

        private void RunParams_Changed(object sender, CogChangedEventArgs e)
        {
            string[] stateFlags = e.GetStateFlagNames(sender).Split('|');
            foreach (string stateFlag in stateFlags)
                LogWriter.Instance.LogTool(string.Format("{0} - Parameter changed: {1} {2}", this.GetType().ToString().Split('.').Last(), stateFlag.Remove(0, 2),
                    sender.GetType().GetProperty(stateFlag.Remove(0, 2)) == null ? "" : sender.GetType().GetProperty(stateFlag.Remove(0, 2)).GetValue(sender).ToString()));
        }

        public CogPMAlign(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.Changed += RunParams_Changed;
            Pattern.Changed += RunParams_Changed;
            Pattern.Composite.Changed += RunParams_Changed;
            Pattern.Origin.Changed += RunParams_Changed;
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
            //Rect schROI = RunParams.SearchRegion.Rect;
            //Point rightbottom = schROI.Location + new Point(schROI.Size.Width - 1, schROI.Size.Height - 1);
            //display.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Pink, 1), AiO.ImageToFixture2D(schROI.Location, InImage.TransformMat), AiO.ImageToFixture2D(rightbottom, InImage.TransformMat));
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
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            matchingResult = null;
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
                CogImage8Grey cogImage8Grey = new CogImage8Grey(InImage.Mat.ToBitmap());
                CogPMAlignTool.InputImage = cogImage8Grey;
                CogPMAlignTool.Run();
                if (CogPMAlignTool.RunStatus.Result != CogToolResultConstants.Accept || CogPMAlignTool.Results.Count == 0) throw new Exception(CogPMAlignTool.RunStatus.Message);
                MatchingResult.Score = (float)CogPMAlignTool.Results[0].Score;
                MatchingResult.CP = new Point3f((float)CogPMAlignTool.Results[0].GetPose().TranslationX, (float)CogPMAlignTool.Results[0].GetPose().TranslationY, (float)CogPMAlignTool.Results[0].GetPose().Rotation);
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
