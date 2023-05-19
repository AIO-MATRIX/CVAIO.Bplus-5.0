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
