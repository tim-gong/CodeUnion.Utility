using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeUnion.Utility.TimeOperator
{
    public static class Tools
    {
        /// <summary>
        /// 根据时间获取当前是第几周
        /// 如果12月31号与下一年的1月1好在同一个星期则算下一年的第一周
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime dtime)
        {
            DateTime dtmFisrtDay = new DateTime(dtime.Year, 1, 1);//获取这一年的1月1号
            int intweek = 0;
            switch (dtmFisrtDay.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    intweek = 7;
                    break;
                case DayOfWeek.Friday:
                    intweek = 6;
                    break;
                case DayOfWeek.Thursday:
                    intweek = 5;
                    break;
                case DayOfWeek.Wednesday:
                    intweek = 4;
                    break;
                case DayOfWeek.Tuesday:
                    intweek = 3;
                    break;
                case DayOfWeek.Monday:
                    intweek = 2;
                    break;
                case DayOfWeek.Sunday:
                    intweek = 1;
                    break;
            }
            int intNow = dtime.DayOfYear; //当前是本年度的第几天
            int intOutPut = 1;
            intOutPut = (intNow + intweek - 1) / 7;
            int intLoop = (intNow + intweek - 1) % 7;
            if (intLoop != 0)
            {
                intOutPut = intOutPut + 1;
            }
            return intOutPut;
        }
    }
}
