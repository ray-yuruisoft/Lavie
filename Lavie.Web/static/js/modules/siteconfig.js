webpackJsonp([6],{"/rEA":function(t,e,n){"use strict";(function(t){function i(t,e,n){this.name="ApiError",this.message=t||"Default Message",this.errorType=e||g.Default,this.innerError=n,this.stack=(new Error).stack}var r=n("//Fk"),o=n.n(r),a=n("mvHQ"),s=n.n(a),l=n("OvRC"),u=n.n(l),f=n("mtWM"),m=n.n(f),d=n("mw3O"),c=n.n(d),p=n("bzuE"),g={Default:"default",Sysetem:"sysetem"};(i.prototype=u()(Error.prototype)).constructor=i;var h=m.a.create({baseURL:p.a,timeout:2e4,responseType:"json",withCredentials:!0});h.interceptors.request.use(function(t){return"post"===t.method||"put"===t.method||"patch"===t.method?(t.headers={"Content-Type":"application/json charset=utf-8"},t.data=s()(t.data)):"delete"!==t.method&&"get"!==t.method&&"head"!==t.method||(t.paramsSerializer=function(t){return c.a.stringify(t,{arrayFormat:"indices"})}),t},function(t){return t}),h.interceptors.response.use(function(e){if(-1===e.headers["content-type"].indexOf("json"))return e;var n=void 0;if("arraybuffer"===e.request.responseType&&"[object ArrayBuffer]"===e.data.toString()){var r=t.from(e.data).toString("utf8");console.log(r),n=JSON.parse(r)}else n=e.data;if(n&&n.url)top.location=n.url;else if(n&&200!==n.code)return console.log(n),o.a.reject(new i(n.message));return e},function(t){return o.a.reject(new i(t.message,g.Sysetem,t))}),e.a={install:function(t){arguments.length>1&&void 0!==arguments[1]&&arguments[1];t.httpClient=h,t.prototype.$httpClient=h}}}).call(e,n("EuP9").Buffer)},D8dk:function(t,e,n){"use strict";var i={render:function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("el-container",{directives:[{name:"loading",rawName:"v-loading.fullscreen.lock",value:t.isLoading,expression:"isLoading",modifiers:{fullscreen:!0,lock:!0}}]},[n("el-header",{staticClass:"header"},[n("el-breadcrumb",{staticClass:"breadcrumb",attrs:{"separator-class":"el-icon-arrow-right"}},[n("el-breadcrumb-item",[t._v("系统管理")]),t._v(" "),n("el-breadcrumb-item",[t._v("系统配置")])],1)],1),t._v(" "),n("el-main",{staticClass:"main"},[n("el-form",{ref:"mainForm",attrs:{model:t.mainForm,rules:t.mainFormRules,"label-position":"right","label-width":"120px",size:"mini"}},[n("el-form-item",{attrs:{label:"系统名称",prop:"name"}},[n("el-input",{ref:"name",attrs:{"auto-complete":"off",placeholder:"请输入系统名称"},model:{value:t.mainForm.name,callback:function(e){t.$set(t.mainForm,"name","string"==typeof e?e.trim():e)},expression:"mainForm.name"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"系统地址",prop:"host"}},[n("el-input",{ref:"host",attrs:{"auto-complete":"off",placeholder:"请输入系统地址"},model:{value:t.mainForm.host,callback:function(e){t.$set(t.mainForm,"host","string"==typeof e?e.trim():e)},expression:"mainForm.host"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"系统主标题",prop:"title"}},[n("el-input",{ref:"title",attrs:{"auto-complete":"off",placeholder:"请输入系统主标题"},model:{value:t.mainForm.title,callback:function(e){t.$set(t.mainForm,"title","string"==typeof e?e.trim():e)},expression:"mainForm.title"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"系统关键字",prop:"keywords"}},[n("el-input",{ref:"keywords",attrs:{"auto-complete":"off",placeholder:"请输入系统关键字"},model:{value:t.mainForm.keywords,callback:function(e){t.$set(t.mainForm,"keywords","string"==typeof e?e.trim():e)},expression:"mainForm.keywords"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"系统描述",prop:"description"}},[n("el-input",{ref:"description",attrs:{type:"textarea",rows:4,"auto-complete":"off",placeholder:"请输入系统描述"},model:{value:t.mainForm.description,callback:function(e){t.$set(t.mainForm,"description","string"==typeof e?e.trim():e)},expression:"mainForm.description"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"版权信息",prop:"copyright"}},[n("el-input",{ref:"copyright",attrs:{type:"textarea",rows:4,"auto-complete":"off",placeholder:"请输入系统描述"},model:{value:t.mainForm.copyright,callback:function(e){t.$set(t.mainForm,"copyright","string"==typeof e?e.trim():e)},expression:"mainForm.copyright"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"系统小图标",prop:"faviconURL"}},[n("el-input",{ref:"faviconURL",attrs:{"auto-complete":"off",placeholder:"请输入系统小图标地址"},model:{value:t.mainForm.faviconURL,callback:function(e){t.$set(t.mainForm,"faviconURL","string"==typeof e?e.trim():e)},expression:"mainForm.faviconURL"}})],1),t._v(" "),n("el-form-item",{attrs:{label:"标题分隔符",prop:"pageTitleSeparator"}},[n("el-input",{ref:"pageTitleSeparator",attrs:{"auto-complete":"off",placeholder:"请输入标题分隔符"},model:{value:t.mainForm.pageTitleSeparator,callback:function(e){t.$set(t.mainForm,"pageTitleSeparator",e)},expression:"mainForm.pageTitleSeparator"}})],1),t._v(" "),n("el-form-item",[n("el-button",{attrs:{type:"primary"},on:{click:t.handleEditSiteConfig}},[t._v("确 认")])],1)],1)],1)],1)},staticRenderFns:[]};e.a=i},S3W6:function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var i=n("tvR6"),r=(n.n(i),n("qBF2")),o=n.n(r),a=n("7+uW"),s=n("uN2V"),l=(n.n(s),n("itJ1")),u=n("/rEA");a.default.config.productionTip=!1,a.default.use(o.a),a.default.use(u.a),new a.default({el:"#app",render:function(t){return t(l.a)}})},VsUZ:function(t,e,n){"use strict";var i=n("7+uW");e.a={login:function(t){return i.default.httpClient.post("/admin/login",t)},logout:function(){return i.default.httpClient.post("/admin/logout")},getProfile:function(){return i.default.httpClient.get("/admin/getProfile")},changeProfile:function(t){return i.default.httpClient.post("/admin/changeProfile",t)},changePassword:function(t){return i.default.httpClient.post("/admin/changePassword",t)},getMenus:function(){return i.default.httpClient.get("/admin/getMenus")},getServerInfo:function(){return i.default.httpClient.get("/admin/getServerinfo")},getSiteConfig:function(){return i.default.httpClient.get("/admin/getSiteconfig")},editSiteConfig:function(t){return i.default.httpClient.post("/admin/editSiteconfig",t)},getBulletin:function(){return i.default.httpClient.get("/admin/getBulletin")},editBulletin:function(t){return i.default.httpClient.post("/admin/editBulletin",t)},getPermissions:function(){return i.default.httpClient.get("/admin/getPermissions")},getModuleConfig:function(){return i.default.httpClient.get("/admin/getModuleConfig")},extractModulePermissions:function(){return i.default.httpClient.get("/admin/extractModulePermissions")},clearModulePermissions:function(){return i.default.httpClient.get("/admin/clearModulePermissions")},getRoles:function(){return i.default.httpClient.get("/admin/getRoles")},addRole:function(t){return i.default.httpClient.post("/admin/addRole",t)},editRole:function(t){return i.default.httpClient.post("/admin/editRole",t)},removeRole:function(t){return i.default.httpClient.post("/admin/removeRole",t)},moveRole:function(t){return i.default.httpClient.post("/admin/moveRole",t)},saveRoleName:function(t){return i.default.httpClient.post("/admin/saveRoleName",t)},getGroupTree:function(){return i.default.httpClient.get("/admin/getGroupTree")},addGroup:function(t){return i.default.httpClient.post("/admin/addGroup",t)},editGroup:function(t){return i.default.httpClient.post("/admin/editGroup",t)},removeGroup:function(t){return i.default.httpClient.post("/admin/removeGroup",t)},moveGroup:function(t){return i.default.httpClient.post("/admin/moveGroup",t)},getUsers:function(t){return i.default.httpClient.post("/admin/getUsers",t)},addUser:function(t){return i.default.httpClient.post("/admin/addUser",t)},editUser:function(t){return i.default.httpClient.post("/admin/editUser",t)},removeUser:function(t){return i.default.httpClient.post("/admin/removeUser",t)},getUserStatus:function(){return i.default.httpClient.get("/admin/getUserStatus")},getNotificationsForManager:function(t){return i.default.httpClient.post("/admin/getNotificationsForManager",t)},addNotification:function(t){return i.default.httpClient.post("/admin/addNotification",t)},editNotification:function(t){return i.default.httpClient.post("/admin/editNotification",t)},removeNotification:function(t){return i.default.httpClient.post("/admin/removeNotification",t)},getNotifications:function(t){return i.default.httpClient.post("/admin/getNotifications",t)},readNotifications:function(t){return i.default.httpClient.post("/admin/readNotifications",t)},deleteNotifications:function(t){return i.default.httpClient.post("/admin/deleteNotifications",t)},getNewestNotification:function(t){return i.default.httpClient.post("/admin/getNewestNotification",t)},getGroups:function(){return i.default.httpClient.get("/admin/getGroups")},getRoleBases:function(){return i.default.httpClient.get("/admin/getRoleBases")},getPermissionTree:function(){return i.default.httpClient.get("/admin/getPermissionTree")},directlyCall:function(t){return i.default.httpClient.get(t)},download:function(t,e){return i.default.httpClient.post(t,e,{responseType:"arraybuffer"})}}},bg88:function(t,e){},bzuE:function(t,e,n){"use strict";n.d(e,"a",function(){return i}),n.d(e,"b",function(){return r}),n.d(e,"c",function(){return o});var i="/manager/api",r="",o=""},itJ1:function(t,e,n){"use strict";var i=n("sDpe"),r=n("D8dk"),o=function(t){n("bg88")},a=n("VU/8")(i.a,r.a,!1,o,"data-v-68fc1296",null);e.a=a.exports},sDpe:function(t,e,n){"use strict";var i=n("VsUZ");e.a={data:function(){return{isLoading:!1,mainForm:{name:null,host:null,title:null,keywords:null,description:null,copyright:null,faviconURL:null,pageTitleSeparator:null},mainFormRules:{name:[{required:!0,message:"请输入系统名称",trigger:"blur"},{max:50,message:"最多支持50个字符",trigger:"blur"}],host:[{required:!0,message:"请输入系统地址",trigger:"blur"},{max:100,message:"最多支持100个字符",trigger:"blur"},{pattern:/^https?:\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\\.,@?^=%&amp;:/~\\+#]*[\w\-\\@?^=%&amp;/~\\+#])?$/,message:"请输入正确的网址",trigger:"blur"}],title:[{max:100,message:"最多支持100个字符",trigger:"blur"}],keywords:[{max:200,message:"最多支持200个字符",trigger:"blur"}],description:[{max:500,message:"最多支持500个字符",trigger:"blur"}],copyright:[{max:1e3,message:"最多支持1000个字符",trigger:"blur"}],faviconURL:[{max:100,message:"最多支持100个字符",trigger:"blur"}],pageTitleSeparator:[{max:50,message:"最多支持50个字符",trigger:"blur"}]}}},mounted:function(){this.getSiteConfig()},methods:{getSiteConfig:function(){var t=this;this.isLoading=!0,i.a.getSiteConfig().then(function(e){t.isLoading=!1,t.mainForm=e.data.item},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})},handleEditSiteConfig:function(){this.editSiteConfig()},editSiteConfig:function(){var t=this;this.$refs.mainForm.validate(function(e){if(!e)return!1;t.isLoading=!0;var n=t.mainForm;i.a.editSiteConfig(n).then(function(e){t.isLoading=!1,t.$message({message:e.data.message,type:"success"})},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})})},showErrorMessage:function(t){this.$message({message:t,type:"error"})}}}},tvR6:function(t,e){},uN2V:function(t,e){}},["S3W6"]);
//# sourceMappingURL=siteconfig.js.map