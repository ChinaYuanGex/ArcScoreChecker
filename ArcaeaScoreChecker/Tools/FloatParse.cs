using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker.Tools
{
    public class FloatParse
    {
        public static string GetString(float value, int dem)
        {
            int vala = (int)(value * (Math.Pow(10, dem)));
            return ((float)vala / (float)Math.Pow(10, dem)).ToString($"f{dem}");
        }
    }
}
