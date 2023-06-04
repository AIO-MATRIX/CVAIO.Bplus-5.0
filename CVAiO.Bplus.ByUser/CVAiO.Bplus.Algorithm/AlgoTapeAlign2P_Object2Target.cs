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

// Mô tả: Đây là thuật toán sử dụng cho

namespace CVAiO.Bplus.Algorithm
{
    [Serializable]
    public class AlgoTapeAlign2P_Object2Target : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage1; // ảnh đầu vào
        [NonSerialized]
        private InteractPoint objPoint1;
        [NonSerialized]
        private InteractPoint tarPoint1;
        [NonSerialized]
        private Image inImage2; // ảnh đầu vào
        [NonSerialized]
        private InteractPoint objPoint2;
        [NonSerialized]
        private InteractPoint tarPoint2;
        [NonSerialized]
        private InteractLine objLine;
        [NonSerialized]
        private InteractLine tarLine;

        private ImageInfo inputImageInfo1;
        private ImageInfo inputImageInfo2;

        private AlgoTapeAlign2P_Object2TargetRunParams runParams; // danh sách các parameters sẽ được sử dụng của công cụ

        [NonSerialized]
        private InteractPoint objOrigin = new InteractPoint();
        [NonSerialized]
        private InteractPoint tarOrigin = new InteractPoint();

