using Communication;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKS.Manager
{
    public class Account
    {
        private static volatile Account _instance = null;
        private static readonly object _locker = new object();
        public static Account Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new Account();
                        }
                    }
                }
                return _instance;
            }
        }

        #region event
        public static event Action<UserType> LoginChangeEvent;

        public static void OnLoginChange(UserType type)
        {
            LoginChangeEvent?.Invoke(type);
        } 
        #endregion
        private Account()
        {
            UserConfigs = Configuration.GetConfig<List<UserConfig>>(nameof(UserConfig));
        }
        private UserType _currentUser = UserType.Operator;
        public UserType CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnLoginChange(_currentUser);
            }
        }

        public List<UserConfig> UserConfigs { get; set; }

    }

    public class UserConfig
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }

    public enum UserType
    {
        Operator,
        Technician,
        Administrator,
    }
}
