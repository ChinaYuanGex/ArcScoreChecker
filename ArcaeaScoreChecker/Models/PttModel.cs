using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker.Models
{
    public class PttModel
    {
        public string Name { get; set; }
        public float pst { get; set; }
        public float prs { get; set; }
        public float ftr { get; set; }
        public float byd { get; set; }
        public float GetPtt(SongDiff diff) {
            switch (diff)
            {
                case SongDiff.Pst: return pst;
                case SongDiff.Prs: return prs;
                case SongDiff.Ftr: return ftr;
                case SongDiff.Byd: return byd;
                default: return 0.0f;
            }
        }
        public void UpdatePtt(SongDiff diff,float value) {
            switch (diff)
            {
                case SongDiff.Pst: 
                    pst = value;
                    break;
                case SongDiff.Prs:
                    prs = value;
                    break;
                case SongDiff.Ftr:
                    ftr = value;
                    break;
                case SongDiff.Byd:
                    byd = value;
                    break;
                default: return;
            }
        }
    }
}
