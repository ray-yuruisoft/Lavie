using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Repositories;
using Lavie.Utilities.Cryptography;
using Lavie.Modules.Mail.Models;
using System.Text;

namespace Lavie.Modules.Admin.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetItemByUserIDAsync(int userID, UserStatus? status = null);
        UserInfo GetItemByUserID(int userID, UserStatus? status = null);
        Task<UserInfo> GetItemByUsernameAsync(string username, UserStatus? status = null);
        Task<UserInfo> GetItemByMobileAsync(string mobile, UserStatus? status = null);
        Task<UserInfo> GetItemByEmailAsync(string email, UserStatus? status = null);
        Task<UserInfo> GetItemByTokenAsync(string token, UserStatus? status = null);
        Task<UserInfo> GetItemByWXOpenIDAsync(string wxOpenID);
        Task<UserInfo> GetItemByWXAOpenIDAsync(string wxaOpenID);
        Task<List<UserInfoWarpper>> GetUserInfoWarpperList(IEnumerable<int> userIDs);
        Task<string> GetHeadURLAsync(int userID);
        Task<bool> IsExistsAsync(int userID, UserStatus? status = null);
        bool IsExists(int userID, UserStatus status);
        Task<bool> IsExistsUsernameAsync(string username);
        Task<bool> IsExistsMobileAsync(string mobile);
        Task<bool> IsExistsEmailAsync(string username);
        Task<bool> VerifyExistsUsernameAsync(int userID, string username);
        Task<bool> VerifyExistsMobileAsync(int userID, string mobile);
        Task<bool> VerifyExistsEmailAsync(int userID, string email);
        Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<IPagedList<UserInfo>> GetPagedListAsync(UserSearchCriteria criteria, PagingInfo pagingInfo);
        Task<Page<UserInfo>> GetPageAsync(UserSearchCriteria criteria, PagingInfo pagingInfo);
        Task<bool> SaveAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeUsernameAsync(int userID, string newUsername, ModelStateDictionary modelState);
        Task<bool> ChangeMobileAsync(int userID, string newMobile, ModelStateDictionary modelState);
        Task<bool> ChangeDisplayNameAsync(int userID, string newDisplayName, ModelStateDictionary modelState);
        Task<bool> ChangeLogoAsync(int userID, string logoURL, ModelStateDictionary modelState);
        Task<bool> ChangePasswordAsync(int userID, string rawPassword, ModelStateDictionary modelState);
        Task<bool> ResetPasswordByMobileAsync(string username, string rawPassword, ModelStateDictionary modelState);
        Task<bool> ChangeHeadAsync(int userID, string newHead);
        Task<bool> GetPasswordAsync(UserGetPasswordInput input, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int userID, ModelStateDictionary modelState);
        Task<bool> ChangeStatusAsync(int userID, UserStatus status);
        Task<bool> UpdateClientAgentAsync(int userID, String clientAgent, String ip);
        Task<bool> UpdateTokenAsync(int userID, String token);
        Task<bool> UpdateWXOpenIDAsync(int userID, String wxOpenID, ModelStateDictionary modelState);
        Task<bool> CleanWXOpenIDAsync(int userID);
        Task<bool> UpdateWXAOpenIDAsync(int userID, String wxaOpenID, ModelStateDictionary modelState);
        Task<bool> CleanWXAOpenIDAsync(int userID);
        Task<bool> ClearClientAgentAsync(int userID, String clientAgent, String uuid);
        Task<bool> SignInAsync(Func<Task<UserInfo>> getUser, Action<UserInfo> afterSignIn);
        Task<bool> SignOutAsync();
        Task<bool> GetMobileValidationCode(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> VerifyMobileValidationCode(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState, string defaultCode = null);
        Task<bool> FinishVerifyMobileValidationCode(string mobile, int typeID, ModelStateDictionary modelState);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ICacheModule _cache;
        private readonly LavieContext _context;
        private readonly IModuleRegistry _moduleRegistry;
        private readonly ISmtpMailModule _smtpMailModule;
        private const string UserCacheKeyFormat = "User:{0}";
        private readonly AppSettingsHelper _appSettings;
        private readonly IGroupService _groupService;

        public UserService(IUserRepository repository
            , IModuleRegistry moduleRegistry
            , LavieContext context
            , AppSettingsHelper appSettings
            , IGroupService groupService
            )
        {
            this._repository = repository;
            this._context = context;
            this._moduleRegistry = moduleRegistry;
            this._cache = moduleRegistry.GetModules<ICacheModule>().Last();
            this._smtpMailModule = moduleRegistry.GetModules<ISmtpMailModule>().Last();
            this._appSettings = appSettings;
            this._groupService = groupService;
        }

        #region IUserService Members
        public async Task<UserInfo> GetItemByUserIDAsync(int userID, UserStatus? status = null)
        {
            if (!status.HasValue)
                return await _repository.GetItemByUserIDAsync(userID, null);

            return _cache != null ? await _cache.GetItemAsync<UserInfo>(
                UserCacheKeyFormat.FormatWith(userID),
                async () => await _repository.GetItemByUserIDAsync(userID, status)
                ) : await _repository.GetItemByUserIDAsync(userID, status);
        }
        public UserInfo GetItemByUserID(int userID, UserStatus? status = null)
        {
            if (!status.HasValue)
                return _repository.GetItemByUserID(userID, null);

            return _cache != null ? _cache.GetItem<UserInfo>(
                UserCacheKeyFormat.FormatWith(userID),
                () => _repository.GetItemByUserID(userID, status)
                ) : _repository.GetItemByUserID(userID, status);
        }
        public async Task<UserInfo> GetItemByUsernameAsync(string username, UserStatus? status = null)
        {
            if (username.IsNullOrWhiteSpace()) return null;
            return await _repository.GetItemByUsernameAsync(username, status);
        }
        public async Task<UserInfo> GetItemByMobileAsync(string mobile, UserStatus? status = null)
        {
            return await _repository.GetItemByMobileAsync(mobile, status);
        }
        public async Task<UserInfo> GetItemByEmailAsync(string email, UserStatus? status = null)
        {
            return await _repository.GetItemByEmailAsync(email, status);
        }
        public async Task<UserInfo> GetItemByTokenAsync(string token, UserStatus? status = null)
        {
            return await _repository.GetItemByTokenAsync(token, status);
        }
        public async Task<UserInfo> GetItemByWXOpenIDAsync(string wxOpenID)
        {
            return await _repository.GetItemByWXOpenIDAsync(wxOpenID);
        }
        public async Task<UserInfo> GetItemByWXAOpenIDAsync(string wxaOpenID)
        {
            return await _repository.GetItemByWXAOpenIDAsync(wxaOpenID);
        }
        public async Task<List<UserInfoWarpper>> GetUserInfoWarpperList(IEnumerable<int> userIDs)
        {
            return await _repository.GetUserInfoWarpperList(userIDs);
        }
        public async Task<string> GetHeadURLAsync(int userID)
        {
            return await _repository.GetHeadURLAsync(userID);
        }
        public async Task<bool> IsExistsAsync(int userID, UserStatus? status = null)
        {
            return await _repository.IsExistsAsync(userID, status);
        }
        public bool IsExists(int userID, UserStatus status)
        {
            return _repository.IsExists(userID, status);
        }
        public async Task<bool> IsExistsUsernameAsync(string username)
        {
            if (username.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsUsernameAsync(username);
        }
        public async Task<bool> IsExistsMobileAsync(string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsMobileAsync(mobile);
        }
        public async Task<bool> IsExistsEmailAsync(string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsEmailAsync(email);
        }
        public async Task<bool> VerifyExistsUsernameAsync(int userID, string username)
        {
            if (username.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsUsernameAsync(userID, username);
        }
        public async Task<bool> VerifyExistsMobileAsync(int userID, string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsMobileAsync(userID, mobile);
        }
        public async Task<bool> VerifyExistsEmailAsync(int userID, string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsEmailAsync(userID, email);
        }
        /// <summary>
        /// 验证用户名是否已经被使用
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="modelState"></param>
        public async Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            return await _repository.VerifyExistsAsync(userInput, modelState);
        }
        /// <summary>
        /// 用户信息分页
        /// </summary>
        /// <param name="criteria">用户信息搜索条件</param>
        /// <param name="pagingInfo">分页条件</param>
        /// <returns></returns>
        public async Task<IPagedList<UserInfo>> GetPagedListAsync(UserSearchCriteria criteria, PagingInfo pagingInfo)
        {
            await GengerateGroupIDs(criteria);
            return await _repository.GetPagedListAsync(criteria, pagingInfo);
        }
        public async Task<Page<UserInfo>> GetPageAsync(UserSearchCriteria criteria, PagingInfo pagingInfo)
        {
            await GengerateGroupIDs(criteria);
            return await _repository.GetPageAsync(criteria, pagingInfo);
        }
        public async Task<bool> SaveAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            //验证用户名和手机号吗是否被占用
            if (await VerifyExistsAsync(userInput, modelState)) 
                return false;

            //生成密码
            if (!userInput.Password.IsNullOrWhiteSpace())
                userInput.Password = userInput.PasswordConfirm = UserRepository.GeneratePassword(userInput.Password);
            else
                userInput.Password = userInput.PasswordConfirm = String.Empty;

            if (userInput.RealName.IsNullOrWhiteSpace())
            {
                userInput.RealNameIsValid = false;
            }
            //如果邮箱或手机为空，则验证也置为未通过
            if (userInput.Email.IsNullOrWhiteSpace())
            {
                userInput.EmailIsValid = false;
            }
            if (userInput.Mobile.IsNullOrWhiteSpace())
            {
                userInput.MobileIsValid = false;
            }            
            //保存实体
            UserInfo user = await _repository.SaveAsync(userInput, modelState);
            if (user != null)
            {
                //清除缓存
                _cache.Invalidate(UserCacheKeyFormat.FormatWith(user.UserID));
                return true;
            }

            return false;

        }
        public async Task<bool> ChangeUsernameAsync(int userID, string newUsername, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeUsernameAsync(userID, newUsername, modelState);
            if (!result)
                modelState.AddModelError("UserID", "修改用户名失败，可能当前用户不存在或新用户名已经被使用");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeMobileAsync(int userID, string newMobile, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeMobileAsync(userID, newMobile, modelState);
            if (!result)
                modelState.AddModelError("UserID", "修改手机号失败，可能当前用户不存在或新手机号已经被使用");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeDisplayNameAsync(int userID, string newDisplayName, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeDisplayNameAsync(userID, newDisplayName);
            if (!result)
                modelState.AddModelError("UserID", "修改昵称失败");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeLogoAsync(int userID, string logoURL, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeLogoAsync(userID, logoURL);
            if (!result)
                modelState.AddModelError("UserID", "修改头像失败");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return result;
        } 
        public async Task<bool> ChangePasswordAsync(int userID, string rawPassword, ModelStateDictionary modelState)
        {
            string newPassword = UserRepository.GeneratePassword(rawPassword);
            bool result = await _repository.ChangePasswordAsync(userID, newPassword);
            if (!result)
            {
                modelState.AddModelError("UserID", "修改密码失败，可能当前用户不存在");
            }
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return result;
        }
        public async Task<bool> ResetPasswordByMobileAsync(string username, string rawPassword, ModelStateDictionary modelState)
        {
            string newPassword = UserRepository.GeneratePassword(rawPassword);
            int userID = await _repository.ResetPasswordByMobileAsync(username, newPassword, modelState);
            if (userID <= 0 || !modelState.IsValid)
            {
                return false;
            }

            //修改成功后清空数据缓存
            string cacheKey = UserCacheKeyFormat.FormatWith(userID);
            _cache.Invalidate(cacheKey);
            return true;
        }
        public async Task<bool> GetPasswordAsync(UserGetPasswordInput input, ModelStateDictionary modelState)
        {
            if (input == null||input.Username.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Username", "请输入用户名");
                return false; 
            }
            if (input.Email.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Email", "请输入安全邮箱");
                return false;
            }
            var userInfo = await _repository.GetItemByUsernameAsync(input.Username);
            if (userInfo == null)
            {
                modelState.AddModelError("Username", "该用户不存在");
                return false;
            }
            else if (userInfo.Status != UserStatus.Normal)
            {
                modelState.AddModelError("Username", "该帐号已被停用");
                return false;
            }
            else if (userInfo.Email.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Email", "该帐号尚未设置安全邮箱");
                return false;
            }
            else if (userInfo.Email!=input.Email)
            {
                modelState.AddModelError("Email", "该邮箱不是您设置的安全邮箱");
                return false;
            }
            //重置密码
            string newPassword = GenerateRandomPassword(6);
            string password = UserRepository.GeneratePassword(newPassword);
            int userID = await  _repository.ChangePasswordAsync(input.Username, password);
            if (userID <= 0)
                modelState.AddModelError("Username", "该用户不存在");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userInfo.UserID);
                _cache.Invalidate(cacheKey);
            }
            //发送邮件
            //string body = String.Format("Hi {0}!<br/><br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;您在www.ebo.so上的管理密码已经成功重置：<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;用户名：{1}<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;新密码：{2}<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;安全邮箱：{3}<br/><br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;请保管好您的新密码，<a href=\"http://www.eliu.so/manager\">马上进行登录</a>。"
            //    , userInfo.DisplayName, userInfo.Username, newPassword, userInfo.Email);

            string body = String.Format(_appSettings.GetString("GetPasswordMessage")
                , userInfo.DisplayName
                , userInfo.Username
                , newPassword
                , userInfo.Email);
            _smtpMailModule.SendMail(input.Email, "管理帐号密码重置", body);
            return true;

        }
        public async Task<bool> ChangeHeadAsync(int userID, string newHead)
        {
            return await _repository.ChangeHeadAsync(userID, newHead);
        }
        public async Task<bool> RemoveAsync(int userID, ModelStateDictionary modelState)
        {
            bool removedUser = await _repository.RemoveAsync(userID, modelState);
            if (removedUser)
            {
                //删除成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userID);
                _cache.Invalidate(cacheKey);
            }
            return removedUser;
        }
        /// <summary>
        /// 改变用户状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> ChangeStatusAsync(int userID, UserStatus status)
        {
            return await _repository.ChangeStatusAsync(userID, status);
        }
        public async Task<bool> UpdateClientAgentAsync(int userID, String clientAgent, String ip)
        {
            return await _repository.UpdateClientAgentAsync(userID, clientAgent , ip);
        }
        public async Task<bool> UpdateTokenAsync(int userID, String token)
        {
            return await _repository.UpdateTokenAsync(userID, token);
        }
        public async Task<bool> UpdateWXOpenIDAsync(int userID, String wxOpenID, ModelStateDictionary modelState)
        {
            return await _repository.UpdateWXOpenIDAsync(userID, wxOpenID, modelState);
        }
        public async Task<bool> CleanWXOpenIDAsync(int userID)
        {
            return await _repository.CleanWXOpenIDAsync(userID);
        }
        public async Task<bool> UpdateWXAOpenIDAsync(int userID, String wxaOpenID, ModelStateDictionary modelState)
        {
            return await _repository.UpdateWXAOpenIDAsync(userID, wxaOpenID, modelState);
        }
        public async Task<bool> CleanWXAOpenIDAsync(int userID)
        {
            return await _repository.CleanWXAOpenIDAsync(userID);
        }
        public async Task<bool> ClearClientAgentAsync(int userID, String clientAgent, String uuid)
        {
            return await _repository.ClearClientAgentAsync(userID, clientAgent);
        }
        public async Task<bool> SignInAsync(Func<Task<UserInfo>> getUser, Action<UserInfo> afterSignIn)
        {
            await Task.Yield();
            UserInfo user = await getUser();

            if (user != null)
            {
                //登录成功后将数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(user.UserID);
                _cache.Invalidate(cacheKey);
                _cache.Insert(cacheKey, user);

                if (afterSignIn != null)
                {
                    afterSignIn(user);
                }

                return true;
            }

            return false;
        }
        public async Task<bool> SignOutAsync()
        {
            await Task.Yield();
            return true;
        }
        public async Task<bool> GetMobileValidationCode(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            var code = await _repository.GetMobileValidationCode(getMobileValidationCodeInput, modelState);
            if (!modelState.IsValid)
            {
                // 可能原因：请求过于频繁
                return false;
            }

            string uri = "https://api.submail.cn/message/xsend.json";
            var client = new HttpClient();
            var httpContent = new FormUrlEncodedContent(new[]
	        {
                // TODO: 改为从配置文件读取
		        new KeyValuePair<string, string>("appid", "15360"),
		        new KeyValuePair<string, string>("project", "5Ijl32"),
		        new KeyValuePair<string, string>("signature", "896a792310a02131f46a11774d37aa01"),
		        new KeyValuePair<string, string>("to", getMobileValidationCodeInput.Mobile),
		        new KeyValuePair<string, string>("vars", "{\"code\":\""+ code +"\",\"time\":\""+ UserRepository.MobileValidationCodeExpirationInterval +"\"}"),
	        });
            try
            {
                var response = await client.PostAsync(uri, httpContent);
                var content = await response.Content.ReadAsStringAsync();
                // TODO: 检查短信发送结果
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> VerifyMobileValidationCode(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState, string defaultCode = null)
        {
            if (!defaultCode.IsNullOrWhiteSpace() &&
                defaultCode.Equals(verifyMobileValidationCodeInput.ValidationCode,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return await _repository.VerifyMobileValidationCode(verifyMobileValidationCodeInput, modelState);
        }
        public async Task<bool> FinishVerifyMobileValidationCode(string mobile, int typeID, ModelStateDictionary modelState)
        {
            return await _repository.FinishVerifyMobileValidationCode(mobile, typeID, modelState);
        }

        #endregion
        private  static string GenerateRandomPassword(int pwdlen)
        {
            const string pwdChars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string tmpstr = "";
            var rnd = new Random();
            for (int i = 0; i < pwdlen; i++)
            {
                int iRandNum = rnd.Next(pwdChars.Length);
                tmpstr += pwdChars[iRandNum];
            }
            return tmpstr;
        }

        private async Task GengerateGroupIDs(UserSearchCriteria criteria)
        {
            if (!criteria.GroupIDs.IsNullOrEmpty())
            {
                var newGroupIDs = new List<Guid>();
                foreach (var groupID in criteria.GroupIDs)
                {
                    var groupIDs = (await _groupService.GetListAsync(groupID)).Select(m => m.GroupID);
                    newGroupIDs.AddRange(groupIDs);
                }
                criteria.GroupIDs = newGroupIDs;
            }
        }
    }
}
