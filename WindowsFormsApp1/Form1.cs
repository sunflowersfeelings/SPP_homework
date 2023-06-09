﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing.Drawing2D;
using MathNet.Numerics.LinearAlgebra;
//基本广播星历块

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        ReadRinex readrinex;
        int EPHEMERISBLOCKNum = 0;
        public Form1()
        {
            InitializeComponent();
        }

       public static int debug;
        double GM = 3.986005e14;
        double ae = 6378136;
        double Oe = 7.2921151467e-5;
        double Oref =-2.6e-9;
        double Aref = 26559710;
        double J0_2 = 1082625.7e-9;
        double c = 299792458; //光速常数
        double u = 3.986004418e14;//地球引力常数
        double w = 7.2921151467e-5;
        //读取观测值文MO时读取的头文件信息存储在下列变量中
        double obs_X, obs_Y, obs_Z;//观测站大致的位置       
        double deltaH, deltaE, deltaN;//天线高、天线中心相对于测站标志在东向偏移量、北向偏移量
        double FL1, FL2;// 缺省的L1和L2载波的波长因子
        double Interval;//历元间隔

     
        public class epoch
        {
            //观测历元时刻
            public int y;
            public int m;
            public int d;
            public int h;
            public int min;
            public double sec;
            public int p_flag;//历元标志
            public int sat_num;//当前历元所观测到的卫星数量
            //public List<int>sPRN=new List<int>();//当前历元所能观测到的卫星的PRN列表
            public List<epochbody> Obs_Data = new List<epochbody>();
        }
        List<epoch> pobs_epoch = new List<epoch>();
        public class epochbody
        {
            public string sPRN;
            public List<double> data = new List<double>();
            public double C1;//伪距值
        }

        public class EPHEMERISBLOCK//每小时一个卫星对应一个基本星历块
        {
            //PRN号 
            public string PRN;
            public double a0, a1, a2;//时间改正数
            public int year, mouth, day,hour, minutes;//年月日时分秒
            public double second;

            //七个轨道参数
            public double IODE, Crs, Deltan, M0;// ORBIT - 1
            public double Cuc, e, Cus, SqrtA;// ORBIT - 2
            public double Toe, Cic, OMEGA, Cis;// ORBIT - 3
            public double i0, Crc, omega, deltaOMEGA;// ORBIT - 4
            public double IDOT, GpsWeekNumber, L2C, L2P;// ORBIT - 5
            public double SVaccuracy, SVhealth, TGD, IODC;// ORBIT - 6
            public double Transmisstion, Fit;//ORBIR-7

            public double Elevarting;//截止高度角；
            public double Azimuth;//方位角
        };
        public class Position
        {
            public double x;
            public double y;
            public double z;
        }
        List<Position> sat_pos = new List<Position>();
        private void Form1_Load(object sender, EventArgs e)
        {
            Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Satelite.Count > 0 && pobs_epoch.Count > 0)
            {
                CBroadCast(EPHEMERISBLOCKNum);
                SPP(EPHEMERISBLOCKNum);
            }
        }

        // EPHEMERISBLOCK[] satelite = new EPHEMERISBLOCK[423];
        List<EPHEMERISBLOCK> Satelite = new List<EPHEMERISBLOCK>();
      
        public void CBroadCast(int EPHEMERISBLOCKNum)
        {
            double A,tk,t,n,n0, Mk, Ek, Ek0,v1,v2,vk,fik;
            double uk,rk,ik,deltauk, deltark, deltaik,Omegak;
            double xk, yk,Xk,Yk,Zk;
          //  FileStream output = new FileStream("output.txt", FileMode.Create, FileAccess.ReadWrite);
           // StreamWriter sw = new StreamWriter(output);
            ProcessData processdata = new ProcessData();
           
            for (int i=0;i<EPHEMERISBLOCKNum;i++)
            {
               
                A = Math.Pow(Satelite[i].SqrtA, 2);
                n0 =Math.Sqrt( GM / Math.Pow(A, 3));
                n = Satelite[i].Deltan + n0;
                t =processdata.Calendar2GpsTime(Satelite[i].year, Satelite[i].mouth, Satelite[i].day, Satelite[i].hour, Satelite[i].minutes, Satelite[i].second);
                tk =t - Satelite[i].Toe;
                Mk = Satelite[i].M0 + tk * n;
                Ek0 = 0;         
                Ek = Mk;
                //从角度到弧度的转化
                
                while (Math.Abs(Ek-Ek0)>1e-15)
                {
                    Ek0 = Ek;
                    Ek = Mk +Satelite[i].e*Math.Sin(Ek0);
                }
                v1 =Math.Sqrt( 1 - Math.Pow(Satelite[i].e, 2)) * Math.Sin(Ek);
                v2 = Math.Cos(Ek) - Satelite[i].e;
                vk = Math.Atan(v1/v2);
                if (v1 > 0 && v2 < 0)
                    vk = vk + Math.PI;
                else if (v1 < 0 && v2 < 0)
                    vk = vk - Math.PI;


               
                fik = vk + Satelite[i].omega;

                deltauk = Satelite[i].Cus * Math.Sin(2 * fik) + Satelite[i].Cuc * Math.Cos(2 * fik);
                deltark = Satelite[i].Crs * Math.Sin(2 * fik) + Satelite[i].Crc * Math.Cos(2 * fik);
                deltaik = Satelite[i].Cis * Math.Sin(2 * fik) + Satelite[i].Cic * Math.Cos(2 * fik);

                uk = fik + deltauk;
                rk = A * (1 - Satelite[i].e * Math.Cos(Ek)) + deltark;
                ik = Satelite[i].i0 + Satelite[i].IDOT * tk  + deltaik;

                
                xk = rk * Math.Cos(uk);
                yk = rk * Math.Sin(uk);

               
              

                Omegak = Satelite[i].OMEGA + (Satelite[i].deltaOMEGA - Oe) * tk - Oe * Satelite[i].Toe;
               
                Xk = xk * Math.Cos(Omegak) - yk * Math.Cos(ik) * Math.Sin(Omegak);
                Yk = xk * Math.Sin(Omegak) + yk * Math.Cos(ik) * Math.Cos(Omegak);
                Zk = yk * Math.Sin(ik);

                Position temp_sat = new Position();
                temp_sat.x = Xk;
                temp_sat.y = Yk;
                temp_sat.z = Zk;
                sat_pos.Add(temp_sat);
               
            }

        }

        public void C_xyz(double t, EPHEMERISBLOCK satelite, ref double Xk, ref double Yk, ref double Zk)
        {
            double A, tk, n, n0, Mk, Ek, Ek0, v1, v2, vk, fik;
            double uk, rk, ik, deltauk, deltark, deltaik, Omegak;
            double xk, yk;
            //  FileStream output = new FileStream("output.txt", FileMode.Create, FileAccess.ReadWrite);
            // StreamWriter sw = new StreamWriter(output);
            ProcessData processdata = new ProcessData();
          
       
                A = Math.Pow(satelite.SqrtA, 2);
                n0 = Math.Sqrt(GM / Math.Pow(A, 3));
                n = satelite.Deltan + n0;               
                tk = t - satelite.Toe;
                Mk = satelite.M0 + tk * n;
                Ek0 = 0;
                Ek = Mk;
                //从角度到弧度的转化

                while (Math.Abs(Ek - Ek0) > 1e-10)
                {
                    Ek0 = Ek;
                    Ek = Mk + satelite.e * Math.Sin(Ek0);
                }
                v1 = Math.Sqrt(1 - Math.Pow(satelite.e, 2)) * Math.Sin(Ek);
                v2 = Math.Cos(Ek) - satelite.e;
                vk = Math.Atan(v1 / v2);
                if (v1 > 0 && v2 < 0)
                    vk = vk + Math.PI;
                else if (v1 < 0 && v2 < 0)
                    vk = vk - Math.PI;



                fik = vk + satelite.omega;

                deltauk = satelite.Cus * Math.Sin(2 * fik) + satelite.Cuc * Math.Cos(2 * fik);
                deltark = satelite.Crs * Math.Sin(2 * fik) + satelite.Crc * Math.Cos(2 * fik);
                deltaik = satelite.Cis * Math.Sin(2 * fik) + satelite.Cic * Math.Cos(2 * fik);

                uk = fik + deltauk;
                rk = A * (1 - satelite.e * Math.Cos(Ek)) + deltark;
                ik = satelite.i0 + satelite.IDOT * tk + deltaik;


                xk = rk * Math.Cos(uk);
                yk = rk * Math.Sin(uk);




                Omegak = satelite.OMEGA + (satelite.deltaOMEGA - Oe) * tk - Oe * satelite.Toe;

                Xk = xk * Math.Cos(Omegak) - yk * Math.Cos(ik) * Math.Sin(Omegak);
                Yk = xk * Math.Sin(Omegak) + yk * Math.Cos(ik) * Math.Cos(Omegak);
                Zk = yk * Math.Sin(ik);
             
        }

        private void CR(double xi, double yi, double zi, double vx, double vy, double vz, double xls, double yls, double zls
            ,ref double f1,ref double f2,ref double f3)
        { 
           double r = Math.Sqrt(xi * xi + yi * yi + zi * zi);
           f1 = -GM * xi / (r *r*r)  - 1.5 * J0_2 * GM * ae * ae * xi * (1 - 5 * zi * zi / (r * r)) /Math.Pow(r,5) + w * w * xi + 2 * w * vy + xls;
           f2 = -GM * yi / (r * r * r) - 1.5 * J0_2 * GM * ae * ae * yi * (1 - 5 * zi * zi / (r * r)) / Math.Pow(r, 5) + w * w * yi - 2 * w * vx + yls;
           f3 = -GM * zi / (r * r * r) - 1.5 * J0_2 * GM * ae * ae * zi * (3 - 5 * zi * zi / (r * r)) / Math.Pow(r, 5) + zls;
           
       }      
        private int foundEpoch_Ephemeris(epoch obs,epochbody obs_data)
        {
            double t,toe;
            ProcessData processdata = new ProcessData();
            t = processdata.Calendar2GpsTime(obs.y, obs.m, obs.d, obs.h, obs.min, obs.sec);//观测数据的历元信息  
            //查找相应的卫星星历
            for (int i = 0; i < Satelite.Count; i++)
            {
                toe = Satelite[i].Toe;
                if (obs_data.sPRN == Satelite[i].PRN && (t - toe)<=3600)
                    return i;
            }
            return -1;
        }
        private double SPP_Trop(double H,double E)//对流层延迟
        {

            double Trop;
            double T = 288.15 - 6.5 * H * 0.001;
            double P = 1013.25 * Math.Pow(288.15 / T, -5.255877);
            double es;
            if (T < 273.16)
                es = Math.Exp(21.3195 - 5327.1157 / T);
            else
                es = Math.Exp(24.3702 - 6162.3496 / T);
            double Dd = 155.2 * 0.0000001 * (P / T) * (40136.0 + 148.72 * (T - 273.16) - H) / Math.Sin(Math.Sqrt(E * E + 6.25) * 3.14159265358979323846 / 180);
            double Dw = 155.2 * 0.0000001 * 4810 * es / (T * T) * (11000 - H) / Math.Sin(Math.Sqrt(E * E + 2.25) * 3.14159265358979323846 / 180);
            return Trop = Dd + Dw;
        }
        //6-13-------------=start--------
        /* troposphere model -----------------------------------------------------------
         * compute tropospheric delay by standard atmosphere and saastamoinen model
         * args   : double b/h          I   接收机天线的纬度和高程
         *          double azimuth      I   卫星方位角
         *          double elevation    I   卫星高度角
         *          double humi         I   相对湿度（目前上海为0.75）
         * return : tropospheric delay (m)
         *-----------------------------------------------------------------------------*/


        private double tropmodel(double b, double h, double azimuth, double elevation, double humi)
        {
            const double temp0 = 15.0; /* temparature at sea level */
            double hgt, pres, temp, e, z, trph, trpw;

            if (h < -100.0 || 1E4 < h || elevation <= 0) return 0.0;

            /* standard atmosphere */
            hgt = h < 0.0 ? 0.0 : h;

            pres = 1013.25 * Math.Pow(1.0 - 2.2557E-5 * hgt, 5.2568);
            temp = temp0 - 6.5E-3 * hgt + 273.16;
            e = 6.108 * humi * Math.Exp((17.15 * temp - 4684.0) / (temp - 38.45));

            /* saastamoninen model */
            z = Math.PI / 2.0 - azimuth;
            trph = 0.0022768 * pres / (1.0 - 0.00266 * Math.Cos(2.0 * b) - 0.00028 * hgt / 1E3) / Math.Cos(z);
            trpw = 0.002277 * (1255.0 / temp + 0.05) * e / Math.Cos(z);
            return trph + trpw;
        }
        //6-13--------------end----------


        public void Angle_Calculate(double B,double L,double H)
        {
            double[,] KK = new double[3, 3];
            double deltX, deltY, deltZ;
            
            KK[0, 0] = -Math.Sin(B) * Math.Cos(L);
            KK[0, 1] = -Math.Sin(B) * Math.Sin(L);
            KK[0, 2] = Math.Cos(B);
            KK[1, 0] = -Math.Sin(L);
            KK[1, 1] = Math.Cos(L);
            KK[1, 2] = 0;
            KK[2, 0] = Math.Cos(B) * Math.Cos(L);
            KK[2, 1] = Math.Cos(B) * Math.Sin(L);
            KK[2, 2] = Math.Sin(B);
            for (int i = 0; i < sat_pos.Count; i++)
            {               
                Position temp_pos = new Position();
                temp_pos = sat_pos[i];
                deltX = KK[0, 0] * (temp_pos.x - obs_X) + KK[0, 1] * (temp_pos.y- obs_Y) + KK[0, 2] * (temp_pos.z - obs_Z);
                deltY = KK[1, 0] * (temp_pos.x - obs_X) + KK[1, 1] * (temp_pos.y - obs_Y) + KK[1, 2] * (temp_pos.z - obs_Z);
                deltZ = KK[2, 0] * (temp_pos.x - obs_X) + KK[2, 1] * (temp_pos.y - obs_Y) + KK[2, 2] * (temp_pos.z - obs_Z);
                Satelite[i].Elevarting = Math.Atan2(deltZ , Math.Sqrt(deltX * deltX + deltY * deltY));
                Satelite[i].Azimuth = Math.Atan2(deltY , deltX);
            }
        }
        private void SPP(int EPHEMERISBLOCKNum)
        {
            ProcessData processdata = new ProcessData();
            double lat=0, lon=0, height=0;
            double humi = 0.75;
            processdata.xyz2BLH(obs_X, obs_Y, obs_Z,ref lat, ref lon,ref height);//根据测站概略位置计算经纬度坐标
            Angle_Calculate(lat, lon, height);
            var X0 = new DenseMatrix(4, 1);
            var X1 = new DenseMatrix(4, 1);

            /*  X0.At(0, 0, obs_X);
              X0.At(1, 0, obs_Y);
              X0.At(2, 0, obs_Z);
              X0.At(3, 0, 0);*/
            X0.At(0, 0, 0);
            X0.At(1, 0, 0);
            X0.At(2, 0, 0);
            X0.At(3, 0, 0);
            X1.At(0, 0, 1);
            X1.At(1, 0, 1);
            X1.At(2, 0, 1);
            double ts0, ts1;
            double dt;
            double delta_ts,delta_tr;//卫星钟差和接收机钟差
            double t, toc;
            double Xk=0, Yk=0, Zk=0,X=0,Y=0,Z=0;
            double pk0=0;
            double ax, ay, az;//A阵里的系数
            double ll;//L阵系数
            for (int j=0;j<pobs_epoch.Count;j++)
            {
                epoch obs = new epoch();
                obs = pobs_epoch[j];
                t = processdata.Calendar2GpsTime(obs.y, obs.m, obs.d, obs.h, obs.min, obs.sec);
                while (Math.Abs(X0.At(0, 0) - X1.At(0, 0)) > 0.001 ||
                      Math.Abs(X0.At(1, 0) - X1.At(1, 0)) > 0.001 ||
                      Math.Abs(X0.At(2, 0) - X1.At(2, 0)) > 0.001)
                {
                    double[] arrayA = { 0, 0, 0, 1 };
                    double[] arrayL = { 0 };
                    Matrix<double> A = CreateMatrix.Dense(1, 4, arrayA);
                    Matrix<double> L = CreateMatrix.Dense(1, 1, arrayL);
                    X1.At(0, 0, X0.At(0, 0));
                    X1.At(1, 0, X0.At(1, 0));
                    X1.At(2, 0, X0.At(2, 0));
                    X1.At(3, 0, X0.At(3, 0));
                    for (int k = 0; k < obs.Obs_Data.Count; k++)
                    {
                        epochbody Obs_Data = new epochbody();
                        Obs_Data = obs.Obs_Data[k];
                        int i = foundEpoch_Ephemeris(obs, Obs_Data);
                        EPHEMERISBLOCK temp_satelite = new EPHEMERISBLOCK();
                        if (i < 0)
                            continue;
                        string types_of_Satelite = Satelite[i].PRN.Substring(0, 1);
                      
                        if (types_of_Satelite != "G")
                        {
                            continue;
                        }
                        temp_satelite = Satelite[i];
                        double a0, a1, a2;
                        a0 = temp_satelite.a0;
                        a1 = temp_satelite.a1;
                        a2 = temp_satelite.a2;
                        toc = processdata.Calendar2GpsTime(Satelite[i].year, Satelite[i].mouth, Satelite[i].day, Satelite[i].hour, Satelite[i].minutes, Satelite[i].second);
                        delta_ts = a0 + a1 * (t - toc) + a2 * Math.Pow((t - toc), 2);


                        delta_tr = X0.At(3, 0)/c;
                        ts1 = t - delta_tr - 0.075;
                       
                        ts0 = ts1 + 1;
                        while (Math.Abs(ts1 - ts0) > 1e-8)
                        {
                            ts0 = ts1;                           
                            C_xyz(ts0, temp_satelite, ref Xk, ref Yk, ref Zk);
                            dt = Obs_Data.C1 / c;
                            //进行地球自转改正
                            X = Xk * Math.Cos(w * dt) + Yk * Math.Sin(w * dt);
                            Y = -Math.Sin(w * dt) * Xk + Yk * Math.Cos(w * dt);
                            Z = Zk;

                            pk0 = Math.Sqrt(Math.Pow(X - X0.At(0, 0), 2) + Math.Pow(Y - X0.At(1, 0), 2) + Math.Pow(Z - X0.At(2, 0), 2));
                            ts1 = t - delta_tr - pk0 / c;
                        }
                        ax = (X0.At(0, 0) - X) / pk0;
                        ay = (X0.At(1, 0) - Y) / pk0;
                        az = (X0.At(2, 0) - Z) / pk0;
                        var temp_matrix = new DenseMatrix(1, 4);
                        var temp_matrixl = new DenseMatrix(1, 1);
                    
                     
                        temp_matrix.At(0, 0, ax);
                        temp_matrix.At(0, 1, ay);
                        temp_matrix.At(0, 2, az);
                        temp_matrix.At(0, 3, 1);

                        if (k == 0)
                        {
                            A[0, 0] = ax;
                            A[0, 1] = ay;
                            A[0, 2] = az;
                            A[0, 3] = 1;
                        }
                        else                                           
                           A= Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,] { { A},{ temp_matrix } });
                        //2023-6-12-start
                       /* double dtrop = SPP_Trop(height, temp_satelite.Elevarting);
                       // ll = Obs_Data.C1 - pk0 + delta_tr * c;
                        ll = Obs_Data.C1 - pk0 + delta_tr * c-dtrop;
                        //2023-6-12-end
                        */
                        //2023-6-13-start
                        double dtrop = tropmodel(lat,height, temp_satelite.Azimuth,temp_satelite.Elevarting,humi);                       
                        ll = Obs_Data.C1 - pk0 + delta_ts* c - dtrop;
                        //2023-6-13-end*/
                   
                       // ll = Obs_Data.C1 - pk0 + delta_ts * c;
                       
                        temp_matrixl.At(0, 0, ll);
                        if (k == 0)
                            L.At(0, 0, ll);
                        else
                            L = L.Append( temp_matrixl);
                    }
                    //最小二乘平差    
                   // A = A.Resize(A.ColumnCount / 4, 4);
                    L = L.Transpose();
                    var trans_A = A.Transpose();
                    var B = (trans_A * A).Inverse();
                    var x = B * trans_A * L;
                    double xx, yy, zz, tt;

                    xx = X0.At(0, 0);
                    yy = X0.At(1, 0);
                    zz = X0.At(2, 0);
                    tt = X0.At(3, 0);

                    X0.At(0, 0, xx + x.At(0, 0));
                    X0.At(1, 0, yy + x.At(1, 0));
                    X0.At(2, 0, zz + x.At(2, 0));
                    X0.At(3, 0,tt+x.At(3, 0));
                }
               
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string fileName, shortName, fileExt;
       

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var temp_matrix = new DenseMatrix(1, 4);
                temp_matrix.At(0, 0, 1);
                fileName = openFileDialog1.FileName;
                int i = fileName.LastIndexOf('\\');
                shortName = fileName.Substring(i + 1);
                this.Text = shortName;
                fileExt = System.IO.Path.GetExtension(shortName).ToUpper();
                string[] strlines = File.ReadAllLines(fileName, Encoding.Default); //读取文件的每一行 
                int j;

                for (j = 0; j < strlines.Length; j ++)//j记录头文件位置
                {
                    string[] end= strlines[j].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (end[0]=="END")                     
                        break;
                }
                j++;
               
                char[] delimiterChars = { ' ', '-' };
              
                string fileformat = "";
                string[] newstr = shortName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                fileformat = newstr[5].Substring(1, 1);
              
                 readrinex = new ReadRinex();
                if (fileformat == "N")
                {
                    readrinex.readN(j, strlines, ref EPHEMERISBLOCKNum, Satelite);
                    Refresh();
                }
                else
                {
                    //存储用于SPP的头文件信息
                    //APPROX POSITION XYZ
                    string[] newstr1 = strlines[7].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    obs_X = Convert.ToDouble(newstr1[0]);
                    obs_Y = Convert.ToDouble(newstr1[1]);
                    obs_Z = Convert.ToDouble(newstr1[2]);
                    string[] newstr2 = strlines[8].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    deltaH = Convert.ToDouble(newstr2[0]);
                    deltaE = Convert.ToDouble(newstr2[1]);
                    deltaN = Convert.ToDouble(newstr2[2]);

                    string[] newstr3 = strlines[21].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Interval = Convert.ToDouble(newstr3[0]);
                    //存储结束
                    readrinex.readO(j, strlines, pobs_epoch);
                }
               // SPP();
               // CBroadCast(EPHEMERISBLOCKNum);
            }
           
    }
}
        }