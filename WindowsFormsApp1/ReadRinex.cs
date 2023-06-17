using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ReadRinex//only use for read file
    {
     
        public void readO(int j, string[] strlines, List<Form1.epoch> epoch)
        {
            //int epoch_line = (strlines.Length - j)/8;
            for (;j< strlines.Length-1;)
            {
                if (strlines[j] == "") 
                break;
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
                    
                     
                    for (int i=0;i<newstr.Length-1;)
                    {
                        i++;
                        if (Convert.ToDouble(newstr[i]) % 1 == 0)
                            continue;
                        obs_data.data.Add( Convert.ToDouble(newstr[i]));                      
                    }
                    obs_data.C1 = Convert.ToDouble(newstr[1]);
                   obs.Obs_Data.Add(obs_data);
                }
                epoch.Add(obs);
                j = j + obs.sat_num;
            }
        }
        private void ProcessGJESCI(ref int j,string[] strlines, Form1.EPHEMERISBLOCK satelite)
        {
            j++;
            ProcessData processdata = new ProcessData();

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

            j++;
            satelite.i0 = processdata.ChangeDataToD(strlines[j].Substring(4, 19));
            satelite.Crc = processdata.ChangeDataToD(strlines[j].Substring(23, 19));
            satelite.omega = processdata.ChangeDataToD(strlines[j].Substring(42, 19));
            satelite.deltaOMEGA = processdata.ChangeDataToD(strlines[j].Substring(61, 19));

            j++;
            satelite.IDOT = processdata.ChangeDataToD(strlines[j].Substring(4, 19));         
        }
        private void ProcessR(ref int j, string[] strlines, Form1.EPHEMERISBLOCK satelite)
        {
            j++;
            ProcessData processdata = new ProcessData();

            //satelite.X = processdata.ChangeDataToD(strlines[j].Substring(4, 20));
           
            j++;

          //  satelite.Y= processdata.ChangeDataToD(strlines[j].Substring(4, 19));          

            j++;

            //satelite.Z = processdata.ChangeDataToD(strlines[j].Substring(4, 19));         
        }
             
        public void readN(int j, string[] strlines, ref int EPHEMERISBLOCKNum, List<Form1.EPHEMERISBLOCK> Satelite)//导航文件里没有SBAS卫星数据，因此只需要对R进行特殊处理即可
        {
            ProcessData processdata = new ProcessData();
            for (; j < strlines.Length;)
            {
               
               
                          
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


                string types_of_Satelite = satelite.PRN.Substring(0, 1);
                if (types_of_Satelite == "R")
                {
                    ProcessR(ref j, strlines, satelite);
                    j++;
                }
                else
                {
                    ProcessGJESCI(ref j, strlines, satelite);
                    j = j + 3;
                }

                Satelite.Add(satelite);
                
                EPHEMERISBLOCKNum++;

            }
        }
    }
}
