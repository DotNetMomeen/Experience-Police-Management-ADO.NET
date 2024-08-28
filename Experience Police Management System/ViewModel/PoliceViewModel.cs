using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience_Police_Management_System.ViewModel
{
    public class PoliceViewModel
    {
        public PoliceViewModel()
        {

        }

        public PoliceViewModel(List<PoliceViewModel> list)
        {

        }

        public int PoliceId { get; set; }
        public string PoliceCode { get; set; }
        public string PoliceName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool IsPermanent { get; set; }
        public int TotalExperience { get; set; }
        public string DesignationTitle { get; set; }
        public string ImagePath { get; set; }
    }
}
