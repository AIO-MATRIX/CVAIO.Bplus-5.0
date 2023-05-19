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
    public partial class AlgoTapeAlign2P_Object2TargetEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public AlgoTapeAlign2P_Object2TargetEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoTapeAlign2P_Object2TargetEdit_SubjectChanged;
        }
        protected override void btnRun_Click(object sender, EventArgs e)
        {
            base.btnRun_Click(sender, e);
            ToolProperty.Refresh();
            ToolProperty.ExpandAllGridItems();
        }
        private void AlgoTapeAlign2P_Object2TargetEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoTapeAlign2P_Object2Target;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoTapeAlign2P_Object2Target Subject
        {
            get { return base.GetSubject() as AlgoTapeAlign2P_Object2Target; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoTapeAlign2P_Object2Target), null);
        }

        private void btnSetMaster_Click(object sender, EventArgs e)
        {
            //Subject.RunParams.LoadPositionX = Subject.AlgoPosition.X;
            //Subject.RunParams.LoadPositionY = Subject.AlgoPosition.Y;
            //Subject.RunParams.LoadPositionT = (float)Subject.AlgoPosition.ThetaRad;
            ToolProperty.Refresh();
        }
    }
}
