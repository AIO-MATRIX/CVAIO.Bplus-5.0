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

// Phạm vi sử dụng: Đây là công cụ thuật toán sử dụng trong trường hợp Align sử dụng 1 camera với 1 ma trận căn chỉnh
// Camera chụp 1 lần và lấy 2 vị trí để tính toán vị trí của vật thể
// Trong trường hợp đầu vào là kết quả của công cụ template matching thì xóa bỏ đi 1 điểm đâu vào

namespace CVAiO.Bplus.Algorithm
{
    // Kế thừa của ToolBase là bắt buộc, và interface IAlgorithm
    // Note: Tên của công cụ bắt buộc phải ở dạng Algo.... để phân biệt với các công cụ khác
    [Serializable]
    public class AlgoObjectAlign1P : ToolBase, IAlgorithm
    {
        // Khai báo các trường sẽ sử dụng trong lớp
        #region Fields
        [NonSerialized]
        private Image inImage; // ảnh đầu vào
        [NonSerialized]
        private InteractPoint point1;
        [NonSerialized]
        private InteractPoint point2;
        [NonSerialized]
        private Image outImage; // ảnh đầu ra
        [NonSerialized]
        private InteractLine line;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private AlgoObjectAlign1PRunParams runParams; // danh sách các parameters sẽ được sử dụng của công cụ

