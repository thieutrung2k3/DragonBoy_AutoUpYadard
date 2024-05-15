using AssemblyCSharp.Xmap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public class XmapData
{
    // Token: 0x06000A0B RID: 2571 RVA: 0x00008391 File Offset: 0x00006591
    private XmapData()
    {
        this.GroupMaps = new List<GroupMap>();
        this.MyLinkMaps = null;
        this.IsLoading = false;
        this.IsLoadingCapsule = false;
    }

    // Token: 0x06000A0C RID: 2572 RVA: 0x000083B9 File Offset: 0x000065B9
    public static XmapData Instance()
    {
        if (XmapData._Instance == null)
        {
            XmapData._Instance = new XmapData();
        }
        return XmapData._Instance;
    }

    // Token: 0x06000A0D RID: 2573 RVA: 0x000083D1 File Offset: 0x000065D1
    public void LoadLinkMaps()
    {
        this.IsLoading = true;
    }

    // Token: 0x06000A0E RID: 2574 RVA: 0x00087810 File Offset: 0x00085A10
    public void Update()
    {
        if (this.IsLoadingCapsule)
        {
            if (!this.IsWaitInfoMapTrans())
            {
                this.LoadLinkMapCapsule();
                this.IsLoadingCapsule = false;
                this.IsLoading = false;
            }
            return;
        }
        this.LoadLinkMapBase();
        if (XmapData.CanUseCapsuleVip())
        {
            XmapController.UseCapsuleVip();
            this.IsLoadingCapsule = true;
            return;
        }
        if (XmapData.CanUseCapsuleNormal())
        {
            XmapController.UseCapsuleNormal();
            this.IsLoadingCapsule = true;
            return;
        }
        this.IsLoading = false;
    }

    // Token: 0x06000A0F RID: 2575 RVA: 0x00087878 File Offset: 0x00085A78
    public void LoadGroupMapsFromFile(string path)
    {
        this.GroupMaps.Clear();
        try
        {
            StreamReader streamReader = new StreamReader(path);
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                text = text.Trim();
                if (!text.StartsWith("#") && !text.Equals(""))
                {
                    List<int> idMaps = Array.ConvertAll<string, int>(streamReader.ReadLine().Trim().Split(new char[]
                    {
                            ' '
                    }), (string s) => int.Parse(s)).ToList<int>();
                    this.GroupMaps.Add(new GroupMap(text, idMaps));
                }
            }
        }
        catch (Exception ex)
        {
            GameScr.info1.addInfo(ex.Message, 0);
        }
        this.RemoveMapsHomeInGroupMaps();
    }

    // Token: 0x06000A10 RID: 2576 RVA: 0x0008794C File Offset: 0x00085B4C
    private void RemoveMapsHomeInGroupMaps()
    {
        int cgender = global::Char.myCharz().cgender;
        foreach (GroupMap groupMap in this.GroupMaps)
        {
            if (cgender != 0)
            {
                if (cgender != 1)
                {
                    groupMap.IdMaps.Remove(21);
                    groupMap.IdMaps.Remove(22);
                }
                else
                {
                    groupMap.IdMaps.Remove(21);
                    groupMap.IdMaps.Remove(23);
                }
            }
            else
            {
                groupMap.IdMaps.Remove(22);
                groupMap.IdMaps.Remove(23);
            }
        }
    }

    // Token: 0x06000A11 RID: 2577 RVA: 0x00087A04 File Offset: 0x00085C04
    private void LoadLinkMapCapsule()
    {
        this.AddKeyLinkMaps(TileMap.mapID);
        string[] mapNames = GameCanvas.panel.mapNames;
        for (int i = 0; i < mapNames.Length; i++)
        {
            int idMapFromName = XmapData.GetIdMapFromName(mapNames[i]);
            if (idMapFromName != -1)
            {
                int[] info = new int[]
                {
                        i
                };
                this.MyLinkMaps[TileMap.mapID].Add(new MapNext(idMapFromName, TypeMapNext.Capsule, info));
            }
        }
    }

    // Token: 0x06000A12 RID: 2578 RVA: 0x000083DA File Offset: 0x000065DA
    private void LoadLinkMapBase()
    {
        this.MyLinkMaps = new Dictionary<int, List<MapNext>>();
        this.LoadLinkMapsFromFile("TextData\\LinkMapsXmap.txt");
        this.LoadLinkMapsAutoWaypointFromFile("TextData\\AutoLinkMapsWaypoint.txt");
        this.LoadLinkMapsHome();
        this.LoadLinkMapSieuThi();
        this.LoadLinkMapToCold();
    }

    // Token: 0x06000A13 RID: 2579 RVA: 0x00087A6C File Offset: 0x00085C6C
    private void LoadLinkMapsFromFile(string path)
    {
        try
        {
            StreamReader streamReader = new StreamReader(path);
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                text = text.Trim();
                if (!text.StartsWith("#") && !text.Equals(""))
                {
                    int[] array = Array.ConvertAll<string, int>(text.Split(new char[]
                    {
                            ' '
                    }), (string s) => int.Parse(s));
                    int num = array.Length - 3;
                    int[] array2 = new int[num];
                    Array.Copy(array, 3, array2, 0, num);
                    this.LoadLinkMap(array[0], array[1], (TypeMapNext)array[2], array2);
                }
            }
        }
        catch (Exception ex)
        {
            GameScr.info1.addInfo(ex.Message, 0);
        }
    }

    // Token: 0x06000A14 RID: 2580 RVA: 0x00087B3C File Offset: 0x00085D3C
    private void LoadLinkMapsAutoWaypointFromFile(string path)
    {
        try
        {
            StreamReader streamReader = new StreamReader(path);
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                text = text.Trim();
                if (!text.StartsWith("#") && !text.Equals(""))
                {
                    int[] array = Array.ConvertAll<string, int>(text.Split(new char[]
                    {
                            ' '
                    }), (string s) => int.Parse(s));
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i != 0)
                        {
                            this.LoadLinkMap(array[i], array[i - 1], TypeMapNext.AutoWaypoint, null);
                        }
                        if (i != array.Length - 1)
                        {
                            this.LoadLinkMap(array[i], array[i + 1], TypeMapNext.AutoWaypoint, null);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            GameScr.info1.addInfo(ex.Message, 0);
        }
    }

    // Token: 0x06000A15 RID: 2581 RVA: 0x00087C1C File Offset: 0x00085E1C
    private void LoadLinkMapsHome()
    {
        int cgender = global::Char.myCharz().cgender;
        int num = 21 + cgender;
        int num2 = 7 * cgender;
        this.LoadLinkMap(num2, num, TypeMapNext.AutoWaypoint, null);
        this.LoadLinkMap(num, num2, TypeMapNext.AutoWaypoint, null);
    }

    // Token: 0x06000A16 RID: 2582 RVA: 0x00087C54 File Offset: 0x00085E54
    private void LoadLinkMapSieuThi()
    {
        int cgender = global::Char.myCharz().cgender;
        int idMapNext = 24 + cgender;
        int[] array = new int[2];
        array[0] = 10;
        int[] info = array;
        this.LoadLinkMap(84, idMapNext, TypeMapNext.NpcMenu, info);
    }

    // Token: 0x06000A17 RID: 2583 RVA: 0x00087C88 File Offset: 0x00085E88
    private void LoadLinkMapToCold()
    {
        if (global::Char.myCharz().taskMaint.taskId > 30)
        {
            int[] array = new int[2];
            array[0] = 12;
            int[] info = array;
            this.LoadLinkMap(19, 109, TypeMapNext.NpcMenu, info);
        }
    }

    // Token: 0x06000A18 RID: 2584 RVA: 0x0000840F File Offset: 0x0000660F
    public List<MapNext> GetMapNexts(int idMap)
    {
        if (this.CanGetMapNexts(idMap))
        {
            return this.MyLinkMaps[idMap];
        }
        return null;
    }

    // Token: 0x06000A19 RID: 2585 RVA: 0x00008428 File Offset: 0x00006628
    public bool CanGetMapNexts(int idMap)
    {
        return this.MyLinkMaps.ContainsKey(idMap);
    }

    // Token: 0x06000A1A RID: 2586 RVA: 0x00087CC0 File Offset: 0x00085EC0
    private void LoadLinkMap(int idMapStart, int idMapNext, TypeMapNext type, int[] info)
    {
        this.AddKeyLinkMaps(idMapStart);
        MapNext item = new MapNext(idMapNext, type, info);
        this.MyLinkMaps[idMapStart].Add(item);
    }

    // Token: 0x06000A1B RID: 2587 RVA: 0x00008436 File Offset: 0x00006636
    private void AddKeyLinkMaps(int idMap)
    {
        if (!this.MyLinkMaps.ContainsKey(idMap))
        {
            this.MyLinkMaps.Add(idMap, new List<MapNext>());
        }
    }

    // Token: 0x06000A1C RID: 2588 RVA: 0x00008457 File Offset: 0x00006657
    private bool IsWaitInfoMapTrans()
    {
        return !Pk9rXmap.IsShowPanelMapTrans;
    }

    // Token: 0x06000A1D RID: 2589 RVA: 0x00008461 File Offset: 0x00006661
    public static int GetIdMapFromPanelXmap(string mapName)
    {
        return int.Parse(mapName.Split(new char[]
        {
                ':'
        })[0]);
    }

    // Token: 0x06000A1E RID: 2590 RVA: 0x00087CF4 File Offset: 0x00085EF4
    public static Waypoint FindWaypoint(int idMap)
    {
        for (int i = 0; i < TileMap.vGo.size(); i++)
        {
            Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
            if (XmapData.GetTextPopup(waypoint.popup).Equals(TileMap.mapNames[idMap]))
            {
                return waypoint;
            }
        }
        return null;
    }

    // Token: 0x06000A1F RID: 2591 RVA: 0x0000847B File Offset: 0x0000667B
    public static int GetPosWaypointX(Waypoint waypoint)
    {
        if (waypoint.maxX < 60)
        {
            return 15;
        }
        if ((int)waypoint.minX > TileMap.pxw - 60)
        {
            return TileMap.pxw - 15;
        }
        return (int)(waypoint.minX + 30);
    }

    // Token: 0x06000A20 RID: 2592 RVA: 0x000084AC File Offset: 0x000066AC
    public static int GetPosWaypointY(Waypoint waypoint)
    {
        return (int)waypoint.maxY;
    }

    // Token: 0x06000A21 RID: 2593 RVA: 0x000084B4 File Offset: 0x000066B4
    public static bool IsMyCharDie()
    {
        return global::Char.myCharz().statusMe == 14 || global::Char.myCharz().cHP <= 0;
    }

    // Token: 0x06000A22 RID: 2594 RVA: 0x000084D6 File Offset: 0x000066D6
    public static bool CanNextMap()
    {
        return !global::Char.isLoadingMap && !global::Char.ischangingMap && !Controller.isStopReadMessage;
    }

    // Token: 0x06000A23 RID: 2595 RVA: 0x00087D44 File Offset: 0x00085F44
    private static int GetIdMapFromName(string mapName)
    {
        int cgender = global::Char.myCharz().cgender;
        if (mapName.Equals("Về nhà"))
        {
            return 21 + cgender;
        }
        if (mapName.Equals("Trạm tàu vũ trụ"))
        {
            return 24 + cgender;
        }
        if (mapName.Contains("Về chỗ cũ: "))
        {
            mapName = mapName.Replace("Về chỗ cũ: ", "");
            if (TileMap.mapNames[Pk9rXmap.IdMapCapsuleReturn].Equals(mapName))
            {
                return Pk9rXmap.IdMapCapsuleReturn;
            }
            if (mapName.Equals("Rừng đá"))
            {
                return -1;
            }
        }
        for (int i = 0; i < TileMap.mapNames.Length; i++)
        {
            if (mapName.Equals(TileMap.mapNames[i]))
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06000A24 RID: 2596 RVA: 0x00087DEC File Offset: 0x00085FEC
    private static string GetTextPopup(PopUp popUp)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < popUp.says.Length; i++)
        {
            stringBuilder.Append(popUp.says[i]);
            stringBuilder.Append(" ");
        }
        return stringBuilder.ToString().Trim();
    }

    // Token: 0x06000A25 RID: 2597 RVA: 0x000084F0 File Offset: 0x000066F0
    private static bool CanUseCapsuleNormal()
    {
        return !XmapData.IsMyCharDie() && Pk9rXmap.IsUseCapsuleNormal && XmapData.HasItemCapsuleNormal();
    }

    // Token: 0x06000A26 RID: 2598 RVA: 0x00087E38 File Offset: 0x00086038
    private static bool HasItemCapsuleNormal()
    {
        Item[] arrItemBag = global::Char.myCharz().arrItemBag;
        for (int i = 0; i < arrItemBag.Length; i++)
        {
            if (arrItemBag[i] != null && arrItemBag[i].template.id == 193)
            {
                return true;
            }
        }
        return false;
    }

    // Token: 0x06000A27 RID: 2599 RVA: 0x00008507 File Offset: 0x00006707
    private static bool CanUseCapsuleVip()
    {
        return !XmapData.IsMyCharDie() && Pk9rXmap.IsUseCapsuleVip && XmapData.HasItemCapsuleVip();
    }

    // Token: 0x06000A28 RID: 2600 RVA: 0x00087E7C File Offset: 0x0008607C
    private static bool HasItemCapsuleVip()
    {
        Item[] arrItemBag = global::Char.myCharz().arrItemBag;
        for (int i = 0; i < arrItemBag.Length; i++)
        {
            if (arrItemBag[i] != null && arrItemBag[i].template.id == 194)
            {
                return true;
            }
        }
        return false;
    }

    // Token: 0x0400125E RID: 4702
    private const int ID_MAP_HOME_BASE = 21;

    // Token: 0x0400125F RID: 4703
    private const int ID_MAP_TTVT_BASE = 24;

    // Token: 0x04001260 RID: 4704
    private const int ID_ITEM_CAPSUAL_VIP = 194;

    // Token: 0x04001261 RID: 4705
    private const int ID_ITEM_CAPSUAL_NORMAL = 193;

    // Token: 0x04001262 RID: 4706
    private const int ID_MAP_TPVGT = 19;

    // Token: 0x04001263 RID: 4707
    private const int ID_MAP_TO_COLD = 109;

    // Token: 0x04001264 RID: 4708
    public List<GroupMap> GroupMaps;

    // Token: 0x04001265 RID: 4709
    public Dictionary<int, List<MapNext>> MyLinkMaps;

    // Token: 0x04001266 RID: 4710
    public bool IsLoading;

    // Token: 0x04001267 RID: 4711
    private bool IsLoadingCapsule;

    // Token: 0x04001268 RID: 4712
    private static XmapData _Instance;
}
}
