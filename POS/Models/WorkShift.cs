using System;

namespace POS.Models
{
    /// <summary>
    /// WorkShift Model
    /// </summary>
    public class WorkShift
    {
        public int ShiftID { get; set; }
        public int EmployeeID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime WorkDate { get; set; }
    }
}
