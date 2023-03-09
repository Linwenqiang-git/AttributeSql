using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AttributeSql.Core.Models
{
    /// <summary>
    /// 返回给调用方的数据格式
    /// </summary>
    public class AttrResultModel : AttrResultModel<object>
    {
        public AttrResultModel()
        {
        }

        public AttrResultModel(object result) : base(result)
        {
        }

        public AttrResultModel(ResultCode code) : base(code)
        {
        }

        public AttrResultModel(ResultCode code, string msg) : base(code, msg)
        {
        }

        public static AttrResultModel Success(object data = null)
        {
            return new AttrResultModel(data);
        }

        public static AttrResultModel Error(string msg, ResultCode code = ResultCode.UnknownError)
        {
            return new AttrResultModel(code, msg);
        }

    }
    /// <summary>
    /// 通用返回结果
    /// </summary>
    public class AttrResultModel<T>
    {
        public AttrResultModel() { }

        public AttrResultModel(ResultCode code)
        {
            Code = code;
        }

        public AttrResultModel(T result)
        {
            Result = result;
        }

        public AttrResultModel(ResultCode code, string msg) : this(code)
        {
            Msg = msg;
        }

        private ResultCode _code = ResultCode.Success;
        /// <summary>
        /// 返回码:0表示请求成功
        /// </summary>
        public ResultCode Code
        {
            get => _code;
            set
            {
                _code = value;
                Msg = value.Description();
            }
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; } = ResultCode.Success.Description();
        /// <summary>
        /// 受影响行数
        /// 列表类返回数据查询总数
        /// </summary>
        public int? Total { get; set; } = 0;

        /// <summary>
        /// 返回结果：json,在列表查询用到
        /// </summary>        
        public object Result { get; set; }

        public static implicit operator AttrResultModel(AttrResultModel<T> d)
        {
            return d;
        }

        public static AttrResultModel<TData> Success<TData>(TData data = default(TData))
        {
            return new AttrResultModel<TData>(data);
        }

        public static AttrResultModel<TData> Error<TData>(string msg, ResultCode code = ResultCode.UnknownError)
        {
            return new AttrResultModel<TData>(code, msg);
        }

    }
    public enum ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("success")]
        Success = 0,
        /// <summary>
        /// Dto模型问题
        /// </summary>
        [Description("success")]
        AttributeError = 205,

        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("unknown error")]
        UnknownError = 500,

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("parameter error")]
        ParamError = 501,

        /// <summary>
        /// 警告
        /// </summary>
        [Description("warn")]
        Warn = 201,
        /// <summary>
        /// 无权限
        /// </summary>
        [Description("NoPermission")]
        NoPermission = 401,

        #region  用于支付环节的状态
        /// <summary>
        /// 支付成功
        /// </summary>
        PaySuccess = 200,
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFaile = 201,
        /// <summary>
        /// 支付调用失败
        /// </summary>
        PayError = 202,
        #endregion
    }
    /// <summary>
    /// 响应结果Code扩展 获取描述
    /// </summary>
    public static class ResultCodeExtensions
    {
        public static string Description(this ResultCode code)
        {
            string desc = null;
            var type = code.GetType();
            var attr = type.GetMember(code.ToString())
                .FirstOrDefault()
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();

            if (attr != null)
            {
                desc = ((DescriptionAttribute)attr).Description;
            }
            return desc;
        }
    }
}
