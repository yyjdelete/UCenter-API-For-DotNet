#if NET4_5
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    public abstract class UcApiAsyncBase : UcApiBase, IHttpAsyncHandler
    {
        private class AsyncResultWrapper : IAsyncResult
        {
            private readonly IAsyncResult innerAsyncResult;

            private AsyncResultWrapper(IAsyncResult ar, object state)
            {
                this.innerAsyncResult = ar;
                this.AsyncState = state;
            }
            public static AsyncResultWrapper Create(Task task, object state)
            {
                return new AsyncResultWrapper(task, state);
            }
            public object AsyncState { get; private set; }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return innerAsyncResult.AsyncWaitHandle;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return innerAsyncResult.CompletedSynchronously;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return innerAsyncResult.IsCompleted;
                }
            }
        }
        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var tsk = ProcessRequestAsync(context);
            var ar = AsyncResultWrapper.Create(tsk, extraData);
            tsk.ContinueWith(t =>
            {
                var e = t.Exception;
                if(e != null)
                    e.Handle(_ => true);
                cb(ar);
            }
            , TaskContinuationOptions.ExecuteSynchronously);
            return ar;
        }

        private async Task ProcessRequestAsync(HttpContext context)
        {
            var Args = new UcRequestArguments(context.Request);
            if (!check(Args, context)) return;
            ApiReturn res;
            try
            {
                res = await switchActionAsync(Args, context);
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

        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        {
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            //异步Handler不实现此方法
            throw new NotImplementedException();
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
            IDictionary<string, Func<UcApiAsyncBase, IUcRequestArguments, HttpContext, Task<ApiReturn>>>
            _actionsAsync;
        /// <summary>
        /// 子类可覆盖并添加父类及自己新加的静态方法到一个新的字典中, 不要修改父类返回结果
        /// </summary>
        /// <returns></returns>
        protected virtual 
            IDictionary<string, Func<UcApiAsyncBase, IUcRequestArguments, HttpContext, Task<ApiReturn>>>
            GetActionsAsync()
        {
            if (_actionsAsync == null)
            {
                _actionsAsync = new Dictionary<string, Func<UcApiAsyncBase, IUcRequestArguments, HttpContext, Task<ApiReturn>>>
                    (/*StringComparer.OrdinalIgnoreCase*/)
                {
                    {UcActions.Test, testAsync},
                    {UcActions.DeleteUser, deleteUserAsync},
                    {UcActions.RenameUser, renameUserAsync},
                    {UcActions.GetTag, getTagAsync},
                    {UcActions.SynLogin, synLoginAsync},
                    {UcActions.SynLogout, synLogoutAsync},
                    {UcActions.UpdatePw, updatePwAsync},
                    {UcActions.UpdateBadWords, updateBadWordsAsync},
                    {UcActions.UpdateHosts, updateHostsAsync},
                    {UcActions.UpdateApps, updateAppsAsync},
                    {UcActions.UpdateClient, updateClientAsync},
                    {UcActions.UpdateCredit, updateCreditAsync},
                    {UcActions.GetCreditSettings, getCreditSettingsAsync},
                    {UcActions.GetCredit, getCreditAsync},
                    {UcActions.UpdateCreditSettings, updateCreditSettingsAsync},
                };

            }
            return _actionsAsync;
        }

        private Task<ApiReturn> switchActionAsync(IUcRequestArguments Args, HttpContext context)
        {
            //Maybe switch or 
            //IDictionary <string, Func<IUcRequestArguments, HttpContext, ApiReturn>> for extend
            Task<ApiReturn> res;
            Func<UcApiAsyncBase, IUcRequestArguments, HttpContext, Task<ApiReturn>> funcAsync;
            if (GetActionsAsync().TryGetValue(Args.Action, out funcAsync))
            {
                res = funcAsync(this, Args, context);
            }
            else
            {
                Func<UcApiBase, IUcRequestArguments, HttpContext, ApiReturn> func;
                if (GetActions().TryGetValue(Args.Action, out func))
                {
                    res = Task.FromResult(func(this, Args, context));
                }
                else
                {
                    res = Task.FromResult(ApiReturn.Forbidden);
                }
            }
            return res;
        }
        #endregion

        #region API实现
        private static Task<ApiReturn> testAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            return Task.FromResult(ApiReturn.Success);
        }
        private static Task<ApiReturn> deleteUserAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiDeleteUser)
                return Task.FromResult(ApiReturn.Forbidden);
            var ids = Args.QueryString["ids"];
            var idArray = new List<int>();
            foreach (var id in ids.Split(','))
            {
                int idInt;
                if (int.TryParse(id, out idInt)) idArray.Add(idInt);
            }
            return apiContext.DeleteUserAsync(idArray);
        }
        private static Task<ApiReturn> renameUserAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiRenameUser)
                return Task.FromResult(ApiReturn.Forbidden);
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            var oldusername = Args.QueryString["oldusername"];
            var newusername = Args.QueryString["newusername"];
            return apiContext.RenameUserAsync(uid, oldusername, newusername);
        }
        private static async Task<ApiReturn> getTagAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetTag)
                return ApiReturn.Forbidden;
            var tagName = Args.QueryString["id"];
            context.writeEnd(await apiContext.GetTagAsync(tagName));
            return ApiReturn.Nop;
        }
        private static Task<ApiReturn> synLoginAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogin)
                return Task.FromResult(ApiReturn.Forbidden);
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            return apiContext.SynLoginAsync(uid);
        }
        private static Task<ApiReturn> synLogoutAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiSynLogout)
                return Task.FromResult(ApiReturn.Forbidden);
            context.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            return apiContext.SynLogoutAsync();
        }
        private static Task<ApiReturn> updatePwAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdatePw)
                return Task.FromResult(ApiReturn.Forbidden);
            var username = Args.QueryString["username"];
            var password = Args.QueryString["password"];
            return apiContext.UpdatePwAsync(username, password);
        }
        private static Task<ApiReturn> updateBadWordsAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateBadWords)
                return Task.FromResult(ApiReturn.Forbidden);
            var badWords = new UcBadWords(Args.FormData);
            return apiContext.UpdateBadWordsAsync(badWords);
        }
        private static Task<ApiReturn> updateHostsAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateHosts)
                return Task.FromResult(ApiReturn.Forbidden);
            var hosts = new UcHosts(Args.FormData);
            return apiContext.UpdateHostsAsync(hosts);
        }
        private static Task<ApiReturn> updateAppsAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateApps)
                return Task.FromResult(ApiReturn.Forbidden);
            var apps = new UcApps(Args.FormData);
            return apiContext.UpdateAppsAsync(apps);
        }
        private static Task<ApiReturn> updateClientAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateClient)
                return Task.FromResult(ApiReturn.Forbidden);
            var client = new UcClientSetting(Args.FormData);
            return apiContext.UpdateClientAsync(client);
        }
        private static Task<ApiReturn> updateCreditAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCredit)
                return Task.FromResult(ApiReturn.Forbidden);
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            int amount;
            int.TryParse(Args.QueryString["amount"], out amount);
            return apiContext.UpdateCreditAsync(uid, credit, amount);
        }
        private static async Task<ApiReturn> getCreditSettingsAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCreditSettings)
                return ApiReturn.Forbidden;
            context.writeEnd(await apiContext.GetCreditSettingsAsync());
            return ApiReturn.Nop;
        }
        private static Task<ApiReturn> getCreditAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiGetCredit)
                return Task.FromResult(ApiReturn.Forbidden);
            int uid;
            int.TryParse(Args.QueryString["uid"], out uid);
            int credit;
            int.TryParse(Args.QueryString["credit"], out credit);
            return apiContext.GetCreditAsync(uid, credit);
        }
        private static Task<ApiReturn> updateCreditSettingsAsync(UcApiAsyncBase apiContext, IUcRequestArguments Args, HttpContext context)
        {
            if (!UcConfig.ApiUpdateCreditSettings)
                return Task.FromResult(ApiReturn.Forbidden);
            var creditSettings = new UcCreditSettings(Args.FormData);
            return apiContext.UpdateCreditSettingsAsync(creditSettings);
        }
        #endregion

        #region 抽象方法(异步)
        /// <summary>
        /// (异步)删除用户
        /// </summary>
        /// <param name="ids">Uid</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> DeleteUserAsync(IEnumerable<int> ids)
        {
            return Task.FromResult(DeleteUser(ids));
        }
        /// <summary>
        /// (异步)重命名
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="oldUserName">旧用户名</param>
        /// <param name="newUserName">新用户名</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> RenameUserAsync(int uid, string oldUserName, string newUserName)
        {
            return Task.FromResult(RenameUser(uid, oldUserName, newUserName));
        }
        /// <summary>
        /// (异步)得到标签
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        public virtual Task<UcTagReturns> GetTagAsync(string tagName)
        {
            return Task.FromResult(GetTag(tagName));
        }
        /// <summary>
        /// (异步)同步登陆
        /// </summary>
        /// <param name="uid">uid</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> SynLoginAsync(int uid)
        {
            return Task.FromResult(SynLogin(uid));
        }
        /// <summary>
        /// (异步)同步登出
        /// </summary>
        /// <returns></returns>
        public virtual Task<ApiReturn> SynLogoutAsync()
        {
            return Task.FromResult(SynLogout());
        }
        /// <summary>
        /// (异步)更新密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdatePwAsync(string userName, string passWord)
        {
            return Task.FromResult(UpdatePw(userName, passWord));
        }
        /// <summary>
        /// (异步)更新不良词汇
        /// </summary>
        /// <param name="badWords">不良词汇</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateBadWordsAsync(UcBadWords badWords)
        {
            return Task.FromResult(UpdateBadWords(badWords));
        }
        /// <summary>
        /// (异步)更新Hosts
        /// </summary>
        /// <param name="hosts">Hosts</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateHostsAsync(UcHosts hosts)
        {
            return Task.FromResult(UpdateHosts(hosts));
        }
        /// <summary>
        /// (异步)更新App
        /// </summary>
        /// <param name="apps">App</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateAppsAsync(UcApps apps)
        {
            return Task.FromResult(UpdateApps(apps));
        }
        /// <summary>
        /// (异步)更新UCenter设置
        /// </summary>
        /// <param name="client">UCenter设置</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateClientAsync(UcClientSetting client)
        {
            return Task.FromResult(UpdateClient(client));
        }
        /// <summary>
        /// (异步)更新用户积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <param name="amount">积分增减</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateCreditAsync(int uid, int credit, int amount)
        {
            return Task.FromResult(UpdateCredit(uid, credit, amount));
        }
        /// <summary>
        /// (异步)得到积分设置
        /// </summary>
        /// <returns></returns>
        public virtual Task<UcCreditSettingReturns> GetCreditSettingsAsync()
        {
            return Task.FromResult(GetCreditSettings());
        }
        /// <summary>
        /// (异步)得到积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> GetCreditAsync(int uid, int credit)
        {
            return Task.FromResult(GetCredit(uid, credit));
        }
        /// <summary>
        /// (异步)更新积分设置
        /// </summary>
        /// <param name="creditSettings">积分设置</param>
        /// <returns></returns>
        public virtual Task<ApiReturn> UpdateCreditSettingsAsync(UcCreditSettings creditSettings)
        {
            return Task.FromResult(UpdateCreditSettings(creditSettings));
        }
        #endregion

        #region 抽象方法(同步)
        /// <summary>
        /// (同步)删除用户
        /// </summary>
        /// <param name="ids">Uid</param>
        /// <returns></returns>
        public override ApiReturn DeleteUser(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)重命名
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="oldUserName">旧用户名</param>
        /// <param name="newUserName">新用户名</param>
        /// <returns></returns>
        public override ApiReturn RenameUser(int uid, string oldUserName, string newUserName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)得到标签
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        public override UcTagReturns GetTag(string tagName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)同步登陆
        /// </summary>
        /// <param name="uid">uid</param>
        /// <returns></returns>
        public override ApiReturn SynLogin(int uid)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)同步登出
        /// </summary>
        /// <returns></returns>
        public override ApiReturn SynLogout()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public override ApiReturn UpdatePw(string userName, string passWord)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新不良词汇
        /// </summary>
        /// <param name="badWords">不良词汇</param>
        /// <returns></returns>
        public override ApiReturn UpdateBadWords(UcBadWords badWords)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新Hosts
        /// </summary>
        /// <param name="hosts">Hosts</param>
        /// <returns></returns>
        public override ApiReturn UpdateHosts(UcHosts hosts)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新App
        /// </summary>
        /// <param name="apps">App</param>
        /// <returns></returns>
        public override ApiReturn UpdateApps(UcApps apps)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新UCenter设置
        /// </summary>
        /// <param name="client">UCenter设置</param>
        /// <returns></returns>
        public override ApiReturn UpdateClient(UcClientSetting client)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新用户积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <param name="amount">积分增减</param>
        /// <returns></returns>
        public override ApiReturn UpdateCredit(int uid, int credit, int amount)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)得到积分设置
        /// </summary>
        /// <returns></returns>
        public override UcCreditSettingReturns GetCreditSettings()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)得到积分
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="credit">积分编号</param>
        /// <returns></returns>
        public override ApiReturn GetCredit(int uid, int credit)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// (同步)更新积分设置
        /// </summary>
        /// <param name="creditSettings">积分设置</param>
        /// <returns></returns>
        public override ApiReturn UpdateCreditSettings(UcCreditSettings creditSettings)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
#endif
