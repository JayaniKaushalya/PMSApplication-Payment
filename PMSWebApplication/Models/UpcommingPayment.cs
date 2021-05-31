using System;

namespace PMSWebApplication.Models
{
    //[System.Runtime.InteropServices.Guid("BECD704C-CA5F-4887-A4E2-22597990ACF3")]
    public class UpcommingPayment
    {
       
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public decimal? Payment { get; set; }
        public DateTime? Deadline { get; set; }
        public string TaskStages { get; set; }
    }
}