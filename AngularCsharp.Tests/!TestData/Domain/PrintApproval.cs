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

        private static int globalId = 0;

        public static PrintApproval GetValid(int id = 1)
        {
            globalId = globalId + id;
            DateTimeOffset recordModifiedAt = new DateTimeOffset(2016, 1, 2, 13, 14, 15, new TimeSpan());

            PrintApproval printApproval = new PrintApproval();
            printApproval.WorkOrder = WorkOrder.GetValid(id);
            printApproval.RecordModifiedAt = recordModifiedAt;

            printApproval.CustomerHintFromControllCenter = $"Customer Hint From Controll Center {globalId}";
            printApproval.SalesRepHintFromControllCenter = $"Sales Rep Hint From Controll Center {globalId}";
            printApproval.CustomerComment = $"Customer Comment {globalId}";
            return printApproval;
        }

        public static List<PrintApproval> GetValidItems(int count, int id = 1)
        {
            List<PrintApproval> printApprovals = new List<PrintApproval>();

            for (int i = 0; i < count; i++)
            {
                printApprovals.Add(PrintApproval.GetValid(id));
            }

            return printApprovals;
        }

        #endregion
    }
}
