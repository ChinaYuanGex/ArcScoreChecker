using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker
{
    public class PttCalc
    {
        public static float ResultPtt(int score,float ptt) {
            float targetPtt = 0.0f;
            if (score >= 10000000)
            {
                targetPtt = ptt + 2.0f;
            }
            else if (score >= 9800000)
            {
                targetPtt = ptt + 1.0f + (((float)score - 9800000.0f) / 200000.0f);
            }
            else if (score > 0)
            {
                targetPtt = ptt + (((float)score - 9500000.0f) / 300000.0f);
            }
            else {
                targetPtt = 0.0f;
            }
            return targetPtt > 0.0f ? targetPtt : 0.0f;
        }
    }
}
