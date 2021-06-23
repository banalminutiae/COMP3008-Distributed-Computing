using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
 

namespace JobLibrary
{
   [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
   public class JobServer : ClientInterface
    {
        // returns job containing python data to be interpreted and displayed
        public Job RequestJob()
        {
            Job newJob = new Job();
            foreach (Job job in JobList.jobList)
            {
                newJob = job;
                break;
            }
            return newJob;
        }

        // once a job is done, update its data in the joblist with the result
        // probably get rid of this, less efficient than just tossing the finished job
        // onto a new list and deleting from original job list
        public void UploadJobSolution(string res, int index)
        {
            for (int i = 0; i < JobList.jobList.Count; i ++)
            {
                if (JobList.jobList.ElementAt(i).jobNum == index)
                {
                    JobList.jobList.ElementAt(i).pythonRes = res;
                    break;
                }
            }
        }

    }
}
