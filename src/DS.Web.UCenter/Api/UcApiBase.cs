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
        /// 
        /// </summary>
        public bool IsReusable
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
            try
            {
                switchAction(Args, context);
            }
            catch (Exception e)
            {
                if (e is NotImplementedException || e is NotSupportedException)
                {
                    context.writeForbidden();
                }
                else if (e is System.Threading.ThreadAbortException)
                {
                    //throw by Response.End, do not handle it
                    throw;
                }
                else
                {
                    context.writeEnd(ApiReturn.Failed);
                }
            }
        }

        #region 私有
        //private HttpResponse Response { get; set; }
        //private HttpRequest Request { get; set; }
        //private IUcRequestArguments Args { get; set; }
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
        private void switchAction(IUcRequestArguments Args, HttpContext context)
        {
            //Maybe switch or 
            //IDictionary <string, Action<IUcRequestArguments, HttpContext>> for extend
            if (Args.Action == UcActions.Test)
            {
                test(Args, context);
            }
            else if (Args.Action == UcActions.DeleteUser)
            {
                deleteUser(Args, context);
            }
            else if (Args.Action == UcActions.RenameUser)
            {
                renameUser(Args, context);
            }
            else if (Args.Action == UcActions.GetTag)
            {
                getTag(Args, context);
            }
            else if (Args.Action == UcActions.SynLogin)
            {
                synLogin(Args, context);
            }
            else if (Args.Action == UcActions.SynLogout)
            {
                synLogout(Args, context);
            }
            else if (Args.Action == UcActions.UpdatePw)
            {
                updatePw(Args, context);
            }
            else if (Args.Action == UcActions.UpdateBadWords)
            {
                updateBadWords(Args, context);
            }
            else if (Args.Action == UcActions.UpdateHosts)
            {
                updateHosts(Args, context);
            }
            else if (Args.Action == UcActions.UpdateApps)
            {
                updateApps(Args, context);
            }
            else if (Args.Action == UcActions.UpdateClient)
            {
                updateClient(Args, context);
            }
            else if (Args.Action == UcActions.UpdateCredit)
            {
                updateCredit(Args, context);
            }
            else if (Args.Action == UcActions.GetCreditSettings)
            {
                getCreditSettings(Args, context);
            }
            else if (Args.Action == UcActions.GetCredit)
            {
                getCredit(Args, context);
            }
            else if (Args.Action == UcActions.UpdateCreditSettings)
            {
                updateCreditSettings(Args, context);
            }
            else
            {
                context.writeForbidden();
            }
        }
        #endregion

        #region API实现
        private void test(IUcRequestArguments Args, HttpContext context)
        {
            context.writeEnd(UcConfig.ApiReturnSucceed);
        }
        private void deleteUser(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiDeleteUser)
            {
                context.writeForbidden();
                return;
            }
            var ids = Args.QueryString["ids"];
            var idArray = new List<int>();
            foreach (var id in ids.Split(','))
            {
                int idInt;
                if (int.TryParse(id, out idInt)) idArray.Add(idInt);
            }
            context.writeEnd(DeleteUser(idArray));
        }
        private void renameUser(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiRenameUser)
            {
                context.writeForbidden();
                return;
            }
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            var oldusername = Args.QueryString["oldusername"];
            var newusername = Args.QueryString["newusername"];
            context.writeEnd(RenameUser(uid, oldusername, newusername));
        }
        private void getTag(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetTag)
            {
                context.writeForbidden();
                return;
            }
            var tagName = Args.QueryString["id"];
            context.writeEnd(GetTag(tagName));
        }
        private void synLogin(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogin)
            {
                context.writeForbidden();
                return;
            }
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            context.writeEnd(SynLogin(uid));
        }
        private void synLogout(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogout)
            {
                context.writeForbidden();
                return;
            }
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            context.writeEnd(SynLogout());
        }
        private void updatePw(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdatePw)
            {
                context.writeForbidden();
                return;
            }
            var username = Args.QueryString["username"];
            var password = Args.QueryString["password"];
            context.writeEnd(UpdatePw(username, password));
        }
        private void updateBadWords(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateBadWords)
            {
                context.writeForbidden();
                return;
            }
            var badWords = new UcBadWords(Args.FormData);
            context.writeEnd(UpdateBadWords(badWords));
        }
        private void updateHosts(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateHosts)
            {
                context.writeForbidden();
                return;
            }
            var hosts = new UcHosts(Args.FormData);
            context.writeEnd(UpdateHosts(hosts));
        }
        private void updateApps(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateApps)
            {
                context.writeForbidden();
                return;
            }
            var apps = new UcApps(Args.FormData);
            context.writeEnd(UpdateApps(apps));
        }
        private void updateClient(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateClient)
            {
                context.writeForbidden();
                return;
            }
            var client = new UcClientSetting(Args.FormData);
            context.writeEnd(UpdateClient(client));
        }
        private void updateCredit(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCredit)
            {
                context.writeForbidden();
                return;
            }
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            int amount;
            int.TryParse(Args.QueryString["amount"], out amount);
            context.writeEnd(UpdateCredit(uid, credit, amount));
        }
        private void getCreditSettings(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCreditSettings)
            {
                context.writeForbidden();
                return;
            }
            context.writeEnd(GetCreditSettings());
        }
        private void getCredit(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCredit)
            {
                context.writeForbidden();
                return;
            }
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            context.writeEnd(GetCredit(uid, credit));
        }
        private void updateCreditSettings(IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCreditSettings)
            {
                context.writeForbidden();
                return;
            }
            var creditSettings = new UcCreditSettings(Args.FormData);
            context.writeEnd(UpdateCreditSettings(creditSettings));
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
            var msg = result == ApiReturn.Success ? UcConfig.ApiReturnSucceed : UcConfig.ApiReturnFailed;
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
    }
}

