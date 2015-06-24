using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace DS.Web.UCenter.Api
{
    /// <summary>
    /// UcApi
    /// Dozer 版权所有
    /// 允许复制、修改，但请保留我的联系方式！
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public abstract class UcApiBase : IHttpHandler, IRequiresSessionState
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
            var Response = context.Response;
            var Request = context.Request;
            var Args = new UcRequestArguments(Request);
            if (!check(Args, Response)) return;
            try
            {
                switchAction(Args, Response);
            }
            catch (Exception e)
            {
                if (e is NotImplementedException || e is NotSupportedException)
                {
                    Response.writeForbidden();
                }
                else
                {
                    Response.writeEnd(ApiReturn.Failed);
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
        private bool check(IUcRequestArguments Args, HttpResponse Response)
        {
            if (Args.IsInvalidRequest)
            {
                Response.writeEnd("Invalid Request");
            }
            if (Args.IsAuthracationExpiried)
            {
                Response.writeEnd("Authracation has expiried");
            }
            return true;
        }
        private void switchAction(IUcRequestArguments Args, HttpResponse Response)
        {
            //Maybe switch or 
            //IDictionary <string, Action<IUcRequestArguments, HttpResponse>> for extend
            if (Args.Action == UcActions.Test)
            {
                test(Args, Response);
            }
            else if (Args.Action == UcActions.DeleteUser)
            {
                deleteUser(Args, Response);
            }
            else if (Args.Action == UcActions.RenameUser)
            {
                renameUser(Args, Response);
            }
            else if (Args.Action == UcActions.GetTag)
            {
                getTag(Args, Response);
            }
            else if (Args.Action == UcActions.SynLogin)
            {
                synLogin(Args, Response);
            }
            else if (Args.Action == UcActions.SynLogout)
            {
                synLogout(Args, Response);
            }
            else if (Args.Action == UcActions.UpdatePw)
            {
                updatePw(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateBadWords)
            {
                updateBadWords(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateHosts)
            {
                updateHosts(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateApps)
            {
                updateApps(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateClient)
            {
                updateClient(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateCredit)
            {
                updateCredit(Args, Response);
            }
            else if (Args.Action == UcActions.GetCreditSettings)
            {
                getCreditSettings(Args, Response);
            }
            else if (Args.Action == UcActions.GetCredit)
            {
                getCredit(Args, Response);
            }
            else if (Args.Action == UcActions.UpdateCreditSettings)
            {
                updateCreditSettings(Args, Response);
            }
            else
            {
                Response.writeForbidden();
            }
        }
        #endregion

        #region API实现
        private void test(IUcRequestArguments Args, HttpResponse Response)
        {
            Response.writeEnd(UcConfig.ApiReturnSucceed);
        }
        private void deleteUser(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiDeleteUser) Response.writeForbidden();
            var ids = Args.QueryString["ids"];
            var idArray = new List<int>();
            foreach (var id in ids.Split(','))
            {
                int idInt;
                if (int.TryParse(id, out idInt)) idArray.Add(idInt);
            }
            Response.writeEnd(DeleteUser(idArray));
        }
        private void renameUser(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiRenameUser) Response.writeForbidden();
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            var oldusername = Args.QueryString["oldusername"];
            var newusername = Args.QueryString["newusername"];
            Response.writeEnd(RenameUser(uid, oldusername, newusername));
        }
        private void getTag(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiGetTag) Response.writeForbidden();
            var tagName = Args.QueryString["id"];
            Response.writeEnd(GetTag(tagName));
        }
        private void synLogin(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiSynLogin) Response.writeForbidden();
            Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            Response.writeEnd(SynLogin(uid));
        }
        private void synLogout(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiSynLogout) Response.writeForbidden();
            Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            Response.writeEnd(SynLogout());
        }
        private void updatePw(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdatePw) Response.writeForbidden();
            var username = Args.QueryString["username"];
            var password = Args.QueryString["password"];
            Response.writeEnd(UpdatePw(username, password));
        }
        private void updateBadWords(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateBadWords) Response.writeForbidden();
            var badWords = new UcBadWords(Args.FormData);
            Response.writeEnd(UpdateBadWords(badWords));
        }
        private void updateHosts(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateHosts) Response.writeForbidden();
            var hosts = new UcHosts(Args.FormData);
            Response.writeEnd(UpdateHosts(hosts));
        }
        private void updateApps(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateApps) Response.writeForbidden();
            var apps = new UcApps(Args.FormData);
            Response.writeEnd(UpdateApps(apps));
        }
        private void updateClient(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateClient) Response.writeForbidden();
            var client = new UcClientSetting(Args.FormData);
            Response.writeEnd(UpdateClient(client));
        }
        private void updateCredit(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateCredit) Response.writeForbidden();
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            int amount;
            int.TryParse(Args.QueryString["amount"], out amount);
            Response.writeEnd(UpdateCredit(uid, credit, amount));
        }
        private void getCreditSettings(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiGetCreditSettings) Response.writeForbidden();
            Response.writeEnd(GetCreditSettings());
        }
        private void getCredit(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiGetCredit) Response.writeForbidden();
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            Response.writeEnd(GetCredit(uid, credit));
        }
        private void updateCreditSettings(IUcRequestArguments Args, HttpResponse Response)
        {
            if (!UcConfig.ApiUpdateCreditSettings) Response.writeForbidden();
            var creditSettings = new UcCreditSettings(Args.FormData);
            Response.writeEnd(UpdateCreditSettings(creditSettings));
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
        public static void writeEnd(this HttpResponse Response, string msg)
        {
            Response.Write(msg);
            Response.End();
        }
        public static void writeEnd<T>(this HttpResponse Response, UcCollectionReturnBase<T> msg)
            where T : UcItemReturnBase
        {
            writeEnd(Response, msg.ToString());
        }
        public static void writeEnd(this HttpResponse Response, ApiReturn result)
        {
            var msg = result == ApiReturn.Success ? UcConfig.ApiReturnSucceed : UcConfig.ApiReturnFailed;
            Response.Write(msg);
            Response.End();
        }
        public static void writeForbidden(this HttpResponse Response)
        {
            writeEnd(Response, UcConfig.ApiReturnForbidden);
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

