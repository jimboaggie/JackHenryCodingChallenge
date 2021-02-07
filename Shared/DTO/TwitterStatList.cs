using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStats.Shared.DTO
{
    public class TwitterStatList
    {
        public TwitterStatList()
        {
            StatList = new HashSet<TwitterStatItem>();
        }
        public virtual IEnumerable<TwitterStatItem> StatList { get; set; }
    }
}
