using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace JobLibrary
{
    [ServiceContract]
    public interface ClientInterface
    {
        [OperationContract]
        Job RequestJob();

        [OperationContract]
        void UploadJobSolution(string res, int index);
    }
}
