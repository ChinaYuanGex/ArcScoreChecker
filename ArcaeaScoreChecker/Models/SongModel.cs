using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker.Models
{
    public class SongModel
    {
        public string ID { get; set; }
        public string SepicalDisplay(SongDiff diff) {
            switch (diff) {
                case SongDiff.Pst:
                    return Display_pst;
                case SongDiff.Prs:
                    return Display_prs;
                case SongDiff.Ftr:
                    return Display_ftr;
                case SongDiff.Byd:
                    return Display_byd;
                default:
                    return Display;
            }
        }
        public string Display { get; set; }
        public string display_pst { get; set; }
        public string Display_pst
        {
            get
            {
                if (display_pst != null) return display_pst;
                else return Display;
            }
        }
        public string display_prs { get; set; }
        public string Display_prs
        {
            get
            {
                if (display_prs != null) return display_prs;
                else return Display;
            }
        }

        public string display_ftr { get; set; }
        public string Display_ftr
        {
            get
            {
                if (display_ftr != null) return display_ftr;
                else return Display;
            }
        }

        public string display_byd { get; set; }
        public string Display_byd
        {
            get
            {
                if (display_byd != null) return display_byd;
                else return Display;
            }
        }
        public string TitleImageBase { get; set; }

        public string SepicalTitleImage_base64(SongDiff diff) {
            switch (diff) {
                case SongDiff.Pst:
                    return pstTitleImage_base64;
                case SongDiff.Prs:
                    return prsTitleImage_base64;
                case SongDiff.Ftr:
                    return ftrTitleImage_base64;
                case SongDiff.Byd:
                    return bydTitleImage_base64;
                default:
                    return DefaultTitleImage;
            }
        }

        public string BaseTitleImage_base64
        {
            get
            {
                if (File.Exists(TitleImageBase + "/base.jpg"))
                {
                    return Convert.ToBase64String(File.ReadAllBytes(TitleImageBase + "/base.jpg"));
                }
                else return Convert.ToBase64String(File.ReadAllBytes(DefaultTitleImage));
            }
        }
        public string pstTitleImage_base64
        {
            get
            {
                if (File.Exists(TitleImageBase + "/0.jpg"))
                {
                    return Convert.ToBase64String(File.ReadAllBytes(TitleImageBase + "/0.jpg"));
                }
                else return BaseTitleImage_base64;
            }
        }
        public string prsTitleImage_base64
        {
            get
            {
                if (File.Exists(TitleImageBase + "/1.jpg"))
                {
                    return Convert.ToBase64String(File.ReadAllBytes(TitleImageBase + "/1.jpg"));
                }
                else return BaseTitleImage_base64;
            }
        }
        public string ftrTitleImage_base64
        {
            get
            {
                if (File.Exists(TitleImageBase + "/2.jpg"))
                {
                    return Convert.ToBase64String(File.ReadAllBytes(TitleImageBase + "/2.jpg"));
                }
                else return BaseTitleImage_base64;
            }
        }
        public string bydTitleImage_base64
        {
            get
            {
                if (File.Exists(TitleImageBase + "/3.jpg"))
                {
                    return Convert.ToBase64String(File.ReadAllBytes(TitleImageBase + "/3.jpg"));
                }
                else return BaseTitleImage_base64;
            }
        }
        public string SepicalTitleImagePath(SongDiff diff) {
            switch (diff) {
                case SongDiff.Pst:
                    return pstTitleImagePath;
                case SongDiff.Prs:
                    return prsTitleImagePath;
                case SongDiff.Ftr:
                    return ftrTitleImagePath;
                case SongDiff.Byd:
                    return bydTitleImagePath;
                default:
                    return baseTitleImagePath;
            }
        }
        public string baseTitleImagePath
        {
            get
            {
                if (File.Exists(TitleImageBase + "/base.jpg"))
                {
                    return TitleImageBase + "/base.jpg";
                }
                else return DefaultTitleImage;
            }
        }
        public string pstTitleImagePath {
            get
            {
                if (File.Exists(TitleImageBase + "/0.jpg"))
                {
                    return TitleImageBase + "/0.jpg";
                }
                else return baseTitleImagePath;
            }
        }
        public string prsTitleImagePath
        {
            get
            {
                if (File.Exists(TitleImageBase + "/1.jpg"))
                {
                    return TitleImageBase + "/1.jpg";
                }
                else return baseTitleImagePath;
            }
        }
        public string ftrTitleImagePath
        {
            get
            {
                if (File.Exists(TitleImageBase + "/2.jpg"))
                {
                    return TitleImageBase + "/2.jpg";
                }
                else return baseTitleImagePath;
            }
        }
        public string bydTitleImagePath
        {
            get
            {
                if (File.Exists(TitleImageBase + "/3.jpg"))
                {
                    return TitleImageBase + "/3.jpg";
                }
                else return baseTitleImagePath;
            }
        }

        public string SepicalArtist(SongDiff diff) {
            switch (diff) {
                case SongDiff.Pst:
                    return Artist_pst;
                case SongDiff.Prs:
                    return Artist_prs;
                case SongDiff.Ftr:
                    return Artist_ftr;
                case SongDiff.Byd:
                    return Artist_byd;
                default:
                    return Artist;
            }
        }

        public string Artist { get; set; }
        public string artist_pst { get; set; }
        public string Artist_pst
        {
            get
            {
                if (artist_pst != null) return artist_pst;
                else return Artist;
            }
        }

        public string artist_prs { get; set; }
        public string Artist_prs
        {
            get
            {
                if (artist_prs != null) return artist_prs;
                else return Artist;
            }
        }
        public string artist_ftr { get; set; }
        public string Artist_ftr
        {
            get
            {
                if (artist_ftr != null) return artist_ftr;
                else return Artist;
            }
        }
        public string artist_byd { get; set; }
        public string Artist_byd
        {
            get
            {
                if (artist_byd != null) return artist_byd;
                else return Artist;
            }
        }
        public readonly static string DefaultTitleImage = FileSystem.AppDataDirectory + "/songs/random/base.jpg";
        public readonly static string DefaultTitleImageBase = FileSystem.AppDataDirectory + "/songs/random";
        public readonly static string DefaultTitleImageBase64 = Convert.ToBase64String(File.ReadAllBytes(DefaultTitleImage));
        public PttModel ptt { get; set; }
        public DateTime time { get; set; }
    }
}