        [NonSerialized]
        private Execution calc; // Đầu vào của tín hiệu tính toán kết thúc của các process
        [NonSerialized]
        private bool algoJudgement; // Phát định kết quả hoạt động của công cụ,  dùng làm căn cứ để kết thúc tính toán của toàn bộ process
        [NonSerialized]
        private InteractPoint algoPosition;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point1 { get => point1; set => point1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point2 { get => point2; set => point2 = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoObjectAlign1PRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoObjectAlign1PRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [InputParam, Browsable(false)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        [OutputParam, ReadOnly(true), Category("4. Output"), TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        [Description("Absolute Position (Robot Coordinate)")]
        public InteractPoint Point { get { return new InteractPoint(Line.CP.X, Line.CP.Y) { ThetaRad = Line.ThetaRad }; } set => throw new Exception("Cant be set."); }

        [Browsable(false)]
        public InteractLine Line { get { if (line == null) line = new InteractLine(); return line; } set => line = value; }

        [OutputParam, Browsable(true), ReadOnly(true), Description("Vision Alignment Result (Offset with Master Position)"), Category("4. Output"), PropertyOrder(12)]
        [TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public InteractPoint AlgoPosition { get { if (algoPosition == null) algoPosition = new InteractPoint(); return algoPosition; } set => algoPosition = value; }

        #endregion

        public AlgoObjectAlign1P()
        {
            toolName = "AlgoObjectAlign1P";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoObjectAlign1P";
            ToolColor = System.Drawing.Color.Orange; // Màu hiển thị của công cụ ở cửa sổ Process Design. Default: Orange
            RunParams.PropertyChanged += RunParams_PropertyChanged; // Event lưu lại log file mỗi khi có sự thay đổi của thuộc tính
        }

        public AlgoObjectAlign1P(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            // Thêm bớt số lượng parameter đầu vào
            // Tên cúa string key phải nằm trong danh sách các input params [InputParam]
            // Giá trị khởi tạo ban đầu mặc định là null
            inParams.Add("InImage", null);
            inParams.Add("Point1", null);
            inParams.Add("Point2", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
            // Algo là công cụ cuối cùng trong process nên ko có tín hiệu đầu ra
        }
        public override void InitImageList()
        {
            // khởi tạo giá trị của các param imageInfo dùng để hiển thị ở tool edit control
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            outputImageInfo.drawingFunc += DrawOutputs; // Thêm function để vẽ vào các hình tương ứng
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }

        public void DrawOutputs(DisplayView display)
        {
            if (display.Image == null) return;
            if (Point1 == null || Point2 == null || InImage == null) return;
            display.GraphicsFuncCollection.Clear();
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Font f = new System.Drawing.Font("굴림", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan);

            if (Line == null) return;
            InteractLine interactLine = new InteractLine(AiO.ImageToFixture2D(Line.SP, inImage.TransformMat), AiO.ImageToFixture2D(Line.EP, inImage.TransformMat));
            double gain1 = (interactLine.SP.Y - interactLine.EP.Y) / (interactLine.SP.X - interactLine.EP.X);
            double gain2 = interactLine.SP.Y - gain1 * interactLine.SP.X;

            System.Drawing.Pen linePen = new System.Drawing.Pen(System.Drawing.Color.AliceBlue, 3);
            linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            display.DrawLine(linePen, new Point2d(-10000, gain1 * -10000 + gain2), new Point2d(10000, gain1 * 10000 + gain2));
            display.GraphicsCollection.Add(Line);
            display.DrawCross(RedPen, AiO.ImageToFixture2D(Point1.Point2d, inImage.TransformMat), 20, 20, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3}, {2:f3})", Point1.X, Point1.Y, 0), f, SB, new Point2d(AiO.ImageToFixture2D(point1.Point2d, inImage.TransformMat).X, AiO.ImageToFixture2D(point1.Point2d, inImage.TransformMat).Y));
            display.DrawCross(RedPen, AiO.ImageToFixture2D(Point2.Point2d, inImage.TransformMat), 20, 20, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3}, {2:f3})", Point2.X, Point2.Y, 0), f, SB, new Point2d(AiO.ImageToFixture2D(Point2.Point2d, inImage.TransformMat).X, AiO.ImageToFixture2D(Point2.Point2d, inImage.TransformMat).Y));

            display.DrawCross(RedPen, AiO.ImageToFixture2D(Point.Point2d, inImage.TransformMat), 20, 20, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3}, {2:f3})", Point.X, Point.Y, 0), f, SB, new Point2d(AiO.ImageToFixture2D(Point.Point2d, inImage.TransformMat).X, AiO.ImageToFixture2D(Point.Point2d, inImage.TransformMat).Y));
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
            if (inImage == null || inImage.Width == 0 || inImage.Height == 0)
            {
                RunStatus = new RunStatus(EToolResult.Error, "InputImage = Null");
                return;
            }
            try
            {
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                if (Point1 == null || Point2 == null)
                {
                    RunStatus = new RunStatus(EToolResult.Error, "Point 1/2 = Null");
                    return;
                }
                Line.SP = Point1.Point2d;
                Line.EP = Point2.Point2d;

                CalibMatrix calibMatrix = new CalibMatrix(InImage.CalibrationMat);
                Point2f targetLeft = AiO.VtoR(calibMatrix, Point1.Point);
                Point2f targetRight = AiO.VtoR(calibMatrix, Point2.Point);

                Point3f targetRobot = new Point3f();
                targetRobot.X = (targetLeft.X + targetRight.X) / 2;
                targetRobot.Y = (targetLeft.Y + targetRight.Y) / 2;
                targetRobot.Z = (float)Math.Atan2(targetRight.Y - targetLeft.Y, targetRight.X - targetLeft.X);

                Point3f objectRobot = new Point3f(RunParams.LoadPositionX, RunParams.LoadPositionY, RunParams.LoadPositionT);
                Point3f Offset3f = AiO.RToT(objectRobot, targetRobot);
                if (Offset3f.Z > Math.PI) Offset3f.Z -= (float)(2 * Math.PI);
                else if (Offset3f.Z < -Math.PI) Offset3f.Z += (float)(2 * Math.PI);
                AlgoPosition = new InteractPoint(Offset3f.X, Offset3f.Y);
                AlgoPosition.ThetaRad = Offset3f.Z;
                outputImageInfo.Image = OutImage;
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
            if (InImage != null)
            {
                InImage.Dispose();
            }
        }
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class AlgoObjectAlign1PRunParams : RunParams, ISerializable
    {
        #region Fields
        private float loadPositionX;
        private float loadPositionY;
        private float loadPositionT;
        #endregion

        #region Properties
        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float LoadPositionX { get => loadPositionX; set => loadPositionX = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float LoadPositionY { get => loadPositionY; set => loadPositionY = value; }

        [TypeConverter(typeof(FloatTypeConverter))]
        [FormattedFloatFormatString("F3")]
        public float LoadPositionT { get => loadPositionT; set => loadPositionT = value; }

        #endregion
        public AlgoObjectAlign1PRunParams()
        {
        }

        #region Do not change
        public AlgoObjectAlign1PRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoObjectAlign1PRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoObjectAlign1PRunParams)runParams;
            }
        }
        #endregion
    }
}
