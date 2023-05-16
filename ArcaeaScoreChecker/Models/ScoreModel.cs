using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker.Models
{
    public enum SongDiff : int
    {
        NULL = -1,
        Pst = 0,
        Prs = 1,
        Ftr = 2,
        Byd = 3
    }
    public enum Modifier : int { 
        Hard = 2,
        Easy = 1,
        Normal = 0
    }
    public enum ClearType : int
    {
        TrackLost = 0,
        NormalClear = 1,
        FullRecall = 2,
        PureMemory = 3,
        EasyClear = 4,
        HardClear = 5
    }
    public enum Rating : int { 
        EXP = 0,
        EX = 1,
        AA = 2,
        A = 3,
        B = 4,
        C = 5,
        D = 6
    }
    public class ScoreModel
    {

        public ScoreModel(string id) {
            SongID = id;
        }

        public string SongID { get; set; }
        public SongModel song
        {
            get
            {
                SongModel[] search = GlobalData.songs.Where((x) => {
                    return x.ID == SongID;
                }).ToArray();
                if (search.Length < 1)
                {
                    return null;
                }
                else return search[0];
            }
        }
        public PttModel ptts
        {
            get
            {
                if (song != null)
                    return song.ptt;
                else
                    return null;
            }
        }
        public float ResultPtt
        {
            get
            {
                if (ptts != null) {
                    switch (Diff) {
                        case SongDiff.Pst:
                            return PttCalc.ResultPtt(Score, (float)ptts.pst);
                            break;
                        case SongDiff.Prs:
                            return PttCalc.ResultPtt(Score, (float)ptts.prs);
                            break;
                        case SongDiff.Ftr:
                            return PttCalc.ResultPtt(Score, (float)ptts.ftr);
                            break;
                        case SongDiff.Byd:
                            return PttCalc.ResultPtt(Score, (float)ptts.byd);
                            break;
                        default:
                            return 0.0f;
                    }
                } else return 0.0f;
            }
        }
        public SongDiff Diff { get; set; }
        public int Score { get; set; }
        public int MaxPure { get; set; }
        public int Pure { get; set; }
        public int Far { get; set; }
        public int Lost { get; set; }

        public ClearType ClearType { get; set; }
        public Modifier Modifier { get; set; }
        public Rating Rating
        {
            get
            {
                if (Score >= 9900000) return Rating.EXP;
                else if (Score >= 9800000) return Rating.EX;
                else if (Score >= 9500000) return Rating.AA;
                else if (Score >= 9200000) return Rating.A;
                else if (Score >= 8900000) return Rating.B;
                else if (Score >= 8600000) return Rating.C;
                else return Rating.D;
            }
        }
        public DateTime Time { get; set; }
    }
}
