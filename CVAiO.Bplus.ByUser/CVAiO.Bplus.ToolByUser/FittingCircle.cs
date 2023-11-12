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

namespace CVAiO.Bplus.ToolByUser
{

    [Serializable]
    public class FittingCircle : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;
        private FittingCircleRunParams runParams;
        [NonSerialized]
        private InteractEllipse circle;
        [NonSerialized]
        private InteractPoint center;
        [NonSerialized]
        private float radius;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public FittingCircleRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new FittingCircleRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [OutputParam, PropertyOrder(13), ReadOnly(true)]
        [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter)), Category("5. Output")]
        public InteractEllipse Circle { get { return circle; } set => circle = value; }

        [OutputParam, PropertyOrder(14), ReadOnly(true), Category("5. Output")]
        public InteractPoint Center { get => center; set => center = value; }

        [OutputParam, PropertyOrder(15), ReadOnly(true), Category("5. Output")]
        public float Radius { get => radius; set => radius = value; }
        #endregion

        public FittingCircle()
        {
            toolName = "Fitting Circle";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Fitting Circle";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public FittingCircle(SerializationInfo info, StreamingContext context) : base(info, context)
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
            outParams.Add("Circle", null);
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
        public virtual void DrawInputs(DisplayView display)
        {
        }
        public virtual void DrawOutputs(DisplayView display)
        {
            if (display.Image == null) return;
            display.GraphicsFuncCollection.Clear();
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Font f = new System.Drawing.Font("굴림", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan);

            if (Circle == null) return;
            System.Drawing.Pen circlePen = new System.Drawing.Pen(System.Drawing.Color.Aqua, 1);
            display.DrawCross(RedPen, AiO.ImageToFixture2D(Circle.Center, inImage.TransformMat), 100, 100, 0);
            display.DrawEllipse(circlePen, AiO.ImageToFixture2D(Circle.Center, inImage.TransformMat), Circle.Height, Circle.Width);
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
                RunParams.SearchRegion.TransformMat = InImage.TransformMat;
                Mat pad = inImage.Mat.SubMat((int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Y + (int)RunParams.SearchRegion.Height,
                                            (int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.X + (int)RunParams.SearchRegion.Width).Clone();
                CVAiO2.Threshold(pad, pad, RunParams.Threshold, 255, ThresholdTypes.BinaryInv);
                //AiO.ShowImage(pad, 1); //Kiểm tra hình sau khi Threshold
                Mat[] contours;
                CVAiO2.FindContours(pad, out contours, new Mat(), RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                int index = contours.ToList().FindIndex(x => x.Height == contours.ToList().Max(x => x.Height));
                Mat mask = new Mat(pad.Height, pad.Width, MatType.CV_8UC1, new Scalar(0));
                CVAiO2.DrawContours(mask, contours, index, new Scalar(255), CVAiO2.FILLED);
                Mat distanceTransform = mask.DistanceTransform(DistanceTypes.L2, DistanceTransformMasks.Precise);
                double minVal, maxVal;
                Point minLoc, maxLoc;
                CVAiO2.MinMaxLoc(distanceTransform, out minVal, out maxVal, out minLoc, out maxLoc);
                Circle = new InteractEllipse(new Point(maxLoc.X + (double)RunParams.SearchRegion.X, maxLoc.Y + (double)RunParams.SearchRegion.Y), (float)maxVal, (float)maxVal);
                Center = new InteractPoint(maxLoc.X + (int)RunParams.SearchRegion.X, maxLoc.Y + (int)RunParams.SearchRegion.Y);
                Radius = (float)maxVal;
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
    public class FittingCircleRunParams : RunParams, ISerializable
    {
        #region Fields
        private InteractRectangle searchRegion;
        private int threshold;
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
        [Description("Ngưỡng để nhị phân ảnh trong quá trình tìm kiếm đường tròn "), PropertyOrder(30)]
        public int Threshold
        {
            get => threshold;
            set
            {
                if (threshold == value || value < 0 || value > 255) return;
                threshold = value;
                NotifyPropertyChanged(nameof(Threshold));
            }
        }
        #endregion
        public FittingCircleRunParams()
        {

        }

        #region Do not change
        public FittingCircleRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public FittingCircleRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (FittingCircleRunParams)runParams;
            }
        }
        #endregion
    }
}
