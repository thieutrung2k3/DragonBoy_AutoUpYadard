using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Mod
{
    public class AutoAttack
{
        public static void autoAk()
        {
            var vMob = new MyVector();
            var vChar = new MyVector();
            Char myChar = new Char();
            if(myChar.mobFocus != null)
            {
                vMob.addElement(myChar.mobFocus);
            }
            else if(myChar.charFocus != null)
            {
                vChar.addElement(myChar.charFocus);
            }
            if(vMob.size() > 0 || vChar.size()>0) {
                var mySkill = myChar.myskill;
                long currentTime = mSystem.currentTimeMillis();
                if(currentTime - mySkill.lastTimeUseThisSkill > mySkill.coolDown)
                {
                    Service.gI().sendPlayerAttack(vMob, vChar, -1);
                    mySkill.lastTimeUseThisSkill = currentTime;
                }
            }
        }
}
}
