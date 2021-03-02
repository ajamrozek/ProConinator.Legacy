using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProConinator.Web.Models
{
    /// <summary>
    /// it's an enum as a bool. so, what? 
    /// </summary>
    public enum ProCon
    {
        Pro = 0,
        Con = 1
    }
    public class ProConItemModel
    {
        public int Id { get; set; }
        public ProCon ProCon { get; set; }
        public string Text { get; set; }
        public short Weight { get; set; }
    }
}