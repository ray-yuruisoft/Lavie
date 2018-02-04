webpackJsonp([9],{"/rEA":function(e,t,n){"use strict";(function(e){function r(e,t,n){this.name="ApiError",this.message=e||"Default Message",this.errorType=t||p.Default,this.innerError=n,this.stack=(new Error).stack}var o=n("//Fk"),a=n.n(o),i=n("mvHQ"),s=n.n(i),l=n("OvRC"),u=n.n(l),d=n("mtWM"),f=n.n(d),c=n("mw3O"),m=n.n(c),g=n("bzuE"),p={Default:"default",Sysetem:"sysetem"};(r.prototype=u()(Error.prototype)).constructor=r;var h=f.a.create({baseURL:g.a,timeout:2e4,responseType:"json",withCredentials:!0});h.interceptors.request.use(function(e){return"post"===e.method||"put"===e.method||"patch"===e.method?(e.headers={"Content-Type":"application/json charset=utf-8"},e.data=s()(e.data)):"delete"!==e.method&&"get"!==e.method&&"head"!==e.method||(e.paramsSerializer=function(e){return m.a.stringify(e,{arrayFormat:"indices"})}),e},function(e){return e}),h.interceptors.response.use(function(t){if(-1===t.headers["content-type"].indexOf("json"))return t;var n=void 0;if("arraybuffer"===t.request.responseType&&"[object ArrayBuffer]"===t.data.toString()){var o=e.from(t.data).toString("utf8");console.log(o),n=JSON.parse(o)}else n=t.data;if(n&&n.url)top.location=n.url;else if(n&&200!==n.code)return console.log(n),a.a.reject(new r(n.message));return t},function(e){return a.a.reject(new r(e.message,p.Sysetem,e))}),t.a={install:function(e){arguments.length>1&&void 0!==arguments[1]&&arguments[1];e.httpClient=h,e.prototype.$httpClient=h}}}).call(t,n("EuP9").Buffer)},I3pT:function(e,t,n){"use strict";var r={render:function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("el-container",{directives:[{name:"loading",rawName:"v-loading.fullscreen.lock",value:e.isLoading,expression:"isLoading",modifiers:{fullscreen:!0,lock:!0}}]},[n("el-header",{staticClass:"header"},[n("el-breadcrumb",{staticClass:"breadcrumb",attrs:{"separator-class":"el-icon-arrow-right"}},[n("el-breadcrumb-item",[e._v("我的资料")])],1)],1),e._v(" "),n("el-main",{staticClass:"main"},[n("el-tabs",{attrs:{type:"card"},model:{value:e.activeTabName,callback:function(t){e.activeTabName=t},expression:"activeTabName"}},[n("el-tab-pane",{attrs:{label:"修改资料",name:"first"}},[n("el-form",{ref:"changeProfileForm",attrs:{model:e.changeProfileForm,rules:e.changeProfileFormRules,"label-position":"right","label-width":"120px",size:"mini"}},[n("el-form-item",{attrs:{label:"昵称",prop:"displayName"}},[n("el-input",{ref:"displayName",attrs:{"auto-complete":"off",placeholder:"请输入昵称"},model:{value:e.changeProfileForm.displayName,callback:function(t){e.$set(e.changeProfileForm,"displayName","string"==typeof t?t.trim():t)},expression:"changeProfileForm.displayName"}})],1),e._v(" "),n("el-form-item",{attrs:{label:"头像",prop:"headURL"}},[n("el-input",{ref:"headURL",attrs:{"auto-complete":"off",placeholder:"请输入头像 URL"},model:{value:e.changeProfileForm.headURL,callback:function(t){e.$set(e.changeProfileForm,"headURL","string"==typeof t?t.trim():t)},expression:"changeProfileForm.headURL"}},[n("el-button",{attrs:{slot:"append",icon:"el-icon-search"},on:{click:e.handleChangeHeadURLBrowser},slot:"append"})],1)],1),e._v(" "),n("el-form-item",{attrs:{label:"Logo",prop:"logoURL"}},[n("el-input",{ref:"logoURL",attrs:{"auto-complete":"off",placeholder:"请输入Logo URL"},model:{value:e.changeProfileForm.logoURL,callback:function(t){e.$set(e.changeProfileForm,"logoURL","string"==typeof t?t.trim():t)},expression:"changeProfileForm.logoURL"}},[n("el-button",{attrs:{slot:"append",icon:"el-icon-search"},on:{click:e.handleChangeLogoURLBrowser},slot:"append"})],1)],1),e._v(" "),n("el-form-item",[n("el-button",{attrs:{type:"primary"},on:{click:e.handleChangeProfile}},[e._v("修改资料")])],1)],1)],1),e._v(" "),n("el-tab-pane",{attrs:{label:"修改密码",name:"second"}},[n("el-form",{ref:"changePasswordForm",attrs:{model:e.changePasswordForm,rules:e.changePasswordFormRules,"label-position":"right","label-width":"120px",size:"mini"}},[n("el-form-item",{attrs:{label:"当前密码",prop:"currentPassword"}},[n("el-input",{ref:"currentPassword",attrs:{type:"password","auto-complete":"off",placeholder:"请输入当前密码"},model:{value:e.changePasswordForm.currentPassword,callback:function(t){e.$set(e.changePasswordForm,"currentPassword","string"==typeof t?t.trim():t)},expression:"changePasswordForm.currentPassword"}})],1),e._v(" "),n("el-form-item",{attrs:{label:"新密码",prop:"newPassword"}},[n("el-input",{ref:"newPassword",attrs:{type:"password","auto-complete":"off",placeholder:"请输入新密码"},model:{value:e.changePasswordForm.newPassword,callback:function(t){e.$set(e.changePasswordForm,"newPassword","string"==typeof t?t.trim():t)},expression:"changePasswordForm.newPassword"}})],1),e._v(" "),n("el-form-item",{attrs:{label:"确认新密码",prop:"newPasswordConfirm"}},[n("el-input",{ref:"newPasswordConfirm",attrs:{type:"password","auto-complete":"off",placeholder:"请确认新密码"},model:{value:e.changePasswordForm.newPasswordConfirm,callback:function(t){e.$set(e.changePasswordForm,"newPasswordConfirm","string"==typeof t?t.trim():t)},expression:"changePasswordForm.newPasswordConfirm"}})],1),e._v(" "),n("el-form-item",[n("el-button",{attrs:{type:"primary"},on:{click:e.handleChangePassword}},[e._v("修改密码")])],1)],1)],1)],1)],1)],1)},staticRenderFns:[]};t.a=r},IV7n:function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0});var r=n("tvR6"),o=(n.n(r),n("qBF2")),a=n.n(o),i=n("7+uW"),s=n("uN2V"),l=(n.n(s),n("tqSN")),u=n("/rEA");i.default.config.productionTip=!1,i.default.use(a.a),i.default.use(u.a),new i.default({el:"#app",render:function(e){return e(l.a)}})},VsUZ:function(e,t,n){"use strict";var r=n("7+uW");t.a={login:function(e){return r.default.httpClient.post("/admin/login",e)},logout:function(){return r.default.httpClient.post("/admin/logout")},getProfile:function(){return r.default.httpClient.get("/admin/getProfile")},changeProfile:function(e){return r.default.httpClient.post("/admin/changeProfile",e)},changePassword:function(e){return r.default.httpClient.post("/admin/changePassword",e)},getMenus:function(){return r.default.httpClient.get("/admin/getMenus")},getServerInfo:function(){return r.default.httpClient.get("/admin/getServerinfo")},getSiteConfig:function(){return r.default.httpClient.get("/admin/getSiteconfig")},editSiteConfig:function(e){return r.default.httpClient.post("/admin/editSiteconfig",e)},getBulletin:function(){return r.default.httpClient.get("/admin/getBulletin")},editBulletin:function(e){return r.default.httpClient.post("/admin/editBulletin",e)},getPermissions:function(){return r.default.httpClient.get("/admin/getPermissions")},getModuleConfig:function(){return r.default.httpClient.get("/admin/getModuleConfig")},extractModulePermissions:function(){return r.default.httpClient.get("/admin/extractModulePermissions")},clearModulePermissions:function(){return r.default.httpClient.get("/admin/clearModulePermissions")},getRoles:function(){return r.default.httpClient.get("/admin/getRoles")},addRole:function(e){return r.default.httpClient.post("/admin/addRole",e)},editRole:function(e){return r.default.httpClient.post("/admin/editRole",e)},removeRole:function(e){return r.default.httpClient.post("/admin/removeRole",e)},moveRole:function(e){return r.default.httpClient.post("/admin/moveRole",e)},saveRoleName:function(e){return r.default.httpClient.post("/admin/saveRoleName",e)},getGroupTree:function(){return r.default.httpClient.get("/admin/getGroupTree")},addGroup:function(e){return r.default.httpClient.post("/admin/addGroup",e)},editGroup:function(e){return r.default.httpClient.post("/admin/editGroup",e)},removeGroup:function(e){return r.default.httpClient.post("/admin/removeGroup",e)},moveGroup:function(e){return r.default.httpClient.post("/admin/moveGroup",e)},getUsers:function(e){return r.default.httpClient.post("/admin/getUsers",e)},addUser:function(e){return r.default.httpClient.post("/admin/addUser",e)},editUser:function(e){return r.default.httpClient.post("/admin/editUser",e)},removeUser:function(e){return r.default.httpClient.post("/admin/removeUser",e)},getUserStatus:function(){return r.default.httpClient.get("/admin/getUserStatus")},getNotificationsForManager:function(e){return r.default.httpClient.post("/admin/getNotificationsForManager",e)},addNotification:function(e){return r.default.httpClient.post("/admin/addNotification",e)},editNotification:function(e){return r.default.httpClient.post("/admin/editNotification",e)},removeNotification:function(e){return r.default.httpClient.post("/admin/removeNotification",e)},getNotifications:function(e){return r.default.httpClient.post("/admin/getNotifications",e)},readNotifications:function(e){return r.default.httpClient.post("/admin/readNotifications",e)},deleteNotifications:function(e){return r.default.httpClient.post("/admin/deleteNotifications",e)},getNewestNotification:function(e){return r.default.httpClient.post("/admin/getNewestNotification",e)},getGroups:function(){return r.default.httpClient.get("/admin/getGroups")},getRoleBases:function(){return r.default.httpClient.get("/admin/getRoleBases")},getPermissionTree:function(){return r.default.httpClient.get("/admin/getPermissionTree")},directlyCall:function(e){return r.default.httpClient.get(e)},download:function(e,t){return r.default.httpClient.post(e,t,{responseType:"arraybuffer"})}}},bzuE:function(e,t,n){"use strict";n.d(t,"a",function(){return r}),n.d(t,"b",function(){return o}),n.d(t,"c",function(){return a});var r="/manager/api",o="",a=""},tqSN:function(e,t,n){"use strict";var r=n("xuOk"),o=n("I3pT"),a=function(e){n("zkHw")},i=n("VU/8")(r.a,o.a,!1,a,null,null);t.a=i.exports},tvR6:function(e,t){},uN2V:function(e,t){},xuOk:function(e,t,n){"use strict";var r=n("VsUZ");t.a={data:function(){return{isLoading:!1,activeTabName:"first",changeProfileForm:{displayName:null,headURL:null,logoURL:null},changeProfileFormRules:{displayName:[{max:20,message:"最多支持20个字符",trigger:"blur"},{pattern:/^[a-zA-Z\u4E00-\u9FA5\uF900-\uFA2D][a-zA-Z0-9-_\u4E00-\u9FA5\uF900-\uFA2D]*$/,message:"昵称包含非法字符",trigger:"blur"}],head:[{max:200,message:"最多支持200个字符",trigger:"blur"}],logo:[{max:200,message:"最多支持200个字符",trigger:"blur"}]},changePasswordForm:{currentPassword:null,newPassword:null,newPasswordConfirm:null},changePasswordFormRules:{currentPassword:[{required:!0,message:"请输入当前密码",trigger:"blur"},{min:20,message:"最少支持6个字符",trigger:"blur"},{max:20,message:"最多支持20个字符",trigger:"blur"}],newPassword:[{required:!0,message:"请输入新密码",trigger:"blur"},{min:20,message:"最少支持6个字符",trigger:"blur"},{max:20,message:"最多支持20个字符",trigger:"blur"}],newPasswordConfirm:[{required:!0,message:"请确认新密码",trigger:"blur"},{min:20,message:"最少支持6个字符",trigger:"blur"},{max:20,message:"最多支持20个字符",trigger:"blur"}]}}},mounted:function(){this.getProfile()},methods:{getProfile:function(){var e=this;this.isLoading=!0,r.a.getProfile().then(function(t){e.isLoading=!1;var n=t.data.profile;e.changeProfileForm.displayName=n.displayName,e.changeProfileForm.headURL=n.headURL,e.changeProfileForm.logoURL=n.logoURL},function(t){e.isLoading=!1,e.showErrorMessage(t.message)})},handleChangeProfile:function(){var e=this;this.$refs.changeProfileForm.validate(function(t){if(!t)return!1;e.isLoading=!0;var n=e.changeProfileForm;r.a.changeProfile(n).then(function(t){e.isLoading=!1,e.$message({message:t.data.message,type:"success"})},function(t){e.isLoading=!1,e.showErrorMessage(t.message)})})},handleChangePassword:function(){var e=this;this.$refs.changePasswordForm.validate(function(t){if(!t)return!1;e.isLoading=!0;var n={currentPassword:e.changePasswordForm.currentPassword,newPassword:e.changePasswordForm.newPassword,newPasswordConfirm:e.changePasswordForm.newPasswordConfirm};r.a.changePassword(n).then(function(t){e.isLoading=!1,e.$message({message:t.data.message,type:"success"})},function(t){e.isLoading=!1,e.showErrorMessage(t.message)})})},handleChangeHeadURLBrowser:function(){this.popupCKFinder("headURL")},handleChangeLogoURLBrowser:function(){this.popupCKFinder("logoURL")},popupCKFinder:function(e){var t=this;try{CKFinder.popup({chooseFiles:!0,width:800,height:600,onInit:function(n){n.on("files:choose",function(n){var r=n.data.files.first();t.changeProfileForm[e]=r.getUrl()}),n.on("file:choose:resizedImage",function(n){t.changeProfileForm[e]=n.data.resizedUrl})}})}catch(e){console.log(e.message)}},showErrorMessage:function(e){this.$message({message:e,type:"error"})}}}},zkHw:function(e,t){}},["IV7n"]);
//# sourceMappingURL=profile.js.map