        [NonSerialized]
        private Execution calc; // Đầu vào của tín hiệu tính toán kết thúc của các process
        [NonSerialized]
        private bool algoJudgement; // Phát định kết quả hoạt động của công cụ,  dùng làm căn cứ để kết thúc tính toán của toàn bộ process
        [NonSerialized]
        private InteractPoint algoObject2Target;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage1 { get => inImage1; set => inImage1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint ObjPoint1 { get => objPoint1; set => objPoint1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint TarPoint1 { get => tarPoint1; set => tarPoint1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage2 { get => inImage2; set => inImage2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint ObjPoint2 { get => objPoint2; set => objPoint2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint TarPoint2 { get => tarPoint2; set => tarPoint2 = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(30)]
        public AlgoTapeAlign2P_Object2TargetRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoTapeAlign2P_Object2TargetRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        [ReadOnly(true), Category("4. Output"), TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        [Description("Absolute Object (Actual Coordinate)"), PropertyOrder(32)]
        public InteractPoint ObjOrigin { get { if (objOrigin == null) objOrigin = new InteractPoint(); return objOrigin; } set => objOrigin = value; }

        [Browsable(false)]
        [Description("Absolute Object Line (Actual Coordinate)")]
        public InteractLine ObjLine { get { if (objLine == null) objLine = new InteractLine(); return objLine; } set => objLine = value; }

        [ReadOnly(true), Category("4. Output"), TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        [Description("Absolute Target without Offset (Actual Coordinate)"), PropertyOrder(31)]
        public InteractPoint TarOrigin { get { if (tarOrigin == null) tarOrigin = new InteractPoint(); return tarOrigin; } set => tarOrigin = value; }

        [Browsable(false)]
        [Description("Absolute Object Line (Actual Coordinate)")]
        public InteractLine TarLine { get { if (tarLine == null) tarLine = new InteractLine(); return tarLine; } set => tarLine = value; }

        [Browsable(true), ReadOnly(true), Description("Vision Alignment Result (Offset from Object Position to Target Position)"), Category("4. Output"), PropertyOrder(33)]
        [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public InteractPoint AlgoObject2Target { get { if (algoObject2Target == null) algoObject2Target = new InteractPoint(); return algoObject2Target; } set => algoObject2Target = value; }

        #endregion

        public AlgoTapeAlign2P_Object2Target()
        {
            toolName = "AlgoTapeAlign2P_O2T";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoTapeAlign2P_O2T";
            ToolColor = System.Drawing.Color.Orange; // Màu hiển thị của công cụ ở cửa sổ Process Design. Default: Orange
            RunParams.PropertyChanged += RunParams_PropertyChanged; // Event lưu lại log file mỗi khi có sự thay đổi của thuộc tính
        }

        public AlgoTapeAlign2P_Object2Target(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage1", null);
            inParams.Add("TarPoint1", null);
            inParams.Add("ObjPoint1", null);
            inParams.Add("InImage2", null);
            inParams.Add("TarPoint2", null);
            inParams.Add("ObjPoint2", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
            inputImageInfo1 = new ImageInfo(string.Format("[{0}] InputImage 1", this.ToString()));
            inputImageInfo2 = new ImageInfo(string.Format("[{0}] InputImage 2", this.ToString()));
            inputImageInfo1.drawingFunc += DrawInputs1;
            inputImageInfo2.drawingFunc += DrawInputs2;
            imageList.Add(inputImageInfo1);
            imageList.Add(inputImageInfo2);
        }

        public void DrawInputs1(DisplayView display)
        {
            if (display.Image == null || InImage1 == null) return;
            display.GraphicsFuncCollection.Clear();
            CalibMatrix calibMatrix1 = new CalibMatrix(InImage1.CalibrationMat);
            System.Drawing.Font font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            if (TarLine != null)
            {
                DrawInfos(display, TarLine, calibMatrix1, System.Drawing.Color.Green, font);
                DrawCoordinate(display, TarOrigin, calibMatrix1, font);
            }    
            if (ObjLine != null)
            {
                DrawInfos(display, ObjLine, calibMatrix1, System.Drawing.Color.AliceBlue, font);
                DrawCoordinate(display, ObjOrigin, calibMatrix1, font);
            }   
        }
        public void DrawInputs2(DisplayView display)
        {
            if (display.Image == null || InImage2 == null) return;
            display.GraphicsFuncCollection.Clear();
            CalibMatrix calibMatrix2 = new CalibMatrix(InImage2.CalibrationMat);
            System.Drawing.Font font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            if (TarLine != null)
            {
                DrawInfos(display, TarLine, calibMatrix2, System.Drawing.Color.Green, font);
                DrawCoordinate(display, TarOrigin, calibMatrix2, font);
            }    
            if (ObjLine != null)
            {
                DrawInfos(display, ObjLine, calibMatrix2, System.Drawing.Color.AliceBlue, font);
                DrawCoordinate(display, ObjOrigin, calibMatrix2, font);
            }    
        }

        private void DrawInfos(DisplayView display, InteractLine line, CalibMatrix calibMatrix, System.Drawing.Color color, System.Drawing.Font font)
        {
            Point2f startLineImg = AiO.RtoV(calibMatrix, new Point2f((float)line.SP.X, (float)line.SP.Y));
            Point2f endLineImg = AiO.RtoV(calibMatrix, new Point2f((float)line.EP.X, (float)line.EP.Y));
            Point2f centerLineImg = AiO.RtoV(calibMatrix, new Point2f((float)line.CP.X, (float)line.CP.Y));

            Point2f startLine = AiO.ImageToFixture2F(startLineImg, display.Image.TransformMat);
            Point2f endLine = AiO.ImageToFixture2F(endLineImg, display.Image.TransformMat);

            System.Drawing.Pen pen = new System.Drawing.Pen(color, 3);
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(color);
            display.DrawCross(pen, startLine, 50, 50, 0);
            display.DrawCross(pen, endLine, 50, 50, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3})", startLineImg.X, startLineImg.Y), font, brush, new Point2d(startLine.X, startLine.Y));
            display.DrawString(string.Format("({0:f3}, {1:f3})", endLineImg.X, endLineImg.Y), font, brush, new Point2d(endLine.X, endLine.Y));
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            display.DrawLine(pen, startLine, endLine);
            DrawCoordinate(display, new InteractPoint(line.CP.X, line.CP.Y, line.ThetaRad), calibMatrix, font);
        }

        private void DrawCoordinate(DisplayView display, InteractPoint point, CalibMatrix calibMatrix, System.Drawing.Font font)
        {
            Point2f pointImg = AiO.RtoV(calibMatrix, new Point2f((float)point.X, (float)point.Y));
            System.Drawing.Color colorX = System.Drawing.Color.Blue;
            System.Drawing.Color colorY = System.Drawing.Color.Green;
            Point3d center = AiO.ImageToFixture3D(new Point3d(pointImg.X, pointImg.Y, 0), display.Image.TransformMat);
            Point2d origin = new Point2d(center.X, center.Y);
            Point2d xaxis = origin + AiO.Rotate(new Point2d(50, 0), point.ThetaRad + center.Z);
            Point2d yaxis = origin + AiO.Rotate(new Point2d(0, 50), point.ThetaRad + center.Z);
            display.DrawArrow(new System.Drawing.Pen(colorX, 3), origin, xaxis, 2, 3);
            display.DrawArrow(new System.Drawing.Pen(colorY, 3), origin, yaxis, 2, 3);
            System.Drawing.SolidBrush brushX = new System.Drawing.SolidBrush(colorX);
            System.Drawing.SolidBrush brushY = new System.Drawing.SolidBrush(colorY);
            display.DrawString("X", font, brushX, new System.Drawing.PointF((float)xaxis.X, (float)xaxis.Y));
            display.DrawString("Y", font, brushX, new System.Drawing.PointF((float)yaxis.X, (float)yaxis.Y));
        }

        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo1.Image = InImage1;
            inputImageInfo2.Image = InImage2;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            if (!AiO.IsPossibleImage(InImage1) || !AiO.IsPossibleImage(InImage2)) throw new Exception("InputImage 1/InputImage 2 = Null");
            try
            {
                if (ObjPoint1 == null || ObjPoint2 == null) throw new Exception("Object Point 1/2 = Null");
                if (TarPoint1 == null || TarPoint2 == null) throw new Exception("Object Point 1/2 = Null");

                CalibMatrix calibMatrix1 = new CalibMatrix(InImage1.CalibrationMat);
                CalibMatrix calibMatrix2 = new CalibMatrix(InImage2.CalibrationMat);

                // Tính toán theo đơn vị mm sau khi đã nhân ma trận căn chỉnh
                Point2f targetLeft = AiO.VtoR(calibMatrix1, TarPoint1.Point);
                Point2f targetRight = AiO.VtoR(calibMatrix2, TarPoint2.Point);
                TarLine.SP = new Point2d(targetLeft.X, targetLeft.Y);
                TarLine.EP = new Point2d(targetRight.X, targetRight.Y);
                Point3f targetTape = new Point3f();
                targetTape.X = (targetLeft.X + targetRight.X) / 2;
                targetTape.Y = (targetLeft.Y + targetRight.Y) / 2;
                targetTape.Z = (float)Math.Atan2(targetRight.Y - targetLeft.Y, targetRight.X - targetLeft.X);

                // Đưa vào giá trị offset của Target Tape theo mm
                tarOrigin.X = targetTape.X + (float)(RunParams.TarOffsetX * Math.Cos(RunParams.TarOffsetT) - RunParams.TarOffsetY * Math.Sin(RunParams.TarOffsetT));
                tarOrigin.Y = targetTape.Y + (float)(RunParams.TarOffsetX * Math.Sin(RunParams.TarOffsetT) + RunParams.TarOffsetY * Math.Cos(RunParams.TarOffsetT));
                tarOrigin.ThetaRad = targetTape.Z + RunParams.TarOffsetT;

                Point2f objectLeft = AiO.VtoR(calibMatrix1, ObjPoint1.Point);
                Point2f objectRight = AiO.VtoR(calibMatrix2, ObjPoint2.Point);
                ObjLine.SP = new Point2d(objectLeft.X, objectLeft.Y);
                ObjLine.EP = new Point2d(objectRight.X, objectRight.Y);

                Point3f objectTape = new Point3f();
                objectTape.X = (objectLeft.X + objectRight.X) / 2;
                objectTape.Y = (objectLeft.Y + objectRight.Y) / 2;
                objectTape.Z = (float)Math.Atan2(objectRight.Y - objectLeft.Y, objectRight.X - objectLeft.X);

                // Đưa vào giá trị offset của Object Tape nếu cần, 
                objOrigin.X = objectTape.X + (float)(RunParams.ObjOffsetX * Math.Cos(RunParams.ObjOffsetT) - RunParams.ObjOffsetY * Math.Sin(RunParams.ObjOffsetT));
                objOrigin.Y = objectTape.Y + (float)(RunParams.ObjOffsetX * Math.Sin(RunParams.ObjOffsetT) + RunParams.ObjOffsetY * Math.Cos(RunParams.ObjOffsetT));
                objOrigin.ThetaRad = objectTape.Z + RunParams.ObjOffsetT;

                Point3f Offset3f = AiO.RToT(new Point3f(objOrigin.X, objOrigin.Y, (float)objOrigin.ThetaRad), new Point3f(tarOrigin.X, tarOrigin.Y, (float)tarOrigin.ThetaRad));
                if (Offset3f.Z > Math.PI) Offset3f.Z -= (float)(2 * Math.PI);
                else if (Offset3f.Z < -Math.PI) Offset3f.Z += (float)(2 * Math.PI);
                AlgoObject2Target = new InteractPoint(Offset3f.X, Offset3f.Y, Offset3f.Z);
                algoJudgement = true;
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
            if (InImage1 != null)
            {
                InImage1.Dispose();
            }
            if (InImage2 != null)
            {
                InImage2.Dispose();
            }
        }
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class AlgoTapeAlign2P_Object2TargetRunParams : RunParams, ISerializable
    {
        #region Fields
        private float tarOffsetX;
        private float tarOffsetY;
        private float tarOffsetT;

        private float objOffsetX;
        private float objOffsetY;
        private float objOffsetT;
        #endregion

        #region Properties
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Target Position in X (mm)"), PropertyOrder(50)]
        public float TarOffsetX { get => tarOffsetX; set => tarOffsetX = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Target Position in Y (mm)"), PropertyOrder(51)]
        public float TarOffsetY { get => tarOffsetY; set => tarOffsetY = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Target Rotation in T (rad)"), PropertyOrder(52)]
        public float TarOffsetT { get => tarOffsetT; set => tarOffsetT = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Object Position in X (mm)"), PropertyOrder(53)]
        public float ObjOffsetX { get => objOffsetX; set => objOffsetX = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Object Position in Y (mm)"), PropertyOrder(54)]
        public float ObjOffsetY { get => objOffsetY; set => objOffsetY = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        [Description("Offset from Object Rotation in T (rad)"), PropertyOrder(55)]
        public float ObjOffsetT { get => objOffsetT; set => objOffsetT = value; }


        #endregion
        public AlgoTapeAlign2P_Object2TargetRunParams()
        {
        }

        #region Do not change
        public AlgoTapeAlign2P_Object2TargetRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoTapeAlign2P_Object2TargetRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoTapeAlign2P_Object2TargetRunParams)runParams;
            }
        }
        #endregion
    }
}
