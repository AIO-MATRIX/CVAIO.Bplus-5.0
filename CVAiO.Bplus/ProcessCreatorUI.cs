using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus
{
    public partial class ProcessCreatorUI : Form
    {
        public ProcessCreatorUI()
        {
            InitializeComponent();
            processCreatorUI1.HalconMVTec = true;
            processCreatorUI1.CognexVisionPro = true;
        }
    }
}
