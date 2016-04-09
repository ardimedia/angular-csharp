using System;
using System.Collections.Generic;

namespace AngularCSharp.Tests._TestData.Domain
{
    public class PrintApproval
    {
        #region Public Properties

        public WorkOrder WorkOrder { get; set; }

        public DateTimeOffset RecordModifiedAt { get; set; }

        public string CustomerHintFromControllCenter { get; set; }

        public string SalesRepHintFromControllCenter { get; set; }

        public string CustomerComment { get; set; }

        #endregion

        #region Public Methods

        public static PrintApproval GetRandom()
        {
            PrintApproval printApproval = new PrintApproval();
            printApproval.WorkOrder = WorkOrder.GetRandom();
            printApproval.RecordModifiedAt = DateTimeOffset.Now;

            printApproval.CustomerHintFromControllCenter = $"Customer Hint From Controll Center {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            printApproval.SalesRepHintFromControllCenter = $"Sales Rep Hint From Controll Center {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            printApproval.CustomerComment = $"Customer Comment {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            return printApproval;
        }

        public static List<PrintApproval> GetRandoms(int count)
        {
            List<PrintApproval> printApprovals = new List<PrintApproval>();

            for (int i = 0; i < count; i++)
            {
                printApprovals.Add(PrintApproval.GetRandom());
            }

            return printApprovals;
        }

        #endregion
    }
}
