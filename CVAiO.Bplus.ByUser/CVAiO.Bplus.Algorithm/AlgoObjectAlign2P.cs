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

// Pham vi sử dụng: Đây là công cụ sử dụng để Align vật thể sử dụng 2 camera độc lập chụp ở 2 vị trí khác nhau hoặc 1 camera chup 2 lần liên tiếp.
// Do sử dụng 2 camera khác nhau nên mỗi camera sẽ sử dụng 1 ma trận căn chỉnh khác nhau.
// Bắt buộc phải chuyển đổi sang hệ tọa độ thực tế trước khi thực hiện tính toán tìm vị trí vật thể.
// Ma trận căn chỉnh cần được load vào từ vị trí của tool accquision

namespace CVAiO.Bplus.Algorithm
{
    // Kế thừa của ToolBase là bắt buộc, và interface IAlgorithm
    // Note: Tên của công cụ bắt buộc phải ở dạng Algo.... để phân biệt với các công cụ khác
    [Serializable]
    public class AlgoObjectAlign2P : ToolBase, IAlgorithm
    {
        // Khai báo các trường sẽ sử dụng trong lớp
        #region Fields
        [NonSerialized]
        private Image inImage1; // ảnh đầu vào
        [NonSerialized]
        private InteractPoint point1;
        [NonSerialized]
        private Image inImage2; // ảnh đầu vào
        [NonSerialized]
        private InteractPoint point2;
        [NonSerialized]
        private InteractLine line;
        private ImageInfo inputImageInfo1;
        private ImageInfo inputImageInfo2;

        private AlgoObjectAlign2PRunParams runParams; // danh sách các parameters sẽ được sử dụng của công cụ

        [NonSerialized]
        private Execution calc; // Đầu vào của tín hiệu tính toán kết thúc của các process
        [NonSerialized]
        private bool algoJudgement; // Phát định kết quả hoạt động của công cụ,  dùng làm căn cứ để kết thúc tính toán của toàn bộ process
        [NonSerialized]
        private InteractPoint algoPosition;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage1 { get => inImage1; set => inImage1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point1 { get => point1; set => point1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage2 { get => inImage2; set => inImage2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InteractPoint Point2 { get => point2; set => point2 = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoObjectAlign2PRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoObjectAlign2PRunParams();
                return runParams;
            }
            set => runParams = value;
        }

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

        public AlgoObjectAlign2P()
        {
            toolName = "AlgoObjectAlign2P";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoObjectAlign2P";
            ToolColor = System.Drawing.Color.Orange; // Màu hiển thị của công cụ ở cửa sổ Process Design. Default: Orange
            RunParams.PropertyChanged += RunParams_PropertyChanged; // Event lưu lại log file mỗi khi có sự thay đổi của thuộc tính
        }

        public AlgoObjectAlign2P(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            // Thêm bớt số lượng parameter đầu vào
            // Tên cúa string key phải nằm trong danh sách các input params [InputParam]
            // Giá trị khởi tạo ban đầu mặc định là null
            inParams.Add("InImage1", null);
            inParams.Add("Point1", null);
            inParams.Add("InImage2", null);
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
            inputImageInfo1 = new ImageInfo(string.Format("[{0}] InputImage 1", this.ToString()));
            inputImageInfo2 = new ImageInfo(string.Format("[{0}] InputImage 2", this.ToString()));
            inputImageInfo1.drawingFunc += DrawInputs1; // Thêm function để vẽ vào các hình tương ứng
            inputImageInfo2.drawingFunc += DrawInputs2; // Thêm function để vẽ vào các hình tương ứng
            imageList.Add(inputImageInfo1);
            imageList.Add(inputImageInfo2);
        }

        public void DrawInputs1(DisplayView display)
        {
            if (display.Image == null) return;
            if (Point1 == null || InImage1 == null) return;
            display.GraphicsFuncCollection.Clear();
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Font f = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan);

            display.DrawCross(RedPen, AiO.ImageToFixture2D(Point1.Point2d, inImage1.TransformMat), 20, 20, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3}, {2:f3})", Point1.X, Point1.Y, 0), f, SB, new Point2d(AiO.ImageToFixture2D(point1.Point2d, inImage1.TransformMat).X, AiO.ImageToFixture2D(point1.Point2d, inImage1.TransformMat).Y));
        }
        public void DrawInputs2(DisplayView display)
        {
            if (display.Image == null) return;
            if (Point2 == null || InImage2 == null) return;
            display.GraphicsFuncCollection.Clear();
            System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
            System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Font f = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush SB = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan);

            display.DrawCross(RedPen, AiO.ImageToFixture2D(Point2.Point2d, inImage2.TransformMat), 20, 20, 0);
            display.DrawString(string.Format("({0:f3}, {1:f3}, {2:f3})", Point2.X, Point2.Y, 0), f, SB, new Point2d(AiO.ImageToFixture2D(point2.Point2d, inImage2.TransformMat).X, AiO.ImageToFixture2D(point2.Point2d, inImage2.TransformMat).Y));
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
                if (Point1 == null || Point2 == null) throw new Exception("Point 1/2 = Null");
                Line.SP = Point1.Point2d;
                Line.EP = Point2.Point2d;

                CalibMatrix calibMatrix1 = new CalibMatrix(InImage1.CalibrationMat);
                CalibMatrix calibMatrix2 = new CalibMatrix(InImage2.CalibrationMat);
                Point2f targetLeft = AiO.VtoR(calibMatrix1, Point1.Point);
                Point2f targetRight = AiO.VtoR(calibMatrix1, Point2.Point);

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
    public class AlgoObjectAlign2PRunParams : RunParams, ISerializable
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
        public AlgoObjectAlign2PRunParams()
        {
        }

        #region Do not change
        public AlgoObjectAlign2PRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoObjectAlign2PRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoObjectAlign2PRunParams)runParams;
            }
        }
        #endregion
    }
}
