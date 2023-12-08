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
using CVAiO.Bplus.ToolByUser;

namespace CVAiO.Bplus.ToolByUser.Controls
{
    public partial class GlueBeadInspectionEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public GlueBeadInspectionEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += GlueBeadInspectionEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }

        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.InImage;
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.StartBeadLine);
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.EndBeadLine);
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.PointOnBead);
            }
            else if (imageInfo.ImageName.Contains("OutputImage"))
            {
                if (Subject.OutImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.OutImage;
            }
        }
        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
            DisplayViewEdit.DisplayViewInteract.GraphicsFuncCollection.Clear();
            // Có sự thay đổi liên quan đến đường bắt đầu và kết thúc của Glue Bead -> xóa toán bộ các dữ liệu đã tính toán -> cần phải phân tích lại contour
            Subject.RunParams.CenterContour.Clear();
            Subject.RunParams.FirstContour.Clear();
            Subject.RunParams.SecondContour.Clear();
            Subject.RunParams.FirstToloranceContour.Clear();
            Subject.RunParams.SecondToloranceContour.Clear();
            Subject.RunParams.InspectionResult.Clear();
        }
        private void GlueBeadInspectionEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as GlueBeadInspection;
            (Subject as GlueBeadInspection).RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        private void RunParams_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Offset")
            {
                (Subject as GlueBeadInspection).FindToloranceContour();
                DisplayViewEdit.RefreshImage();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GlueBeadInspection Subject
        {
            get { return base.GetSubject() as GlueBeadInspection; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(GlueBeadInspection), null);
        }

        private void btnBeadAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmMessageBox.Show(EMessageIcon.Information, "Bead Analysis takes about 10s to finish, Dont do anything else, do you agree ?") != DialogResult.Yes) return;
                int result = Subject.ContourReferenceAnalysis();
                switch (result)
                {
                    case 0:
                        frmMessageBox.Show(EMessageIcon.Information, "Analysis Done. No Error");
                        break;
                    case -1:
                        frmMessageBox.Show(EMessageIcon.Error, "Input Image Empty");
                        break;
                    case -2:
                        frmMessageBox.Show(EMessageIcon.Error, "Could not find Bead at start Position");
                        break;
                    case -3:
                        frmMessageBox.Show(EMessageIcon.Error, "Could not find Bead at end Position");
                        break;
                    case -4:
                        frmMessageBox.Show(EMessageIcon.Error, "Could not find Bead contour");
                        break;
                    case -5:
                        frmMessageBox.Show(EMessageIcon.Error, "Cound not find the Bead Edge");
                        break;

                }
                DisplayViewEdit.SetLastImage();
                DisplayViewEdit.RefreshImage();
            }
            catch (Exception ex)
            {
                frmMessageBox.Show(EMessageIcon.Error, ex.ToString());
            }
        }
    }
}
