using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services.DataStructures;

namespace Municiple_Project_st10259527.ViewModels
{
    //===========================================================================
    // ViewModel for Service Request Status Page
    //===========================================================================
    public class ServiceRequestStatusViewModel
    {
        public int UserId { get; set; }
        public Basic<ServiceRequestModel> RequestsTree { get; set; }
        public MinHeap<int, ServiceRequestModel> PriorityHeap { get; set; }
        public RedBlack<int, (int Ahead, int Total)> QueueAheadIndex { get; set; }
    }
    //===========================================================================
}
