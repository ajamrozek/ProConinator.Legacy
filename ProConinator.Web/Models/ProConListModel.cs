using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProConinator.Web.Models
{
    public class ProConListModel
    {
        public string Title { get; set; }
        public IEnumerable<ProConItemModel> ProCons { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        public short SequenceId { get; set; }
    }
}