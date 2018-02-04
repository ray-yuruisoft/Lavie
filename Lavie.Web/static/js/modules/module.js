webpackJsonp([13],{"/rEA":function(t,e,n){"use strict";(function(t){function i(t,e,n){this.name="ApiError",this.message=t||"Default Message",this.errorType=e||g.Default,this.innerError=n,this.stack=(new Error).stack}var r=n("//Fk"),a=n.n(r),o=n("mvHQ"),l=n.n(o),u=n("OvRC"),s=n.n(u),d=n("mtWM"),c=n.n(d),f=n("mw3O"),p=n.n(f),m=n("bzuE"),g={Default:"default",Sysetem:"sysetem"};(i.prototype=s()(Error.prototype)).constructor=i;var h=c.a.create({baseURL:m.a,timeout:2e4,responseType:"json",withCredentials:!0});h.interceptors.request.use(function(t){return"post"===t.method||"put"===t.method||"patch"===t.method?(t.headers={"Content-Type":"application/json charset=utf-8"},t.data=l()(t.data)):"delete"!==t.method&&"get"!==t.method&&"head"!==t.method||(t.paramsSerializer=function(t){return p.a.stringify(t,{arrayFormat:"indices"})}),t},function(t){return t}),h.interceptors.response.use(function(e){if(-1===e.headers["content-type"].indexOf("json"))return e;var n=void 0;if("arraybuffer"===e.request.responseType&&"[object ArrayBuffer]"===e.data.toString()){var r=t.from(e.data).toString("utf8");console.log(r),n=JSON.parse(r)}else n=e.data;if(n&&n.url)top.location=n.url;else if(n&&200!==n.code)return console.log(n),a.a.reject(new i(n.message));return e},function(t){return a.a.reject(new i(t.message,g.Sysetem,t))}),e.a={install:function(t){arguments.length>1&&void 0!==arguments[1]&&arguments[1];t.httpClient=h,t.prototype.$httpClient=h}}}).call(e,n("EuP9").Buffer)},F45B:function(t,e,n){"use strict";var i=n("KQ8b"),r=n("fip7"),a=function(t){n("KrQV")},o=n("VU/8")(i.a,r.a,!1,a,"data-v-161f1392",null);e.a=o.exports},KQ8b:function(t,e,n){"use strict";var i=n("VsUZ");e.a={data:function(){return{isLoading:!1,item:{modules:[],backgroundServices:[],dataProviders:[],connectionStrings:[]}}},mounted:function(){this.getItem()},computed:{emptyText:function(){return this.isLoading?"加载中...":"暂无数据"}},methods:{getItem:function(){var t=this;this.isLoading=!0,i.a.getModuleConfig().then(function(e){t.isLoading=!1,t.item=e.data.item},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})},showErrorMessage:function(t){this.$message({message:t,type:"error"})}}}},KrQV:function(t,e){},TPK5:function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var i=n("tvR6"),r=(n.n(i),n("qBF2")),a=n.n(r),o=n("7+uW"),l=n("uN2V"),u=(n.n(l),n("F45B")),s=n("/rEA");o.default.config.productionTip=!1,o.default.use(a.a),o.default.use(s.a),new o.default({el:"#app",render:function(t){return t(u.a)}})},VsUZ:function(t,e,n){"use strict";var i=n("7+uW");e.a={login:function(t){return i.default.httpClient.post("/admin/login",t)},logout:function(){return i.default.httpClient.post("/admin/logout")},getProfile:function(){return i.default.httpClient.get("/admin/getProfile")},changeProfile:function(t){return i.default.httpClient.post("/admin/changeProfile",t)},changePassword:function(t){return i.default.httpClient.post("/admin/changePassword",t)},getMenus:function(){return i.default.httpClient.get("/admin/getMenus")},getServerInfo:function(){return i.default.httpClient.get("/admin/getServerinfo")},getSiteConfig:function(){return i.default.httpClient.get("/admin/getSiteconfig")},editSiteConfig:function(t){return i.default.httpClient.post("/admin/editSiteconfig",t)},getBulletin:function(){return i.default.httpClient.get("/admin/getBulletin")},editBulletin:function(t){return i.default.httpClient.post("/admin/editBulletin",t)},getPermissions:function(){return i.default.httpClient.get("/admin/getPermissions")},getModuleConfig:function(){return i.default.httpClient.get("/admin/getModuleConfig")},extractModulePermissions:function(){return i.default.httpClient.get("/admin/extractModulePermissions")},clearModulePermissions:function(){return i.default.httpClient.get("/admin/clearModulePermissions")},getRoles:function(){return i.default.httpClient.get("/admin/getRoles")},addRole:function(t){return i.default.httpClient.post("/admin/addRole",t)},editRole:function(t){return i.default.httpClient.post("/admin/editRole",t)},removeRole:function(t){return i.default.httpClient.post("/admin/removeRole",t)},moveRole:function(t){return i.default.httpClient.post("/admin/moveRole",t)},saveRoleName:function(t){return i.default.httpClient.post("/admin/saveRoleName",t)},getGroupTree:function(){return i.default.httpClient.get("/admin/getGroupTree")},addGroup:function(t){return i.default.httpClient.post("/admin/addGroup",t)},editGroup:function(t){return i.default.httpClient.post("/admin/editGroup",t)},removeGroup:function(t){return i.default.httpClient.post("/admin/removeGroup",t)},moveGroup:function(t){return i.default.httpClient.post("/admin/moveGroup",t)},getUsers:function(t){return i.default.httpClient.post("/admin/getUsers",t)},addUser:function(t){return i.default.httpClient.post("/admin/addUser",t)},editUser:function(t){return i.default.httpClient.post("/admin/editUser",t)},removeUser:function(t){return i.default.httpClient.post("/admin/removeUser",t)},getUserStatus:function(){return i.default.httpClient.get("/admin/getUserStatus")},getNotificationsForManager:function(t){return i.default.httpClient.post("/admin/getNotificationsForManager",t)},addNotification:function(t){return i.default.httpClient.post("/admin/addNotification",t)},editNotification:function(t){return i.default.httpClient.post("/admin/editNotification",t)},removeNotification:function(t){return i.default.httpClient.post("/admin/removeNotification",t)},getNotifications:function(t){return i.default.httpClient.post("/admin/getNotifications",t)},readNotifications:function(t){return i.default.httpClient.post("/admin/readNotifications",t)},deleteNotifications:function(t){return i.default.httpClient.post("/admin/deleteNotifications",t)},getNewestNotification:function(t){return i.default.httpClient.post("/admin/getNewestNotification",t)},getGroups:function(){return i.default.httpClient.get("/admin/getGroups")},getRoleBases:function(){return i.default.httpClient.get("/admin/getRoleBases")},getPermissionTree:function(){return i.default.httpClient.get("/admin/getPermissionTree")},directlyCall:function(t){return i.default.httpClient.get(t)},download:function(t,e){return i.default.httpClient.post(t,e,{responseType:"arraybuffer"})}}},bzuE:function(t,e,n){"use strict";n.d(e,"a",function(){return i}),n.d(e,"b",function(){return r}),n.d(e,"c",function(){return a});var i="/manager/api",r="",a=""},fip7:function(t,e,n){"use strict";var i={render:function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("el-container",{directives:[{name:"loading",rawName:"v-loading.fullscreen.lock",value:t.isLoading,expression:"isLoading",modifiers:{fullscreen:!0,lock:!0}}]},[n("el-header",{staticClass:"header"},[n("el-breadcrumb",{staticClass:"breadcrumb",attrs:{"separator-class":"el-icon-arrow-right"}},[n("el-breadcrumb-item",[t._v("模块管理")]),t._v(" "),n("el-breadcrumb-item",[t._v("模块信息")])],1)],1),t._v(" "),n("el-main",{staticClass:"main"},[n("el-row",[n("el-table",{staticStyle:{width:"100%"},attrs:{data:t.item.modules,size:"small","empty-text":t.emptyText}},[n("el-table-column",{attrs:{prop:"Name",label:"模块名称",width:"160"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Type",label:"类型"}}),t._v(" "),n("el-table-column",{attrs:{prop:"DataProvider",label:"数据代理名称",width:"140"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Enabled",label:"启用",width:"60"},scopedSlots:t._u([{key:"default",fn:function(t){return[n("i",{directives:[{name:"show",rawName:"v-show",value:t.row.Enabled,expression:"scope.row.Enabled"}],staticClass:"el-icon-check"})]}}])})],1)],1),t._v(" "),n("el-row",[n("el-table",{staticStyle:{width:"100%"},attrs:{data:t.item.backgroundServices,size:"small","empty-text":t.emptyText}},[n("el-table-column",{attrs:{prop:"Name",label:"后台服务器名称",width:"160"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Type",label:"类型"}}),t._v(" "),n("el-table-column",{attrs:{prop:"ExecutorType",label:"执行器类型"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Interval",label:"执行间隔(毫秒)",width:"140"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Enabled",label:"启用",width:"60"},scopedSlots:t._u([{key:"default",fn:function(t){return[n("i",{directives:[{name:"show",rawName:"v-show",value:t.row.Enabled,expression:"scope.row.Enabled"}],staticClass:"el-icon-check"})]}}])})],1)],1),t._v(" "),n("el-row",[n("el-table",{staticStyle:{width:"100%"},attrs:{data:t.item.providers,size:"small","empty-text":t.emptyText}},[n("el-table-column",{attrs:{prop:"Name",label:"数据代理名称",width:"160"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Type",label:"类型"}}),t._v(" "),n("el-table-column",{attrs:{prop:"ConnectionString",label:"数据库链接配置"}}),t._v(" "),n("el-table-column",{attrs:{prop:"Enabled",label:"启用",width:"60"},scopedSlots:t._u([{key:"default",fn:function(t){return[n("i",{directives:[{name:"show",rawName:"v-show",value:t.row.Enabled,expression:"scope.row.Enabled"}],staticClass:"el-icon-check"})]}}])})],1)],1),t._v(" "),n("el-row",[n("el-table",{staticStyle:{width:"100%"},attrs:{data:t.item.connectionStrings,size:"small","empty-text":t.emptyText}},[n("el-table-column",{attrs:{prop:"Name",label:"数据库链接名称",width:"160"}}),t._v(" "),n("el-table-column",{attrs:{prop:"ConnectionString",label:"链接字符串"}}),t._v(" "),n("el-table-column",{attrs:{prop:"ProviderName",label:"数据代理"}})],1)],1)],1)],1)},staticRenderFns:[]};e.a=i},tvR6:function(t,e){},uN2V:function(t,e){}},["TPK5"]);
//# sourceMappingURL=module.js.map