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
    public partial class FittingCircleEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public FittingCircleEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += FittingCircleEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }
        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.InImage;
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.SearchRegion);
            }
        }
        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
            DisplayViewEdit.DisplayViewInteract.GraphicsFuncCollection.Clear();
        }
        private void FittingCircleEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as FittingCircle;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FittingCircle Subject
        {
            get { return base.GetSubject() as FittingCircle; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(FittingCircle), null);
        }
        protected override void btnRun_Click(object sender, EventArgs e)
        {
            base.btnRun_Click(sender, e);
            ToolProperty.Refresh();
        }
    }
}
