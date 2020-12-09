using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Result
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        [JsonIgnore] 
        public int ErrorCode { get; set; }

        public Result(string msg , int err = 0)
        {
            IsSuccess = false;
            Message = msg;
            ErrorCode = err;
        }
        public Result(bool isSuccess = true, string msg = "", int err = 0)
        {
            IsSuccess = isSuccess;
            Message = msg;
            ErrorCode = err;
        }
        public virtual object GetContent() { return null; }
    }

    public class Result<T>:Result
    {
        public Result() { }
        public Result(string msg, int err = 0)
        {
            IsSuccess = false;
            ErrorCode = err;
            Message = msg;
        }
        public Result(T content,bool isSuccess = true, string msg = "成功", int err = 0)
        {
            Content = content;
            IsSuccess = isSuccess;
            Message = msg;
            ErrorCode = err;
        }
        public T Content { get; set; }
        public override object GetContent()
        {
            return Content;
        }
    }

    public enum ErrorCode
    {
        fatalError =440,
    }
}
