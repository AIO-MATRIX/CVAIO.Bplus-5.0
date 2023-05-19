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
    public partial class HDataCode2DEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        private ESymbolType symbolType;
        private EDefaultParameters defaultParameters;
        #endregion

        #region Properties

        public ESymbolType SymbolType
        {
            get => symbolType;
            set
            {
                if (symbolType == value) return;
                symbolType = value;
                this.Subject_Ran(this, null);
            }
        }
        public EDefaultParameters DefaultParameters
        {
            get => defaultParameters;
            set
            {
                if (defaultParameters == value) return;
                defaultParameters = value;
                this.Subject_Ran(this, null);
            }
        }

        #endregion

        public HDataCode2DEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += HDataCode2DEdit_SubjectChanged;
        }

        private void HDataCode2DEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            Subject.Ran += Subject_Ran;
            ToolProperty.SelectedObject = Subject as HDataCode2D;
        }
        private void Subject_Ran(object sender, ToolBase tool)
        {
            try
            {
                if (Subject.RunParams.Parameters == null || Subject.RunParams.Parameters.Count == 0) return;
                gvPoints.Rows.Clear();
                for (int i = 0; i < Subject.RunParams.Parameters.Keys.ToList().Count; i++)
                {
                    string key = Subject.RunParams.Parameters.Keys.ToList()[i];
                    gvPoints.Rows.Add(
                        i.ToString(),
                        key,
                        Subject.RunParams.Parameters[key]);
                }
            }
            catch { }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HDataCode2D Subject
        {
            get { return base.GetSubject() as HDataCode2D; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(HDataCode2D), null);
            this.DataBindings.Add("SymbolType", source, "RunParams.SymbolType", true, DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("DefaultParameters", source, "RunParams.DefaultParameters", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
