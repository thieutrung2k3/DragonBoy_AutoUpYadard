using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssemblyCSharp.Mod;
using Assets.src.g;
using System.Threading;
using System.IO;
using AssemblyCSharp.Xmap;

namespace AssemblyCSharp.Mod
{
    public class TzMod
{
        public static float speedGame = 2.5f;
        public static bool isChangeFocus;
        public static bool isAutoSkill;
        public static int skillSelected = 1;
        public static bool isAutoPorata;
        public static bool isAutoYadard;
        public static int speedChar = 10;
        public static int quantity;
        public static int x;
        public static void akChar()
        {
            try
            {
                MyVector myVector = new MyVector();
                myVector.addElement(Char.myCharz().charFocus);
                Service.gI().sendPlayerAttack(new MyVector(), myVector, -1);
            }
            catch (Exception ex)
            {

            }
        }
        public static void adMob()
        {
            try
            {
                MyVector myVector = new MyVector();
                myVector.addElement(Char.myCharz().mobFocus);
                Service.gI().sendPlayerAttack(myVector, new MyVector(), -1);
            }
            catch (Exception ex)
            {

            }
        }
        public static void autoAk()
        {
            while (isAutoSkill)
            {
                if (global::Char.myCharz().mobFocus != null)
                {
                    TzMod.adMob();
                }
                if (global::Char.myCharz().charFocus != null)
                {
                    TzMod.akChar();
                }
                Thread.Sleep(500);
            }
        }
        public static void chat(string text)
        {
            
            if (text.Contains("cheat_"))
            {
                Time.timeScale = int.Parse(text.Replace("cheat_", ""));
            }
            else if (text.Contains("map_"))
            {
                int id = int.Parse(text.Replace("map_", ""));
                XmapController.StartRunToMapId(id);
            }
            else if (text.Contains("yd"))
            {
                TzMod.isChangeFocus = !TzMod.isChangeFocus;
                TzMod.isAutoSkill = !TzMod.isAutoSkill;
                new Thread(new ThreadStart(TzMod.autoFocusMob)).Start();
                new Thread(new ThreadStart(TzMod.autoAk)).Start(); 
            }
            else if (text.Contains("s_"))
            {
                speedChar = int.Parse(text.Replace("s_", ""));
            }
            else if (text.Contains("x_"))
            {
                x = int.Parse(text.Replace("x_", ""));
            }
            /*else if (text.Contains("ak"))
            {
                timeAk = 1;
                new Thread(new ThreadStart(TzMod.autoAk)).Start();
            }*/
            else if (text.Contains("bt")){
                TzMod.isAutoPorata = !TzMod.isAutoPorata;
                new Thread(new ThreadStart(TzMod.usePorata)).Start();
            }
        }
        public static void checkMapYadard()
        {
            if(TileMap.mapID != 133)
            {
                XmapController.StartRunToMapId(133);
            }
        }
        public static void usePorata()
        {
            sbyte count = 0;
            
                while ((int)count < global::Char.myCharz().arrItemBag.Length)
                {
                    if (global::Char.myCharz().arrItemBag[(int)count].template.id == 921 || global::Char.myCharz().arrItemBag[(int)count].template.id == 454)
                    {   
                        break;
                    }
                    count++;
                }
            while (TzMod.isAutoPorata)
            {
                Service.gI().useItem(0, 1, count, -1);
                Service.gI().petStatus(3);
                Thread.Sleep(1000);
            }
        }
        public static void useCapsule()
        {
            try
            {
                sbyte count = 0;
                while((int)count < global::Char.myCharz().arrItemBag.Length)
                {
                    if (global::Char.myCharz().arrItemBag[(int)count].template.id == 193 || global::Char.myCharz().arrItemBag[(int)count].template.id == 194)
                    {
                        Service.gI().useItem(0, 1, count, -1);
                        
                        break;
                    }
                    count++;
                }
            }
            catch {
                GameScr.info1.addInfo("Khong tim thay Capsule!!", 0);
            }
        }
        public static void autoFocusMob()
        {
            while (TzMod.isChangeFocus) 
            {
                for(int i = 0; i < GameScr.vCharInMap.size(); i++)
                {
                    
                    global::Char @Char = (global::Char)GameScr.vCharInMap.elementAt(i);
                    if(@Char.charID < 0 && Res.abs(@Char.cx - global::Char.myCharz().cx) <= 100 && Res.abs(@Char.cy - global::Char.myCharz().cy) <= 100)
                    {
                        
                        global::Char.myCharz().charFocus = (global::Char)@Char;
                        Thread.Sleep(1000);
                    }
                }
            }
            
        }
        public static void upYadard()
        {
            if(global::Char.myCharz().cx < 590 && isAutoYadard)
            {
                
            }
        }
        public static long time;
        public static void autoPick()
        {
            try
            {
                if (mSystem.currentTimeMillis() - time > 1000L)
                {
                    for(int i = 0; i < GameScr.vItemMap.size(); i++)
                    {
                        ItemMap item = (ItemMap)GameScr.vItemMap.elementAt(i);
                        if(item.template.id == 590)
                        {
                            Service.gI().pickItem(item.itemMapID);
                            time = mSystem.currentTimeMillis();
                        }
                    }
                }
            }catch(Exception e) { }
        }
        public static void biKiep()
        {
            for( int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
            {
                if (global::Char.myCharz().arrItemBag[i].template.id == 590)
                {
                    quantity = global::Char.myCharz().arrItemBag[i].itemOption[0].param;
                }
            }
        }
        public static void autoLogin()
        {
            Thread.Sleep(30000);
            GameCanvas.gI().keyPressedz(-5);
            GameCanvas.loginScr.doLogin();
            GameCanvas.serverScreen.perform(3, null);
           
        }
        /*public static void AutoSkill()
        {
            while (TzMod.isAutoSkill)
            {
                
                Thread.Sleep(500);
            }
        }*/
        
        public static void onUpdateGameScr()
        {
            global::Char.myCharz().cspeed = speedChar;
            autoPick();
            
        }

}
}
