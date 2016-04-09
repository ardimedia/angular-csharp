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

        public static WorkOrder GetRandom()
        {
            WorkOrder workOrder = new WorkOrder();
            workOrder.NestleOrderNo = $"{DateTime.Now.Year}-{new Random(DateTime.Now.Millisecond).Next(0,999)}-{new Random(DateTime.Now.Millisecond).Next(0, 999999)}";
            workOrder.CustomerName = $"Customer Name {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            workOrder.CustomerAddressCity = $"Customer Address City {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            return workOrder;
        }

        #endregion
    }
}
