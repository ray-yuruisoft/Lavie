using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{
    public interface INotificationRepository
    {
        Task<Page<XM.NotificationUser>> GetPageAsync(NotificationSearchCriteria criteria, PagingInfo pagingInfo);
        Task<bool> SaveAsync(NotificationInput notificationInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int notificationID, ModelStateDictionary modelState);
        Task<bool> ReadAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState);
        Task<bool> DeleteAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState);
        Task<XM.NotificationUser> GetNewestAsync(int userID, int? currentNotificationID = null);
    }

    public class NotificationRepository : RepositoryBase, INotificationRepository
    {
        private readonly Expression<Func<Notification, XM.Notification>> _notificationSelector;
        private readonly Expression<Func<Notification, XM.NotificationUser>> _notificationUserSelector;

        public NotificationRepository()
        {
            _notificationSelector = m => new XM.Notification
            {
                NotificationID = m.NotificationID,
                FromUser = new UserInfoWarpper {
                    UserID = m.FromUserID.HasValue ? m.FromUser.UserID : 0,
                    Username = m.FromUserID.HasValue ? m.FromUser.Username: "",
                    DisplayName = m.FromUserID.HasValue ? m.FromUser.DisplayName : "",
                    HeadURL = m.FromUserID.HasValue ? m.FromUser.HeadURL : "",
                    LogoURL = m.FromUserID.HasValue ? m.FromUser.LogoURL : "",
                },
                ToUser = new UserInfoWarpper
                {
                    UserID = m.ToUserID.HasValue ? m.ToUser.UserID : 0,
                    Username = m.ToUserID.HasValue ? m.ToUser.Username : "",
                    DisplayName = m.ToUserID.HasValue ? m.ToUser.DisplayName : "",
                    HeadURL = m.ToUserID.HasValue ? m.ToUser.HeadURL : "",
                    LogoURL = m.ToUserID.HasValue ? m.ToUser.LogoURL : "",
                },
                Title = m.Title,
                Message = m.Message,
                CreationDate = m.CreationDate,
                URL = m.URL,
            };

            _notificationUserSelector = m => new XM.NotificationUser
            {
                NotificationID = m.NotificationID,
                FromUser = new UserInfoWarpper
                {
                    UserID = m.FromUserID.HasValue ? m.FromUser.UserID : 0,
                    Username = m.FromUserID.HasValue ? m.FromUser.Username : "",
                    DisplayName = m.FromUserID.HasValue ? m.FromUser.DisplayName : "",
                    HeadURL = m.FromUserID.HasValue ? m.FromUser.HeadURL : "",
                    LogoURL = m.FromUserID.HasValue ? m.FromUser.LogoURL : "",
                },
                ToUser = new UserInfoWarpper
                {
                    UserID = m.ToUserID.HasValue ? m.ToUser.UserID : 0,
                    Username = m.ToUserID.HasValue ? m.ToUser.Username : "",
                    DisplayName = m.ToUserID.HasValue ? m.ToUser.DisplayName : "",
                    HeadURL = m.ToUserID.HasValue ? m.ToUser.HeadURL : "",
                    LogoURL = m.ToUserID.HasValue ? m.ToUser.LogoURL : "",
                },
                Title = m.Title,
                Message = m.Message,
                CreationDate = m.CreationDate,
                URL = m.URL,
                
                ReadTime = null,
                DeleteTime = null,
            };
        }

        public async Task<Page<XM.NotificationUser>> GetPageAsync(NotificationSearchCriteria criteria, PagingInfo pagingInfo)
        {
            if (criteria.ToUserID.HasValue)
            {
                return await GetNotificationUserPageAsync(criteria, pagingInfo);
            }

            // 备注：忽略搜索条件的 IsReaded, ToUserID
            // 备注：因为查询所有 ToUserID, 所有不会标记已读未读

            IQueryable<Notification> query = DbContext.Notifications;
            criteria = criteria ?? new NotificationSearchCriteria();
            if (criteria.FromUserID.HasValue)
            {
                query = query.Where(m => m.FromUserID == criteria.FromUserID);
            }
            if (criteria.Keyword != null)
            {
                var keyword = criteria.Keyword.Trim();
                if (keyword.Length != 0)
                {
                    query = query.Where(m => m.Title.Contains(keyword));
                }
            }
            if (criteria.CreationDateBegin.HasValue)
            {
                var begin = criteria.CreationDateBegin.Value.Date;
                query = query.Where(m => m.CreationDate >= begin);
            }
            if (criteria.CreationDateEnd.HasValue)
            {
                var end = criteria.CreationDateEnd.Value.Date.AddDays(1);
                query = query.Where(m => m.CreationDate < end);
            }

            IOrderedQueryable<Notification> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query.OrderByDescending(m => m.NotificationID);
            }

            var page = await orderedQuery.Select(_notificationUserSelector).GetPageAsync(pagingInfo);
            return page;
        }

        private async Task<Page<XM.NotificationUser>> GetNotificationUserPageAsync(NotificationSearchCriteria criteria, PagingInfo pagingInfo)
        {
            if (!criteria.ToUserID.HasValue)
            {
                throw new ArgumentNullException("必须输入 ToUserID");
            }

            // 备注：查询发送给所有人的以及本人的、未删除的记录

            var query1 = from n in DbContext.Notifications
                         where !n.ToUserID.HasValue || n.ToUserID == criteria.ToUserID.Value
                         select n;

            query1 = query1.AsNoTracking();

            if (criteria.FromUserID.HasValue)
            {
                query1 = query1.Where(m => m.FromUserID == criteria.FromUserID);
            }
            if (criteria.Keyword != null)
            {
                var keyword = criteria.Keyword.Trim();
                if (keyword.Length != 0)
                {
                    query1 = query1.Where(m => m.Title.Contains(keyword));
                }
            }
            if (criteria.CreationDateBegin.HasValue)
            {
                var begin = criteria.CreationDateBegin.Value.Date;
                query1 = query1.Where(m => m.CreationDate >= begin);
            }
            if (criteria.CreationDateEnd.HasValue)
            {
                var end = criteria.CreationDateEnd.Value.Date.AddDays(1);
                query1 = query1.Where(m => m.CreationDate < end);
            }

            IQueryable<XM.NotificationUser> query2;
            query2 = from m in query1
                     join pu in DbContext.NotificationUsers.Where(n=>n.UserID == criteria.ToUserID.Value) on m equals pu.Notification into purd
                     from x in purd.DefaultIfEmpty()
                     where x == null || !x.DeleteTime.HasValue
                     select new XM.NotificationUser
                     {
                         NotificationID = m.NotificationID,
                         FromUser = new UserInfoWarpper
                         {
                             UserID = m.FromUserID.HasValue ? m.FromUser.UserID : 0,
                             Username = m.FromUserID.HasValue ? m.FromUser.Username : "",
                             DisplayName = m.FromUserID.HasValue ? m.FromUser.DisplayName : "",
                             HeadURL = m.FromUserID.HasValue ? m.FromUser.HeadURL : "",
                             LogoURL = m.FromUserID.HasValue ? m.FromUser.LogoURL : "",
                         },
                         ToUser = new UserInfoWarpper
                         {
                             UserID = m.ToUserID.HasValue ? m.ToUser.UserID : 0,
                             Username = m.ToUserID.HasValue ? m.ToUser.Username : "",
                             DisplayName = m.ToUserID.HasValue ? m.ToUser.DisplayName : "",
                             HeadURL = m.ToUserID.HasValue ? m.ToUser.HeadURL : "",
                             LogoURL = m.ToUserID.HasValue ? m.ToUser.LogoURL : "",
                         },
                         Title = m.Title,
                         Message = m.Message,
                         URL = m.URL,
                         CreationDate = m.CreationDate,

                         ReadTime = x != null ? x.ReadTime : null,
                         DeleteTime = x != null ? x.DeleteTime : null,
                     };

            if (criteria.IsReaded.HasValue)
            {
                if (criteria.IsReaded.Value)
                {
                    // 备注，读取已读，也可通过用户的 NotificationsToUser 取
                    query2 = query2.Where(m => m.ReadTime.HasValue);
                }
                else
                {
                    query2 = query2.Where(m => !m.ReadTime.HasValue);
                }
            }

            IOrderedQueryable<XM.NotificationUser> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query2.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query2.OrderByDescending(m => m.NotificationID);
            }

            var page = await orderedQuery.GetPageAsync(pagingInfo);
            return page;
        }

        public async Task<bool> SaveAsync(NotificationInput notificationInput, ModelStateDictionary modelState)
        {
            User fromUser = null;
            User toUser = null;
            if (notificationInput.FromUserID.HasValue)
            {
                fromUser = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == notificationInput.FromUserID);
                if (fromUser == null)
                {
                    modelState.AddModelError("FromUserID", "无法获取通知发布者");
                    return false;
                }
            }
            if (notificationInput.ToUserID.HasValue)
            {
                toUser = await DbContext.Users.FirstOrDefaultAsync(m => m.UserID == notificationInput.ToUserID);
                if (toUser == null)
                {
                    modelState.AddModelError("FromUserID", "无法获取通知接收者");
                    return false;
                }
            }
            Notification itemToSave;
            if (notificationInput.NotificationID.HasValue)
            {
                itemToSave = await DbContext.Notifications.FirstOrDefaultAsync(m => m.NotificationID == notificationInput.NotificationID);
                if (itemToSave == null)
                {
                    modelState.AddModelError("FromUserID", "无法获取编辑的记录");
                    return false;
                }
            }
            else
            {
                itemToSave = new Notification
                {
                    FromUser = fromUser,
                    ToUser = toUser,
                    CreationDate = DateTime.Now,
                    URL = notificationInput.URL,
                };

                DbContext.Notifications.Add(itemToSave);
            }

            itemToSave.Title = notificationInput.Title;
            itemToSave.Message = notificationInput.Message;

            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAsync(int notificationID, ModelStateDictionary modelState)
        {
            // 需删除 NotificationUser 的记录

            var sql = "DELETE [NotificationUser] WHERE NotificationID = @NotificationID; DELETE [Notification] WHERE NotificationID = @NotificationID;";
            await DbContext.Database.ExecuteSqlCommandAsync(sql
                , new SqlParameter("NotificationID", notificationID)
                );

            return true;
        }

        public async Task<bool> ReadAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState)
        {
            var notifications = await DbContext.Notifications.AsNoTracking().Where(m => notificationIDs.Contains(m.NotificationID)).
                Select(m => new {
                    m.NotificationID,
                    m.ToUserID,
                }).
                ToArrayAsync();
            if (notifications != null && notifications.Any(m => m.ToUserID.HasValue && m.ToUserID != userID))
            {
                modelState.AddModelError("Error", "尝试读取不存在或非发给本人的通知");
                return false;
            }

            // TODO: 批量查询出 NotificationUsers，或以其他方式实现
            foreach (var notification in notifications)
            {
                var notificationUser = await DbContext.NotificationUsers.Where(m => m.NotificationID == notification.NotificationID && m.UserID == userID).FirstOrDefaultAsync();
                if (notificationUser == null)
                {
                    var nu = new NotificationUser
                    {
                        UserID = userID,
                        NotificationID = notification.NotificationID,
                        ReadTime = DateTime.Now,
                    };
                    DbContext.NotificationUsers.Add(nu);
                }
                else if (!notificationUser.ReadTime.HasValue)
                {
                    notificationUser.ReadTime = DateTime.Now;
                }

            }
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int userID, int[] notificationIDs, ModelStateDictionary modelState)
        {
            var notifications = await DbContext.Notifications.AsNoTracking().Where(m => notificationIDs.Contains(m.NotificationID)).
                Select(m => new {
                    m.NotificationID,
                    m.ToUserID,
                }).
                ToArrayAsync();
            if (notifications != null && notifications.Any(m => m.ToUserID.HasValue && m.ToUserID != userID))
            {
                modelState.AddModelError("Error", "尝试读取不存在或非发给本人的通知");
                return false;
            }

            // TODO: 批量查询出 NotificationUsers，或以其他方式实现
            foreach (var notification in notifications)
            {
                var notificationUser = await DbContext.NotificationUsers.Where(m => m.NotificationID == notification.NotificationID && m.UserID == userID).FirstOrDefaultAsync();
                if (notificationUser == null)
                {
                    var nu = new NotificationUser
                    {
                        UserID = userID,
                        NotificationID = notification.NotificationID,
                        DeleteTime = DateTime.Now,
                    };
                    DbContext.NotificationUsers.Add(nu);
                }
                else if (!notificationUser.DeleteTime.HasValue)
                {
                    notificationUser.DeleteTime = DateTime.Now;
                }
            }

            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<XM.NotificationUser> GetNewestAsync(int userID, int? currentNotificationID = null)
        {
            var query1 = from n in DbContext.Notifications.AsNoTracking()
                         where !n.ToUserID.HasValue || n.ToUserID == userID
                         select n;
            if (currentNotificationID.HasValue)
            {
                query1 = query1.Where(n => n.NotificationID > currentNotificationID.Value);
            }

            IQueryable<XM.NotificationUser> query2;
            query2 = from m in query1
                     join pu in DbContext.NotificationUsers.Where(n => n.UserID == userID) on m equals pu.Notification into purd
                     from x in purd.DefaultIfEmpty()
                     where x == null || (!x.DeleteTime.HasValue && !x.ReadTime.HasValue)
                     orderby m.NotificationID descending
                     select new XM.NotificationUser
                     {
                         NotificationID = m.NotificationID,
                         FromUser = new UserInfoWarpper
                         {
                             UserID = m.FromUserID.HasValue ? m.FromUser.UserID : 0,
                             Username = m.FromUserID.HasValue ? m.FromUser.Username : "",
                             DisplayName = m.FromUserID.HasValue ? m.FromUser.DisplayName : "",
                             HeadURL = m.FromUserID.HasValue ? m.FromUser.HeadURL : "",
                             LogoURL = m.FromUserID.HasValue ? m.FromUser.LogoURL : "",
                         },
                         ToUser = new UserInfoWarpper
                         {
                             UserID = m.ToUserID.HasValue ? m.ToUser.UserID : 0,
                             Username = m.ToUserID.HasValue ? m.ToUser.Username : "",
                             DisplayName = m.ToUserID.HasValue ? m.ToUser.DisplayName : "",
                             HeadURL = m.ToUserID.HasValue ? m.ToUser.HeadURL : "",
                             LogoURL = m.ToUserID.HasValue ? m.ToUser.LogoURL : "",
                         },
                         Title = m.Title,
                         Message = m.Message,
                         URL = m.URL,
                         CreationDate = m.CreationDate,

                         ReadTime = x != null ? x.ReadTime : null,
                         DeleteTime = x != null ? x.DeleteTime : null,
                     };

            return await query2.FirstOrDefaultAsync();
        }

    }
}
