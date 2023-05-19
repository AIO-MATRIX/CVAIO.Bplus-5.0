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
    public partial class BlobFillEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public BlobFillEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += BlobFillEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }

        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
        }

        protected override void btnRun_Click(object sender, EventArgs e)
        {
            base.btnRun_Click(sender, e);
            if (Subject.ContourRect == null || Subject.ContourRect.Count == 0 || Subject.ContourArea == null || Subject.ContourArea.Count == 0) return;
            lvBlobResult.Items.Clear();
            for (int i = 0; i < Subject.ContourRect.Count; i ++)
            {
                string[] items = new string[] { i.ToString(), Subject.ContourArea[i].ToString(),
                    Subject.ContourRect[i].Location.X.ToString("F3"), Subject.ContourRect[i].Location.Y.ToString("F3"), Subject.ContourRect[i].Width.ToString(), Subject.ContourRect[i].Height.ToString() };
                lvBlobResult.Items.Add(new ListViewItem(items));
            }
        }

        private void BlobFillEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as BlobFill;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BlobFill Subject
        {
            get { return base.GetSubject() as BlobFill; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(BlobFill), null);
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

        private void lvBlobResult_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem listViewItem = lvBlobResult.SelectedItems[0];
            Subject.ContourSelected = int.Parse(lvBlobResult.SelectedItems[0].Text);
            DisplayViewEdit.RefreshImage();
        }
    }
}
