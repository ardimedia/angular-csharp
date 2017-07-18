using System;
using AngularCsharp.Tests.Properties;

namespace AngularCsharp.Tests.TestData.Domain
{
    public class UseCase1
    {
        public static object GetObjectFromJson()
        {
            var anonymousType = new
            {
                salesAgent = new
                {
                    FullName = "",
                    EmailAddress = ""
                },
                customerPrintApprovals = new[] { new { } },
                salesRepPrintApprovals = new[] {
                    new {
                            CustomerComment = "",
                            CustomerHintFromControllCenter = "",
                            SalesRepHintFromControllCenter = "",
                            WorkOrder = new
                            {
                                CustomerName = "",
                                CustomerAddressCity = "",
                                NestleOrderNo = "",
                                RecordModifiedAt = "" //DateTimeOffset.Now
                            }
                        }
                },
                isCustomerPrintApproval = false,
                isSalesRepPrintApproval = true
            };

            string json = Resources.usecase1_json;
            return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, anonymousType);
        }
    }
}
