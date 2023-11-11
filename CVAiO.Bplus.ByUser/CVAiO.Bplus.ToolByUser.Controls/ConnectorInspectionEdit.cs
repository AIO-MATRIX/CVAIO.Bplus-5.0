using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.ToolByUser;

namespace CVAiO.Bplus.ToolByUser.Controls
{
    public partial class ConnectorInspectionEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        private InspectionRegion currentRegion;
        private List<InspectionRegion> inspectionRegions = new List<InspectionRegion>();
        #endregion

        public ConnectorInspectionEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += ConnectorInspectionEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
            InitHistogram();
        }
        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
                foreach (InspectionRegion inspectionRegion in inspectionRegions)
                    DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(inspectionRegion.RegionCenter);
            }
        }
        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
            DisplayViewEdit.DisplayViewInteract.GraphicsFuncCollection.Clear();
            Subject.DrawInputs(DisplayViewEdit.DisplayViewInteract as DisplayView);

            if (Subject.InImage.Mat.Channels() != 3) return;
            foreach (InspectionRegion inspectionRegion in inspectionRegions)
                if (drawObject.GetHashCode() == inspectionRegion.RegionCenter.GetHashCode())
                    currentRegion = inspectionRegion;
            UpdateHistogram();
        }
        private void ConnectorInspectionEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as ConnectorInspection;
            inspectionRegions.Clear();
            inspectionRegions.Add(Subject.RunParams.OrangeWire);
            inspectionRegions.Add(Subject.RunParams.YellowWire);
            inspectionRegions.Add(Subject.RunParams.GreenWire);
            inspectionRegions.Add(Subject.RunParams.BlueWire);
            inspectionRegions.Add(Subject.RunParams.PurpleWire);
            inspectionRegions.Add(Subject.RunParams.GrayWire);
            inspectionRegions.Add(Subject.RunParams.WhiteWire);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ConnectorInspection Subject
        {
            get { return base.GetSubject() as ConnectorInspection; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(ConnectorInspection), null);
        }

        private void InitHistogram()
        {
            HistogramBoxRed.EnableRangeDrag = HistogramBoxBlue.EnableRangeDrag = HistogramBoxGreen.EnableRangeDrag = true;
            HistogramBoxRed.RangeChange += delegate
            {
                Subject.RunParams.RangeLow = HistogramBoxRed.RangeLow;
                if (HistogramBoxRed.RangeHigh < 256)
                    Subject.RunParams.RangeHigh = HistogramBoxRed.RangeHigh;
                if (currentRegion == null) return;
                currentRegion.RedMin = Subject.RunParams.RangeLow;
                currentRegion.RedMax = Subject.RunParams.RangeHigh;
                ToolProperty.Refresh();
            };
            HistogramBoxBlue.RangeChange += delegate
            {
                Subject.RunParams.RangeLow = HistogramBoxBlue.RangeLow;
                if (HistogramBoxBlue.RangeHigh < 256)
                    Subject.RunParams.RangeHigh = HistogramBoxBlue.RangeHigh;
                if (currentRegion == null) return;
                currentRegion.BlueMin = Subject.RunParams.RangeLow;
                currentRegion.BlueMax = Subject.RunParams.RangeHigh;
                ToolProperty.Refresh();
            };
            HistogramBoxGreen.RangeChange += delegate
            {
                Subject.RunParams.RangeLow = HistogramBoxGreen.RangeLow;
                if (HistogramBoxGreen.RangeHigh < 256)
                    Subject.RunParams.RangeHigh = HistogramBoxGreen.RangeHigh;
                if (currentRegion == null) return;
                currentRegion.GreenMin = Subject.RunParams.RangeLow;
                currentRegion.GreenMax = Subject.RunParams.RangeHigh;
                ToolProperty.Refresh();
            };
        }

        private void UpdateHistogram()
        {
            Mat currentRoi = new Mat();
            Mat[] RGB = new Mat[3];
            Scalar mean, dev;
            currentRoi = Subject.InImage.Mat.SubMat((int)(currentRegion.RegionCenter.Y - currentRegion.RegionRadius), (int)(currentRegion.RegionCenter.Y + currentRegion.RegionRadius),
                                                                    (int)(currentRegion.RegionCenter.X - currentRegion.RegionRadius), (int)(currentRegion.RegionCenter.X + currentRegion.RegionRadius));
            CVAiO2.Split(currentRoi, out RGB);
            CVAiO2.MeanStdDev(RGB[0], out mean, out dev);
            currentRegion.BlueMean = (float)mean;
            CVAiO2.MeanStdDev(RGB[1], out mean, out dev);
            currentRegion.GreenMean = (float)mean;
            CVAiO2.MeanStdDev(RGB[2], out mean, out dev);
            currentRegion.RedMean = (float)mean;

            HistogramBoxBlue.Image = RGB[0];
            HistogramBoxBlue.Refresh();
            HistogramBoxGreen.Image = RGB[1];
            HistogramBoxGreen.Refresh();
            HistogramBoxRed.Image = RGB[2];
            HistogramBoxRed.Refresh();
        }
        private void btnPreviousWire_Click(object sender, EventArgs e)
        {
            int currentPos = 0;
            if (currentRegion == null) currentRegion = inspectionRegions[0];
            for (int i = 0; i < inspectionRegions.Count; i++)
            {
                if (currentRegion.GetHashCode() == inspectionRegions[i].GetHashCode())
                    currentPos = i;
                inspectionRegions[i].RegionCenter.Color = Color.Blue;
            }
            if (currentPos == 0)
                currentRegion = inspectionRegions.Last();
            else
                currentRegion = inspectionRegions[currentPos - 1];
            currentRegion.RegionCenter.Color = Color.Red;
            DisplayViewEdit.SetImage(0);
            DisplayViewEdit.RefreshImage();
            UpdateHistogram();
            HistogramBoxRed.RangeHigh = 255;
            HistogramBoxRed.RangeLow = (int)currentRegion.RedMin;
            HistogramBoxRed.RangeHigh = (int)currentRegion.RedMax;
            HistogramBoxRed.Refresh();
            HistogramBoxBlue.RangeHigh = 255;
            HistogramBoxBlue.RangeLow = (int)currentRegion.BlueMin;
            HistogramBoxBlue.RangeHigh = (int)currentRegion.BlueMax;
            HistogramBoxBlue.Refresh();
            HistogramBoxGreen.RangeHigh = 255;
            HistogramBoxGreen.RangeLow = (int)currentRegion.GreenMin;
            HistogramBoxGreen.RangeHigh = (int)currentRegion.GreenMax;
            HistogramBoxGreen.Refresh();
        }

        private void btnNextWire_Click(object sender, EventArgs e)
        {
            int currentPos = 0;
            if (currentRegion == null) currentRegion = inspectionRegions[0];
            for (int i = 0; i < inspectionRegions.Count; i++)
            {
                if (currentRegion.GetHashCode() == inspectionRegions[i].GetHashCode())
                    currentPos = i;
                inspectionRegions[i].RegionCenter.Color = Color.Blue;
            }
            if (currentPos == inspectionRegions.Count - 1)
                currentRegion = inspectionRegions.First();
            else
                currentRegion = inspectionRegions[currentPos + 1];
            currentRegion.RegionCenter.Color = Color.Red;
            DisplayViewEdit.SetImage(0);
            DisplayViewEdit.RefreshImage();
            UpdateHistogram();
            HistogramBoxRed.RangeHigh = 255;
            HistogramBoxRed.RangeLow = (int)currentRegion.RedMin;
            HistogramBoxRed.RangeHigh = (int)currentRegion.RedMax;
            HistogramBoxRed.Refresh();
            HistogramBoxBlue.RangeHigh = 255;
            HistogramBoxBlue.RangeLow = (int)currentRegion.BlueMin;
            HistogramBoxBlue.RangeHigh = (int)currentRegion.BlueMax;
            HistogramBoxBlue.Refresh();
            HistogramBoxGreen.RangeHigh = 255;
            HistogramBoxGreen.RangeLow = (int)currentRegion.GreenMin;
            HistogramBoxGreen.RangeHigh = (int)currentRegion.GreenMax;
            HistogramBoxGreen.Refresh();
        }
    }
}
