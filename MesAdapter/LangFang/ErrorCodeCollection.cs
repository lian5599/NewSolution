using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace MesAdapter
{
    /// <summary>
    /// 获取.cdv文件的类
    /// </summary>
    public static class ErrorCodeCollection
    {
        public static List<ErrorCfg> Errors { get; set; }
        public static void LoadCfg(string filePath = @"Config\MachineErrors.csv")
        {
            List<ErrorCfg> errorCodes = new List<ErrorCfg>();
            try
            {
                Encoding encoding = Encoding.Default;//Common.GetType(filePath); 
                DataTable dt = new DataTable();
                FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                StreamReader sr = new StreamReader(fs, encoding);
                //string fileContent = sr.ReadToEnd();
                //encoding = sr.CurrentEncoding;
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = null;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    //strLine = Common.ConvertStringUTF8(strLine, encoding);
                    //strLine = Common.ConvertStringUTF8(strLine);

                    if (IsFirst == true)
                    {
                        IsFirst = false;
                        continue;
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        errorCodes.Add(new ErrorCfg(Convert.ToInt32(aryLine[0]), aryLine[1]));
                    }
                }


                sr.Close();
                fs.Close();
                Errors = errorCodes;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                Errors = new List<ErrorCfg>();
            }
        }
        public static string GetError(int code)
        {
            ErrorCfg error = Errors.Find(item =>  item.Code == code);
            if (error != null) return error.Description;
            else throw new Exception("未定义错误码：" + code);
        }
    }

    public class ErrorCfg
    {
        public ErrorCfg(int code, string description)
        {
            Code = code;
            Description = description;
        }
        public int Code { get; set; }
        public string Description { get; set; }
    }
}
