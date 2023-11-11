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
using CVAiO.Bplus.ImageProcessing;

namespace CVAiO.Bplus.ToolByUser
{
    // Tham khảo video setting tại:
    // https://youtu.be/HgjLVuVjfa0?si=D3duO0rDCywbw15I
    // Kết quả tại:
    // https://youtu.be/K3oWyQgw80s?si=aY-PmjsATRYAN8qJ
    [Serializable]
    public class ConnectorInspection : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private ConnectorInspectionRunParams runParams;
        [NonSerialized]
        private bool inspectionResult;

        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public ConnectorInspectionRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new ConnectorInspectionRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }
        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspectionResult { get => inspectionResult; set => inspectionResult = value; }

        #endregion

        public ConnectorInspection()
        {
            toolName = "Connector Inspection";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Connector Inspection";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public ConnectorInspection(SerializationInfo info, StreamingContext context) : base(info, context)
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
            outParams.Add("InspectionResult", null);
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
            if (!AiO.IsPossibleImage(InImage)) return;
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Orange, 1), RunParams.OrangeWire.RegionCenter.Point2d, RunParams.OrangeWire.RegionRadius, RunParams.OrangeWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Yellow, 1), RunParams.YellowWire.RegionCenter.Point2d, RunParams.YellowWire.RegionRadius, RunParams.YellowWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Green, 1), RunParams.GreenWire.RegionCenter.Point2d, RunParams.GreenWire.RegionRadius, RunParams.GreenWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), RunParams.BlueWire.RegionCenter.Point2d, RunParams.BlueWire.RegionRadius, RunParams.BlueWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Purple, 1), RunParams.PurpleWire.RegionCenter.Point2d, RunParams.PurpleWire.RegionRadius, RunParams.PurpleWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Gray, 1), RunParams.GrayWire.RegionCenter.Point2d, RunParams.GrayWire.RegionRadius, RunParams.GrayWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.White, 1), RunParams.WhiteWire.RegionCenter.Point2d, RunParams.WhiteWire.RegionRadius, RunParams.WhiteWire.RegionRadius);
        }
        public virtual void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage)) return;
            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);
            System.Drawing.Pen bluePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Orange, 1), RunParams.OrangeWire.RegionCenter.Point2d, RunParams.OrangeWire.RegionRadius, RunParams.OrangeWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Yellow, 1), RunParams.YellowWire.RegionCenter.Point2d, RunParams.YellowWire.RegionRadius, RunParams.YellowWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Green, 1), RunParams.GreenWire.RegionCenter.Point2d, RunParams.GreenWire.RegionRadius, RunParams.GreenWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), RunParams.BlueWire.RegionCenter.Point2d, RunParams.BlueWire.RegionRadius, RunParams.BlueWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Purple, 1), RunParams.PurpleWire.RegionCenter.Point2d, RunParams.PurpleWire.RegionRadius, RunParams.PurpleWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Gray, 1), RunParams.GrayWire.RegionCenter.Point2d, RunParams.GrayWire.RegionRadius, RunParams.GrayWire.RegionRadius);
            display.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.White, 1), RunParams.WhiteWire.RegionCenter.Point2d, RunParams.WhiteWire.RegionRadius, RunParams.WhiteWire.RegionRadius);

            List<InspectionRegion> inspectionRegions = new List<InspectionRegion>();
            inspectionRegions.Add(RunParams.OrangeWire); inspectionRegions.Add(RunParams.YellowWire);
            inspectionRegions.Add(RunParams.GreenWire); inspectionRegions.Add(RunParams.BlueWire);
            inspectionRegions.Add(RunParams.PurpleWire); inspectionRegions.Add(RunParams.GrayWire);
            inspectionRegions.Add(RunParams.WhiteWire);
            float fontSize = 1.5f / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("Arial", fontSize, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

            foreach (InspectionRegion inspectionRegion in inspectionRegions)
            {
                if (inspectionRegion.RedStatus && inspectionRegion.BlueStatus && inspectionRegion.GreenStatus)
                    display.DrawEllipse(bluePen, inspectionRegion.RegionCenter.Point2d, 4 * inspectionRegion.RegionRadius, 4 * inspectionRegion.RegionRadius);
                else
                {
                    display.DrawEllipse(redPen, inspectionRegion.RegionCenter.Point2d, 4 * inspectionRegion.RegionRadius, 4 * inspectionRegion.RegionRadius);
                    display.DrawString(string.Format("NG"), font, solidBrush, inspectionRegion.RegionCenter.Point2d + new Point2d(0, 20));
                }

                //int count = 0;
                //if (!inspectionRegion.RedStatus)
                //    display.DrawString(string.Format("Red: {0} ({1} - {2})", inspectionRegion.RedMean, inspectionRegion.RedMin, inspectionRegion.RedMax), font, solidBrush, inspectionRegion.RegionCenter.Point2d + new Point2d(0, 20 * count++));
                //if (!inspectionRegion.RedStatus)
                //    display.DrawString(string.Format("Blue: {0} ({1} - {2})", inspectionRegion.BlueMean, inspectionRegion.BlueMin, inspectionRegion.BlueMax), font, solidBrush, inspectionRegion.RegionCenter.Point2d + new Point2d(0, 20 * count++));
                //if (!inspectionRegion.RedStatus)
                //    display.DrawString(string.Format("Green: {0} ({1} - {2})", inspectionRegion.GreenMean, inspectionRegion.GreenMin, inspectionRegion.GreenMax), font, solidBrush, inspectionRegion.RegionCenter.Point2d + new Point2d(0, 20 * count++));
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
                if (InImage.Mat.Channels() != 3) throw new Exception("InputImage != Color Image");
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                inspectionResult = true;
                List<InspectionRegion> inspectionRegions = new List<InspectionRegion>();
                inspectionRegions.Add(RunParams.OrangeWire); inspectionRegions.Add(RunParams.YellowWire);
                inspectionRegions.Add(RunParams.GreenWire); inspectionRegions.Add(RunParams.BlueWire);
                inspectionRegions.Add(RunParams.PurpleWire); inspectionRegions.Add(RunParams.GrayWire);
                inspectionRegions.Add(RunParams.WhiteWire);

                Mat currentRoi = new Mat();
                Mat[] RGB = new Mat[3];
                Scalar mean, dev;
                foreach (InspectionRegion inspectionRegion in inspectionRegions)
                {
                    currentRoi = InImage.Mat.SubMat((int)(inspectionRegion.RegionCenter.Y - inspectionRegion.RegionRadius), (int)(inspectionRegion.RegionCenter.Y + inspectionRegion.RegionRadius),
                                                                        (int)(inspectionRegion.RegionCenter.X - inspectionRegion.RegionRadius), (int)(inspectionRegion.RegionCenter.X + inspectionRegion.RegionRadius));
                    CVAiO2.Split(currentRoi, out RGB);
                    // Kiểm tra kênh BLUE
                    CVAiO2.MeanStdDev(RGB[0], out mean, out dev);
                    inspectionRegion.BlueMean = (float)mean;
                    if (inspectionRegion.BlueMean < inspectionRegion.BlueMin || inspectionRegion.BlueMean > inspectionRegion.BlueMax) { inspectionRegion.BlueStatus = false; inspectionResult = false; }
                    else inspectionRegion.BlueStatus = true;

                    // Kiểm tra kênh BLUE
                    CVAiO2.MeanStdDev(RGB[1], out mean, out dev);
                    inspectionRegion.GreenMean = (float)mean;
                    if (inspectionRegion.GreenMean < inspectionRegion.GreenMin || inspectionRegion.GreenMean > inspectionRegion.GreenMax) { inspectionRegion.GreenStatus = false; inspectionResult = false; }
                    else inspectionRegion.GreenStatus = true;

                    // Kiểm tra kênh RED
                    CVAiO2.MeanStdDev(RGB[2], out mean, out dev);
                    inspectionRegion.RedMean = (float)mean;
                    if (inspectionRegion.RedMean < inspectionRegion.RedMin || inspectionRegion.RedMean > inspectionRegion.RedMax) { inspectionRegion.RedStatus = false; inspectionResult = false; }
                    else inspectionRegion.RedStatus = true;
                }

                outputImageInfo.Image = OutImage;
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
                inspectionResult = false;
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
    public class ConnectorInspectionRunParams : RunParams, ISerializable
    {
        #region Fields
        private InspectionRegion orangeWire;
        private InspectionRegion yellowWire;
        private InspectionRegion greenWire;
        private InspectionRegion blueWire;
        private InspectionRegion purpleWire;
        private InspectionRegion grayWire;
        private InspectionRegion whiteWire;
        private int rangeLow = 0;
        private int rangeHigh = 255;
        #endregion

        #region Properties

        [Description("Vùng kiểm tra của dây màu da cam"), PropertyOrder(30)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion OrangeWire
        {
            get
            {
                if (orangeWire == null) orangeWire = new InspectionRegion();
                return orangeWire;
            }
            set => orangeWire = value;
        }

        [Description("Vùng kiểm tra của dây màu vàng"), PropertyOrder(31)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion YellowWire
        {
            get
            {
                if (yellowWire == null) yellowWire = new InspectionRegion();
                return yellowWire;
            }
            set => yellowWire = value;
        }

        [Description("Vùng kiểm tra của dây màu xanh lá"), PropertyOrder(32)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion GreenWire
        {
            get
            {
                if (greenWire == null) greenWire = new InspectionRegion();
                return greenWire;
            }
            set => greenWire = value;
        }

        [Description("Vùng kiểm tra của dây màu xanh nước biển"), PropertyOrder(33)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion BlueWire
        {
            get
            {
                if (blueWire == null) blueWire = new InspectionRegion();
                return blueWire;
            }
            set => blueWire = value;
        }

        [Description("Vùng kiểm tra của dây màu tím"), PropertyOrder(34)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion PurpleWire
        {
            get
            {
                if (purpleWire == null) purpleWire = new InspectionRegion();
                return purpleWire;
            }
            set => purpleWire = value;
        }

        [Description("Vùng kiểm tra của dây màu xám"), PropertyOrder(35)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion GrayWire
        {
            get
            {
                if (grayWire == null) grayWire = new InspectionRegion();
                return grayWire;
            }
            set => grayWire = value;
        }

        [Description("Vùng kiểm tra của dây màu trắng"), PropertyOrder(36)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InspectionRegion WhiteWire
        {
            get
            {
                if (whiteWire == null) whiteWire = new InspectionRegion();
                return whiteWire;
            }
            set => whiteWire = value;
        }

        [Description("Range Low"), PropertyOrder(33), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int RangeLow { get => rangeLow; set { if (rangeLow == value || value < 0 || value > 255) return; rangeLow = value; NotifyPropertyChanged(nameof(RangeLow)); } }

        [Description("Range High"), PropertyOrder(34), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int RangeHigh { get => rangeHigh; set { if (rangeHigh == value || value < 0 || value > 255) return; rangeHigh = value; NotifyPropertyChanged(nameof(RangeHigh)); } }

        #endregion
        public ConnectorInspectionRunParams()
        {

        }

        #region Do not change
        public ConnectorInspectionRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public ConnectorInspectionRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (ConnectorInspectionRunParams)runParams;
            }
        }
        #endregion
    }

    [Serializable]
    public class InspectionRegion
    {
        #region Fields
        private InteractPoint regionCenter;
        private float regionRadius;

        private float redMin;
        private float redMean;
        private float redMax;
        private bool redStatus;

        private float blueMin;
        private float blueMean;
        private float blueMax;
        private bool blueStatus;

        private float greenMin;
        private float greenMean;
        private float greenMax;
        private bool greenStatus;

        #endregion

        #region Properties
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [Description("Tâm của vùng kiểm tra, tính toán Histogram"), PropertyOrder(50)]
        public InteractPoint RegionCenter
        {
            get
            {
                if (regionCenter == null) regionCenter = new InteractPoint(100, 100) { Color = System.Drawing.Color.Blue, CrossSize = 20 };
                return regionCenter;
            }
            set => regionCenter = value;
        }

        [Description("Bán kính của vùng kiểm tra, tính toán Histogram"), PropertyOrder(51)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F3")]
        public float RegionRadius { get => regionRadius; set => regionRadius = value; }

        [Description("Ngưỡng dưới của kênh màu RED"), PropertyOrder(52)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float RedMin
        {
            get => redMin;
            set
            {
                if (redMin == value || value < 0 || value > redMax) return;
                redMin = value;
            }
        }

        [Description("Giá trị Mean của kênh màu RED"), PropertyOrder(53)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F3"), ReadOnly(true)]
        public float RedMean { get => redMean; set => redMean = value; }

        [Description("Ngưỡng trên của kênh màu RED"), PropertyOrder(54)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float RedMax
        {
            get => redMax;
            set
            {
                if (redMax == value || value < RedMin) return;
                redMax = value;
            }
        }

        [Browsable(false)]
        public bool RedStatus { get => redStatus; set => redStatus = value; }

        [Description("Ngưỡng dưới của kênh màu BLUE"), PropertyOrder(55)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float BlueMin
        {
            get => blueMin;
            set
            {
                if (blueMin == value || value < 0 || value > blueMax) return;
                blueMin = value;
            }
        }

        [Description("Giá trị Mean của kênh màu BLUE"), PropertyOrder(56)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F3"), ReadOnly(true)]
        public float BlueMean { get => blueMean; set => blueMean = value; }

        [Description("Ngưỡng trên của kênh màu BLUE"), PropertyOrder(57)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float BlueMax
        {
            get => blueMax;
            set
            {
                if (blueMax == value || value < blueMin) return;
                blueMax = value;
            }
        }
        [Browsable(false)]
        public bool BlueStatus { get => blueStatus; set => blueStatus = value; }

        [Description("Ngưỡng dưới của kênh màu GREEN"), PropertyOrder(58)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float GreenMin
        {
            get => greenMin;
            set
            {
                if (greenMin == value || value < 0 || value > greenMax) return;
                greenMin = value;
            }
        }

        [Description("Giá trị Mean của kênh màu GREEN"), PropertyOrder(59)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F3"), ReadOnly(true)]
        public float GreenMean { get => greenMean; set => greenMean = value; }

        [Description("Ngưỡng trên của kênh màu GREEN"), PropertyOrder(60)]
        [TypeConverter(typeof(FloatTypeConverter)), FormattedFloatFormatString("F0")]
        public float GreenMax
        {
            get => greenMax;
            set
            {
                if (greenMax == value || value < greenMin) return;
                greenMax = value;
            }
        }

        [Browsable(false)]
        public bool GreenStatus { get => greenStatus; set => greenStatus = value; }

        #endregion
        public InspectionRegion()
        {
            regionRadius = 5;
            redMin = 100;
            redMax = 150;
            blueMin = 100;
            blueMax = 150;
            greenMin = 100;
            greenMax = 150;
        }
        public override string ToString()
        {
            return "";
        }
    }
}
