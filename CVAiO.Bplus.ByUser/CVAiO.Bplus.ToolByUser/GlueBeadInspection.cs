using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.OpenCV;

namespace CVAiO.Bplus.ToolByUser
{
    public struct BeadInspection
    {
        public bool status;
        public Point3f firstPoint;
        public Point3f secondPoint;
        public string message;
    }

    [Serializable]
    public class GlueBeadInspection : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        [NonSerialized]
        private Mat blurImage = new Mat();
        private GlueBeadInspectionRunParams runParams;
        [NonSerialized]
        private bool inspectionResult;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public GlueBeadInspectionRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new GlueBeadInspectionRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }
        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspectionResult { get => inspectionResult; set => inspectionResult = value; }

        #endregion

        public GlueBeadInspection()
        {
            toolName = "Glue Bead Inspection";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Glue Bead Inspectionn";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public GlueBeadInspection(SerializationInfo info, StreamingContext context) : base(info, context)
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
        // Vẽ lên hình ảnh input
        public virtual void DrawInputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(InImage)) return;

        }

        // Vẽ lên hình ảnh output
        public virtual void DrawOutputs(DisplayView display)
        {
            if (!AiO.IsPossibleImage(OutImage)) return;
            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
            System.Drawing.Pen bluePen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1);
            System.Drawing.Pen greenPen = new System.Drawing.Pen(System.Drawing.Color.Green, 1);
            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.WhiteSmoke, 1);
            float fontSize = 6 / display.ZoomRatio;
            System.Drawing.Font font = new System.Drawing.Font("arial", fontSize, System.Drawing.FontStyle.Bold);
            System.Drawing.SolidBrush solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            //if (RunParams.FirstContour != null && RunParams.FirstContour.Count > 0)
            //{
            //    for (int i = 0; i < RunParams.FirstContour.Count - 1; i++)
            //        display.DrawLine(whitePen, AiO.ImageToFixture2D(new Point2d((float)RunParams.FirstContour[i].X, (float)RunParams.FirstContour[i].Y), outImage.TransformMat),
            //                                   AiO.ImageToFixture2D(new Point2d((float)RunParams.FirstContour[i + 1].X, (float)RunParams.FirstContour[i + 1].Y), outImage.TransformMat));

            //}
            //if (RunParams.SecondContour != null && RunParams.SecondContour.Count > 0)
            //{
            //    for (int i = 0; i < RunParams.SecondContour.Count - 1; i++)
            //        display.DrawLine(whitePen, AiO.ImageToFixture2D(new Point2d((float)RunParams.SecondContour[i].X, (float)RunParams.SecondContour[i].Y), outImage.TransformMat),
            //                                   AiO.ImageToFixture2D(new Point2d((float)RunParams.SecondContour[i + 1].X, (float)RunParams.SecondContour[i + 1].Y), outImage.TransformMat));
            //}
            if (RunParams.FirstToloranceContour != null && RunParams.FirstToloranceContour.Count > 0)
            {
                for (int i = 0; i < RunParams.FirstToloranceContour.Count - 1; i++)
                    display.DrawLine(bluePen, AiO.ImageToFixture2D(new Point2d((float)RunParams.FirstToloranceContour[i].X, (float)RunParams.FirstToloranceContour[i].Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.FirstToloranceContour[i + 1].X, (float)RunParams.FirstToloranceContour[i + 1].Y), outImage.TransformMat));
            }
            if (RunParams.SecondToloranceContour != null && RunParams.SecondToloranceContour.Count > 0)
            {
                for (int i = 0; i < RunParams.SecondToloranceContour.Count - 1; i++)
                    display.DrawLine(bluePen, AiO.ImageToFixture2D(new Point2d((float)RunParams.SecondToloranceContour[i].X, (float)RunParams.SecondToloranceContour[i].Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.SecondToloranceContour[i + 1].X, (float)RunParams.SecondToloranceContour[i + 1].Y), outImage.TransformMat));
            }

            if (RunParams.CenterContour != null && RunParams.CenterContour.Count > 0)
            {
                for (int i = 0; i < RunParams.CenterContour.Count - 1; i++)
                    display.DrawLine(greenPen, AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i].X, (float)RunParams.CenterContour[i].Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i + 1].X, (float)RunParams.CenterContour[i + 1].Y), outImage.TransformMat));
            }

            if (RunParams.InspectionResult != null && RunParams.InspectionResult.Count > 0)
            {
                for (int i = 0; i < RunParams.InspectionResult.Count - 1; i++)
                {
                    System.Drawing.Pen centerPen = RunParams.InspectionResult[i].status ? greenPen : redPen;
                    System.Drawing.Pen beadPen = RunParams.InspectionResult[i].status ? whitePen : redPen;
                    centerPen.Width = RunParams.InspectionResult[i].status ? 1 : 1;
                    display.DrawLine(centerPen, AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i].X, (float)RunParams.CenterContour[i].Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i + 1].X, (float)RunParams.CenterContour[i + 1].Y), outImage.TransformMat));

                    if (RunParams.InspectionResult[i].message.Contains("No Bead"))
                    {
                        if (i == 0 || RunParams.InspectionResult[i - 1].status)
                            display.DrawString("No Bead", font, solidBrush, AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i].X, (float)RunParams.CenterContour[i].Y), outImage.TransformMat));
                        continue;
                    }
                    display.DrawLine(beadPen, AiO.ImageToFixture2D(new Point2d((float)RunParams.InspectionResult[i].firstPoint.X, (float)RunParams.InspectionResult[i].firstPoint.Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.InspectionResult[i + 1].firstPoint.X, (float)RunParams.InspectionResult[i + 1].firstPoint.Y), outImage.TransformMat));

                    display.DrawLine(beadPen, AiO.ImageToFixture2D(new Point2d((float)RunParams.InspectionResult[i].secondPoint.X, (float)RunParams.InspectionResult[i].secondPoint.Y), outImage.TransformMat),
                                               AiO.ImageToFixture2D(new Point2d((float)RunParams.InspectionResult[i + 1].secondPoint.X, (float)RunParams.InspectionResult[i + 1].secondPoint.Y), outImage.TransformMat));
                    if (i > 1 && !RunParams.InspectionResult[i].status && RunParams.InspectionResult[i - 1].status)
                    {
                        display.DrawString(RunParams.InspectionResult[i].message, font, solidBrush, AiO.ImageToFixture2D(new Point2d((float)RunParams.CenterContour[i].X, (float)RunParams.CenterContour[i].Y), outImage.TransformMat));
                    }
                }
                if (InspectionResult)
                    display.DrawString("PASS", font, new System.Drawing.SolidBrush(System.Drawing.Color.Blue), new Point2d(OutImage.Width / 2, OutImage.Height / 2));
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
                CVAiO2.Blur(InImage.Mat, blurImage, new Size(5, 5));
                OutImage = inImage.Clone(true);
                outputImageInfo.Image = OutImage;

                if (RunParams.CenterContour.Count == 0) throw new Exception("Contour empty, please run Bead Analysis");
                RunParams.InspectionResult.Clear();

                float searchLength = RunParams.Offset;
                double angle = 0;
                inspectionResult = true;
                for (int i = 0; i < RunParams.CenterContour.Count; i++)
                {
                    if (i < RunParams.CenterContour.Count - 2)
                        angle = Math.Atan2(RunParams.CenterContour[i + 2].Y - RunParams.CenterContour[i].Y, RunParams.CenterContour[i + 2].X - RunParams.CenterContour[i].X);
                    System.Drawing.PointF EP = new System.Drawing.PointF((float)(RunParams.CenterContour[i].X + searchLength * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y + searchLength * Math.Sin(angle + Math.PI / 2)));
                    System.Drawing.PointF SP = new System.Drawing.PointF((float)(RunParams.CenterContour[i].X - searchLength * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y - searchLength * Math.Sin(angle + Math.PI / 2)));
                    bool findEdge = true;
                    LineCaliperEx lineCaliperEx = new LineCaliperEx() { SP = SP, EP = EP, MinContrastThreshold = RunParams.StartBeadLine.MinContrastThreshold };
                    Point3f firstPoint, secondPoint, tempPoint;
                    findEdge &= FindBeadEdge(lineCaliperEx, out firstPoint, out tempPoint);
                    lineCaliperEx.SP = EP;
                    lineCaliperEx.EP = SP;
                    findEdge &= FindBeadEdge(lineCaliperEx, out secondPoint, out tempPoint);

                    if (!findEdge)
                    {
                        // không tìm được đủ 2 điểm biên dạng của Bead
                        inspectionResult = false;
                        if (RunParams.InspectionResult.Count > 0)
                            RunParams.InspectionResult.Add(new BeadInspection() { status = false, firstPoint = RunParams.InspectionResult.Last().firstPoint, secondPoint = RunParams.InspectionResult.Last().secondPoint, message = "No Bead or Bead Incorrect Position" });
                        else
                            RunParams.InspectionResult.Add(new BeadInspection() { status = false, firstPoint = new Point3f(0, 0, 0), secondPoint = new Point3f(0, 0, 0), message = "No Bead or Bead Incorrect Position" });
                    }
                    else
                    {
                        float length = AiO.getLength(new System.Drawing.PointF(firstPoint.X, firstPoint.Y), new System.Drawing.PointF(secondPoint.X, secondPoint.Y));
                        if (length > RunParams.BeadWidthMax)
                        {
                            RunParams.InspectionResult.Add(new BeadInspection() { status = false, firstPoint = firstPoint, secondPoint = secondPoint, message = "Bead too thick" });
                        }
                        else if (length < RunParams.BeadWidthMin)
                        {
                            RunParams.InspectionResult.Add(new BeadInspection() { status = false, firstPoint = firstPoint, secondPoint = secondPoint, message = "Bead too thin" });
                        }
                        else
                        {
                            RunParams.InspectionResult.Add(new BeadInspection() { status = true, firstPoint = firstPoint, secondPoint = secondPoint, message = "OK" });
                        }
                    }
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

        public int ContourReferenceAnalysis()
        {
            if (!AiO.IsPossibleImage(InImage.Mat)) return -1;
            CVAiO2.Blur(InImage.Mat, blurImage, new Size(5, 5));
            // Kiểm tra hình sau khi masking
            //AiO.ShowImage(maskedImage, 1);
            // Tìm kiếm tại vị trí bắt đầu của Glue Bead
            Point3f firstStartBeadPoint, secondStartBeadPoint, centerStartBeadPoint;
            if (!FindBeadEdge(RunParams.StartBeadLine, out firstStartBeadPoint, out secondStartBeadPoint)) return -2;
            centerStartBeadPoint = new Point3f((firstStartBeadPoint.X + secondStartBeadPoint.X) / 2, (firstStartBeadPoint.Y + secondStartBeadPoint.Y) / 2, 0);
            // Tìm kiếm tại ví trí kết thúc của Glue Bead
            Point3f firstEndBeadPoint, secondEndBeadPoint, centerEndBeadPoint;
            if (!FindBeadEdge(RunParams.EndBeadLine, out firstEndBeadPoint, out secondEndBeadPoint)) return -3;
            centerEndBeadPoint = new Point3f((firstEndBeadPoint.X + secondEndBeadPoint.X) / 2, (firstEndBeadPoint.Y + secondEndBeadPoint.Y) / 2, 0);

            Mat binaryMat = new Mat();
            CVAiO2.Threshold(blurImage, binaryMat, 60, 255, ThresholdTypes.BinaryInv);

            // Vẽ 2 đường line màu đen thông qua các đường bắt đầu và kết thúc của Bead giúp tách biệt đc Bead để phân tích tiếp
            binaryMat.Line((int)RunParams.StartBeadLine.SP.X, (int)RunParams.StartBeadLine.SP.Y, (int)RunParams.StartBeadLine.EP.X, (int)RunParams.StartBeadLine.EP.Y, new Scalar(0), 2);
            binaryMat.Line((int)RunParams.EndBeadLine.SP.X, (int)RunParams.EndBeadLine.SP.Y, (int)RunParams.EndBeadLine.EP.X, (int)RunParams.EndBeadLine.EP.Y, new Scalar(0), 2);

            //binaryMat.Line((int)RunParams.StartBeadLine.CP.X, (int)RunParams.StartBeadLine.CP.Y, (int)RunParams.EndBeadLine.CP.X, (int)RunParams.EndBeadLine.CP.Y, new Scalar(0), 2);

            // Hiển thị hình ảnh của binary để kiểm tra
            //AiO.ShowImage(binaryMat, 1);

            Point[][] contours;
            HierarchyIndex[] hierarchyIndex;
            CVAiO2.FindContours(binaryMat, out contours, out hierarchyIndex, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            //Lựa chọn contour có chưa điểm PointOnBead để xử lý tiếp
            int contourIndex = -1;
            for (int i = 0; i < contours.Length; i++)
                if (CVAiO2.PointPolygonTest(contours[i], RunParams.PointOnBead.Point, false) == 1 && CVAiO2.ContourArea(contours[i]) < 20000)
                {
                    contourIndex = i;
                    break;
                }
            // Kiểm tra nếu không tìm thấy contour chưa glue bead
            if (contourIndex == -1) return -4;

            // Vẽ lại contour chứa Glue Bead
            Mat Marker = new Mat(binaryMat.Size(), MatType.CV_8UC1, new Scalar(255));
            Marker.DrawContours(contours, contourIndex, new Scalar(0), CVAiO2.FILLED);

            Mat Marker1 = new Mat(binaryMat.Size(), MatType.CV_8UC1, new Scalar(255));
            Marker1.DrawContours(contours, -1, new Scalar(0), 1);
            // Kiểm tra contour đã tìm được
            //AiO.ShowImage(Marker1, 1);
            // Tìm đường tâm của Glue Bead
            Mat thinContour = ZhangSuenThinning(Marker);
            bool loopTeminate = false;

            //Kiểm tra đường tâm tìm được
            //AiO.ShowImage(thinContour, 1);
            RunParams.CenterContourRaw.Clear();
            for (int y = 2; y < thinContour.Rows - 2; y++)
            {
                for (int x = 2; x < thinContour.Cols - 2; x++)
                {
                    byte p1 = thinContour.Get<byte>(y, x);
                    if (p1 != 0) continue; // tìm kiếm điểm bắt đầu của đường tâm của Bead

                    byte p2 = thinContour.Get<byte>(y - 1, x);
                    byte p3 = thinContour.Get<byte>(y - 1, x + 1);
                    byte p4 = thinContour.Get<byte>(y, x + 1);
                    byte p5 = thinContour.Get<byte>(y + 1, x + 1);
                    byte p6 = thinContour.Get<byte>(y + 1, x);
                    byte p7 = thinContour.Get<byte>(y + 1, x - 1);
                    byte p8 = thinContour.Get<byte>(y, x - 1);
                    byte p9 = thinContour.Get<byte>(y - 1, x - 1);

                    int transitions = 0;
                    if (p2 == 255 && p3 == 0) transitions++;
                    if (p3 == 255 && p4 == 0) transitions++;
                    if (p4 == 255 && p5 == 0) transitions++;
                    if (p5 == 255 && p6 == 0) transitions++;
                    if (p6 == 255 && p7 == 0) transitions++;
                    if (p7 == 255 && p8 == 0) transitions++;
                    if (p8 == 255 && p9 == 0) transitions++;
                    if (p9 == 255 && p2 == 0) transitions++;

                    if (p1 == 0 && transitions == 1)
                    {
                        int x_l = x;
                        int y_l = y;
                        thinContour.Set<byte>(y_l, x_l, 255);
                        // thêm vào điểm đầu tiên của StartBeadPoint hoặc EndBeadPoint

                        if (Math.Pow(x_l - centerStartBeadPoint.X, 2) + Math.Pow(y_l - centerStartBeadPoint.Y, 2)
                            > Math.Pow(x_l - centerEndBeadPoint.X, 2) + Math.Pow(y_l - centerEndBeadPoint.Y, 2))
                            RunParams.CenterContourRaw.Add(new Point(centerEndBeadPoint.X, centerEndBeadPoint.Y));
                        else
                            RunParams.CenterContourRaw.Add(new Point(centerStartBeadPoint.X, centerStartBeadPoint.Y));
                        RunParams.CenterContourRaw.Add(new Point(x, y));
                        do
                        {
                            // kiểm tra 4 pixel vuông góc trước, nếu có mầu đen chuyển điểm tâm tìm kiếm thành điểm tìm được
                            if (thinContour.Get<byte>(y_l - 1, x_l) == 0) { y_l = y_l - 1; }
                            else if (thinContour.Get<byte>(y_l, x_l + 1) == 0) { x_l = x_l + 1; }
                            else if (thinContour.Get<byte>(y_l + 1, x_l) == 0) { y_l = y_l + 1; }
                            else if (thinContour.Get<byte>(y_l, x_l - 1) == 0) { x_l = x_l - 1; }
                            // 4 điểm góc
                            else if (thinContour.Get<byte>(y_l - 1, x_l + 1) == 0) { y_l = y_l - 1; x_l = x_l + 1; }
                            else if (thinContour.Get<byte>(y_l + 1, x_l + 1) == 0) { y_l = y_l + 1; x_l = x_l + 1; }
                            else if (thinContour.Get<byte>(y_l + 1, x_l - 1) == 0) { y_l = y_l + 1; x_l = x_l - 1; }
                            else if (thinContour.Get<byte>(y_l - 1, x_l - 1) == 0) { y_l = y_l - 1; x_l = x_l - 1; }
                            else loopTeminate = true;

                            p2 = thinContour.Get<byte>(y_l - 1, x_l);
                            p3 = thinContour.Get<byte>(y_l - 1, x_l + 1);
                            p4 = thinContour.Get<byte>(y_l, x_l + 1);
                            p5 = thinContour.Get<byte>(y_l + 1, x_l + 1);
                            p6 = thinContour.Get<byte>(y_l + 1, x_l);
                            p7 = thinContour.Get<byte>(y_l + 1, x_l - 1);
                            p8 = thinContour.Get<byte>(y_l, x_l - 1);
                            p9 = thinContour.Get<byte>(y_l - 1, x_l - 1);

                            transitions = 0;
                            if (p2 == 255 && p3 == 0) transitions++;
                            if (p3 == 255 && p4 == 0) transitions++;
                            if (p4 == 255 && p5 == 0) transitions++;
                            if (p5 == 255 && p6 == 0) transitions++;
                            if (p6 == 255 && p7 == 0) transitions++;
                            if (p7 == 255 && p8 == 0) transitions++;
                            if (p8 == 255 && p9 == 0) transitions++;
                            if (p9 == 255 && p2 == 0) transitions++;

                            thinContour.Set<byte>(y_l, x_l, 255);
                            RunParams.CenterContourRaw.Add(new Point(x_l, y_l));
                            if (transitions == 0) loopTeminate = true;
                            if (x_l > thinContour.Cols) loopTeminate = true;
                            if (y_l > thinContour.Rows) loopTeminate = true;
                        } while (!loopTeminate);
                    }
                    if (loopTeminate) break;
                }
                if (loopTeminate) break;
            }
            if (Math.Pow(RunParams.CenterContourRaw.Last().X - centerStartBeadPoint.X, 2) + Math.Pow(RunParams.CenterContourRaw.Last().Y - centerStartBeadPoint.Y, 2)
                            > Math.Pow(RunParams.CenterContourRaw.Last().X - centerEndBeadPoint.X, 2) + Math.Pow(RunParams.CenterContourRaw.Last().Y - centerEndBeadPoint.Y, 2))
                RunParams.CenterContourRaw.Add(new Point(centerEndBeadPoint.X, centerEndBeadPoint.Y));
            else
                RunParams.CenterContourRaw.Add(new Point(centerStartBeadPoint.X, centerStartBeadPoint.Y));
            // Loại bỏ 1 số điểm đầu, cuối để làm trơn contour
            RunParams.CenterContourRaw.RemoveRange(1, 5);
            RunParams.CenterContourRaw.RemoveRange(RunParams.CenterContourRaw.Count - 6, 5);
            // Hiển thị hình ảnh sau khi đã xử lý, nếu hình trắng toàn bộ thì quá trình xử lý là OK
            //AiO.ShowImage(thinContour, 1);
            FindToloranceContour();
            return 0;
        }

        public int FindToloranceContour()
        {
            RunParams.CenterContour.Clear();
            RunParams.CenterContour = CVAiO2.ApproxPolyDP(RunParams.CenterContourRaw.ToArray(), 2, false).ToList();

            // Xấp xỉ bằng 1 contour mới với 150 điểm
            int count = 150;
            float[] X = new float[RunParams.CenterContour.Count];
            float[] Y = new float[RunParams.CenterContour.Count];
            for (int i = 0; i < RunParams.CenterContour.Count; i++)
            {
                X[i] = RunParams.CenterContour[i].X;
                Y[i] = RunParams.CenterContour[i].Y;
            }
            (float[] xm, float[] ym) = Cubic.InterpolateXY(X, Y, count);
            RunParams.CenterContour.Clear();
            for (int i = 0; i < count; i++)
                RunParams.CenterContour.Add(new Point(xm[i], ym[i]));

            RunParams.FirstContour.Clear();
            RunParams.SecondContour.Clear();
            RunParams.FirstToloranceContour.Clear();
            RunParams.SecondToloranceContour.Clear();
            float searchLength = RunParams.Offset;
            double angle = 0;
            for (int i = 0; i < RunParams.CenterContour.Count; i++)
            {
                if (i < RunParams.CenterContour.Count - 2)
                    angle = Math.Atan2(RunParams.CenterContour[i + 2].Y - RunParams.CenterContour[i].Y, RunParams.CenterContour[i + 2].X - RunParams.CenterContour[i].X);
                System.Drawing.PointF SP = new System.Drawing.PointF((float)(RunParams.CenterContour[i].X + searchLength * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y + searchLength * Math.Sin(angle + Math.PI / 2)));
                System.Drawing.PointF EP = new System.Drawing.PointF((float)(RunParams.CenterContour[i].X - searchLength * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y - searchLength * Math.Sin(angle + Math.PI / 2)));
                LineCaliperEx lineCaliperEx = new LineCaliperEx() { SP = SP, EP = EP, MinContrastThreshold = RunParams.StartBeadLine.MinContrastThreshold };
                Point3f firstPoint, secondPoint;
                if (!FindBeadEdge(lineCaliperEx, out firstPoint, out secondPoint)) continue;
                RunParams.FirstContour.Add(firstPoint);
                RunParams.SecondContour.Add(secondPoint);
                RunParams.FirstToloranceContour.Add(new Point3f((float)(RunParams.CenterContour[i].X + RunParams.Offset * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y + RunParams.Offset * Math.Sin(angle + Math.PI / 2)), 0));
                RunParams.SecondToloranceContour.Add(new Point3f((float)(RunParams.CenterContour[i].X - RunParams.Offset * Math.Cos(angle + Math.PI / 2)), (float)(RunParams.CenterContour[i].Y - RunParams.Offset * Math.Sin(angle + Math.PI / 2)), 0));
            }

            return 0;
        }

        public bool FindBeadEdge(LineCaliperEx lineCaliperEx, out Point3f firstPoint, out Point3f secondPoint)
        {
            firstPoint = new Point3f();
            secondPoint = new Point3f();
            if (blurImage.Empty()) return false;
            //Tìm điểm trắng sang đen
            lineCaliperEx.Polarity = EEdgePolarity.Light2Dark;
            List<Point3f> L2DPoints = lineCaliperEx.CalCaliperPoints(blurImage);
            if (L2DPoints.Count == 0) return false; // không tìm được điểm trắng sang đen
            // Điểm biến đổi trắng sang đen đầu tiên sẽ được tính là Edge đầu tiên của Bead
            firstPoint = L2DPoints[0];
            lineCaliperEx.Polarity = EEdgePolarity.Dark2Light;
            List<Point3f> D2LPoints = lineCaliperEx.CalCaliperPoints(blurImage);
            if (D2LPoints.Count == 0) return false; // không tìm được điểm đen sang trắng
            // Phải tìm điểm đen sang trắng đầu tiên sau điểm trắng sang đen bên trên
            foreach (Point3f point3f in D2LPoints)
            {
                if (Math.Pow(point3f.X - lineCaliperEx.SP.X, 2) + Math.Pow(point3f.Y - lineCaliperEx.SP.Y, 2) > Math.Pow(firstPoint.X - lineCaliperEx.SP.X, 2) + Math.Pow(firstPoint.Y - lineCaliperEx.SP.Y, 2))
                {
                    secondPoint = point3f;
                    return true;
                }
            }
            return false;
        }

        // http://rstudio-pubs-static.s3.amazonaws.com/302782_e337cfbc5ad24922bae96ca5977f4da8.html#:~:text=The%20Zhang%2DSuen%20Thinning%20algorithm,remove%20pixels%20from%20the%20image.
        // https://github.com/linbojin/Skeletonization-by-Zhang-Suen-Thinning-Algorithm/tree/master
        // phương pháp tìm thinnig 

        private Mat ZhangSuenThinning(Mat inputImage)
        {
            Mat img = inputImage.Clone();
            Mat prevImg = new Mat();
            int iteration = 0;

            while (true)
            {
                img.CopyTo(prevImg);
                Mat masking = new Mat(img.Size(), MatType.CV_8UC1, new Scalar(0));
                // Step 1
                ZhangSuenThinningStep1(img, masking);
                for (int y = 1; y < img.Rows - 1; y++)
                {
                    for (int x = 1; x < img.Cols - 1; x++)
                    {
                        if (masking.Get<byte>(y, x) != 0)
                        {
                            img.Set<byte>(y, x, 255);
                        }
                    }
                }

                masking = new Mat(img.Size(), MatType.CV_8UC1, new Scalar(0));

                // Step 2
                ZhangSuenThinningStep2(img, masking);

                for (int y = 1; y < img.Rows - 1; y++)
                {
                    for (int x = 1; x < img.Cols - 1; x++)
                    {
                        if (masking.Get<byte>(y, x) != 0)
                        {
                            img.Set<byte>(y, x, 255);
                        }
                    }
                }
                Mat compareImage2 = new Mat();
                CVAiO2.Compare(img, prevImg, compareImage2, CmpType.EQ);
                if (compareImage2.CountNonZero() == img.Rows * img.Cols) break;
                iteration++;
            }
            return img;
        }

        private void ZhangSuenThinningStep1(Mat img, Mat masking)
        {
            for (int y = 1; y < img.Rows - 1; y++)
            {
                for (int x = 1; x < img.Cols - 1; x++)
                {
                    byte p2 = img.Get<byte>(y - 1, x);
                    byte p3 = img.Get<byte>(y - 1, x + 1);
                    byte p4 = img.Get<byte>(y, x + 1);
                    byte p5 = img.Get<byte>(y + 1, x + 1);
                    byte p6 = img.Get<byte>(y + 1, x);
                    byte p7 = img.Get<byte>(y + 1, x - 1);
                    byte p8 = img.Get<byte>(y, x - 1);
                    byte p9 = img.Get<byte>(y - 1, x - 1);

                    int transitions = 0;
                    if (p2 == 255 && p3 == 0) transitions++;
                    if (p3 == 255 && p4 == 0) transitions++;
                    if (p4 == 255 && p5 == 0) transitions++;
                    if (p5 == 255 && p6 == 0) transitions++;
                    if (p6 == 255 && p7 == 0) transitions++;
                    if (p7 == 255 && p8 == 0) transitions++;
                    if (p8 == 255 && p9 == 0) transitions++;
                    if (p9 == 255 && p2 == 0) transitions++;

                    byte p1 = img.Get<byte>(y, x);

                    if (p1 == 0 && transitions == 1 && 2 <= BlackPixels(p2, p3, p4, p5, p6, p7, p8, p9) && BlackPixels(p2, p3, p4, p5, p6, p7, p8, p9) <= 6 && (p2 == 255 || p4 == 255 || p6 == 255) && (p4 == 255 || p6 == 255 || p8 == 255))
                    {
                        masking.Set<byte>(y, x, 1);
                    }
                }
            }
        }

        private void ZhangSuenThinningStep2(Mat img, Mat masking)
        {
            for (int y = 1; y < img.Rows - 1; y++)
            {
                for (int x = 1; x < img.Cols - 1; x++)
                {
                    byte p2 = img.Get<byte>(y - 1, x);
                    byte p3 = img.Get<byte>(y - 1, x + 1);
                    byte p4 = img.Get<byte>(y, x + 1);
                    byte p5 = img.Get<byte>(y + 1, x + 1);
                    byte p6 = img.Get<byte>(y + 1, x);
                    byte p7 = img.Get<byte>(y + 1, x - 1);
                    byte p8 = img.Get<byte>(y, x - 1);
                    byte p9 = img.Get<byte>(y - 1, x - 1);

                    int transitions = 0;
                    if (p2 == 255 && p3 == 0) transitions++;
                    if (p3 == 255 && p4 == 0) transitions++;
                    if (p4 == 255 && p5 == 0) transitions++;
                    if (p5 == 255 && p6 == 0) transitions++;
                    if (p6 == 255 && p7 == 0) transitions++;
                    if (p7 == 255 && p8 == 0) transitions++;
                    if (p8 == 255 && p9 == 0) transitions++;
                    if (p9 == 255 && p2 == 0) transitions++;

                    byte p1 = img.Get<byte>(y, x);

                    if (p1 == 0 && transitions == 1 && 2 <= BlackPixels(p2, p3, p4, p5, p6, p7, p8, p9) && BlackPixels(p2, p3, p4, p5, p6, p7, p8, p9) <= 6 && (p2 == 255 || p4 == 255 || p8 == 255) && (p2 == 255 || p6 == 255 || p8 == 255))
                    {
                        masking.Set<byte>(y, x, 1);
                    }
                }
            }
        }

        private int BlackPixels(byte p2, byte p3, byte p4, byte p5, byte p6, byte p7, byte p8, byte p9)
        {
            int bp1 = (p2 == 0) ? 1 : 0;
            int bp2 = (p3 == 0) ? 1 : 0;
            int bp3 = (p4 == 0) ? 1 : 0;
            int bp4 = (p5 == 0) ? 1 : 0;
            int bp5 = (p6 == 0) ? 1 : 0;
            int bp6 = (p7 == 0) ? 1 : 0;
            int bp7 = (p8 == 0) ? 1 : 0;
            int bp8 = (p9 == 0) ? 1 : 0;
            int bp = bp1 + bp2 + bp3 + bp4 + bp5 + bp6 + bp7 + bp8;
            return bp;
        }
    }

    public static class Cubic
    {
        /// <summary>
        /// Generate a smooth (interpolated) curve that follows the path of the given X/Y points
        /// </summary>
        public static (float[] xs, float[] ys) InterpolateXY(float[] xs, float[] ys, int count)
        {
            if (xs is null || ys is null || xs.Length != ys.Length)
                throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have same length");

            int inputPointCount = xs.Length;
            float[] inputDistances = new float[inputPointCount];
            for (int i = 1; i < inputPointCount; i++)
            {
                float dx = xs[i] - xs[i - 1];
                float dy = ys[i] - ys[i - 1];
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                inputDistances[i] = inputDistances[i - 1] + distance;
            }

            float meanDistance = inputDistances.Last() / (count - 1);
            float[] evenDistances = Enumerable.Range(0, count).Select(x => x * meanDistance).ToArray();
            float[] xsOut = Interpolate(inputDistances, xs, evenDistances);
            float[] ysOut = Interpolate(inputDistances, ys, evenDistances);
            return (xsOut, ysOut);
        }

        private static float[] Interpolate(float[] xOrig, float[] yOrig, float[] xInterp)
        {
            (float[] a, float[] b) = FitMatrix(xOrig, yOrig);

            float[] yInterp = new float[xInterp.Length];
            for (int i = 0; i < yInterp.Length; i++)
            {
                int j;
                for (j = 0; j < xOrig.Length - 2; j++)
                    if (xInterp[i] <= xOrig[j + 1])
                        break;

                float dx = xOrig[j + 1] - xOrig[j];
                float t = (xInterp[i] - xOrig[j]) / dx;
                float y = (1 - t) * yOrig[j] + t * yOrig[j + 1] +
                    t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
                yInterp[i] = y;
            }

            return yInterp;
        }

        private static (float[] a, float[] b) FitMatrix(float[] x, float[] y)
        {
            int n = x.Length;
            float[] a = new float[n - 1];
            float[] b = new float[n - 1];
            float[] r = new float[n];
            float[] A = new float[n];
            float[] B = new float[n];
            float[] C = new float[n];

            float dx1, dx2, dy1, dy2;

            dx1 = x[1] - x[0];
            C[0] = 1.0f / dx1;
            B[0] = 2.0f * C[0];
            r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);

            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];
                A[i] = 1.0f / dx1;
                C[i] = 1.0f / dx2;
                B[i] = 2.0f * (A[i] + C[i]);
                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            dx1 = x[n - 1] - x[n - 2];
            dy1 = y[n - 1] - y[n - 2];
            A[n - 1] = 1.0f / dx1;
            B[n - 1] = 2.0f * A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));

            float[] cPrime = new float[n];
            cPrime[0] = C[0] / B[0];
            for (int i = 1; i < n; i++)
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);

            float[] dPrime = new float[n];
            dPrime[0] = r[0] / B[0];
            for (int i = 1; i < n; i++)
                dPrime[i] = (r[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);

            float[] k = new float[n];
            k[n - 1] = dPrime[n - 1];
            for (int i = n - 2; i >= 0; i--)
                k[i] = dPrime[i] - cPrime[i] * k[i + 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1] * dx1 - dy1;
                b[i - 1] = -k[i] * dx1 + dy1;
            }

            return (a, b);
        }
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class GlueBeadInspectionRunParams : RunParams, ISerializable
    {
        #region Fields
        private List<Point3f> firstContour;
        private List<Point3f> firstToloranceContour;
        private List<Point3f> secondContour;
        private List<Point3f> secondToloranceContour;

        private List<Point> centerContourRaw;
        private List<Point> centerContour;

        [NonSerialized]
        private List<BeadInspection> inspectionResult;

        private LineCaliperEx startBeadLine;
        private LineCaliperEx endBeadLine;
        private InteractPoint pointOnBead;
        private InteractRectangle searchRegion;
        private float offset;
        private float beadWidthMax;
        private float beadWidthMin;

        #endregion

        #region Properties
        [Description("Đường line xác định vị trí bắt đầu của Bead, lựa chọn đảm bảo tìm ra được đường Bead"), PropertyOrder(30)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public LineCaliperEx StartBeadLine
        {
            get
            {
                if (startBeadLine == null) startBeadLine = new LineCaliperEx(370, 420, 420, 450) { MinContrastThreshold = 30 };
                return startBeadLine;
            }
            set => startBeadLine = value;
        }

        [Description("Đường line xác định vị trí kết thúc của Bead, lựa chọn đảm bảo tìm ra được đường Bead"), PropertyOrder(31)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public LineCaliperEx EndBeadLine
        {
            get
            {
                if (endBeadLine == null) endBeadLine = new LineCaliperEx(995, 440, 930, 510) { MinContrastThreshold = 30 };
                return endBeadLine;
            }
            set => endBeadLine = value;
        }

        [Description("Đường line xác định vị trí kết thúc của Bead, lựa chọn đảm bảo tìm ra được đường Bead"), PropertyOrder(32)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public InteractPoint PointOnBead
        {
            get
            {
                if (pointOnBead == null) pointOnBead = new InteractPoint(700, 700) { Color = System.Drawing.Color.Blue, CrossSize = 40 };
                return pointOnBead;
            }
            set => pointOnBead = value;
        }

        [Description("Đường contour mẫu phải "), Browsable(false)]
        public List<Point3f> FirstContour
        {
            get
            {
                if (firstContour == null) firstContour = new List<Point3f>();
                return firstContour;
            }
            set => firstContour = value;
        }

        [Description("Đường ngưỡng phải cho phép của Bead "), Browsable(false)]
        public List<Point3f> FirstToloranceContour
        {
            get
            {
                if (firstToloranceContour == null) firstToloranceContour = new List<Point3f>();
                return firstToloranceContour;
            }
            set => firstToloranceContour = value;
        }

        [Browsable(false)]
        public List<Point3f> SecondContour
        {
            get
            {
                if (secondContour == null) secondContour = new List<Point3f>();
                return secondContour;
            }
            set => secondContour = value;
        }

        [Description("Đường ngưỡng trái cho phép của Bead "), Browsable(false)]
        public List<Point3f> SecondToloranceContour
        {
            get
            {
                if (secondToloranceContour == null) secondToloranceContour = new List<Point3f>();
                return secondToloranceContour;
            }
            set => secondToloranceContour = value;
        }

        [Description("Kết quả kiểm tra contour"), Browsable(false)]
        public List<BeadInspection> InspectionResult
        {
            get
            {
                if (inspectionResult == null) inspectionResult = new List<BeadInspection>();
                return inspectionResult;
            }
            set => inspectionResult = value;
        }

        [Description("Đường tâm của Bead có được nhờ phân tích Glue Bead mẫu"), Browsable(false)]
        public List<Point> CenterContourRaw
        {
            get
            {
                if (centerContourRaw == null) centerContourRaw = new List<Point>();
                return centerContourRaw;
            }
            set => centerContourRaw = value;
        }

        [Description("Đường tâm của Bead có được nhờ phân tích Glue Bead mẫu"), Browsable(false)]
        public List<Point> CenterContour
        {
            get
            {
                if (centerContour == null) centerContour = new List<Point>();
                return centerContour;
            }
            set => centerContour = value;
        }

        [Description("Offset tính từ đường tâm của Bead"), PropertyOrder(34)]
        public float Offset
        {
            get => offset;
            set
            {
                if (offset == value) return;
                offset = value;
                NotifyPropertyChanged(nameof(Offset));
            }
        }

        [Description("Độ rộng lớn nhất của Bead"), PropertyOrder(35)]
        public float BeadWidthMax
        {
            get => beadWidthMax;
            set
            {
                if (beadWidthMax == value) return;
                beadWidthMax = value;
                NotifyPropertyChanged(nameof(BeadWidthMax));
            }
        }

        [Description("Độ rộng nhỏ nhất của Bead"), PropertyOrder(36)]
        public float BeadWidthMin
        {
            get => beadWidthMin;
            set
            {
                if (beadWidthMin == value) return;
                beadWidthMin = value;
                NotifyPropertyChanged(nameof(BeadWidthMin));
            }
        }

        #endregion
        public GlueBeadInspectionRunParams()
        {
            offset = 25;
            beadWidthMin = 7;
            beadWidthMax = 20;
        }

        #region Do not change
        public GlueBeadInspectionRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public GlueBeadInspectionRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (GlueBeadInspectionRunParams)runParams;
            }
        }
        #endregion
    }
}
