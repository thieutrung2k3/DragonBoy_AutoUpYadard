using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Xmap
{
    public class Pk9rXmap
{
        public static bool Chat(string text)
        {
            if (text == "xmp")
            {
                if (Pk9rXmap.IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    XmapController.ShowXmapMenu();
                }
            }
            else if (Pk9rXmap.IsGetInfoChat<int>(text, "xmp"))
            {
                if (Pk9rXmap.IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    XmapController.StartRunToMapId(Pk9rXmap.GetInfoChat<int>(text, "xmp"));
                }
            }
            else if (text == "csb")
            {
                Pk9rXmap.IsUseCapsuleNormal = !Pk9rXmap.IsUseCapsuleNormal;
                GameScr.info1.addInfo("Sử dụng capsule thường Xmap: " + (Pk9rXmap.IsUseCapsuleNormal ? "Bật" : "Tắt"), 0);
            }
            else
            {
                if (!(text == "csdb"))
                {
                    return false;
                }
                Pk9rXmap.IsUseCapsuleVip = !Pk9rXmap.IsUseCapsuleVip;
                GameScr.info1.addInfo("Sử dụng capsule đặc biệt Xmap: " + (Pk9rXmap.IsUseCapsuleVip ? "Bật" : "Tắt"), 0);
            }
            return true;
        }

        // Token: 0x060009E0 RID: 2528 RVA: 0x000870BC File Offset: 0x000852BC
        public static bool HotKeys()
        {
            int keyAsciiPress = GameCanvas.keyAsciiPress;
            if (keyAsciiPress != 99)
            {
                if (keyAsciiPress != 120)
                {
                    return false;
                }
                Pk9rXmap.Chat("xmp");
            }
            else
            {
                Pk9rXmap.Chat("csb");
            }
            return true;
        }

        // Token: 0x060009E1 RID: 2529 RVA: 0x00008181 File Offset: 0x00006381
        public static void Update()
        {
            if (XmapData.Instance().IsLoading)
            {
                XmapData.Instance().Update();
            }
            if (Pk9rXmap.IsXmapRunning)
            {
                XmapController.Update();
            }
        }

        // Token: 0x060009E2 RID: 2530 RVA: 0x000081A5 File Offset: 0x000063A5
        public static void Info(string text)
        {
            if (text.Equals("Bạn chưa thể đến khu vực này"))
            {
                XmapController.FinishXmap();
            }
        }

        // Token: 0x060009E3 RID: 2531 RVA: 0x000870F4 File Offset: 0x000852F4
        public static bool XoaTauBay(object obj)
        {
            Teleport teleport = (Teleport)obj;
            if (teleport.isMe)
            {
                global::Char.myCharz().isTeleport = false;
                if (teleport.type == 0)
                {
                    Controller.isStopReadMessage = false;
                    global::Char.ischangingMap = true;
                }
                Teleport.vTeleport.removeElement(teleport);
                return true;
            }
            return false;
        }

        // Token: 0x060009E4 RID: 2532 RVA: 0x000081B9 File Offset: 0x000063B9
        public static void SelectMapTrans(int selected)
        {
            if (Pk9rXmap.IsMapTransAsXmap)
            {
                XmapController.HideInfoDlg();
                XmapController.StartRunToMapId(XmapData.GetIdMapFromPanelXmap(GameCanvas.panel.mapNames[selected]));
                return;
            }
            XmapController.SaveIdMapCapsuleReturn();
            Service.gI().requestMapSelect(selected);
        }

        // Token: 0x060009E5 RID: 2533 RVA: 0x000081EE File Offset: 0x000063EE
        public static void ShowPanelMapTrans()
        {
            Pk9rXmap.IsMapTransAsXmap = false;
            if (Pk9rXmap.IsShowPanelMapTrans)
            {
                GameCanvas.panel.setTypeMapTrans();
                GameCanvas.panel.show();
                return;
            }
            Pk9rXmap.IsShowPanelMapTrans = true;
        }

        // Token: 0x060009E6 RID: 2534 RVA: 0x00008218 File Offset: 0x00006418
        public static void FixBlackScreen()
        {
            Controller.gI().loadCurrMap(0);
            Service.gI().finishLoadMap();
            global::Char.isLoadingMap = false;
        }

        // Token: 0x060009E7 RID: 2535 RVA: 0x00087140 File Offset: 0x00085340
        private static bool IsGetInfoChat<T>(string text, string s)
        {
            if (text.StartsWith(s))
            {
                try
                {
                    Convert.ChangeType(text.Substring(s.Length), typeof(T));
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        // Token: 0x060009E8 RID: 2536 RVA: 0x00008235 File Offset: 0x00006435
        private static T GetInfoChat<T>(string text, string s)
        {
            return (T)((object)Convert.ChangeType(text.Substring(s.Length), typeof(T)));
        }

        // Token: 0x04001244 RID: 4676
        public static bool IsXmapRunning = false;

        // Token: 0x04001245 RID: 4677
        public static bool IsMapTransAsXmap = false;

        // Token: 0x04001246 RID: 4678
        public static bool IsShowPanelMapTrans = true;

        // Token: 0x04001247 RID: 4679
        public static bool IsUseCapsuleNormal = false;

        // Token: 0x04001248 RID: 4680
        public static bool IsUseCapsuleVip = true;

        // Token: 0x04001249 RID: 4681
        public static int IdMapCapsuleReturn = -1;
    }
}
