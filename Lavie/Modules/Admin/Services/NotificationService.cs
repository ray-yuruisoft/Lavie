using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Hubs;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Repositories;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Services
{
    public interface INotificationService
    {
        Task<Page<XM.NotificationUser>> GetPageAsync(NotificationSearchCriteria criteria, PagingInfo pagingInfo);
        Task<bool> SaveAsync(NotificationInput notificationInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int notificationID, ModelStateDictionary modelState);
        Task<bool> ReadAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState);
        Task<bool> DeleteAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState);
        Task<XM.NotificationUser> GetNewestAsync(int userID, int? currentNotificationID = null);

    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Page<XM.NotificationUser>> GetPageAsync(NotificationSearchCriteria criteria, PagingInfo pagingInfo)
        {
            return await _notificationRepository.GetPageAsync(criteria, pagingInfo);
        }

        public async Task<bool> SaveAsync(NotificationInput notificationInput, ModelStateDictionary modelState)
        {
            var result = await _notificationRepository.SaveAsync(notificationInput, modelState);
            if (result && !notificationInput.NotificationID.HasValue)
            {
                var apiResultNotification = new ApiResultNotification
                {
                    Code = 201,
                    Title = notificationInput.Title,
                    Message = notificationInput.Message,
                };
                if (notificationInput.ToUserID.HasValue)
                {
                    await NotificationHub.SendMessageByUserID(notificationInput.ToUserID.Value, apiResultNotification);
                }
                else
                {
                    await NotificationHub.BroadcastMessage(apiResultNotification);
                }
            }
            return result;
        }

        public async Task<bool> RemoveAsync(int notificationID, ModelStateDictionary modelState)
        {
            return await _notificationRepository.RemoveAsync(notificationID, modelState);
        }

        public async Task<bool> ReadAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState)
        {
            return await _notificationRepository.ReadAsync(userID, notificationIDs, modelState);
        }

        public async Task<bool> DeleteAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState)
        {
            return await _notificationRepository.DeleteAsync(userID, notificationIDs, modelState);
        }

        public async Task<XM.NotificationUser> GetNewestAsync(int userID, int? currentNotificationID = null)
        {
            return await _notificationRepository.GetNewestAsync(userID, currentNotificationID);
        }

    }
}
