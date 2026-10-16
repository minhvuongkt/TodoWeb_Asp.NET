using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21T1020851_Todo.DomainModels
{
    public class TodoTask
    {
        public int TaskID { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public bool IsFinished { get; set; }
        public int UserID { get; set; }
        public DateTime TimeStart { get; set; } = DateTime.Now;
        public DateTime TimeEnd { get; set; } = DateTime.Now;
    }
}
