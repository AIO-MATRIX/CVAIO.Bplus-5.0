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

namespace CVAiO.Bplus.Algorithm.Controls
{
    public partial class AlgoObjectAlign1PEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public AlgoObjectAlign1PEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoObjectAlign1PEdit_SubjectChanged;
        }
        protected override void btnRun_Click(object sender, EventArgs e)
        {
            base.btnRun_Click(sender, e);
            ToolProperty.Refresh();
            ToolProperty.ExpandAllGridItems();
        }
        private void AlgoObjectAlign1PEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoObjectAlign1P;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoObjectAlign1P Subject
        {
            get { return base.GetSubject() as AlgoObjectAlign1P; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoObjectAlign1P), null);
        }

        private void btnSetMaster_Click(object sender, EventArgs e)
        {
            Subject.RunParams.LoadPositionX = Subject.AlgoPosition.X;
            Subject.RunParams.LoadPositionY = Subject.AlgoPosition.Y;
            Subject.RunParams.LoadPositionT = (float)Subject.AlgoPosition.ThetaRad;
            ToolProperty.Refresh();
        }
    }
}
