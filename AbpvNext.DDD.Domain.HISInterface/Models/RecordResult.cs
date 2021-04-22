using System;
using System.Collections.Generic;
using System.Text;

namespace AbpvNext.DDD.Domain.HISInterface.Models
{
   public class RecordResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
