using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTD.MI.X5;
using OTD.MI.X5Entity;

namespace MesAdapter
{
    public static class BeijingXiaoMi
    {
        //初始化，软件启动时调用
        public static void Init()
        {
            Settings.AddSetting("AppId");
            Settings.AddSetting("AppKey");
            Settings.AddSetting("EQP_ID");
            Settings.AddSetting("CLIENT_MAC");
            Settings.AddSetting("USER_ID");
            Settings.AddSetting("KEY_PART_MATERIAL");

            var methods = typeof(BeijingXiaoMi).GetMethods().Where(item => item.IsPublic && item.ReturnType == typeof(Result));
            foreach (var item in methods)
            {
                Settings.AddSetting(item.Name + "格式");
                Settings.AddSetting(item.Name + "地址");
                Settings.AddSetting(item.Name + "方法");
            }
        }

        public static Result 过途程(string FrameSN)
        {
            //{
            // "EQP_ID":"S01-MOUNT1",
            //"CLIENT_MAC":"04-D4-C4-4B-A7-C7",
            //"CLIENT_TIME":"2020-06-05 16:12:53",
            //"UNIT_SN":"P1V025N000245",
            //"USER_ID":""
            //}
            //{{"EQP_ID":"{0}","CLIENT_MAC":"{1}","CLIENT_TIME":"{2}","UNIT_SN":"{3}","USER_ID":"{4}"}}
            string currentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string BodyFormat = Settings.GetSetting(currentMethod + "格式");
            string EQP_ID = Settings.GetSetting("EQP_ID");
            string CLIENT_MAC = Settings.GetSetting("CLIENT_MAC");
            string CLIENT_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string USER_ID = Settings.GetSetting("USER_ID");
            //string KEY_PART_MATERIAL = Settings.GetSetting("KEY_PART_MATERIAL");
            string body = string.Format(BodyFormat, EQP_ID, CLIENT_MAC, CLIENT_TIME, FrameSN, USER_ID);
            Result result = X5(body, Settings.GetSetting(currentMethod + "地址"), Settings.GetSetting(currentMethod + "方法"));
            result.Message = string.Format("{0}({1})=>{2}", currentMethod, FrameSN, result.Message);
            return result;
        }
        public static Result 组装校验(string TpSN, string FrameSN)
        {
            //{
            // "EQP_ID":"S01-MOUNT1",
            //"CLIENT_TIME":"2020-06-05 16:12:53",
            //"USER_ID":"",
            //"KEY_PART_MATERIAL":"",
            //"KEY_PART_BARCODE":"",
            //"UNIT_SN":""     
            //}
            //{{"EQP_ID":"{0}","CLIENT_TIME":"{1}","USER_ID":"{2}","KEY_PART_MATERIAL":"{3}","KEY_PART_BARCODE":"{4}","UNIT_SN":"{5}"}}
            string currentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string BodyFormat = Settings.GetSetting(currentMethod + "格式");
            string EQP_ID = Settings.GetSetting("EQP_ID");
            //string CLIENT_MAC = Settings.GetSetting("CLIENT_MAC");
            string CLIENT_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string USER_ID = Settings.GetSetting("USER_ID");
            string KEY_PART_MATERIAL = Settings.GetSetting("KEY_PART_MATERIAL");
            string body = string.Format(BodyFormat, EQP_ID, CLIENT_TIME, USER_ID, KEY_PART_MATERIAL,TpSN, FrameSN);
            Result result = X5(body, Settings.GetSetting(currentMethod + "地址"), Settings.GetSetting(currentMethod + "方法"));
            result.Message = string.Format("{0}({1},{2})=>{3}", currentMethod, TpSN,FrameSN, result.Message);
            return result;
        }
        public static Result 组装绑定(string TpSN, string FrameSN)
        {
            //{
            // "EQP_ID":"S01-MOUNT1",
            //"CLIENT_TIME":"2020-06-05 16:12:53",
            //"USER_ID":"",
            //"KEY_PART_MATERIAL":"",
            //"KEY_PART_BARCODE":"",
            //"UNIT_SN":""     
            //}
            //{{"EQP_ID":"{0}","CLIENT_TIME":"{1}","USER_ID":"{2}","KEY_PART_MATERIAL":"{3}","KEY_PART_BARCODE":"{4}","UNIT_SN":"{5}"}}
            string currentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string BodyFormat = Settings.GetSetting(currentMethod + "格式");
            string EQP_ID = Settings.GetSetting("EQP_ID");
            //string CLIENT_MAC = Settings.GetSetting("CLIENT_MAC");
            string CLIENT_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string USER_ID = Settings.GetSetting("USER_ID");
            string KEY_PART_MATERIAL = Settings.GetSetting("KEY_PART_MATERIAL");
            string body = string.Format(BodyFormat, EQP_ID, CLIENT_TIME, USER_ID, KEY_PART_MATERIAL,TpSN, FrameSN);
            Result result = X5(body, Settings.GetSetting(currentMethod + "地址"), Settings.GetSetting(currentMethod + "方法"));
            result.Message = string.Format("{0}({1},{2})=>{3}", currentMethod, TpSN, FrameSN, result.Message);
            return result;
        }
        public static Result 出站(string FrameSN)
        {
            //{
            //  "EQP_ID":"S01-MOUNT1",
            //	"CLIENT_MAC":"04-D4-C4-4B-A7-C7",
            //	"CLIENT_TIME":"2020-06-05 16:12:53",
            //	"UNIT_SN":"P1V025N000245",
            //	"STATE":"pass",
            //	"USER_ID":"",
            //	"UNIT_DATA":{
            //       "INSPECTION_ITEM_DATA":"",
            //		"INSPECTION_POINT_DATA":"",
            //		"PROCESS_DATA":""
            //    },
            //	"UPLOADING_FILE_NUM":0,
            //	"IGNORE_MULTIPLE_NG":"N"
            //}

            //{{"EQP_ID":"{0}","CLIENT_MAC":"{1}","CLIENT_TIME":"{2}","UNIT_SN":"{3}","STATE":"pass","USER_ID":"{4}","UNIT_DATA":{{"INSPECTION_ITEM_DATA":"","INSPECTION_POINT_DATA":"","PROCESS_DATA":""}},"UPLOADING_FILE_NUM":0,"IGNORE_MULTIPLE_NG":"N"}}
            string currentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string BodyFormat = Settings.GetSetting(currentMethod + "格式");
            string EQP_ID = Settings.GetSetting("EQP_ID");
            string CLIENT_MAC = Settings.GetSetting("CLIENT_MAC");
            string CLIENT_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string USER_ID = Settings.GetSetting("USER_ID");
            //string KEY_PART_MATERIAL = Settings.GetSetting("KEY_PART_MATERIAL");
            string body = string.Format(BodyFormat, EQP_ID, CLIENT_MAC, CLIENT_TIME, FrameSN, USER_ID);
            Result result = X5(body, Settings.GetSetting(currentMethod + "地址"), Settings.GetSetting(currentMethod + "方法"));
            result.Message = string.Format("{0}({1})=>{2}", currentMethod, FrameSN, result.Message);
            return result;
        }

        private static Result<string> X5(string Body, string Url, string Method)
        {
            Result<string> result = new Result<string>();
            X5Client x5Client = new X5Client();
            var response = x5Client.X5Request
                (
                new X5RequestContent
                {
                    body = Body,
                    header = new X5RequestHeader
                    {
                        appid = Settings.GetSetting("AppId"),
                        appKey = Settings.GetSetting("AppKey"),
                        url = Url,
                        method = Method
                    }
                });
            result.IsSuccess = response.header.code == 200;
            if (result.IsSuccess) result.Message = "成功";
            else result.Message ="错误码:" + response.header.code +";描述:"+ response.header.desc;
            result.Content = response.body;
            return result;
        }
    }
}
