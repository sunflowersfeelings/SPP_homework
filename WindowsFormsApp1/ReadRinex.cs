using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ReadRinex//only use for read file
    {
        public enum dataNum
        {
            G=15,
            S=3,
            R=15,
            E=15,
            J=15,
            C=18,
            I=6
        }

        public void readO(int j, string[] strlines, List<Form1.epoch> epoch)
        {
            //int epoch_line = (strlines.Length - j)/8;
            for (;j< strlines.Length-1;)
            {             
                Form1.epoch obs = new Form1.epoch();
                obs.y = Convert.ToInt32(strlines[j].Substring(1, 5));
                obs.m = Convert.ToInt32(strlines[j].Substring(6, 3));
                obs.d = Convert.ToInt32(strlines[j].Substring(9, 3));
                obs.h= Convert.ToInt32(strlines[j].Substring(12, 3));
                obs.min = Convert.ToInt32(strlines[j].Substring(15, 3));
                obs.sec = Convert.ToDouble(strlines[j].Substring(20, 9));
                obs.p_flag = Convert.ToInt32(strlines[j].Substring(31, 1));
                obs.sat_num = Convert.ToInt32(strlines[j].Substring(33, 2));
                j++;
                for (int k=j;k<j+ obs.sat_num; k++)
                {
                    Form1.epochbody obs_data = new Form1.epochbody();                    
                    string[] newstr = strlines[k].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    obs_data.sPRN = newstr[0];
                    string types_of_Satelite = strlines[j].Substring(0, 1);
                    int datanum = 0;
                    if (types_of_Satelite == "G")
                        datanum =(int) dataNum.G;
                    else if (types_of_Satelite == "J")
                        datanum = (int)dataNum.J;
                    else if (types_of_Satelite == "E")
                        datanum = (int)dataNum.E;
                    else if (types_of_Satelite == "C")
                        datanum = (int)dataNum.C;
                    else if (types_of_Satelite == "I")
                        datanum = (int)dataNum.I;
                    else if (types_of_Satelite == "R")
                        datanum = (int)dataNum.R;
                    else if (types_of_Satelite == "S")
                        datanum = (int)dataNum.S;
                   // for (int i=1;i<newstr.Length;i++)
                    //{

                      //  obs_data.data.Add( Convert.ToDouble(newstr[i]));                      
                    //}
                    obs_data.C1 = Convert.ToDouble(newstr[1]);
                   obs.Obs_Data.Add(obs_data);
                }
                epoch.Add(obs);
                j = j + obs.sat_num;
            }
        }
        private void ProcessG()
        {

        }
        private void ProcessJ()
        {

        }
        private void ProcessR()
        {

        }
        private void ProcessS()
        {

        }
        private void ProcessE()
        {

        }
        private void ProcessC()
        {

        }
        private void ProcessI()
        {

        }
        /*  public void readN(int j,string[] strlines, ref int EPHEMERISBLOCKNum,List<Form1. EPHEMERISBLOCK> Satelite)
          {
              ProcessData processdata=new ProcessData();
              for (; j < strlines.Length; j = j + 3)
              {
                  Form1.EPHEMERISBLOCK satelite = new Form1.EPHEMERISBLOCK();
                  satelite.PRN = Convert.ToInt32(strlines[j].Substring(0, 2));
                  satelite.year = Convert.ToInt32(strlines[j].Substring(2, 3));
                  satelite.mouth = Convert.ToInt32(strlines[j].Substring(5, 3));
                  satelite.day = Convert.ToInt32(strlines[j].Substring(8, 3));
                  satelite.hour = Convert.ToInt32(strlines[j].Substring(11, 3));
                  satelite.minutes = Convert.ToInt32(strlines[j].Substring(14, 3));
                  satelite.second = Convert.ToDouble(strlines[j].Substring(17, 5));
                  satelite.a0 =processdata. ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.a1 = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.a2 = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  j++;
                  satelite.IODE = processdata.ChangeDataToD(strlines[j].Substring(3, 19));
                  satelite.Crs = processdata.ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.Deltan = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.M0 = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  j++;

                  satelite.Cuc = processdata.ChangeDataToD(strlines[j].Substring(3, 19));
                  satelite.e = processdata.ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.Cus = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.SqrtA = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  j++;
                  satelite.Toe = processdata.ChangeDataToD(strlines[j].Substring(3, 19));
                  satelite.Cic = processdata.ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.OMEGA = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.Cis = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  j++;
                  satelite.i0 = processdata.ChangeDataToD(strlines[j].Substring(3, 19));
                  satelite.Crc = processdata.ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.omega = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.deltaOMEGA = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  j++;
                  satelite.IDOT = processdata.ChangeDataToD(strlines[j].Substring(3, 19));
                  satelite.L2C = processdata.ChangeDataToD(strlines[j].Substring(22, 19));
                  satelite.GpsWeekNumber = processdata.ChangeDataToD(strlines[j].Substring(41, 19));
                  satelite.L2P = processdata.ChangeDataToD(strlines[j].Substring(60, 19));

                  Satelite.Add(satelite);

                  EPHEMERISBLOCKNum++;

              }
          }*/
        public void readN(int j, string[] strlines, ref int EPHEMERISBLOCKNum, List<Form1.EPHEMERISBLOCK> Satelite)
        {
            ProcessData processdata = new ProcessData();
            for (; j < strlines.Length;)
            {
                Form1.debug=j;
                if (j ==80)
                    ;
                          
                Form1.EPHEMERISBLOCK satelite = new Form1.EPHEMERISBLOCK();

                satelite.PRN = strlines[j].Substring(0, 3);               
                satelite.year = Convert.ToInt32(strlines[j].Substring(4, 5));
                satelite.mouth = Convert.ToInt32(strlines[j].Substring(9, 2));
                satelite.day = Convert.ToInt32(strlines[j].Substring(12,2));
                satelite.hour = Convert.ToInt32(strlines[j].Substring(15, 2));
                satelite.minutes = Convert.ToInt32(strlines[j].Substring(18, 2));
                satelite.second = Convert.ToDouble(strlines[j].Substring(21, 2));
                satelite.a0 = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                satelite.a1 = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                satelite.a2 = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

                j++;
                satelite.IODE = processdata.ChangeDataToD(strlines[j].Substring(4, 20));
                satelite.Crs = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                satelite.Deltan = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                satelite.M0 = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

                j++;

                satelite.Cuc = processdata.ChangeDataToD(strlines[j].Substring(4, 19));
                satelite.e = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                satelite.Cus = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                satelite.SqrtA = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

                j++;

                satelite.Toe = processdata.ChangeDataToD(strlines[j].Substring(4, 19));
                satelite.Cic = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                satelite.OMEGA = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                satelite.Cis = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

                /*  j++;
                  satelite.i0 = processdata.ChangeDataToD(strlines[j].Substring(4, 19));
                  satelite.Crc = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                  satelite.omega = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                  satelite.deltaOMEGA = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

                  j++;
                  satelite.IDOT = processdata.ChangeDataToD(strlines[j].Substring(4, 19));
                  satelite.L2C = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
                  //satelite.GpsWeekNumber = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
                //  satelite.L2P = processdata.ChangeDataToD(strlines[j].Substring(61, 19));
                */
                string types_of_Satelite = satelite.PRN.Substring(0, 1);
                if (types_of_Satelite == "R")
                    j++;
                else
                    j = j + 5;

                Satelite.Add(satelite);
                
                EPHEMERISBLOCKNum++;

            }
        }
    }
}
