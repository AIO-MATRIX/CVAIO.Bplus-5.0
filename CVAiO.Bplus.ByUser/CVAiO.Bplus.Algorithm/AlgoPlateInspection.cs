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
using System.Reflection;

namespace CVAiO.Bplus.Algorithm
{
    // Tham khảo video setting tại:
    // https://youtu.be/AydIndceEjk?si=URTZAxEbfgGIDlcG
    // Kết quả tại:
    // https://youtu.be/WniBMbrb0Fo?si=z5u_p4Zdb1-rVFpp
    [Serializable]
    public class AlgoPlateInspection : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        private ImageInfo inputImageInfo;
        private AlgoPlateInspectionRunParams runParams;

        [NonSerialized]
        private Execution calc;
        [NonSerialized]
        private bool algoJudgement;

        [NonSerialized]
        private InteractLine line1;
        [NonSerialized]
        private InteractEllipse circle1;
        [NonSerialized]
        private InteractEllipse circle2;

        [NonSerialized]
        private InteractLine line2;
        [NonSerialized]
        private InteractEllipse circle3;
        [NonSerialized]
        private InteractEllipse circle4;

        [NonSerialized]
        private float length1;
        [NonSerialized]
        private float radius1;
        [NonSerialized]
        private float radius2;
        [NonSerialized]
        private float length2;
        [NonSerialized]
        private float radius3;
        [NonSerialized]
        private float radius4;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractLine Line1 { get => line1; set => line1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractEllipse Circle1 { get => circle1; set => circle1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractEllipse Circle2 { get => circle2; set => circle2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractLine Line2 { get => line2; set => line2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractEllipse Circle3 { get => circle3; set => circle3 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractEllipse Circle4 { get => circle4; set => circle4 = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoPlateInspectionRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoPlateInspectionRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false), Description("Vision Algorithm Execute"), PropertyOrder(11)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        [ReadOnly(true), Description("Độ dài Line 1 (mm)"), Category("4. Output"), PropertyOrder(20)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Length1 { get => length1; set => length1 = value; }

        [ReadOnly(true), Description("Bán kính đường tròn Circle 1 (mm)"), Category("4. Output"), PropertyOrder(21)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Radius1 { get => radius1; set => radius1 = value; }

        [ReadOnly(true), Description("Bán kính đường tròn Circle 2 (mm)"), Category("4. Output"), PropertyOrder(22)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Radius2 { get => radius2; set => radius2 = value; }

        [ReadOnly(true), Description("Độ dài Line 1 (mm)"), Category("4. Output"), PropertyOrder(23)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Length2 { get => length2; set => length2 = value; }

        [ReadOnly(true), Description("Bán kính đường tròn Circle 3 (mm)"), Category("4. Output"), PropertyOrder(24)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Radius3 { get => radius3; set => radius3 = value; }

        [ReadOnly(true), Description("Bán kính đường tròn Circle 4 (mm)"), Category("4. Output"), PropertyOrder(25)]
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float Radius4 { get => radius4; set => radius4 = value; }

        #endregion

        public AlgoPlateInspection()
        {
            toolName = "Algo Plate Inspection";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "Algo Plate Inspection";
            ToolColor = System.Drawing.Color.Orange;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public AlgoPlateInspection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("Line1", null);
            inParams.Add("Circle1", null);
            inParams.Add("Circle2", null);
            inParams.Add("Line2", null);
            inParams.Add("Circle3", null);
            inParams.Add("Circle4", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            inputImageInfo.drawingFunc += DrawInput1;
            imageList.Add(inputImageInfo);
        }
        public void DrawInput1(DisplayView display)
        {
            if (display.Image == null) return;
            if (!AiO.IsPossibleImage(InImage)) return;
            System.Drawing.Pen BluePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1);
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);

            float fontSize = 6 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("arial", fontSize, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);

            if (Line1 != null)
            {
                display.DrawLine(BluePen, Line1.SPf, Line1.EPf);
                display.DrawString(string.Format("L1 = {0:f3} mm", Length1), font, solidBrush, AiO.ImageToFixture2D(Line1.CP, InImage.TransformMat));
            }
            if (Circle1 != null)
            {
                display.DrawEllipse(BluePen, Circle1.Center, Circle1.Width, Circle1.Height);
                display.DrawString(string.Format("R1 = {0:f3} mm", Radius1), font, solidBrush, AiO.ImageToFixture2D(Circle1.Center, InImage.TransformMat));
            }
            if (Circle2 != null)
            {
                display.DrawEllipse(BluePen, Circle2.Center, Circle2.Width, Circle2.Height);
                display.DrawString(string.Format("R2 = {0:f3} mm", Radius2), font, solidBrush, AiO.ImageToFixture2D(Circle2.Center, InImage.TransformMat));
            }

            if (Line2 != null)
            {
                display.DrawLine(CyanPen, Line2.SPf, Line2.EPf);
                display.DrawString(string.Format("L2 = {0:f3} mm", Length2), font, solidBrush, AiO.ImageToFixture2D(Line2.CP, InImage.TransformMat));
            }
            if (Circle3 != null)
            {
                display.DrawEllipse(CyanPen, Circle3.Center, Circle3.Width, Circle3.Height);
                display.DrawString(string.Format("R3 = {0:f3} mm", Radius3), font, solidBrush, AiO.ImageToFixture2D(Circle3.Center, InImage.TransformMat));
            }
            if (Circle4 != null)
            {
                display.DrawEllipse(CyanPen, Circle4.Center, Circle4.Width, Circle4.Height);
                display.DrawString(string.Format("R4 = {0:f3} mm", Radius4), font, solidBrush, AiO.ImageToFixture2D(Circle4.Center, InImage.TransformMat));
            }
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            if (!AiO.IsPossibleImage(inImage)) throw new Exception("InputImage = Null");
            try
            {
                algoJudgement = true; // kết quả phán định quá trình kiểm tra

                // Tính toán hệ số chuyển đổi px to mm
                Point2f origin = AiO.VtoR(RunParams.CalibMatrix, new Point2f(0, 0));
                Point2f xDirection = AiO.VtoR(RunParams.CalibMatrix, new Point2f(1, 0));
                float px2mm = AiO.getLength(new System.Drawing.PointF(origin.X, origin.Y), new System.Drawing.PointF(xDirection.X, xDirection.Y));

                // Kiểm tra độ dài đường line 1
                Length1 = (float)Line1.GetLineLength() * px2mm;
                if (Length1 > RunParams.MaxLength1 || Length1 < RunParams.MinLength1) algoJudgement = false;

                // Kiểm tra kích thước đường tròn 1
                Radius1 = Circle1.Height * px2mm;
                if (Radius1 > RunParams.MaxRadius12 || Radius1 < RunParams.MinRadius12) algoJudgement = false;

                // Kiểm tra kích thước đường tròn 2
                Radius2 = Circle2.Height * px2mm;
                if (Radius2 > RunParams.MaxRadius12 || Radius2 < RunParams.MinRadius12) algoJudgement = false;

                Length2 = (float)Line2.GetLineLength() * px2mm;
                if (Length2 > RunParams.MaxLength2 || Length2 < RunParams.MinLength2) algoJudgement = false;

                // Kiểm tra kích thước đường tròn 3
                Radius3 = Circle3.Height * px2mm;
                if (Radius3 > RunParams.MaxRadius34 || Radius3 < RunParams.MaxRadius34) algoJudgement = false;

                // Kiểm tra kích thước đường tròn 4
                Radius4 = Circle4.Height * px2mm;
                if (Radius4 > RunParams.MaxRadius34 || Radius4 < RunParams.MaxRadius34) algoJudgement = false;

                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                OnRan();
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
    public class AlgoPlateInspectionRunParams : RunParams, ISerializable
    {
        #region Fields
        private float maxLength1;
        private float minLength1;
        private float maxRadius12;
        private float minRadius12;
        private float maxLength2;
        private float minLength2;
        private float maxRadius34;
        private float minRadius34;

        private string calibrationFile;
        private CalibMatrix calibMatrix;


        #endregion

        #region Properties
        [Description("Giá trị ngưỡng trên cho độ dài đường line 1"), PropertyOrder(50)]
        public float MaxLength1
        {
            get => maxLength1;
            set
            {
                if (value == maxLength1 || value < minLength1) return;
                maxLength1 = value;
                NotifyPropertyChanged(nameof(MaxLength1));
            }
        }

        [Description("Giá trị ngưỡng dưới cho độ dài đường line 1"), PropertyOrder(51)]
        public float MinLength1
        {
            get => minLength1;
            set
            {
                if (value == minLength1 || value > maxLength1 || value < 0) return;
                minLength1 = value;
                NotifyPropertyChanged(nameof(MinLength1));
            }
        }

        [Description("Giá trị ngưỡng dưới cho bán kính đường tròn lớn"), PropertyOrder(52)]
        public float MaxRadius12
        {
            get => maxRadius12;
            set
            {
                if (value == maxRadius12 || value < maxRadius12) return;
                maxRadius12 = value;
                NotifyPropertyChanged(nameof(MaxRadius12));
            }
        }

        [Description("Giá trị ngưỡng trên cho bán kính đường tròn lớn"), PropertyOrder(53)]
        public float MinRadius12
        {
            get => minRadius12;
            set
            {
                if (value == minRadius12 || value > maxRadius12 || value < 0) return;
                minRadius12 = value;
                NotifyPropertyChanged(nameof(MinRadius12));
            }
        }

        [Description("Giá trị ngưỡng trên cho độ dài đường line 2"), PropertyOrder(54)]
        public float MaxLength2
        {
            get => maxLength2;
            set
            {
                if (value == maxLength2 || value < minLength2) return;
                maxLength2 = value;
                NotifyPropertyChanged(nameof(MaxLength2));
            }
        }

        [Description("Giá trị ngưỡng dưới cho độ dài đường line 2"), PropertyOrder(55)]
        public float MinLength2
        {
            get => minLength2;
            set
            {
                if (value == minLength2 || value > maxLength2 || value < 0) return;
                minLength2 = value;
                NotifyPropertyChanged(nameof(MinLength2));
            }
        }

        [Description("Giá trị ngưỡng trên cho bán kính đường tròn nhỏ"), PropertyOrder(56)]
        public float MaxRadius34
        {
            get => maxRadius34;
            set
            {
                if (value == maxRadius34 || value < minRadius34) return;
                maxRadius34 = value;
                NotifyPropertyChanged(nameof(MaxRadius34));
            }
        }

        [Description("Giá trị ngưỡng dưới cho bán kính đường tròn nhỏ"), PropertyOrder(57)]
        public float MinRadius34
        {
            get => minRadius34;
            set
            {
                if (value == minRadius34 || value > maxRadius34 || value < 0) return;
                minRadius34 = value;
                NotifyPropertyChanged(nameof(MinRadius34));
            }
        }

        [Description("calibration file")]
        [Editor(typeof(CalibrationFileSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CalibrationFile
        {
            get => calibrationFile;
            set
            {
                calibrationFile = value;
                CalibMatrix calibMatrix = new Serializer().Deserializing(value) as CalibMatrix;
                if (CalibMatrix == null)
                {
                    frmMessageBox.Show(EMessageIcon.Error, "Fail to load calibration matrix: " + value, 2500);
                    return;
                }
                this.CalibMatrix = calibMatrix;
            }
        }

        [Browsable(false)]
        public CalibMatrix CalibMatrix
        {
            get
            {
                if (calibMatrix == null) calibMatrix = new CalibMatrix();
                return calibMatrix;
            }
            set => calibMatrix = value;
        }

        #endregion
        public AlgoPlateInspectionRunParams()
        {
        }

        #region Do not change
        public AlgoPlateInspectionRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoPlateInspectionRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoPlateInspectionRunParams)runParams;
            }
        }
        #endregion
    }
}
