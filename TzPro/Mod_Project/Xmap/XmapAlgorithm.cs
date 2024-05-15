using AssemblyCSharp.Xmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public class XmapAlgorithm
{
    // Token: 0x060009EB RID: 2539 RVA: 0x00087190 File Offset: 0x00085390
    public static List<int> FindWay(int idMapStart, int idMapEnd)
    {
        List<int> wayPassedStart = XmapAlgorithm.GetWayPassedStart(idMapStart);
        return XmapAlgorithm.FindWay(idMapEnd, wayPassedStart);
    }

    // Token: 0x060009EC RID: 2540 RVA: 0x000871AC File Offset: 0x000853AC
    private static List<int> FindWay(int idMapEnd, List<int> wayPassed)
    {
        int num = wayPassed[wayPassed.Count - 1];
        if (num == idMapEnd)
        {
            return wayPassed;
        }
        if (!XmapData.Instance().CanGetMapNexts(num))
        {
            return null;
        }
        List<List<int>> list = new List<List<int>>();
        foreach (MapNext mapNext in XmapData.Instance().GetMapNexts(num))
        {
            List<int> list2 = null;
            if (!wayPassed.Contains(mapNext.MapID))
            {
                List<int> wayPassedNext = XmapAlgorithm.GetWayPassedNext(wayPassed, mapNext.MapID);
                list2 = XmapAlgorithm.FindWay(idMapEnd, wayPassedNext);
            }
            if (list2 != null)
            {
                list.Add(list2);
            }
        }
        return XmapAlgorithm.GetBestWay(list);
    }

    // Token: 0x060009ED RID: 2541 RVA: 0x00087264 File Offset: 0x00085464
    private static List<int> GetBestWay(List<List<int>> ways)
    {
        if (ways.Count == 0)
        {
            return null;
        }
        List<int> list = ways[0];
        for (int i = 1; i < ways.Count; i++)
        {
            if (XmapAlgorithm.IsWayBetter(ways[i], list))
            {
                list = ways[i];
            }
        }
        return list;
    }

    // Token: 0x060009EE RID: 2542 RVA: 0x0000827D File Offset: 0x0000647D
    private static List<int> GetWayPassedStart(int idMapStart)
    {
        return new List<int>
            {
                idMapStart
            };
    }

    // Token: 0x060009EF RID: 2543 RVA: 0x0000828B File Offset: 0x0000648B
    private static List<int> GetWayPassedNext(List<int> wayPassed, int idMapNext)
    {
        return new List<int>(wayPassed)
            {
                idMapNext
            };
    }

    // Token: 0x060009F0 RID: 2544 RVA: 0x000872AC File Offset: 0x000854AC
    private static bool IsWayBetter(List<int> way1, List<int> way2)
    {
        bool flag = XmapAlgorithm.IsBadWay(way1);
        bool flag2 = XmapAlgorithm.IsBadWay(way2);
        return (!flag || flag2) && ((!flag && flag2) || way1.Count < way2.Count);
    }

    // Token: 0x060009F1 RID: 2545 RVA: 0x0000829A File Offset: 0x0000649A
    private static bool IsBadWay(List<int> way)
    {
        return XmapAlgorithm.IsWayGoFutureAndBack(way);
    }

    // Token: 0x060009F2 RID: 2546 RVA: 0x000872EC File Offset: 0x000854EC
    private static bool IsWayGoFutureAndBack(List<int> way)
    {
        List<int> list = new List<int>
            {
                27,
                28,
                29
            };
        for (int i = 1; i < way.Count - 1; i++)
        {
            if (way[i] == 102 && way[i + 1] == 24 && list.Contains(way[i - 1]))
            {
                return true;
            }
        }
        return false;
    }
}
}
