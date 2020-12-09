using Communication.Plc;
using Helper;
using System;

namespace TKS
{

    public static class FlagBusiness
    {
        public static Result Execute(string flagId, Func<Result> func)
        {
            FlagModule flag;
            PlcBase plc;
            try
            {
                flag = Manager1.GetInstance().flag(flagId);
                plc = Manager1.GetInstance().plc(flag.PlcId);
            }
            catch (Exception e)
            {
                return new Result(e.Message, 410);
            }


            var readResult = plc.Read(flag.Address);
            Result writeResult;
            if (readResult.IsSuccess)
            {
                if (readResult.Content == flag.ValidValue)
                {
                    var funcResult = func();
                    int writeValue = funcResult.IsSuccess ? flag.OkValue : flag.NgValue;
                    writeResult = plc.Write(flag.Address, writeValue);
                    if (writeResult.IsSuccess) return funcResult;
                    else return writeResult;
                }
                else return new Result() { ErrorCode = 201, Message = flag.Id + "不是有效信号" };
            }
            else return readResult;
        }

        public static Result<int> Read(string flagId)
        {
            FlagModule flag;
            PlcBase plc;
            try
            {
                flag = Manager1.GetInstance().flag(flagId);
                plc = Manager1.GetInstance().plc(flag.PlcId);
            }
            catch (Exception e)
            {
                return new Result<int>(e.Message, 410);
            }


            var readResult = plc.Read(flag.Address);
            if (!readResult.IsSuccess)
            {
                readResult.Content = -1;
            }
            flag.CurrentValue = readResult.Content;
            return readResult;
        }

        public static Result Write(string flagId, int value)
        {
            FlagModule flag;
            PlcBase plc;
            try
            {
                flag = Manager1.GetInstance().flag(flagId);
                plc = Manager1.GetInstance().plc(flag.PlcId);
            }
            catch (Exception e)
            {
                return new Result(e.Message, 410);
            }

            return plc.WriteAsync(flag.Address, value).Result;
        }

    }
}
