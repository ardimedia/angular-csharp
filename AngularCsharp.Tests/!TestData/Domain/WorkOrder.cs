using System;

namespace AngularCSharp.Tests._TestData.Domain
{
    public class WorkOrder
    {
        #region Public Properties

        public string NestleOrderNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddressCity { get; set; }

        #endregion

        #region Public Methods

        private static int globalId = 0;

        public static WorkOrder GetValid(int id, int year = 2016)
        {
            id = globalId + id;
            WorkOrder workOrder = new WorkOrder();
            workOrder.NestleOrderNo = $"{year}-{globalId}-{globalId}";
            workOrder.CustomerName = $"Customer Name {globalId}";
            workOrder.CustomerAddressCity = $"Customer Address City {globalId}";
            return workOrder;
        }

        #endregion
    }
}
