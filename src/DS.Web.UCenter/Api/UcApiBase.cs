using System;
using System.Collections.Generic;
using System.Web;

namespace DS.Web.UCenter.Api
{
    /// <summary>
    /// UcApi
    /// Dozer 版权所有
    /// 允许复制、修改，但请保留我的联系方式！
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public abstract class UcApiBase : IHttpHandler
    {
        /// <summary>
        /// 如果子类的实现不是线程安全的请覆盖此方法, 并返回false
        /// </summary>
        public virtual bool IsReusable
        {
            get { return true; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var Args = new UcRequestArguments(context.Request);
            if (!check(Args, context)) return;
            ApiReturn res;
            try
            {
                res = switchAction(Args, context);
            }
            catch (Exception e)
            {
                if (e is NotImplementedException || e is NotSupportedException)
                {
                    res = ApiReturn.Forbidden;
                }
                else if (e is System.Threading.ThreadAbortException)
                {
                    //may throw by inner Response.End, do not handle it
                    throw;
                }
                else
                {
                    res = ApiReturn.Failed;
                }
            }
            context.writeEnd(res);
        }

        #region 私有
        /// <summary>
        /// 检查合法性
        /// </summary>
        /// <returns></returns>
        private bool check(IUcRequestArguments Args, HttpContext context)
        {
            if (Args.IsInvalidRequest)
            {
                context.writeEnd("Invalid Request");
                return false;
            }
            if (Args.IsAuthracationExpiried)
            {
                context.writeEnd("Authracation has expiried");
                return false;
            }
            return true;
        }

        private static
            IDictionary<string, Func<UcApiBase, IUcRequestArguments, HttpContext, ApiReturn>>
            _actions;
        /// <summary>
        /// 子类可覆盖并添加父类及自己新加的静态方法到一个新的字典中, 不要修改父类返回结果
        /// </summary>
        /// <returns></returns>
        protected virtual 
            IDictionary<string, Func<UcApiBase, IUcRequestArguments, HttpContext, ApiReturn>>
            GetActions()
        {
            if (_actions == null)
            {
                _actions = new Dictionary<string, Func<UcApiBase, IUcRequestArguments, HttpContext, ApiReturn>>
                    (/*StringComparer.OrdinalIgnoreCase*/)
                {
                    {UcActions.Test, test},
                    {UcActions.DeleteUser, deleteUser},
                    {UcActions.RenameUser, renameUser},
                    {UcActions.GetTag, getTag},
                    {UcActions.SynLogin, synLogin},
                    {UcActions.SynLogout, synLogout},
                    {UcActions.UpdatePw, updatePw},
                    {UcActions.UpdateBadWords, updateBadWords},
                    {UcActions.UpdateHosts, updateHosts},
                    {UcActions.UpdateApps, updateApps},
                    {UcActions.UpdateClient, updateClient},
                    {UcActions.UpdateCredit, updateCredit},
                    {UcActions.GetCreditSettings, getCreditSettings},
                    {UcActions.GetCredit, getCredit},
                    {UcActions.UpdateCreditSettings, updateCreditSettings},
                };

            }
            return _actions;
        }

        private ApiReturn switchAction(IUcRequestArguments Args, HttpContext context)
        {
            //Maybe switch or 
            //IDictionary <string, Func<IUcRequestArguments, HttpContext, ApiReturn>> for extend
            ApiReturn res;
            Func<UcApiBase, IUcRequestArguments, HttpContext, ApiReturn> func;
            if (GetActions().TryGetValue(Args.Action, out func))
            {
                res = func(this, Args, context);
            }
            else
            {
                res = ApiReturn.Forbidden;
            }
            return res;
        }
        #endregion

        #region API实现
        private static ApiReturn test(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            return ApiReturn.Success;
        }
        private static ApiReturn deleteUser(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiDeleteUser)
                return ApiReturn.Forbidden;
            var ids = Args.QueryString["ids"];
            var idArray = new List<int>();
            foreach (var id in ids.Split(','))
            {
                int idInt;
                if (int.TryParse(id, out idInt)) idArray.Add(idInt);
            }
            return apiContext.DeleteUser(idArray);
        }
        private static ApiReturn renameUser(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiRenameUser)
                return ApiReturn.Forbidden;
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            var oldusername = Args.QueryString["oldusername"];
            var newusername = Args.QueryString["newusername"];
            return apiContext.RenameUser(uid, oldusername, newusername);
        }
        private static ApiReturn getTag(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetTag)
                return ApiReturn.Forbidden;
            var tagName = Args.QueryString["id"];
            context.writeEnd(apiContext.GetTag(tagName));
            return ApiReturn.Nop;
        }
        private static ApiReturn synLogin(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogin)
                return ApiReturn.Forbidden;
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            return apiContext.SynLogin(uid);
        }
        private static ApiReturn synLogout(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogout)
                return ApiReturn.Forbidden;
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            return apiContext.SynLogout();
        }
        private static ApiReturn updatePw(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdatePw)
                return ApiReturn.Forbidden;
            var username = Args.QueryString["username"];
            var password = Args.QueryString["password"];
            return apiContext.UpdatePw(username, password);
        }
        private static ApiReturn updateBadWords(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateBadWords)
                return ApiReturn.Forbidden;
            var badWords = new UcBadWords(Args.FormData);
            return apiContext.UpdateBadWords(badWords);
        }
        private static ApiReturn updateHosts(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateHosts)
                return ApiReturn.Forbidden;
            var hosts = new UcHosts(Args.FormData);
            return apiContext.UpdateHosts(hosts);
        }
        private static ApiReturn updateApps(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateApps)
                return ApiReturn.Forbidden;
            var apps = new UcApps(Args.FormData);
            return apiContext.UpdateApps(apps);
        }
        private static ApiReturn updateClient(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateClient)
                return ApiReturn.Forbidden;
            var client = new UcClientSetting(Args.FormData);
            return apiContext.UpdateClient(client);
        }
        private static ApiReturn updateCredit(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCredit)
                return ApiReturn.Forbidden;
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            int amount;
            int.TryParse(Args.QueryString["amount"], out amount);
            return apiContext.UpdateCredit(uid, credit, amount);
        }
        private static ApiReturn getCreditSettings(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCreditSettings)
                return ApiReturn.Forbidden;
            context.writeEnd(apiContext.GetCreditSettings());
            return ApiReturn.Nop;
        }
        private static ApiReturn getCredit(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCredit)
                return ApiReturn.Forbidden;
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            return apiContext.GetCredit(uid, credit);
        }
        private static ApiReturn updateCreditSettings(UcApiBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCreditSettings)
                return ApiReturn.Forbidden;
            var creditSettings = new UcCreditSettings(Args.FormData);
            return apiContext.UpdateCreditSettings(creditSettings);
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids">Uid</param>
        /// <returns></returns>
        public abstract ApiReturn DeleteUser(IEnumerable<int> ids);
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="oldUserName">旧用户名</param>
        /// <param name="newUserName">新用户名</param>
        /// <returns></returns>
        public abstract ApiReturn RenameUser(int uid, string oldUserName, string newUserName);
        /// <summary>
        /// 得到标签
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        public abstract UcTagReturns GetTag(string tagName);
        /// <summary>
        /// 同步登陆
        /// </summary>
        /// <param name="uid">uid</param>
        /// <returns></returns>
        public abstract ApiReturn SynLogin(int uid);
        /// <summary>
        /// 同步登出
        /// </summary>
        /// <returns></returns>
        public abstract ApiReturn SynLogout();
        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public abstract ApiReturn UpdatePw(string userName, string passWord);
        /// <summary>
        /// 更新不良词汇
        /// </summary>
        /// <param name="badWords">不良词汇</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateBadWords(UcBadWords badWords);
        /// <summary>
        /// 更新Hosts
        /// </summary>
        /// <param name="hosts">Hosts</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateHosts(UcHosts hosts);
        /// <summary>
        /// 更新App
        /// </summary>
        /// <param name="apps">App</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateApps(UcApps apps);
        /// <summary>
        /// 更新UCenter设置
        /// </summary>
        /// <param name="client">UCenter设置</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateClient(UcClientSetting client);
        /// <summary>
        /// 更新用户积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <param name="amount">积分增减</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateCredit(int uid, int credit, int amount);
        /// <summary>
        /// 得到积分设置
        /// </summary>
        /// <returns></returns>
        public abstract UcCreditSettingReturns GetCreditSettings();
        /// <summary>
        /// 得到积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <returns></returns>
        public abstract ApiReturn GetCredit(int uid, int credit);
        /// <summary>
        /// 更新积分设置
        /// </summary>
        /// <param name="creditSettings">积分设置</param>
        /// <returns></returns>
        public abstract ApiReturn UpdateCreditSettings(UcCreditSettings creditSettings);
        #endregion
    }

    /// <summary>
    /// 对HttpResponse进行写操作的工具类
    /// </summary>
    internal static class HttpResponseUtils
    {
        public static void writeEnd(this HttpContext context, string msg)
        {
            var Response = context.Response;
            Response.Write(msg);
            //Response.End();
            Response.Flush();
            var app = context.ApplicationInstance;
            if (app != null)
                app.CompleteRequest();
        }
        public static void writeEnd<T>(this HttpContext context, UcCollectionReturnBase<T> msg)
            where T : UcItemReturnBase
        {
            writeEnd(context, msg.ToString());
        }
        public static void writeEnd(this HttpContext context, ApiReturn result)
        {
            string msg;
            switch (result) {
                case ApiReturn.Success:
                    msg = UcConfig.ApiReturnSucceed;
                    break;
                case ApiReturn.Failed:
                    msg = UcConfig.ApiReturnFailed;
                    break;
                case ApiReturn.Nop:
                    return;
                default:
                case ApiReturn.Forbidden:
                    msg = UcConfig.ApiReturnFailed;
                    break;
            }
            writeEnd(context, msg);
        }
        public static void writeForbidden(this HttpContext context)
        {
            writeEnd(context, UcConfig.ApiReturnForbidden);
        }
    }

    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ApiReturn
    {
        /// <summary>
        /// 失败
        /// </summary>
        Failed,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 禁止
        /// </summary>
        Forbidden,
        /// <summary>
        /// 指示已处理, 但不做输出(常用于输出自定义类型)
        /// </summary>
        Nop,
    }
}

