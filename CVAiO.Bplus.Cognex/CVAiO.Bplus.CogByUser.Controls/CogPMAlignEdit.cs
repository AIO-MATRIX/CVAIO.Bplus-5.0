using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.CogByUser;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System;

namespace CVAiO.Bplus.CogByUser.Controls
{
    public partial class CogPMAlignEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public CogPMAlignEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += CogPMAlignEdit_SubjectChanged;
        }

        private void CogPMAlignEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as CogPMAlign;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CogPMAlign Subject
        {
            get { return base.GetSubject() as CogPMAlign; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(CogPMAlign), null);
        }

        private void btnCogToolView_Click(object sender, System.EventArgs e)
        {
            CogToolEdit cogToolEdit = new CogToolEdit(new CogPMAlignEditV2());
            (cogToolEdit.toolEditBase as CogPMAlignEditV2).Subject = Subject.CogPMAlignTool;
            cogToolEdit.ShowDialog();
            ToolProperty.Refresh();
        }
    }
}
