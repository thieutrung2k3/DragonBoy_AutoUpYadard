using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AssemblyCSharp.Mod
{
    public class showBoss
{
        public string nameBoss;
        public string mapName;
        public int mapID;
        public DateTime time;
 
        public showBoss(string a) {
            a = a.Replace("BOSS ", "");
            a = a.Replace(" vừa xuất hiện tại", "|");
            a = a.Replace("khu vực", "|");
            string[] array = a.Split(new char[]
            {
                '|'
            });
            this.nameBoss = array[0].Trim();
            this.mapName = array[1].Trim();
            this.mapID = this.MapID(this.mapName);
            this.time = DateTime.Now;  
        }
        public int MapID(string a)
        {
            for(int i = 0; i < TileMap.mapNames.Length; i++)
            {
                if (TileMap.mapNames[i].Equals(a)) return i;
            }
            return -1;
        }
        public void paintBoss(mGraphics a, int b, int c, int d)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(this.time);
            int num = (int)timeSpan.TotalSeconds;
            mFont mFont = mFont.tahoma_7_yellow;
            if(TileMap.mapID == this.mapID)
            {
                mFont = mFont.tahoma_7_red;
                for(int i = 0; i < GameScr.vCharInMap.size(); i++)
                {
                    if (((global::Char)GameScr.vCharInMap.elementAt(i)).cName.Equals(this.nameBoss))
                    {
                        mFont = mFont.tahoma_7b_blue;
                        break;
                    }
                }
            }
            mFont.drawString(a, string.Concat(new string[]
            {
                this.nameBoss,
                " - ",
                this.mapName,
                " - ",
                (num < 60) ?(num  + "s"):(timeSpan.Minutes + "p"),
                " trước",
            }), b,c, d);

        }
}
}
