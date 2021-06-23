using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// static model list of clients for the server and networking threads 
namespace JobLibrary
{
    public static class JobList
    {
        public static List<Job> jobList = new List<Job>();
    }
}
