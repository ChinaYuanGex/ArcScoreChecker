using ArcaeaScoreChecker.Models;
using ArcaeaScoreChecker.Pages;
using ArcaeaScoreChecker.Properties;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcaeaScoreChecker.Tools;

namespace ArcaeaScoreChecker
{
    public class ScoreData
    {
        public class Best {
            public static string dbPath = FileSystem.AppDataDirectory + "/score.db";
            public static SqliteConnection db;
            public static void OpenDatabase() {
                if (!File.Exists(dbPath))
                {
                    File.WriteAllBytes(dbPath, fileResource.score);
                }
                db = new SqliteConnection("Data Source=" + dbPath);
                db.Open();
            }
            public static void CreateDBFile()
            {
                File.WriteAllBytes(dbPath, fileResource.score);
            }
            /// <summary>
            /// 从已安装的
            /// </summary>
            /// <returns></returns>
            public static void ReadAndConvertLocalSt3Database(string path) {
                string localBase = FileSystem.CacheDirectory + "/st3.db";
                ShellInterface.RootExecute("cp", $"{path} {localBase}");
                ShellInterface.RootExecute("chmod", $"0777 {localBase}");
                ConvertGameSt3Database(localBase);
                File.Delete(localBase);
            }

            public static void ConvertGameSt3Database(string dbpath) {
                SqliteConnection gamedb = new SqliteConnection("Data Source=" + dbpath);
                gamedb.Open();

                SqliteCommand cmd = gamedb.CreateCommand();
                cmd.CommandText = "SELECT * FROM scores";

                SqliteDataReader gamedbRead = cmd.ExecuteReader();

                while (gamedbRead.Read())
                {
                    string songID = gamedbRead["songId"].ToString();
                    int diffint = Convert.ToInt32(gamedbRead["songDifficulty"]);
                    SongDiff diff = (SongDiff)diffint;

                    Modifier mod = (Modifier)Convert.ToInt32(gamedbRead["modifier"]);

                    SqliteCommand nextcmd = gamedb.CreateCommand();
                    nextcmd.CommandText = $"SELECT clearType FROM cleartypes WHERE songId='{songID}' AND songDifficulty={(int)diff}";
                    SqliteDataReader r = nextcmd.ExecuteReader();
                    if (!r.Read()) continue;

                    ClearType clear = (ClearType)Convert.ToInt32(r[0]);

                    int score = Convert.ToInt32(gamedbRead["score"]);
                    int maxpure = Convert.ToInt32(gamedbRead["shinyPerfectCount"]);
                    int pure = Convert.ToInt32(gamedbRead["perfectCount"]);
                    int far = Convert.ToInt32(gamedbRead["nearCount"]);
                    int lost = Convert.ToInt32(gamedbRead["missCount"]);
                    UpdateBestOrCreateRecord(songID, diff, mod, clear, score, maxpure, pure, far, lost);
                }
                gamedb.Close();
            }

