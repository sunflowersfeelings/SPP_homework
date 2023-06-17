using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ProcessData
    {
        public double Calendar2GpsTime(int nYear, int nMounth, int nDay, int nHour, int nMinute, double dSecond)
        {
            int DayofMonth = 0;
            int DayofYear = 0;
            nYear = nYear + 2000;
            int weekno = 0;
            int dayofweek;
            int m;
            double WeekSecond;
            if (nYear < 1980 || nMounth < 1 || nMounth > 12 || nDay < 1 || nDay > 31) return -1;
            //计算从1980年到当前的前一年的天数
            for (m = 1980; m < nYear; m++)
            {
                if ((m % 4 == 0 && m % 100 != 0) || (m % 400 == 0))
                {
                    DayofYear += 366;
                }
                else
                    DayofYear += 365;
            }
            //计算当前一年内从元月到当前前一月的天数
            for (m = 1; m < nMounth; m++)
            {
                if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
                    DayofMonth += 31;
                else if (m == 4 || m == 6 || m == 9 || m == 11)
                    DayofMonth += 30;
                else if (m == 2)
                {
                    if ((nYear % 4 == 0 && nYear % 100 != 0) || (nYear % 400 == 0))
                        DayofMonth += 29;
                    else
                        DayofMonth += 28;

                }
            }
            DayofMonth = DayofMonth + nDay - 6;//加上当月天数/减去1980年元月的6日		
            weekno = (DayofYear + DayofMonth) / 7;//计算GPS周
            dayofweek = (DayofYear + DayofMonth) % 7;
            //计算GPS 周秒时间
            WeekSecond = dayofweek * 86400 + nHour * 3600 + nMinute * 60 + dSecond;
            return WeekSecond;


        }
        public double ChangeDataToD(string strData)
        {           
            char[] delimiterChars = { 'D' };
            string[] newstr = strData.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            double data;
            data = Convert.ToDouble(newstr[0]) * Math.Pow(10, Convert.ToDouble(newstr[1]));
            return data;
        }
        public void xyz2BLH(double X, double Y, double Z, ref double B, ref double L, ref double H)//xyz坐标转BLH坐标
        {
            double R = Math.Sqrt(X * X + Y * Y);
            double B0 = Math.Atan2(Z, R);
            double B_ = 0;
            double L_ = 0;

            double a = 6378245;
            double f = 1 / 298.3;
            double N = 0;
            while (true)
            {
                N = a / Math.Sqrt(1.0 - f * (2 - f) * Math.Sin(B0) * Math.Sin(B0));
                B_ = Math.Atan2(Z + N * f * (2 - f) * Math.Sin(B0), R);
                if (Math.Abs(B_ - B0) < 1.0e-7)
                    break;
                B0 = B_;             
            }
            L_ = Math.Atan2(Y, X);
            H = R / Math.Cos(B_) - N;

            B = B_;
            L = L_;

        }
    }
}
