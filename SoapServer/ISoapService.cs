using ConsoleApp1;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SoapServer
{
    [ServiceContract()]
    public interface ISoapService
    {
        [OperationContract]
        string Test(string s);
        [OperationContract]
        Task<TrainerResponse> GetTrainers();
        [OperationContract]
        Task<TrainerResponse> GetTrainer(int id);
        [OperationContract]
        Task<TrainerResponse> RegisterNewTrainer(Trainer trainer);
        [OperationContract]
        Task<TrainerResponse> UpdateTrainer(Trainer trainer);
        [OperationContract]
        Task<TrainerResponse> DeleteTrainer(int id);

    }
}
