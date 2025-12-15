using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.DTOs
{
    public class ZaloPayCreateOrderResponse
    {
        public int return_code { get; set; }
        public string return_message { get; set; }
        public string sub_return_message { get; set; }
        public string order_url { get; set; }
        public string zp_trans_token { get; set; }
    }

}
