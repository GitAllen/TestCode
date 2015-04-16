using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMvcWebApiLoadTest.Models
{
    public class PostDataViewModel
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}