using Schedule.Expressions;
using Schedule.Parser;
using System;

namespace Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            var schedule = new ScheduleParser().ParseSchedule("*.*.13 5 0:0:0.0");

            var now = DateTime.UtcNow;

            while (schedule.Find(now.AddMilliseconds(1), ref now, true))
            {
                Console.WriteLine(now);
            }
            Console.WriteLine("Done");
        }
    }
}
