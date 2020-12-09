using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public static class Security
    {
        public static Action<TimeSpan> periodOfValidityChanged;
        public static int Length = 10;
        private static string LicenseFile = @"Config\License";
        public static string CreateLicense(string device,DateTime dateTime)
        {
            Random rd = new Random();
            int key= rd.Next(0, 128);

            string DateValidStr = dateTime.ToString("yyyy-MM-dd");//有效期
            byte[] dateValidByte = Encoding.ASCII.GetBytes(DateValidStr);
            //string DateNowStr = DateTime.Now.ToString("yyMMdd");//当前日期
            //byte[] dateNowByte = Encoding.ASCII.GetBytes(DateNowStr);
            byte[] deviceByte = Encoding.ASCII.GetBytes(device);
            //关键
            byte[] EncryptByte = new byte[Length + 2];
            EncryptByte[Length] = (byte)key;
            for (int j = 0; j < Length; j++)
            {
                EncryptByte[j] = (byte)(deviceByte[j] ^ dateValidByte[j]  ^ key);
                EncryptByte[Length + 1] = (byte)(EncryptByte[Length + 1] ^ EncryptByte[j]);//parity
            }
            //byte[] by = new byte[Length + 1];
            //Array.Copy(EncryptByte, by, EncryptByte.Length);
            string EncryptStr = Convert.ToBase64String(EncryptByte);//BitConverter.ToString(EncryptByte).Replace("-","");// string.Join("", EncryptByte);
            File.Write(LicenseFile, EncryptStr); 
            return EncryptStr;
        }
        public static bool IsRegisterd(out TimeSpan t)
        {
            try
            {
                string license = File.Read(LicenseFile);
                return CheckLicense(license,out t);
            }
            catch (Exception e)
            {
                t = default;
                return false;
            }
        }

        public static bool CheckLicense(string license, out TimeSpan t)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(license);
                int parity = bytes[bytes.Length - 1];//Convert.ToInt32(license.Substring(license.Length - 2, 2), 16);
                int key = bytes[bytes.Length - 2];//Convert.ToInt32(license.Substring(license.Length - 4, 2), 16);
                byte[] deviceByte = Encoding.ASCII.GetBytes(GetCpu().Substring(0, Length));

                //byte[] DecryptByte = new byte[Length];
                byte[] dateByte = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    //DecryptByte[i] = (byte)Convert.ToInt32(license.Substring(2 * i, 2), 16);
                    dateByte[i] = (byte)(bytes[i] ^ deviceByte[i] ^ key);
                    parity = parity ^ bytes[i];
                }
                if (parity != 0)
                {
                    throw new Exception("校验位出错");
                }
                string temp = Encoding.ASCII.GetString(dateByte);
                //temp = temp.Insert(6, "-");
                //temp = temp.Insert(4, "-");
                DateTime periodOfValidity = Convert.ToDateTime(temp);
                t = periodOfValidity - DateTime.Now.Date;
                if (t.Days > 0)
                {
                    periodOfValidityChanged?.Invoke(t);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                t = default;
                return false;
            }
        }

        public static bool Register(string license, out TimeSpan t)
        {
            if (CheckLicense(license,out t))
            {
                File.Write(LicenseFile, license);
                return true;
            }
            else return false;
        }
        ///<summary>
        /// 获取CPU序列号
        ///</summary>
        ///<returns></returns>
        public static string GetCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");//cmd：wmic cpu get processorid
            ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuCollection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
            }
            return strCpu;
        }
    }
}
