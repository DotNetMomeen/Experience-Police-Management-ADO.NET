using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Experience_Police_Management_System
{
    public partial class PoliceReport : Form
    {
        public PoliceReport(List<ViewModel.PoliceViewModel> policeList)
        {
            InitializeComponent();
        }
    }
}
