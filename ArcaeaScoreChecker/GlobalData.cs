using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcaeaScoreChecker.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Data.Sqlite;

namespace ArcaeaScoreChecker
{
    public class GlobalData
    {
        public static string versionStr = "(v1.0)[理论兼容所有版本]";
        public static PttModel[] ptts = new PttModel[0];
        public static SongModel[] songs = new SongModel[0];

        public static void initAllData() {
            initPtts();
            initSongs();
        }
        public static void initPtts() {
            string pttFile = Path.Combine(FileSystem.AppDataDirectory, "ptt.json");
            if (File.Exists(pttFile))
            {
                JObject list = JObject.Parse(File.ReadAllText(pttFile));
                List<PttModel> current = new List<PttModel>();
                foreach (KeyValuePair<string, JToken> p in list)
                {
                    float pst = p.Value.ToObject<JObject>()["pst"].ToObject<float>();
                    float prs = p.Value.ToObject<JObject>()["prs"].ToObject<float>();
                    float ftr = p.Value.ToObject<JObject>()["ftr"].ToObject<float>();
                    float byd = p.Value.ToObject<JObject>()["byd"].ToObject<float>();
                    PttModel m = new PttModel
                    {
                        Name = p.Key,
                        pst = pst,
                        prs = prs,
                        ftr = ftr,
                        byd = byd,
                    };
                    current.Add(m);
                }
                ptts = current.ToArray();
                Console.WriteLine("Read ptts=" + ptts.Length);
            }
        }
        public static void initSongs() {
            string songFile = Path.Combine(FileSystem.AppDataDirectory, "songs/songlist");
            if (File.Exists(songFile))
            {
                JObject songlist = JObject.Parse(File.ReadAllText(songFile));
                JArray songArray = songlist["songs"].ToObject<JArray>();
                List<SongModel> current = new List<SongModel>();
                foreach (JObject song in songArray)
                {

                    string id = song["id"].ToString();

                    string display = song["title_localized"]["en"].ToString();

                    long date = song["date"].ToObject<long>();

                    string[] sepicalDisplay = new string[4] { null, null, null, null };
                    foreach (JObject diff in song["difficulties"])
                    {
                        int ratingClass = diff["ratingClass"].ToObject<int>();
                        if (diff.ContainsKey("title_localized"))
                        {
                            sepicalDisplay[ratingClass] = diff["title_localized"]["en"].ToString();
                        }
                    }


                    string artist = song["artist"].ToString();

                    string[] sepicalArtist = new string[4] { null, null, null, null };

                    foreach (JObject diff in song["difficulties"])
                    {
                        int ratingClass = diff["ratingClass"].ToObject<int>();
                        if (diff.ContainsKey("artist"))
                        {
                            sepicalArtist[ratingClass] = diff["artist"].ToString();
                        }
                    }

                    string TitleImageBasePath = FileSystem.AppDataDirectory + $"/songs/{id}";
                    string TitleImageBaseDLPath = FileSystem.AppDataDirectory + $"/songs/dl_{id}";
                    string DefaultTitleImageBasePath = FileSystem.AppDataDirectory + $"/songs/random";

                    

                    string TitleImageBase = Directory.Exists(TitleImageBasePath) ? 
                        TitleImageBasePath : 
                        Directory.Exists(TitleImageBaseDLPath) ? TitleImageBaseDLPath : DefaultTitleImageBasePath;

                    SongModel m = new SongModel
                    {
                        ID = id,
                        Display = display,
                        Artist = artist,
                        TitleImageBase = TitleImageBase,
                        display_pst = sepicalDisplay[0],
                        display_prs = sepicalDisplay[1],
                        display_ftr = sepicalDisplay[2],
                        display_byd = sepicalDisplay[3],
                        artist_pst = sepicalArtist[0],
                        artist_prs = sepicalArtist[1],
                        artist_ftr = sepicalArtist[2],
                        artist_byd = sepicalArtist[3],
                        time = new Tools.Timestamp(date).time
                    };

                    PttModel[] pttMatch = ptts.Where((x) => { 
                        return (x.Name == m.ID) || (x.Name == m.Display); 
                    }).ToArray();
                    if (pttMatch.Length > 0)
                    {

                        if (pttMatch.Length != 1)
                        {
                            bool finded = false;
                            foreach (PttModel a in pttMatch)
                            {
                                if (a.Name == m.ID)
                                {
                                    m.ptt = a;
                                    finded = true;
                                }
                            }
                            if (!finded) m.ptt = pttMatch[0];
                        }
                        else
                        {
                            m.ptt = pttMatch[0];
                        }
                    }
                    else m.ptt = new PttModel() { Name = "", pst = 0.0f, prs = 0.0f, ftr = 0.0f, byd = 0.0f };

                    current.Add(m);
                }
                songs = current.ToArray();
                Console.WriteLine("Read songs=" + songs.Length);
            }
        }
        public static bool UpdatePtts(SongModel[] list)
        {
            JObject newOutput = new JObject();

            foreach (SongModel m in list) {
                newOutput.Add(m.ID, new JObject
                {
                    { "pst",m.ptt.pst},
                    { "prs",m.ptt.prs},
                    { "ftr",m.ptt.ftr},
                    { "byd",m.ptt.byd}
                });
            }
            try
            {
                File.WriteAllText(FileSystem.AppDataDirectory + "/ptt.json", JsonConvert.SerializeObject(newOutput));
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
