using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.HByUser;

namespace CVAiO.Bplus.HByUser.Controls
{
    public partial class HOCRCnnEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion
        public HOCRCnnEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += HOCRCnnEdit_SubjectChanged;
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
            Subject.DrawInputs(DisplayViewEdit.DisplayViewInteract);
        }
        private void HOCRCnnEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as HOCRCnn;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HOCRCnn Subject
        {
            get { return base.GetSubject() as HOCRCnn; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(HOCRCnn), null);
        }
    }
}
