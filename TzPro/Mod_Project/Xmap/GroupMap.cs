using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public struct GroupMap
    {
        public GroupMap(string nameGroup, List<int> idMaps)
        {
            this.NameGroup = nameGroup;
            this.IdMaps = idMaps;
        }

       
        public string NameGroup;
        public List<int> IdMaps;
    }
}
