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
using CVAiO.Bplus.CogByUser;
using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Caliper.Implementation;

namespace CVAiO.Bplus.CogByUser.Controls
{
    public partial class CogFindLineEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion
        public CogFindLineEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += CogFindLineEdit_SubjectChanged;
        }
        private void CogFindLineEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as CogFindLine;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CogFindLine Subject
        {
            get { return base.GetSubject() as CogFindLine; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(CogFindLine), null);
        }

        private void btnCogToolView_Click(object sender, EventArgs e)
        {
            CogToolEdit cogToolEdit = new CogToolEdit(new CogFindLineEditV2());
            (cogToolEdit.toolEditBase as CogFindLineEditV2).Subject = Subject.CogFindLineTool;
            cogToolEdit.ShowDialog();
            ToolProperty.Refresh();
        }
    }
}
