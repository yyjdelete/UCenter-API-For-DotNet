#if NET4_5
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DS.Web.UCenter.Client
{
    ///<summary>
    /// UcApi Client
    /// Dozer ��Ȩ����
    /// �����ơ��޸ģ����뱣���ҵ���ϵ��ʽ��
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    ///</summary>
    public interface IUcClientAsync
    {
        /// <summary>
        /// �û�ע��
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="passWord">����</param>
        /// <param name="email">Email</param>
        /// <param name="questionId">��½����</param>
        /// <param name="answer">��</param>
        /// <returns></returns>
        Task<UcUserRegister> UserRegisterAsync(string userName, string passWord, string email, int questionId = 0, string answer = "");

        /// <summary>
        /// �û���½
        /// </summary>
        /// <param name="userName">�û���/Uid/Email</param>
        /// <param name="passWord">����</param>
        /// <param name="loginMethod">��¼��ʽ</param>
        /// <param name="checkques">��Ҫ��½����</param>
        /// <param name="questionId">����ID</param>
        /// <param name="answer">��</param>
        /// <returns></returns>
        Task<UcUserLogin> UserLoginAsync(string userName, string passWord, LoginMethod loginMethod = LoginMethod.UserName, bool checkques = false, int questionId = 0, string answer = "");

        /// <summary>
        /// �õ��û���Ϣ
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <returns></returns>
        Task<UcUserInfo> UserInfoAsync(string userName);

        /// <summary>
        /// �õ��û���Ϣ
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<UcUserInfo> UserInfoAsync(int uid);

        /// <summary>
        /// �����û���Ϣ
        /// ������������֤�û���ԭ�����Ƿ���ȷ������ָ�� ignoreoldpw Ϊ 1��
        /// ���ֻ�޸� Email ���޸����룬���� newpw Ϊ�գ�
        /// ͬ�����ֻ�޸����벻�޸� Email������ email Ϊ�ա�
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="oldPw">������</param>
        /// <param name="newPw">������</param>
        /// <param name="email">Email</param>
        /// <param name="ignoreOldPw">�Ƿ���Ծ�����</param>
        /// <param name="questionId">������ʾ������</param>
        /// <param name="answer">������ʾ�����</param>
        /// <returns></returns>
        Task<UcUserEdit> UserEditAsync(string userName, string oldPw, string newPw, string email, bool ignoreOldPw = false, int questionId = 0, string answer = "");

        /// <summary>
        /// ɾ���û�
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<bool> UserDeleteAsync(params int[] uid);

        /// <summary>
        /// ɾ���û�ͷ��
        /// </summary>
        /// <param name="uid">Uid</param>
        Task UserDeleteAvatarAsync(params int[] uid);

        /// <summary>
        /// ͬ����½
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns>ͬ����½�� Html ����</returns>
        Task<string> UserSynloginAsync(int uid);

        /// <summary>
        /// ͬ���ǳ�
        /// </summary>
        /// <returns>ͬ���ǳ��� Html ����</returns>
        Task<string> UserSynLogoutAsync();

        /// <summary>
        /// ��� Email ��ʽ
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        Task<UcUserCheckEmail> UserCheckEmailAsync(string email);

        /// <summary>
        /// �����ܱ����û�
        /// </summary>
        /// <param name="admin">��������Ա</param>
        /// <param name="userName">�û���</param>
        /// <returns></returns>
        Task<bool> UserAddProtectedAsync(string admin, params string[] userName);

        /// <summary>
        /// ɾ���ܱ����û�
        /// </summary>
        /// <param name="admin">��������Ա</param>
        /// <param name="userName">�û���</param>
        /// <returns></returns>
        Task<bool> UserDeleteProtectedAsync(string admin, params string[] userName);

        /// <summary>
        /// �õ��ܱ����û�
        /// </summary>
        /// <returns></returns>
        Task<UcUserProtecteds> UserGetProtectedAsync();

        /// <summary>
        /// �ϲ��û�
        /// </summary>
        /// <param name="oldUserName">���û���</param>
        /// <param name="newUserName">���û���</param>
        /// <param name="uid">Uid</param>
        /// <param name="passWord">����</param>
        /// <param name="email">Email</param>
        /// <returns></returns>
        Task<UcUserMerge> UserMergeAsync(string oldUserName, string newUserName, int uid, string passWord, string email);

        /// <summary>
        /// �Ƴ������û���¼
        /// </summary>
        /// <param name="userName">�û���</param>
        Task UserMergeRemoveAsync(string userName);

        /// <summary>
        /// �õ��û�����
        /// </summary>
        /// <param name="appId">Ӧ�ó���Id</param>
        /// <param name="uid">Uid</param>
        /// <param name="credit">���ֱ��</param>
        /// <returns></returns>
        Task<int> UserGetCreditAsync(int appId, int uid, int credit);

        /// <summary>
        /// �������Ϣ
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<UcPmCheckNew> PmCheckNewAsync(int uid);

        /// <summary>
        /// ���Ͷ���Ϣ
        /// </summary>
        /// <param name="fromUid">�������û� ID��0 Ϊϵͳ��Ϣ</param>
        /// <param name="replyPmId">�ظ�����Ϣ ID��0:�����µĶ���Ϣ������ 0:�ظ�ָ���Ķ���Ϣ</param>
        /// <param name="subject">��Ϣ����</param>
        /// <param name="message">��Ϣ����</param>
        /// <param name="msgTo">�ռ���ID</param>
        /// <returns></returns>
        Task<UcPmSend> PmSendAsync(int fromUid, int replyPmId, string subject, string message, params int[] msgTo);

        /// <summary>
        /// ���Ͷ���Ϣ
        /// </summary>
        /// <param name="fromUid">�������û� ID��0 Ϊϵͳ��Ϣ</param>
        /// <param name="replyPmId">�ظ�����Ϣ ID��0:�����µĶ���Ϣ������ 0:�ظ�ָ���Ķ���Ϣ</param>
        /// <param name="subject">��Ϣ����</param>
        /// <param name="message">��Ϣ����</param>
        /// <param name="msgTo">�ռ����û���</param>
        /// <returns></returns>
        Task<UcPmSend> PmSendAsync(int fromUid, int replyPmId, string subject, string message, params string[] msgTo);

        /// <summary>
        /// ɾ������Ϣ
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="folder">�ļ���</param>
        /// <param name="pmIds">����ϢID</param>
        /// <returns>ɾ��������</returns>
        Task<int> PmDeleteAsync(int uid, PmDeleteFolder folder, params int[] pmIds);

        /// <summary>
        /// ɾ���Ự
        /// </summary>
        /// <param name="uid">������</param>
        /// <param name="toUids">�ռ���</param>
        /// <returns>ɾ��������</returns>
        Task<int> PmDeleteAsync(int uid, params int[] toUids);

        /// <summary>
        /// �޸��Ķ�״̬
        /// </summary>
        /// <param name="uid">������</param>
        /// <param name="toUids">�ռ���</param>
        /// <param name="pmIds">����ϢID</param>
        /// <param name="readStatus">�Ķ�״̬</param>
        Task PmReadStatusAsync(int uid, int toUids, int pmIds = 0, ReadStatus readStatus = ReadStatus.Readed);

        /// <summary>
        /// �޸��Ķ�״̬
        /// </summary>
        /// <param name="uid">������</param>
        /// <param name="toUids">�ռ�������</param>
        /// <param name="pmIds">����ϢID����</param>
        /// <param name="readStatus">�Ķ�״̬</param>
        Task PmReadStatusAsync(int uid, IEnumerable<int> toUids, IEnumerable<int> pmIds, ReadStatus readStatus = ReadStatus.Readed);

        /// <summary>
        /// ��ȡ����Ϣ�б�
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="page">��ǰҳ��ţ�Ĭ��ֵ 1</param>
        /// <param name="pageSize">ÿҳ�����Ŀ����Ĭ��ֵ 10</param>
        /// <param name="folder">����Ϣ���ڵ��ļ���</param>
        /// <param name="filter">���˷�ʽ</param>
        /// <param name="msgLen">��ȡ����Ϣ�������ֵĳ��ȣ�0 Ϊ����ȡ��Ĭ��ֵ 0</param>
        /// <returns></returns>
        Task<UcPmList> PmListAsync(int uid, int page = 1, int pageSize = 10, PmReadFolder folder = PmReadFolder.NewBox, PmReadFilter filter = PmReadFilter.NewPm, int msgLen = 0);

        /// <summary>
        /// ��ȡ����Ϣ����
        /// ���ӿں������ڷ���ָ���û���ָ����Ϣ ID ����Ϣ�����ص������а�����������Ϣ�Ļظ���
        /// ���ָ�� touid ��������ô����Ϣ���г����� uid �� touid ֮��Ķ���Ϣ��daterange ����ָ��������Ϣ�����ڷ�Χ��
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="pmId">����ϢID</param>
        /// <param name="toUid">�ռ���ID</param>
        /// <param name="dateRange">���ڷ�Χ</param>
        /// <returns></returns>
        Task<UcPmView> PmViewAsync(int uid, int pmId, int toUid = 0, DateRange dateRange = DateRange.Today);

        /// <summary>
        /// ��ȡ��������Ϣ����
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="type">����</param>
        /// <param name="pmId">����ϢID</param>
        /// <returns></returns>
        Task<UcPm> PmViewNodeAsync(int uid, ViewType type = ViewType.Specified, int pmId = 0);

        /// <summary>
        /// ����δ����Ϣ��ʾ
        /// </summary>
        /// <param name="uid">Uid</param>
        Task PmIgnoreAsync(int uid);

        /// <summary>
        /// �õ�������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<UcPmBlacklsGet> PmBlacklsGetAsync(int uid);

        /// <summary>
        /// ���ú�����Ϊ��ֹ�����ˣ����ԭ���ݣ�
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<bool> PmBlacklsSetAllAsync(int uid);

        /// <summary>
        /// ���ú����������ԭ���ݣ�
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="userName">�������û���</param>
        /// <returns></returns>
        Task<bool> PmBlacklsSetAsync(int uid, params string[] userName);

        /// <summary>
        /// ��Ӻ�����Ϊ��ֹ������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task<bool> PmBlacklsAddAllAsync(int uid);

        /// <summary>
        /// ���Ӻ�����
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="userName">�������û���</param>
        /// <returns></returns>
        Task<bool> PmBlacklsAddAsync(int uid, params string[] userName);

        /// <summary>
        /// ɾ���������еĽ�ֹ������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <returns></returns>
        Task PmBlacklsDeleteAllAsync(int uid);

        /// <summary>
        /// ɾ��������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="userName">�������û���</param>
        Task PmBlacklsDeleteAsync(int uid, params string[] userName);

        /// <summary>
        /// ���Ӻ���
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="friendId">����ID</param>
        /// <param name="comment">��ע</param>
        /// <returns></returns>
        Task<bool> FriendAddAsync(int uid, int friendId, string comment = "");

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="friendIds">����ID</param>
        /// <returns></returns>
        Task<bool> FriendDeleteAsync(int uid, params int[] friendIds);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="direction">����</param>
        /// <returns>������Ŀ</returns>
        Task<int> FriendTotalNumAsync(int uid, FriendDirection direction = FriendDirection.All);

        /// <summary>
        /// �����б�
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="page">��ǰҳ���</param>
        /// <param name="pageSize">ÿҳ�����Ŀ��</param>
        /// <param name="totalNum">��������</param>
        /// <param name="direction">����</param>
        /// <returns></returns>
        Task<UcFriends> FriendListAsync(int uid, int page = 1, int pageSize = 10, int totalNum = 10, FriendDirection direction = FriendDirection.All);

        /// <summary>
        /// ���ֶһ�����
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="from">ԭ����</param>
        /// <param name="to">Ŀ�����</param>
        /// <param name="toAppId">Ŀ��Ӧ��ID</param>
        /// <param name="amount">��������</param>
        /// <returns></returns>
        Task<bool> CreditExchangeRequestAsync(int uid, int from, int to, int toAppId, int amount);

        ///<summary>
        /// �޸�ͷ��
        ///</summary>
        ///<param name="uid">Uid</param>
        ///<param name="type"></param>
        ///<returns></returns>
        Task<string> AvatarAsync(int uid, AvatarType type = AvatarType.Virtual);

        /// <summary>
        /// �õ�ͷ���ַ
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="size">��С</param>
        /// <param name="type">����</param>
        /// <returns></returns>
        Task<string> AvatarUrlAsync(int uid,AvatarSize size,AvatarType type = AvatarType.Virtual);

        /// <summary>
        /// ���ͷ���Ƿ����
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> AvatarCheckAsync(int uid, AvatarSize size = AvatarSize.Middle, AvatarType type = AvatarType.Virtual);

        /// <summary>
        /// ��ȡ��ǩ����
        /// </summary>
        /// <param name="tagName">��ǩ��</param>
        /// <param name="number">Ӧ�ó���ID��Ӧ������</param>
        /// <returns></returns>
        Task<UcTags> TagGetAsync(string tagName, IEnumerable<KeyValuePair<string, string>> number);

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="icon">ͼ�����ͣ��磺thread��post��video��goods��reward��debate��blog��album��comment��wall��friend</param>
        /// <param name="uid">Uid</param>
        /// <param name="userName">�û���</param>
        /// <param name="titleTemplate">����ģ��</param>
        /// <param name="titleData">������������</param>
        /// <param name="bodyTemplate">����ģ��</param>
        /// <param name="bodyData">ģ������</param>
        /// <param name="bodyGeneral">��ͬ�¼��ϲ�ʱ�õ������ݣ��ض������飬ֻ�����name��link������</param>
        /// <param name="targetIds">����</param>
        /// <param name="images">���ͼƬ�� URL �����ӵ�ַ��һ��ͼƬ��ַ��һ�����ӵ�ַ</param>
        /// <returns></returns>
        Task<int> FeedAddAsync(FeedIcon icon, int uid, string userName, string titleTemplate, string titleData, string bodyTemplate, string bodyData, string bodyGeneral, string targetIds, params string[] images);

        /// <summary>
        /// �õ�Feed
        /// </summary>
        /// <param name="limit">��������</param>
        /// <returns></returns>
        Task<UcFeeds> FeedGetAsync(int limit);

        /// <summary>
        /// �õ�Ӧ���б�
        /// </summary>
        /// <returns></returns>
        Task<UcApps> AppListAsync();

        /// <summary>
        /// ����ʼ�������
        /// </summary>
        /// <param name="subject">����</param>
        /// <param name="message">����</param>
        /// <param name="uids">Uid</param>
        /// <returns></returns>
        Task<UcMailQueue> MailQueueAsync(string subject, string message,params int[] uids);

        /// <summary>
        /// ����ʼ�������
        /// </summary>
        /// <param name="subject">����</param>
        /// <param name="message">����</param>
        /// <param name="emails">Ŀ��Email</param>
        /// <returns></returns>
        Task<UcMailQueue> MailQueueAsync(string subject, string message, params string[] emails);

        /// <summary>
        /// ����ʼ�������
        /// </summary>
        /// <param name="subject">����</param>
        /// <param name="message">����</param>
        /// <param name="uids">Uid</param>
        /// <param name="emails">Ŀ��email</param>
        /// <returns></returns>
        Task<UcMailQueue> MailQueueAsync(string subject, string message, int[] uids, string[] emails);

        /// <summary>
        /// ����ʼ�������
        /// </summary>
        /// <param name="subject">����</param>
        /// <param name="message">����</param>
        /// <param name="fromMail">�����ˣ���ѡ������Ĭ��Ϊ�գ�uc��̨���õ��ʼ���Դ��Ϊ�����˵�ַ</param>
        /// <param name="charset">�ʼ��ַ�������ѡ������Ĭ��Ϊgbk</param>
        /// <param name="htmlOn">�Ƿ���html��ʽ���ʼ�����ѡ������Ĭ��ΪFALSE�����ı��ʼ�</param>
        /// <param name="level">�ʼ����𣬿�ѡ������Ĭ��Ϊ1�����ִ�����ȷ��ͣ�ȡֵΪ0��ʱ���������ͣ��ʼ��������</param>
        /// <param name="uids">Uid</param>
        /// <param name="emails">Ŀ��email</param>
        /// <returns></returns>
        Task<UcMailQueue> MailQueueAsync(string subject,string message,string fromMail,string charset,bool htmlOn,int level,int[] uids,string[] emails);
    }
}
#endif