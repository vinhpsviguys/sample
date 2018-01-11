using System;
namespace CoreLib
{
    public delegate void doJob(long times, int timesLimit);

    public class Timeouter
    {
        private long timeout = 0;
        private int timesLimit = 0;
        private doJob jobsToDo;
        private long lastMoment = 0;
        private long times = -1;

        public Timeouter(long timoutInMs, int times, doJob jobs)
        {
            this.timeout = timoutInMs;
            this.timesLimit = times;
            this.jobsToDo = jobs;
            this.lastMoment = CurrentTimeMillis() - timeout;
        }

		private static readonly DateTime Jan1st1970 = new DateTime
	(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

        public void update() {
            
            if (CurrentTimeMillis() - lastMoment >= timeout) {
                times++;
                if (times <= timesLimit)
                    jobsToDo(times, timesLimit);
                lastMoment = CurrentTimeMillis();
            }
        }

        public long getRemainingTime() {
            return Math.Max(0, timeout - (CurrentTimeMillis() - lastMoment));
        }

        public void end() {
            this.times = timesLimit;
        }

        public bool isEnd() {
            return times >= timesLimit;
        }
    }
}
