using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaeaScoreChecker.Tools
{
    public class Timestamp
    {
        public readonly static DateTime Zero = new DateTime(1970, 1, 1, 0, 0, 0);
        public DateTime time { get; set; }
        public long timestamp
        {
            get
            {
                return (time.Ticks - Zero.Ticks) / TimeSpan.TicksPerSecond;
            }
        }
        public long timestamp_mill
        {
            get {
                return (time.Ticks - Zero.Ticks) / TimeSpan.TicksPerMillisecond;
            }
        }
        /// <summary>
        /// 根据给出的时间生成一个实例
        /// </summary>
        /// <param name="time"></param>
        public Timestamp(DateTime time) {
            this.time = time;
        }
        /// <summary>
        /// 以当前时间生成一个实例
        /// </summary>
        public Timestamp() {
            time = DateTime.Now;
        }
        /// <summary>
        /// 以unix时间戳生成一个实例,单位为毫秒
        /// </summary>
        /// <param name="timestamp"></param>
        public Timestamp(long timestamp) {
            time = Zero.AddMilliseconds(timestamp);
        }
        public Timestamp ToLocal() {
            return new Timestamp(time.ToLocalTime());
        }
        public Timestamp ToUTC() { 
            return new Timestamp(time.ToUniversalTime());
        }
    }
}
