using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lavie.Models;
using Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Repositories;
using Lavie.Modules.Admin.Services;

namespace Lavie.Hubs
{
    // 错误码：
    // 200 连接通知成功
    // 201 新消息(可带url参数)
    // 202 清除新消息标记
    // 400 连接通知失败等错误

    public class ApiResultNotification : ApiResult
    {
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }
    public class Client
    {
        public string ConnectionID { get; set; }
        public int UserID { get; set; }
    }

    public partial class NotificationHub : Hub
    {
        public static ConcurrentDictionary<string, Client> ClientCache = new ConcurrentDictionary<string, Client>();

        public NotificationHub()
        {
        }
        public static async Task SendMessageByUserID(int userID, ApiResultNotification message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var client = ClientCache.Values.FirstOrDefault(m => m.UserID == userID);
            if (client == null) return;

            IClientProxy proxy = context.Clients.Client(client.ConnectionID);
            await proxy.Invoke("receviedMessage", message);
        }
        public static async Task SendMessage(string connectionID, ApiResultNotification message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            IClientProxy proxy = context.Clients.Client(connectionID);
            await proxy.Invoke("receviedMessage", message);
        }
        public static async Task BroadcastMessage(ApiResultNotification message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            IClientProxy proxy = context.Clients.All;
            //context.Clients.All.receviedMessage(typeid, message);
            await proxy.Invoke("receviedMessage", message);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Client oldClient;
            if (!ClientCache.TryRemove(Context.ConnectionId, out oldClient))
            {
                Debug.WriteLine("客户端移除失败(ConnectionId:{0})", Context.ConnectionId);
            }

            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

    }

    public partial class NotificationHub
    {
        public async Task Join(string token)
        {
            // 暂未使用 Token 值
            if (token.IsNullOrWhiteSpace())
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "连接通知失败, Token为空" });
                return;
            }
            int userID;
            if (!HttpContext.Current.User.Identity.IsAuthenticated || !Int32.TryParse(HttpContext.Current.User.Identity.Name, out userID))
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "连接通知失败, 用户未登录" });
                return;
            };

            var userService = DependencyResolver.Current.GetService<IUserService>();
            if (userService == null)
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "连接通知失败, 内部错误：1001" });
                return;
            };

            var user = await userService.GetItemByUserIDAsync(userID, UserStatus.Normal);
            if (user == null)
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "连接通知失败, 用户已不存在或非正常状态" });
                return;
            };

            Client oldClient = null;
            oldClient = ClientCache.Values.FirstOrDefault(m => m.UserID == userID);
            if (oldClient != null)
            {
                if (oldClient.ConnectionID == Context.ConnectionId)
                {
                    // 如果 ConnectionID 相同，直接返回
                    // await SendMessage(Context.ConnectionId, new ApiResultNotification { Code = 200, Title = "消息", Message = "重新连接通知成功" });
                    await SendNewNotification(userID);
                    return;
                }

                Client clientToRemove;
                ClientCache.TryRemove(oldClient.ConnectionID, out clientToRemove);
            }

            var newClient = new Client
            {
                ConnectionID = Context.ConnectionId,
                UserID = userID,
            };
            if (!ClientCache.TryAdd(Context.ConnectionId, newClient))
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "连接通知失败, 内部错误：1002" });
                return;
            }

            // await SendMessage(new ApiResultNotification { Code = 200, Message = "连接通知成功" });
            await SendNewNotification(userID);
        }

        public async Task SendMessage(ApiResultNotification message) {
            await SendMessage(Context.ConnectionId, message);
        }

        private async Task SendNewNotification(int userID)
        {
            var notificationService = DependencyResolver.Current.GetService<INotificationService>();
            if (notificationService == null)
            {
                await SendMessage(new ApiResultNotification { Code = 400, Message = "获取最新通知失败, 内部错误：1003" });
                return;
            };

            var newest = await notificationService.GetNewestAsync(userID);
            if (newest != null)
            {
                await SendMessageByUserID(userID, new ApiResultNotification
                {
                    Code = 201,
                    Title = newest.Title,
                    Message = newest.Message,
                });
            }
        }
    }
}