            public static ScoreModel[] ReadBest30() {

                ScoreModel[] scores = ReadAllDataAndSorted();

                return scores.Take(scores.Length > 30 ? 30 : scores.Length).ToArray();
            }
            public static ScoreModel[] ReadBest40() {
                ScoreModel[] scores = ReadAllDataAndSorted();

                return scores.Take(scores.Length > 40 ? 40 : scores.Length).ToArray();
            }
            public static ScoreModel[] ReadAllDataAndSorted() {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = "SELECT * FROM scores";
                SqliteDataReader read = cmd.ExecuteReader();

                List<ScoreModel> scores = new List<ScoreModel>();

                while (read.Read())
                {
                    scores.Add(new ScoreModel(read.GetString(1))
                    {
                        Diff = (SongDiff)Convert.ToInt32(read["diff"]),
                        Modifier = (Modifier)Convert.ToInt32(read["mod"]),
                        ClearType = (ClearType)Convert.ToInt32(read["clear"]),
                        Score = Convert.ToInt32(read["score"]),
                        MaxPure = Convert.ToInt32(read["maxpure"]),
                        Pure = Convert.ToInt32(read["pure"]),
                        Far = Convert.ToInt32(read["far"]),
                        Lost = Convert.ToInt32(read["lost"]),
                        Time = new Timestamp(Convert.ToInt64(read["time"])).ToLocal().time
                    });
                }
                scores.Sort((x, y) =>
                {
                    float a = x.ResultPtt;
                    float b = y.ResultPtt;
                    return a > b ? -1 : 1;
                });
                
                return scores.ToArray();
            }
            public static float ReadCurrentPTT()
            {
                ScoreModel[] b30 = ReadBest30();
                float current = 0.0f;
                foreach(ScoreModel b in b30 )
                {
                    current += b.ResultPtt;
                }
                return current / 40;
            }
            public static float ReadPTT() {
                Models.ScoreModel[] best30 = ReadBest30();
                float totalPttValue = 0.0f;
                foreach (Models.ScoreModel rec in best30)
                {
                    totalPttValue += rec.ResultPtt;
                }
                return (totalPttValue / 30);
            }
            public static ScoreModel GetRecord(string songid,SongDiff diff) {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT * FROM scores WHERE `songid`='{songid}' AND diff={(int)diff}";
                SqliteDataReader read = cmd.ExecuteReader();

                if (!read.Read()) return null;
                return new ScoreModel(songid) {
                    Diff = diff,
                    ClearType = (ClearType)Convert.ToInt32(read["clear"]),
                    Modifier = (Modifier)Convert.ToInt32(read["mod"]),
                    Score = Convert.ToInt32(read["score"]),
                    MaxPure = Convert.ToInt32(read["maxpure"]),
                    Pure = Convert.ToInt32(read["pure"]),
                    Far = Convert.ToInt32(read["far"]),
                    Lost = Convert.ToInt32(read["lost"]),
                    Time = new Timestamp((long)read["time"]).ToLocal().time
                };
            }
            public static int InsertNewRecord(string songid, SongDiff diff,Modifier mod,ClearType clear, int score,int maxpure,int pure,int far,int lost,DateTime time) {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = "INSERT INTO " +
                    "scores(`songid`,`diff`,`mod`,`clear`,`score`,`maxpure`,`pure`,`far`,`lost`,`time`) " +
                    $"VALUES('{songid}',{(int)diff},{(int)mod},{(int)clear},{score},{maxpure},{pure},{far},{lost},{new Timestamp(time).ToUTC().timestamp_mill})";
                return cmd.ExecuteNonQuery();
            }
            public static int UpdateRecord(string songid, SongDiff diff, Modifier mod, ClearType clear, int score = -1, int maxpure =  -1, int pure = -1, int far = -1, int lost = -1)
            {
                int tscore = score < 0 ? 0 : score;
                int tmaxpure = maxpure < 0 ? 0 : maxpure;
                int tpure = pure < 0 ? 0 : pure;
                int tfar = far < 0 ? 0 : far;
                int tlost = lost < 0 ? 0 : lost;

                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"UPDATE scores SET " +
                    $"`score`={tscore}," +
                    $"`maxpure`={tmaxpure}," +
                    $"`pure`={tpure}," +
                    $"`far`={tfar}," +
                    $"`lost`={tlost}," +
                    $"`time`={new Timestamp().ToUTC().timestamp_mill}, " +
                    $"`mod`={(int)mod}, " +
                    $"`clear`={(int)clear} " +
                    $"WHERE `songid`='{songid}' " +
                    $"AND `diff`={(int)diff}";

                return cmd.ExecuteNonQuery();
            }
            public static int DeleteRecord(string songid, SongDiff diff)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores WHERE `songid`='{songid}' AND `diff`={(int)diff}";
                return cmd.ExecuteNonQuery();
            }
            public static int DeleteRecordBySong(string songid) {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores WHERE `songid`='{songid}'";
                return cmd.ExecuteNonQuery();
            }
            public static int DeleteAllRecord() {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores;";
                return cmd.ExecuteNonQuery();
            }
            public static void VacUUM() {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"VACUUM";
                cmd.ExecuteNonQuery();
            }
            public static bool RecordExists(string songid, SongDiff diff) {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM scores WHERE `songid`='{songid}' AND `diff`={(int)diff}";
                SqliteDataReader read = cmd.ExecuteReader();
                read.Read();
                if (Convert.ToInt32(read[0]) > 0)
                {
                    return true;
                }
                else return false;
            }
            /// <summary>
            /// 当存在记录时则更新，否则插入新数据
            /// </summary>
            /// <param name="songid"></param>
            /// <param name="diff"></param>
            /// <param name="score"></param>
            /// <param name="maxpure"></param>
            /// <param name="pure"></param>
            /// <param name="far"></param>
            /// <param name="lost"></param>
            /// <returns></returns>
            public static int UpdateOrCreateRecord(string songid, SongDiff diff,Modifier mod,ClearType clear, int score = -1, int maxpure = -1, int pure = -1, int far = -1, int lost = -1)
            {
                if (RecordExists(songid, diff))
                {
                    return UpdateRecord(songid, diff, mod, clear, score, maxpure, pure, far, lost);
                }
                else {
                    return InsertNewRecord(songid, diff, mod, clear, score, maxpure, pure, far, lost, DateTime.Now);
                }
            }
            /// <summary>
            /// 当记录存在时，如果插入的数据分数大于原有记录的分数则更新，否则插入新数据
            /// </summary>
            /// <param name="songid"></param>
            /// <param name="diff"></param>
            /// <param name="score"></param>
            /// <param name="maxpure"></param>
            /// <param name="pure"></param>
            /// <param name="far"></param>
            /// <param name="lost"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public static int UpdateBestOrCreateRecord(string songid, SongDiff diff, Modifier mod, ClearType clear, int score = -1, int maxpure = -1, int pure = -1, int far = -1, int lost = -1) {
                ScoreModel m = GetRecord(songid, diff);
                if (m != null)
                {
                    if (m.Score < score)
                    {
                        return UpdateRecord(songid, diff, mod, clear, score, maxpure, pure, far, lost);
                    }
                    else return 0;
                }
                else return InsertNewRecord(songid, diff, mod, clear, score, maxpure, pure, far, lost, DateTime.Now);
            }
            public static float GetMaxPttProber()
            {
                ScoreModel[] best10 = ReadBest30().Take(10).ToArray();
                float bestestr10 = 0.0f;
                foreach (ScoreModel m in best10) {
                    bestestr10 += m.ResultPtt;
                }
                bestestr10 = bestestr10 / 40;

                return ReadCurrentPTT() + bestestr10;
            }
        }
        public class Recent {
            public static string dbPath = FileSystem.AppDataDirectory + "/recent.db";
            public static SqliteConnection db;
            public static void OpenDatabase() {
                if (!File.Exists(dbPath))
                {
                    File.WriteAllBytes(dbPath, fileResource.score);
                }
                db = new SqliteConnection("Data Source=" + dbPath);
                db.Open();
            }
            public static ScoreModel GetRecord(string songid, SongDiff diff)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT * FROM scores WHERE `songid`='{songid}' AND diff={(int)diff}";
                SqliteDataReader read = cmd.ExecuteReader();

                if (!read.Read()) return null;
                return new ScoreModel(songid)
                {
                    Diff = diff,
                    ClearType = (ClearType)Convert.ToInt32(read["clear"]),
                    Modifier = (Modifier)Convert.ToInt32(read["mod"]),
                    Score = Convert.ToInt32(read["score"]),
                    MaxPure = Convert.ToInt32(read["maxpure"]),
                    Pure = Convert.ToInt32(read["pure"]),
                    Far = Convert.ToInt32(read["far"]),
                    Lost = Convert.ToInt32(read["lost"]),
                    Time = new Timestamp((long)read["time"]).ToLocal().time
                };
            }
            public static int InsertNewRecord(string songid, SongDiff diff, Modifier mod, ClearType clear, int score, int maxpure, int pure, int far, int lost, DateTime time)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = "INSERT INTO " +
                    "scores(`songid`,`diff`,`mod`,`clear`,`score`,`maxpure`,`pure`,`far`,`lost`,`time`) " +
                    $"VALUES('{songid}',{(int)diff},{(int)mod},{(int)clear},{score},{maxpure},{pure},{far},{lost},{new Timestamp(time).ToUTC().timestamp_mill})";
                return cmd.ExecuteNonQuery();
            }
            public static int UpdateRecord(string songid, SongDiff diff, Modifier mod, ClearType clear, int score = -1, int maxpure = -1, int pure = -1, int far = -1, int lost = -1)
            {
                int tscore = score < 0 ? 0 : score;
                int tmaxpure = maxpure < 0 ? 0 : maxpure;
                int tpure = pure < 0 ? 0 : pure;
                int tfar = far < 0 ? 0 : far;
                int tlost = lost < 0 ? 0 : lost;

                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"UPDATE scores SET " +
                    $"`score`={tscore}," +
                    $"`maxpure`={tmaxpure}," +
                    $"`pure`={tpure}," +
                    $"`far`={tfar}," +
                    $"`lost`={tlost}," +
                    $"`time`={new Timestamp().ToUTC().timestamp_mill} " +
                    $"`mod`={(int)mod} " +
                    $"`clear`={(int)clear} " +
                    $"WHERE `songid`='{songid}' " +
                    $"AND `diff`={(int)diff}";

                return cmd.ExecuteNonQuery();
            }
            public static int DeleteRecord(string songid, SongDiff diff)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores WHERE `songid`='{songid}' AND `diff`={(int)diff}";
                return cmd.ExecuteNonQuery();
            }
            public static int DeleteRecordBySong(string songid)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores WHERE `songid`='{songid}'";
                return cmd.ExecuteNonQuery();
            }
            public static int DeleteRecordWithTime(string songid, SongDiff diff, DateTime time) {
                SqliteCommand cmd = db.CreateCommand();
                Timestamp t = new Timestamp(time);
                cmd.CommandText = $"DELETE FROM scores WHERE `songid`='{songid}' AND `diff`={(int)diff} AND `time`={t.ToUTC().timestamp_mill}";
                return cmd.ExecuteNonQuery();
            }
            public static int DeleteAllRecord()
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DELETE FROM scores;";
                return cmd.ExecuteNonQuery();
            }
            public static void VacUUM()
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"VACUUM";
                cmd.ExecuteNonQuery();
            }
            public static bool RecordExists(string songid, SongDiff diff)
            {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM scores WHERE `songid`='{songid}' AND `diff`={(int)diff}";
                SqliteDataReader read = cmd.ExecuteReader();
                read.Read();
                if (Convert.ToInt32(read[0]) > 0)
                {
                    return true;
                }
                else return false;
            }

            /// <summary>
            /// 获取Recent30，根据时间排序
            /// </summary>
            /// <returns></returns>
            public static ScoreModel[] ReadRecent30() {
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT * FROM scores ORDER BY `time` DESC LIMIT 0,30";
                SqliteDataReader read = cmd.ExecuteReader();

                List<ScoreModel> m = new List<ScoreModel>();

                while (read.Read()) {
                    m.Add(new ScoreModel(read["songid"].ToString())
                    {
                        Diff = (SongDiff)Convert.ToInt32(read["diff"]),
                        Modifier = (Modifier)Convert.ToInt32(read["mod"]),
                        ClearType = (ClearType)Convert.ToInt32(read["clear"]),
                        Score = Convert.ToInt32(read["score"]),
                        MaxPure = Convert.ToInt32(read["maxpure"]),
                        Pure = Convert.ToInt32(read["pure"]),
                        Far = Convert.ToInt32(read["far"]),
                        Lost = Convert.ToInt32(read["lost"]),
                        Time = new Timestamp((long)read["time"]).ToLocal().time
                    });
                }
                return m.ToArray();
            }
            public static ScoreModel[] ReadRecent10() {
                ScoreModel[] r30 = ReadRecent30AndSortByPTT();
                List<ScoreModel> m = new List<ScoreModel>();

                foreach (ScoreModel r in r30) {
                    if (m.Count >= 10) { break; }
                    ScoreModel[] search = m.Where((x) => {
                        return x.SongID == r.SongID && x.Diff == r.Diff;
                    }).ToArray();
                    if (search.Length < 1)
                    {
                        m.Add(r);
                    }
                }

                m.Sort((x, y) => {
                    return x.ResultPtt > y.ResultPtt ? -1 : 1;
                });

                return m.ToArray();
            }
            /// <summary>
            /// 获取R30,按照Ptt排序
            /// </summary>
            /// <returns></returns>
            public static ScoreModel[] ReadRecent30AndSortByPTT() {
                List<ScoreModel> m = ReadRecent30().ToList();
                m.Sort((x, y) => {
                    return x.ResultPtt > y.ResultPtt ? -1 : 1;
                });
                return m.ToArray();
            }
            /// <summary>
            /// 整个Recent系统最关键的计算函数
            /// </summary>
            /// <param name="songid"></param>
            /// <param name="diff"></param>
            /// <param name="mod"></param>
            /// <param name="clear"></param>
            /// <param name="score"></param>
            /// <param name="maxpure"></param>
            /// <param name="pure"></param>
            /// <param name="far"></param>
            /// <param name="lost"></param>
            /// <param name="time"></param>
            /// <returns></returns>
            public static int InsertAndUpdateRecent(string songid, SongDiff diff, Modifier mod, ClearType clear, int score, int maxpure, int pure, int far, int lost, DateTime time) {
                return InsertAndUpdateRecent_method1(songid, diff, mod, clear, score, maxpure, pure, far, lost, time);
            }
            public static bool CheckIfLatest(DateTime time)
            {
                long curtime = new Tools.Timestamp(time).ToUTC().timestamp_mill;
                SqliteCommand cmd = db.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM scores WHERE time>={curtime}";
                SqliteDataReader read = cmd.ExecuteReader();
                
                if (read.Read() && read.GetInt64(0) > 0)
                {
                    return false;
                }
                else return true;
            }
            public static int InsertAndUpdateRecent_method1(string songid, SongDiff diff, Modifier mod, ClearType clear, int score, int maxpure, int pure, int far, int lost,DateTime time)
            {
                ScoreModel curScore = new ScoreModel(songid)
                {
                    Diff = diff,
                    Modifier = mod,
                    ClearType = clear,
                    Score = score,
                    MaxPure = maxpure,
                    Pure = pure,
                    Far = far,
                    Lost = lost,
                    Time = time
                };

                if (CheckIfLatest(time) == false) {
                    return -4;
                }

                if (curScore.song == null) {
                    return -1;
                }

                ScoreModel[] r30 = ReadRecent30();
                ScoreModel[] r30Sorted = ReadRecent30AndSortByPTT();

                if (r30.Length < 30)
                { //如果R30的数目不足
                    return InsertNewRecord(curScore.SongID, curScore.Diff, curScore.Modifier, curScore.ClearType, curScore.Score, curScore.MaxPure, curScore.Pure, curScore.Far, curScore.Lost, time);
                }
                else if (curScore.Modifier == Modifier.Hard && curScore.ClearType == ClearType.TrackLost)
                {  //Hard Lost
                    return -2; //Hard lost 不用更新r30
                }
                else if (curScore.Score >= 9800000)
                { //分数评级为EX
                    ScoreModel last = r30Sorted.Last();
                    if (curScore.ResultPtt > last.ResultPtt)
                    {
                        DeleteRecordWithTime(last.SongID, last.Diff, last.Time);
                        return InsertNewRecord(curScore.SongID, curScore.Diff, curScore.Modifier, curScore.ClearType, curScore.Score, curScore.MaxPure, curScore.Pure, curScore.Far, curScore.Lost, time);
                    }
                    return -3;
                }
                else //分数小于EX
                {
                    //相同铺面的计数
                    Dictionary<KeyValuePair<string, SongDiff>, int> chartCount = new Dictionary<KeyValuePair<string, SongDiff>, int>();
                    foreach (ScoreModel m in r30)
                    {
                        KeyValuePair<string, SongDiff> chart = new KeyValuePair<string, SongDiff>(m.SongID, m.Diff);
                        if (chartCount.ContainsKey(chart))
                        {
                            chartCount[chart]++;
                        }
                        else
                        {
                            chartCount.Add(chart, 1);
                        }
                    }
                    var sorted = (from obj in chartCount orderby obj.Value descending select obj).ToArray();
                    if (sorted.Length - 1 < 10) //若写出后小于10个
                    {
                        KeyValuePair<string, SongDiff> maxRep = sorted[0].Key;
                        //选择重复最多的最早的记录
                        List<ScoreModel> reps = r30.Where((x) => { return x.SongID == maxRep.Key && x.Diff == maxRep.Value; }).ToList();
                        reps.Sort((x, y) =>
                        {
                            return x.Time > y.Time ? -1 : 1;
                        });
                        ScoreModel removeTarget = reps.Last();
                        DeleteRecordWithTime(removeTarget.SongID, removeTarget.Diff, removeTarget.Time);
                        return InsertNewRecord(songid, diff, mod, clear, score, maxpure, pure, far, lost, time);
                    }
                    else //大于10个就直接写出
                    {
                        ScoreModel target = r30.Last();
                        DeleteRecordWithTime(target.SongID, target.Diff, target.Time);
                        return InsertNewRecord(curScore.SongID, curScore.Diff, curScore.Modifier, curScore.ClearType, curScore.Score, curScore.MaxPure, curScore.Pure, curScore.Far, curScore.Lost, time);
                    }

                }
            }
            public static float ReadCurrentPTT()
            {
                ScoreModel[] r10 = ReadRecent10();
                float current = 0.0f;
                foreach (ScoreModel b in r10)
                {
                    current += b.ResultPtt;
                }
                return current / 40;
            }
            public static float ReadPTT() {
                ScoreModel[] r10 = ReadRecent10();
                float current = 0.0f;
                foreach (ScoreModel b in r10)
                {
                    current += b.ResultPtt;
                }
                return current / 10;
            }
        }
    }
}
