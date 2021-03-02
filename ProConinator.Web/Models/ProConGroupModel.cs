using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProConinator.Web.Models
{
    public class ProConGroupModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<ProConListModel> ProConLists { get; set; }

        public DateTime ModifiedDate { get; set; }


    }
}