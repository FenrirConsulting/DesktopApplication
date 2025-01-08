using Nager.Date;
using System;
using System.Windows;

namespace IAMHeimdall.Core
{
    public class TimespanCalculation
    {
        public TimespanCalculation()
        {

        }

        // Calculate a Time Span, Removing Weekend and Holidays from the Calculated TimeSpan
        public static DaySpan ComputeDaysDifference(DateTime FromDate, DateTime ToDate, bool WorkOnSaturday = false, bool WorkOnSunday = false)
        {

            DateTime TDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, FromDate.Hour, FromDate.Minute, FromDate.Second, FromDate.Millisecond);
            if (ToDate.Year == 1900)
            {
                ToDate = DateTime.Now;
            }

            int DaysOverdue = 0;
            int HoursOverdue = 0;
            int MinsOverdue = 0;
            int SecsOverdue = 0;

            // Time Difference between FromDate and ToDate in TimeSpan format
            TimeSpan difference = ToDate.Subtract(FromDate);

            // subtract the bank holidays
            DateTime dummy = new DateTime(1900, 1, 1, 0, 0, 0);
            DateTime nextDummyDay = dummy.AddDays(1); // AddDays() to add number of days
            DateTime nextDummyHour = dummy.AddHours(1); // AddHours() to add number of hours
            TimeSpan timespanOfOneDay = nextDummyDay.Subtract(dummy);
            TimeSpan timespanOfOneHour = nextDummyHour.Subtract(dummy);

            try
            {
                DateTime intermediate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day);

                while (intermediate < ToDate)
                {
                    if (intermediate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (!WorkOnSaturday)
                        {
                            difference = difference - timespanOfOneDay;
                        }
                    }

                    if (intermediate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (!WorkOnSunday)
                        {
                            difference = difference - timespanOfOneDay;
                        }
                    }

                    intermediate = intermediate.AddDays(1);
                }


                var publicHolidays = DateSystem.GetPublicHolidays(FromDate, ToDate, CountryCode.US);
                foreach (var publicHoliday in publicHolidays)
                {
                    difference = difference - timespanOfOneDay;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            finally
            {
                DaysOverdue = difference.Days;
                HoursOverdue = difference.Hours;
                MinsOverdue = difference.Minutes;
                SecsOverdue = difference.Seconds;
            }

            return new DaySpan { Days = DaysOverdue, Hours = HoursOverdue, Minutes = MinsOverdue, Seconds = SecsOverdue };
        }

        // Custom DaySpan to be returned by ComputeDaysDifference()
        public struct DaySpan
        {
            public int Days;
            public int Hours;
            public int Minutes;
            public int Seconds;
        }
    }
}
