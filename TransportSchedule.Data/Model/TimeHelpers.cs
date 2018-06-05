using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportSchedule.Data
{
    class TimeHelpers {
        public const int MinutesInADay = 24 * 60;

        public static bool IsTimeWithinInterval(int time, int begin,
            int end) {

            return (begin <= end && time >= begin && time <= end) ||
                (begin > end && (time > begin || time < end));
        }

        public static int AddMinutes(int time, int minutes) {
            time += minutes;
            if (time >= MinutesInADay)
                time -= MinutesInADay;
            return time;
        }
        
        public static int Difference(int time1, int time2) {
            int result = time2 - time1;
            if (result < 0)
                result += MinutesInADay;
            return result;
        }

    }
}
