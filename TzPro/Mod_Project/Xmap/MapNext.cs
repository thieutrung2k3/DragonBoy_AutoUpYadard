using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public struct MapNext
{
    // Token: 0x060009DE RID: 2526 RVA: 0x0000816A File Offset: 0x0000636A
    public MapNext(int mapID, TypeMapNext type, int[] info)
    {
        this.MapID = mapID;
        this.Type = type;
        this.Info = info;
    }

    // Token: 0x04001241 RID: 4673
    public int MapID;

    // Token: 0x04001242 RID: 4674
    public TypeMapNext Type;

    // Token: 0x04001243 RID: 4675
    public int[] Info;
}
}
