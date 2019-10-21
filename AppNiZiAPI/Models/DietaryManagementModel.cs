using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models
{
    public class DietaryManagementModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public bool IsActive { get; set; }
        public int PatientId { get; set; }
    }
}
