using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLibrary
{
    public class Job
    {
        public string pythonSrc { get;  set; } // src code entered by user on display
        public string pythonRes { get; set; } // stored result of the python code
        public int jobNum { get; set; } // index in job list for when posting job entry info with result

        public bool processed { get; set; } // check if input has already been interpreted
        public byte[] hash { get; set; } // encode and decode when sending over server

        public Job()
        {

        }
    }
}
