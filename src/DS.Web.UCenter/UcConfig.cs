using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DS.Web.UCenter
{
    /// <summary>
    /// ����
    /// Dozer ��Ȩ����
    /// �����ơ��޸ģ����뱣���ҵ���ϵ��ʽ��
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public static class UcConfig
    {
        private static IDictionary<string, string> _items;
        private static IDictionary<string,string> Items{get { return _items ?? (_items = new Dictionary<string, string>()); }}

        /// <summary>
        /// ��ȡ��ֵ����������(���̰߳�ȫ)
        /// </summary>
        /// <param name="key">��</param>
        /// <returns>ֵ</returns>
        private static string getValueTemp(string key)
        {
            string val;
            if (!Items.TryGetValue(key, out val))
            {
                val = ConfigurationManager.AppSettings[key];
                Items[key] = val;
            }
            return val;
        }

        /// <summary>
        /// �õ�����
        /// </summary>
        /// <param name="key">KEY</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <param name="checkEmpty">�Ƿ����Ƿ�Ϊ��</param>
        /// <exception cref="ConfigurationErrorsException">������Ϣ��ʧ����</exception>
        /// <returns>�ַ�������</returns>
        private static string getStringValue(string key, string defaultValue = "", bool checkEmpty = false)
        {
            var str = getValueTemp(key);
            if (str != null)
                str = str.Trim();
            if (checkEmpty && string.IsNullOrEmpty(str))
                throw new ConfigurationErrorsException(string.Format("ȱ�� {0} ��������Ϣ", key));
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// �õ�����
        /// </summary>
        /// <param name="key">KEY</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <param name="checkEmpty">�Ƿ����Ƿ�Ϊ��</param>
        /// <exception cref="ConfigurationErrorsException">������Ϣ��ʧ����</exception>
        /// <returns>bool����</returns>
        private static bool getBoolValue(string key, bool defaultValue = false, bool checkEmpty = false)
        {
            var str = getValueTemp(key);
            if (checkEmpty && string.IsNullOrEmpty(str))
                throw new ConfigurationErrorsException(string.Format("ȱ�� {0} ��������Ϣ", key));
            bool result;
            return bool.TryParse(str, out result) ? result : defaultValue;
        }

        /// <summary>
        /// �õ�����
        /// </summary>
        /// <param name="key">KEY</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <param name="checkEmpty">�Ƿ����Ƿ�Ϊ��</param>
        /// <exception cref="ConfigurationErrorsException">������Ϣ��ʧ����</exception>
        /// <returns>int����</returns>
        private static int getIntValue(string key, int defaultValue = 0, bool checkEmpty = false)
        {
            var str = getValueTemp(key);
            if (checkEmpty && string.IsNullOrEmpty(str))
                throw new ConfigurationErrorsException(string.Format("ȱ�� {0} ��������Ϣ", key));
            int result;
            return int.TryParse(str, out result) ? result : defaultValue;
        }


        /// <summary>
        /// �ͻ��˰汾
        /// </summary>
        public static string UcClientVersion
        {
            get { return getStringValue("UC_CLIENT_VERSION", "1.5.2"); }
        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public static string UcClientRelease
        {
            get { return getStringValue("UC_CLIENT_RELEASE", "20101001"); }
        }
        /// <summary>
        /// �Ƿ�����ɾ���û�
        /// </summary>
        public static bool ApiDeleteUser
        {
            get { return getBoolValue("API_DELETEUSER", true); }
        }
        /// <summary>
        /// �Ƿ������������û�
        /// </summary>
        public static bool ApiRenameUser
        {
            get { return getBoolValue("API_RENAMEUSER", true); }
        }
        /// <summary>
        /// �Ƿ�����õ���ǩ
        /// </summary>
        public static bool ApiGetTag
        {
            get { return getBoolValue("API_GETTAG", true); }
        }
        /// <summary>
        /// �Ƿ�����ͬ����¼
        /// </summary>
        public static bool ApiSynLogin
        {
            get { return getBoolValue("API_SYNLOGIN", true); }
        }
        /// <summary>
        /// �Ƿ�����ͬ���ǳ�
        /// </summary>
        public static bool ApiSynLogout
        {
            get { return getBoolValue("API_SYNLOGOUT", true); }
        }
        /// <summary>
        /// �Ƿ������������
        /// </summary>
        public static bool ApiUpdatePw
        {
            get { return getBoolValue("API_UPDATEPW", true); }
        }
        /// <summary>
        /// �Ƿ�������¹ؼ���
        /// </summary>
        public static bool ApiUpdateBadWords
        {
            get { return getBoolValue("API_UPDATEBADWORDS", true); }
        }
        /// <summary>
        /// �Ƿ��������������������
        /// </summary>
        public static bool ApiUpdateHosts
        {
            get { return getBoolValue("API_UPDATEHOSTS", true); }
        }
        /// <summary>
        /// �Ƿ��������Ӧ���б�
        /// </summary>
        public static bool ApiUpdateApps
        {
            get { return getBoolValue("API_UPDATEAPPS", true); }
        }
        /// <summary>
        /// �Ƿ�������¿ͻ��˻���
        /// </summary>
        public static bool ApiUpdateClient
        {
            get { return getBoolValue("API_UPDATECLIENT", true); }
        }
        /// <summary>
        /// �Ƿ���������û�����
        /// </summary>
        public static bool ApiUpdateCredit
        {
            get { return getBoolValue("API_UPDATECREDIT", true); }
        }
        /// <summary>
        /// �Ƿ�������UCenter�ṩ��������
        /// </summary>
        public static bool ApiGetCreditSettings
        {
            get { return getBoolValue("API_GETCREDITSETTINGS", true); }
        }
        /// <summary>
        /// �Ƿ������ȡ�û���ĳ�����
        /// </summary>
        public static bool ApiGetCredit
        {
            get { return getBoolValue("API_GETCREDIT", true); }
        }
        /// <summary>
        /// �Ƿ��������Ӧ�û�������
        /// </summary>
        public static bool ApiUpdateCreditSettings
        {
            get { return getBoolValue("API_UPDATECREDITSETTINGS", true); }
        }

        /// <summary>
        /// ���سɹ�
        /// </summary>
        public static string ApiReturnSucceed
        {
            get { return getStringValue("API_RETURN_SUCCEED", "1"); }
        }
        /// <summary>
        /// ����ʧ��
        /// </summary>
        public static string ApiReturnFailed
        {
            get { return getStringValue("API_RETURN_FAILED", "-1"); }
        }
        /// <summary>
        /// ���ؽ���
        /// </summary>
        public static string ApiReturnForbidden
        {
            get { return getStringValue("API_RETURN_FORBIDDEN", "-2"); }
        }


        /// <summary>
        /// �� UCenter ��ͨ����Կ, Ҫ�� UCenter ����һ��
        /// </summary>
        public static string UcKey
        {
            get { return getStringValue("UC_KEY", checkEmpty: true); }
        }

        /// <summary>
        /// UCenter��ַ
        /// </summary>
        public static string UcApi
        {
            get
            {
                var str = getStringValue("UC_API", checkEmpty: true);
                if (!str.EndsWith("/")) str = str + "/";
                return str;
            }
        }

        private static Encoding ucEncoding;
        /// <summary>
        /// Ĭ�ϱ���
        /// </summary>
        public static Encoding UcEncoding
        {
            get
            {
                return ucEncoding ?? (ucEncoding = Encoding.GetEncoding(UcCharset));
            }
        }

        /// <summary>
        /// Ĭ�ϱ�������
        /// </summary>
        public static string UcCharset
        {
            get
            {
                return getStringValue("UC_CHARSET", checkEmpty:true);
            }
        }

        /// <summary>
        /// UCenter IP
        /// </summary>
        public static string UcIp
        {
            get
            {
                return getStringValue("UC_IP");
            }
        }

        /// <summary>
        /// Ӧ��ID
        /// </summary>
        public static string UcAppid
        {
            get { return getStringValue("UC_APPID", checkEmpty: true); }
        }
    }
}

