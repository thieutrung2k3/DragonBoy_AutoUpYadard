using AssemblyCSharp.Xmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public class XmapController : IActionListener
{
    // Token: 0x060009F4 RID: 2548 RVA: 0x00087358 File Offset: 0x00085558
    public static void Update()
    {
        if (XmapController.IsWaiting() || XmapData.Instance().IsLoading)
        {
            return;
        }
        if (XmapController.IsWaitNextMap)
        {
            XmapController.Wait(200);
            XmapController.IsWaitNextMap = false;
            return;
        }
        if (XmapController.IsNextMapFailed)
        {
            XmapData.Instance().MyLinkMaps = null;
            XmapController.WayXmap = null;
            XmapController.IsNextMapFailed = false;
            return;
        }
        if (XmapController.WayXmap == null)
        {
            if (XmapData.Instance().MyLinkMaps == null)
            {
                XmapData.Instance().LoadLinkMaps();
                return;
            }
            XmapController.WayXmap = XmapAlgorithm.FindWay(TileMap.mapID, XmapController.IdMapEnd);
            XmapController.IndexWay = 0;
            if (XmapController.WayXmap == null)
            {
                GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
                XmapController.FinishXmap();
                return;
            }
        }
        if (TileMap.mapID == XmapController.WayXmap[XmapController.WayXmap.Count - 1] && !XmapData.IsMyCharDie())
        {
            GameScr.info1.addInfo("Đã đến: " + TileMap.mapNames[TileMap.mapID], 0);
            XmapController.FinishXmap();
            return;
        }
        if (TileMap.mapID == XmapController.WayXmap[XmapController.IndexWay])
        {
            if (XmapData.IsMyCharDie())
            {
                Service.gI().returnTownFromDead();
                XmapController.IsWaitNextMap = (XmapController.IsNextMapFailed = true);
            }
            else if (XmapData.CanNextMap())
            {
                XmapController.NextMap(XmapController.WayXmap[XmapController.IndexWay + 1]);
                XmapController.IsWaitNextMap = true;
            }
            XmapController.Wait(500);
            return;
        }
        if (TileMap.mapID == XmapController.WayXmap[XmapController.IndexWay + 1])
        {
            XmapController.IndexWay++;
            return;
        }
        XmapController.IsNextMapFailed = true;
    }

    // Token: 0x060009F5 RID: 2549 RVA: 0x000082A2 File Offset: 0x000064A2
    public void perform(int idAction, object p)
    {
        if (idAction == 1)
        {
            XmapController.ShowPanelXmap((List<int>)p);
        }
    }

    // Token: 0x060009F6 RID: 2550 RVA: 0x000082B3 File Offset: 0x000064B3
    private static void Wait(int time)
    {
        XmapController.IsWait = true;
        XmapController.TimeStartWait = mSystem.currentTimeMillis();
        XmapController.TimeWait = (long)time;
    }

    // Token: 0x060009F7 RID: 2551 RVA: 0x000082CC File Offset: 0x000064CC
    private static bool IsWaiting()
    {
        if (XmapController.IsWait && mSystem.currentTimeMillis() - XmapController.TimeStartWait >= XmapController.TimeWait)
        {
            XmapController.IsWait = false;
        }
        return XmapController.IsWait;
    }

    // Token: 0x060009F8 RID: 2552 RVA: 0x000874E0 File Offset: 0x000856E0
    public static void ShowXmapMenu()
    {
        XmapData.Instance().LoadGroupMapsFromFile("TextData\\GroupMapsXmap.txt");
        MyVector myVector = new MyVector();
        foreach (GroupMap groupMap in XmapData.Instance().GroupMaps)
        {
            myVector.addElement(new Command(groupMap.NameGroup, XmapController._Instance, 1, groupMap.IdMaps));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    // Token: 0x060009F9 RID: 2553 RVA: 0x00087570 File Offset: 0x00085770
    public static void ShowPanelXmap(List<int> idMaps)
    {
        Pk9rXmap.IsMapTransAsXmap = true;
        int count = idMaps.Count;
        GameCanvas.panel.mapNames = new string[count];
        GameCanvas.panel.planetNames = new string[count];
        for (int i = 0; i < count; i++)
        {
            string str = TileMap.mapNames[idMaps[i]];
            GameCanvas.panel.mapNames[i] = idMaps[i].ToString() + ": " + str;
            GameCanvas.panel.planetNames[i] = "";
        }
        GameCanvas.panel.setTypeMapTrans();
        GameCanvas.panel.show();
    }

    // Token: 0x060009FA RID: 2554 RVA: 0x000082F2 File Offset: 0x000064F2
    public static void StartRunToMapId(int idMap)
    {
        XmapController.IdMapEnd = idMap;
        Pk9rXmap.IsXmapRunning = true;
    }

    // Token: 0x060009FB RID: 2555 RVA: 0x00008300 File Offset: 0x00006500
    public static void FinishXmap()
    {
        Pk9rXmap.IsXmapRunning = false;
        XmapController.IsNextMapFailed = false;
        XmapData.Instance().MyLinkMaps = null;
        XmapController.WayXmap = null;
    }

    // Token: 0x060009FC RID: 2556 RVA: 0x0000831F File Offset: 0x0000651F
    public static void SaveIdMapCapsuleReturn()
    {
        Pk9rXmap.IdMapCapsuleReturn = TileMap.mapID;
    }

    // Token: 0x060009FD RID: 2557 RVA: 0x00087610 File Offset: 0x00085810
    private static void NextMap(int idMapNext)
    {
        List<MapNext> mapNexts = XmapData.Instance().GetMapNexts(TileMap.mapID);
        if (mapNexts != null)
        {
            foreach (MapNext mapNext in mapNexts)
            {
                if (mapNext.MapID == idMapNext)
                {
                    XmapController.NextMap(mapNext);
                    return;
                }
            }
        }
        GameScr.info1.addInfo("Lỗi tại dữ liệu", 0);
    }

    // Token: 0x060009FE RID: 2558 RVA: 0x0008768C File Offset: 0x0008588C
    private static void NextMap(MapNext mapNext)
    {
        switch (mapNext.Type)
        {
            case TypeMapNext.AutoWaypoint:
                XmapController.NextMapAutoWaypoint(mapNext);
                return;
            case TypeMapNext.NpcMenu:
                XmapController.NextMapNpcMenu(mapNext);
                return;
            case TypeMapNext.NpcPanel:
                XmapController.NextMapNpcPanel(mapNext);
                return;
            case TypeMapNext.Position:
                XmapController.NextMapPosition(mapNext);
                return;
            case TypeMapNext.Capsule:
                XmapController.NextMapCapsule(mapNext);
                return;
            default:
                return;
        }
    }

    // Token: 0x060009FF RID: 2559 RVA: 0x000876E0 File Offset: 0x000858E0
    private static void NextMapAutoWaypoint(MapNext mapNext)
    {
        Waypoint waypoint = XmapData.FindWaypoint(mapNext.MapID);
        if (waypoint != null)
        {
            int posWaypointX = XmapData.GetPosWaypointX(waypoint);
            int posWaypointY = XmapData.GetPosWaypointY(waypoint);
            XmapController.MoveMyChar(posWaypointX, posWaypointY);
            XmapController.RequestChangeMap(waypoint);
        }
    }
    public static void findCalich()
    {
        if (TileMap.mapID == 27)
        {
            XmapController.NextMap(28);
            XmapController.IsWaitNextMap = true;
            XmapController.step = 0;
            return;
        }
        if (TileMap.mapID == 29)
        {
            XmapController.NextMap(28);
            XmapController.IsWaitNextMap = true;
            XmapController.step = 1;
            return;
        }
        if (XmapController.step == 0)
        {
            XmapController.NextMap(29);
            XmapController.IsWaitNextMap = true;
            return;
        }
        if (XmapController.step == 1)
        {
            XmapController.NextMap(27);
            XmapController.IsWaitNextMap = true;
        }
    }
    private static int step;

    // Token: 0x06000A00 RID: 2560 RVA: 0x00087718 File Offset: 0x00085918
    private static void NextMapNpcMenu(MapNext mapNext)
    {
        int num = mapNext.Info[0];
        if (GameScr.findNPCInMap((short)num) == null)
        {
            XmapController.findCalich();
            return;
        }
        Service.gI().openMenu(num);
        for (int i = 1; i < mapNext.Info.Length; i++)
        {
            int num2 = mapNext.Info[i];
            Service.gI().confirmMenu((short)num, (sbyte)num2);
        }
    }

    // Token: 0x06000A01 RID: 2561 RVA: 0x00087764 File Offset: 0x00085964
    private static void NextMapNpcPanel(MapNext mapNext)
    {
        int num = mapNext.Info[0];
        int num2 = mapNext.Info[1];
        int selected = mapNext.Info[2];
        Service.gI().openMenu(num);
        Service.gI().confirmMenu((short)num, (sbyte)num2);
        Service.gI().requestMapSelect(selected);
    }

    // Token: 0x06000A02 RID: 2562 RVA: 0x000877B0 File Offset: 0x000859B0
    private static void NextMapPosition(MapNext mapNext)
    {
        int x = mapNext.Info[0];
        int y = mapNext.Info[1];
        XmapController.MoveMyChar(x, y);
        Service.gI().requestChangeMap();
        Service.gI().getMapOffline();
    }

    // Token: 0x06000A03 RID: 2563 RVA: 0x000877E8 File Offset: 0x000859E8
    private static void NextMapCapsule(MapNext mapNext)
    {
        XmapController.SaveIdMapCapsuleReturn();
        int selected = mapNext.Info[0];
        Service.gI().requestMapSelect(selected);
    }

    // Token: 0x06000A04 RID: 2564 RVA: 0x0000832B File Offset: 0x0000652B
    public static void UseCapsuleNormal()
    {
        Pk9rXmap.IsShowPanelMapTrans = false;
        Service.gI().useItem(0, 1, -1, 193);
    }

    // Token: 0x06000A05 RID: 2565 RVA: 0x00008345 File Offset: 0x00006545
    public static void UseCapsuleVip()
    {
        Pk9rXmap.IsShowPanelMapTrans = false;
        Service.gI().useItem(0, 1, -1, 194);
    }

    // Token: 0x06000A06 RID: 2566 RVA: 0x0000835F File Offset: 0x0000655F
    public static void HideInfoDlg()
    {
        InfoDlg.hide();
    }

    // Token: 0x06000A07 RID: 2567 RVA: 0x000414C4 File Offset: 0x0003F6C4
    private static void MoveMyChar(int x, int y)
    {
        global::Char.myCharz().cx = x;
        global::Char.myCharz().cy = y;
        Service.gI().charMove();
        if (!ItemTime.isExistItem(4387))
        {
            global::Char.myCharz().cx = x;
            global::Char.myCharz().cy = y + 1;
            Service.gI().charMove();
            global::Char.myCharz().cx = x;
            global::Char.myCharz().cy = y;
            Service.gI().charMove();
        }
    }

    // Token: 0x06000A08 RID: 2568 RVA: 0x00008366 File Offset: 0x00006566
    private static void RequestChangeMap(Waypoint waypoint)
    {
        if (waypoint.isOffline)
        {
            Service.gI().getMapOffline();
            return;
        }
        Service.gI().requestChangeMap();
    }

    // Token: 0x04001250 RID: 4688
    private const int TIME_DELAY_NEXTMAP = 200;

    // Token: 0x04001251 RID: 4689
    private const int TIME_DELAY_RENEXTMAP = 500;

    // Token: 0x04001252 RID: 4690
    private const int ID_ITEM_CAPSULE_VIP = 194;

    // Token: 0x04001253 RID: 4691
    private const int ID_ITEM_CAPSULE = 193;

    // Token: 0x04001254 RID: 4692
    private const int ID_ICON_ITEM_TDLT = 4387;

    // Token: 0x04001255 RID: 4693
    private static readonly XmapController _Instance = new XmapController();

    // Token: 0x04001256 RID: 4694
    private static int IdMapEnd;

    // Token: 0x04001257 RID: 4695
    private static List<int> WayXmap;

    // Token: 0x04001258 RID: 4696
    private static int IndexWay;

    // Token: 0x04001259 RID: 4697
    private static bool IsNextMapFailed;

    // Token: 0x0400125A RID: 4698
    private static bool IsWait;

    // Token: 0x0400125B RID: 4699
    private static long TimeStartWait;

    // Token: 0x0400125C RID: 4700
    private static long TimeWait;

    // Token: 0x0400125D RID: 4701
    private static bool IsWaitNextMap;
}
}
