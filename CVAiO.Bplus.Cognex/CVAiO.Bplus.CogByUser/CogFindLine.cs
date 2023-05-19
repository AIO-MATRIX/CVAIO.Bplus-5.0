using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using System.Drawing.Drawing2D;
using Cognex.VisionPro.CalibFix;

namespace CVAiO.Bplus.CogByUser
{
    [Serializable]
    public class CogFindLine : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private Cognex.VisionPro.Caliper.CogFindLine runParams;
        [NonSerialized]
        private CogFindLineTool cogFindLineTool;
        [NonSerialized]
        private InteractLine line;
        [NonSerialized]
        private InteractPoint center;
        private System.Drawing.Color lineColor;
        private int lineWidth;
        private System.Drawing.Drawing2D.DashStyle lineStyle;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(10)]
        public Cognex.VisionPro.Caliper.CogFindLine RunParams
        {
            get
            {
                if (runParams == null) runParams = new Cognex.VisionPro.Caliper.CogFindLine();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, Browsable(false)]
        public InteractLine Line
        {
            get
            {
                if (line == null) return null;
                return line;
            }
            set
            {
                line = value;
            }
        }
        [OutputParam, Browsable(false)]
        public InteractPoint Center
        {
            get
            {
                if (line == null) return null;
                return new InteractPoint(line.CP.X, line.CP.Y, line.ThetaRad);
            }
            set
            {
                center = value;
            }
        }
        [PropertyOrder(12), Category("4. Output")]
        public System.Drawing.Color LineColor { get => lineColor; set => lineColor = value; }

        [PropertyOrder(13), Category("4. Output"), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int LineWidth { get => lineWidth; set { if (lineWidth == value || value < 1) return; lineWidth = value; } }

        [PropertyOrder(14), Category("4. Output")]
        public DashStyle LineStyle { get => lineStyle; set => lineStyle = value; }
        [Browsable(false)]
        public CogFindLineTool CogFindLineTool
        {
            get
            {
                if (cogFindLineTool == null)
                {
                    cogFindLineTool = new CogFindLineTool();
                    cogFindLineTool.RunParams = this.RunParams;
                }
                return cogFindLineTool;
            }
            set => cogFindLineTool = value;
        }

        #endregion

        public CogFindLine()
        {
            toolName = "Cognex Find Line";
            toolGroup = "Cognex Vision"; // Don't change tool Group
            name = "Cognex Find Line";
            RunParams.Changed += RunParams_Changed;
            RunParams.CaliperRunParams.Changed += RunParams_Changed;
            RunParams.ExpectedLineSegment.Changed += RunParams_Changed;
        }

        private void RunParams_Changed(object sender, CogChangedEventArgs e)
        {
            string[] stateFlags = e.GetStateFlagNames(sender).Split('|');
            foreach (string stateFlag in stateFlags)
                LogWriter.Instance.LogTool(string.Format("{0} - Parameter changed: {1} {2}", this.GetType().ToString().Split('.').Last(), stateFlag.Remove(0, 2),
                    sender.GetType().GetProperty(stateFlag.Remove(0, 2)) == null ? "" : sender.GetType().GetProperty(stateFlag.Remove(0, 2)).GetValue(sender).ToString() ));
        }

        public CogFindLine(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            RunParams.Changed += RunParams_Changed;
            RunParams.CaliperRunParams.Changed += RunParams_Changed;
            RunParams.ExpectedLineSegment.Changed += RunParams_Changed;
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

        public void DrawOutputs(DisplayView display)
        {
            if (Line == null) return;
            if (InImage == null) return;
            if (Line != null)
            {
                System.Drawing.Pen linePen = new System.Drawing.Pen(lineColor, lineWidth);
                linePen.DashStyle = lineStyle;
                Point2d sp = AiO.ImageToFixture2D(Line.SP, InImage.TransformMat);
                Point2d ep = AiO.ImageToFixture2D(Line.EP, InImage.TransformMat);
                display.DrawLine(linePen, sp, ep);
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
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                outputImageInfo.Image = OutImage;
                CogFixtureTool cogFixtureTool = new CogFixtureTool() { InputImage = new CogImage8Grey(InImage.Mat.ToBitmap()) };
                cogFixtureTool.RunParams.UnfixturedFromFixturedTransform = new CogTransform2DLinear()
                {
                    TranslationX = AiO.FixtureToImage3D(new Point3d(0,0,0), InImage.TransformMat).X,
                    TranslationY = AiO.FixtureToImage3D(new Point3d(0,0,0), InImage.TransformMat).Y,
                    Rotation = AiO.FixtureToImage3D(new Point3d(0,0,0), InImage.TransformMat).Z
                 };
                cogFixtureTool.Run();
                if (cogFixtureTool.RunStatus.Result != CogToolResultConstants.Accept) throw new Exception(cogFixtureTool.RunStatus.Message);
                CogFindLineTool.InputImage =  cogFixtureTool.OutputImage as CogImage8Grey;
                CogFindLineTool.Run();
                if (CogFindLineTool.RunStatus.Result != CogToolResultConstants.Accept) throw new Exception(CogFindLineTool.RunStatus.Message);
                Line = new InteractLine(CogFindLineTool.Results.GetLine().X, CogFindLineTool.Results.GetLine().Y,
                                        CogFindLineTool.Results.GetLine().X + 100 * Math.Cos(CogFindLineTool.Results.GetLine().Rotation), CogFindLineTool.Results.GetLine().Y + 100 * Math.Sin(CogFindLineTool.Results.GetLine().Rotation));
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
