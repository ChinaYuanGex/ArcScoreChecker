using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcaeaScoreChecker.Models;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;

namespace ArcaeaScoreChecker.ImageDraw
{
    public class ScoreImage
    {
        /* 别看了，这里还没有做出来的想法 */
        public List<ScoreModel[]> scoreBlocks;

        //限制列数
        public int ColumnLimit = 3;
        public ScoreImage() {
            scoreBlocks = new List<ScoreModel[]>();
        }
        /// <summary>
        /// 添加一个分数区块
        /// </summary>
        public void AddScoreBlock(ScoreModel[] scores) {
            scoreBlocks.Add(scores);
        }

    }
}
