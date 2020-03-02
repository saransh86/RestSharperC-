using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTests
{
    public class ToDo
    {
        public string uid { get; set; }
        public string user { get; set; }
        public string title { get; set; }
        public string body { get; set; }



        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
        public bool? Checked  {get;set;}

        public string Message { get; set; }

    }
}
