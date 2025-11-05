using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services.DataStructures;

namespace Municiple_Project_st10259527.ViewModels
{
    public class ServiceRequestStatusViewModel
    {
        public int UserId { get; set; }
        public BasicTree<ServiceRequestModel> RequestsTree { get; set; }
        public MinHeap<int, ServiceRequestModel> PriorityHeap { get; set; }
        public Graph<ServiceRequestModel> RelationshipGraph { get; set; }
    }
}
