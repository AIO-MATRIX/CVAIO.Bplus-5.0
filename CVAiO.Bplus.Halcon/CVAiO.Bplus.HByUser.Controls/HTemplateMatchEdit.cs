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
using CVAiO.Bplus.HByUser;
using CVAiO.Bplus.OpenCV;

namespace CVAiO.Bplus.HByUser.Controls
{
    public partial class HTemplateMatchEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        private int selectedPatternIndex = -1;
        #endregion
        public HTemplateMatchEdit()
        {
            InitializeComponent();
            InitBinding();
            this.DisplayViewEdit.InsertCustomImage(1, new ImageInfo("[HTemplateMatch] TrainImage"));
            this.SubjectChanged += TemplateMatchEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }

        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
        }

        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("TrainImage"))
            {
                if (Subject.InImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.InImage;
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.Origin);
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.TrainRegion);
            }
            else if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.InImage;
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.SearchRegion);
            }
        }

        private void TemplateMatchEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as HTemplateMatch;
            splitPropertyControl.IsSplitterFixed = true;
            splitPropertyControl.FixedPanel = FixedPanel.Panel2;
            btnMain_Click(this, null);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HTemplateMatch Subject
        {
            get { return base.GetSubject() as HTemplateMatch; }
            set { base.SetSubject(value); }
        }

        public int SelectedPatternIndex
        {
            get => selectedPatternIndex;
            set
            {
                if (selectedPatternIndex == value) return;
                selectedPatternIndex = value;
                displayPattern.Image = Subject.RunParams.PatternDatas[selectedPatternIndex].PatternImage;
                displayPattern.FitToWindow();
            }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(HTemplateMatch), null);
            this.DataBindings.Add("SelectedPatternIndex", source, "SelectedPatternIndex", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnPatternLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Pattern files (*.pti)|*.pti";
            fd.FilterIndex = 1;
            fd.Multiselect = true;
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (string.IsNullOrEmpty(fd.FileName)) return;
            Subject.RunParams.PatternDatas = (List<PatternData>)(new Serializer().Deserializing(fd.FileName));
            LogWriter.Instance.LogTool(string.Format("Pattern Load: {0}", fd.FileName));
        }

        private void btnPatternSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Pattern files (*.pti)|*.pti";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            if (string.IsNullOrEmpty(fd.FileName)) return;
            new Serializer().Serializing(fd.FileName, Subject.RunParams.PatternDatas);
            LogWriter.Instance.LogTool(string.Format("Pattern Save: {0}", fd.FileName));
        }

        private void btnPatternRemove_Click(object sender, EventArgs e)
        {
            if (frmMessageBox.Show(EMessageIcon.Question, string.Format("Do you really want to remove {0} Mark ?", Subject.SelectedPatternIndex)) != DialogResult.Yes) return;
            Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex] = new PatternData();
            RefreshPattern();
        }

        private void btnPatternTrain_Click(object sender, EventArgs e)
        {
            ImageInfo imageInfo = (ImageInfo)DisplayViewEdit.cbImageList.SelectedItem;
            if (!imageInfo.ImageName.Contains("TrainImage")) return;

            Core.Image image = DisplayViewEdit.DisplayViewInteract.Image;
            if (image == null) return;

            Mat templateImg = Subject.RunParams.TrainRegion.GetROIRegion(image.Mat);
            if (templateImg == null)
            {
                frmMessageBox.Show(EMessageIcon.Error, "There is no Image in ROI");
                return;
            }
            Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex] = new PatternData(new Core.Image(templateImg),
                                                                         new Point3f(Subject.RunParams.TrainRegion.Center.X, Subject.RunParams.TrainRegion.Center.Y, 0));
            RefreshPattern();
        }

        private void btnPatternMask_Click(object sender, EventArgs e)
        {
            if (Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex].PatternImage == null) return;
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Subject.SelectedPatternIndex = 0;
            Subject.SelectedPattern = Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex];
            btnMain.BackColor = Color.Aqua;
            btnSubA.BackColor = Color.Transparent;
            btnSubB.BackColor = Color.Transparent;
            RefreshPattern();
        }

        private void btnSubA_Click(object sender, EventArgs e)
        {
            Subject.SelectedPatternIndex = 1;
            Subject.SelectedPattern = Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex];
            btnMain.BackColor = Color.Transparent;
            btnSubA.BackColor = Color.Aqua;
            btnSubB.BackColor = Color.Transparent;
            RefreshPattern();
        }

        private void btnSubB_Click(object sender, EventArgs e)
        {
            Subject.SelectedPatternIndex = 2;
            Subject.SelectedPattern = Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex];
            btnMain.BackColor = Color.Transparent;
            btnSubA.BackColor = Color.Transparent;
            btnSubB.BackColor = Color.Aqua;
            RefreshPattern();
        }
        private void RefreshPattern()
        {
            displayPattern.Image = Subject.RunParams.PatternDatas[Subject.SelectedPatternIndex].PatternImage;
            displayPattern.FitToWindow();
        }
    }
}
