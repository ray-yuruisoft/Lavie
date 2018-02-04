webpackJsonp([1],{"+cgv":function(t,e){},"/rEA":function(t,e,i){"use strict";(function(t){function n(t,e,i){this.name="ApiError",this.message=t||"Default Message",this.errorType=e||p.Default,this.innerError=i,this.stack=(new Error).stack}var o=i("//Fk"),r=i.n(o),a=i("mvHQ"),l=i.n(a),s=i("OvRC"),u=i.n(s),c=i("mtWM"),d=i.n(c),f=i("mw3O"),m=i.n(f),h=i("bzuE"),p={Default:"default",Sysetem:"sysetem"};(n.prototype=u()(Error.prototype)).constructor=n;var g=d.a.create({baseURL:h.a,timeout:2e4,responseType:"json",withCredentials:!0});g.interceptors.request.use(function(t){return"post"===t.method||"put"===t.method||"patch"===t.method?(t.headers={"Content-Type":"application/json charset=utf-8"},t.data=l()(t.data)):"delete"!==t.method&&"get"!==t.method&&"head"!==t.method||(t.paramsSerializer=function(t){return m.a.stringify(t,{arrayFormat:"indices"})}),t},function(t){return t}),g.interceptors.response.use(function(e){if(-1===e.headers["content-type"].indexOf("json"))return e;var i=void 0;if("arraybuffer"===e.request.responseType&&"[object ArrayBuffer]"===e.data.toString()){var o=t.from(e.data).toString("utf8");console.log(o),i=JSON.parse(o)}else i=e.data;if(i&&i.url)top.location=i.url;else if(i&&200!==i.code)return console.log(i),r.a.reject(new n(i.message));return e},function(t){return r.a.reject(new n(t.message,p.Sysetem,t))}),e.a={install:function(t){arguments.length>1&&void 0!==arguments[1]&&arguments[1];t.httpClient=g,t.prototype.$httpClient=g}}}).call(e,i("EuP9").Buffer)},"0eOK":function(t,e,i){"use strict";var n={render:function(){var t=this,e=t.$createElement,i=t._self._c||e;return i("el-container",{directives:[{name:"loading",rawName:"v-loading.fullscreen.lock",value:t.isLoading,expression:"isLoading",modifiers:{fullscreen:!0,lock:!0}}]},[i("el-header",{staticClass:"header"},[i("el-breadcrumb",{staticClass:"breadcrumb",attrs:{"separator-class":"el-icon-arrow-right"}},[i("el-breadcrumb-item",[t._v("通知管理")])],1)],1),t._v(" "),i("el-main",{staticClass:"main"},[i("el-form",{ref:"searchCriteriaForm",staticClass:"searchCriteriaForm",attrs:{model:t.searchCriteriaForm,inline:"",size:"mini"}},[i("el-row",[i("el-form-item",[i("el-input",{staticClass:"filterText",attrs:{placeholder:"关键字(标题)",clearable:""},model:{value:t.searchCriteriaForm.keyword,callback:function(e){t.$set(t.searchCriteriaForm,"keyword",e)},expression:"searchCriteriaForm.keyword"}})],1),t._v(" "),i("el-form-item",[i("el-date-picker",{attrs:{"value-format":"yyyy-MM-dd",type:"daterange","range-separator":"至","start-placeholder":"创建日期开始","end-placeholder":"创建日期结束"},model:{value:t.searchCriteriaForm.creationDate,callback:function(e){t.$set(t.searchCriteriaForm,"creationDate",e)},expression:"searchCriteriaForm.creationDate"}})],1),t._v(" "),i("el-form-item",[i("el-button-group",[i("el-button",{attrs:{type:"primary",icon:"el-icon-search"},on:{click:function(e){t.handleSearch()}}},[t._v("搜索")]),t._v(" "),i("el-button",{attrs:{type:"primary",icon:"el-icon-search"},on:{click:function(e){t.handleSearchAll()}}},[t._v("全部")])],1),t._v(" "),i("el-button",{attrs:{type:"primary",icon:"el-icon-circle-plus-outline"},on:{click:function(e){t.handleAdd()}}},[t._v("添加")])],1)],1)],1),t._v(" "),i("el-row",[i("el-table",{ref:"mainTable",staticStyle:{width:"100%"},attrs:{data:t.page.list,size:"small","empty-text":t.emptyText},on:{"row-click":t.handleRowClick,"sort-change":t.handleSortChange}},[i("el-table-column",{attrs:{type:"expand",label:"查看"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("div",{domProps:{innerHTML:t._s(e.row.message)}})]}}])}),t._v(" "),i("el-table-column",{attrs:{prop:"notificationID",label:"#",width:"60",sortable:"custom"}}),t._v(" "),i("el-table-column",{attrs:{prop:"title",label:"标题"}}),t._v(" "),t.searchCriteriaForm.toUserID?i("el-table-column",{attrs:{label:"已读",width:"100"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("span",[t._v(t._s(e.row.readTime?"√":""))])]}}])}):t._e(),t._v(" "),i("el-table-column",{attrs:{label:"来自",width:"100"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("span",[t._v(t._s(e.row.fromUser?e.row.fromUser.displayName:"系统"))])]}}])}),t._v(" "),i("el-table-column",{attrs:{label:"送至",width:"100"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("span",[t._v(t._s(e.row.toUser?e.row.toUser.displayName:"全部"))])]}}])}),t._v(" "),t.searchCriteriaForm.toUserID?i("el-table-column",{attrs:{label:"已删",width:"100"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("span",[t._v(t._s(e.row.deleteTime?"√":""))])]}}])}):t._e(),t._v(" "),i("el-table-column",{attrs:{prop:"creationDate",label:"创建时间",width:"160"}}),t._v(" "),i("el-table-column",{attrs:{align:"center",width:"42"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("el-button",{attrs:{type:"text",size:"small",icon:"el-icon-edit"},on:{click:function(i){t.handleEdit(e.row)}}})]}}])}),t._v(" "),i("el-table-column",{attrs:{align:"center",width:"42"},scopedSlots:t._u([{key:"default",fn:function(e){return[i("el-button",{attrs:{type:"text",size:"small",icon:"el-icon-delete"},on:{click:function(i){t.handleRemove(e.row)}}})]}}])})],1)],1),t._v(" "),i("el-dialog",{attrs:{visible:t.mainFormDialogVisible,"close-on-click-modal":!1,width:"780px"},on:{"update:visible":function(e){t.mainFormDialogVisible=e}},nativeOn:{submit:function(t){t.preventDefault()}}},[i("span",{attrs:{slot:"title"},slot:"title"},[t._v("\n        "+t._s(t.editActive?"编辑":"添加")+"\n      ")]),t._v(" "),i("el-form",{ref:"mainForm",attrs:{model:t.mainForm,rules:t.mainFormRules,"label-position":"right","label-width":"80px",size:"mini"}},[i("el-form-item",{attrs:{label:"标题",prop:"title"}},[i("el-input",{attrs:{type:"text"},model:{value:t.mainForm.title,callback:function(e){t.$set(t.mainForm,"title",e)},expression:"mainForm.title"}})],1),t._v(" "),i("el-form-item",{attrs:{label:"消息",prop:"message"}},[i("quill-editor",{ref:"content",attrs:{options:t.editorOption},model:{value:t.mainForm.message,callback:function(e){t.$set(t.mainForm,"message",e)},expression:"mainForm.message"}})],1)],1),t._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{on:{click:function(e){t.handleMainFormSure(!1)}}},[t._v("取 消")]),t._v(" "),i("el-button",{attrs:{type:"primary"},on:{click:function(e){t.handleMainFormSure(!0)}}},[t._v("确 定")])],1)],1),t._v(" "),i("el-dialog",{attrs:{title:"提示",visible:t.removeConfirmDialogVisible,width:"320px",center:""},on:{"update:visible":function(e){t.removeConfirmDialogVisible=e}}},[i("span",[t._v("删除该通知后，相关的数据也将被删除。"),i("br"),t._v("确定要删除吗？")]),t._v(" "),i("div",{staticClass:"dialog-footer",attrs:{slot:"footer"},slot:"footer"},[i("el-button",{on:{click:function(e){t.handleRemoveSure(!1)}}},[t._v("取 消")]),t._v(" "),i("el-button",{attrs:{type:"primary"},on:{click:function(e){t.handleRemoveSure(!0)}}},[t._v("确 定")])],1)])],1),t._v(" "),i("el-footer",{staticClass:"footer"},[t.page.totalItemCount?i("el-pagination",{attrs:{"current-page":t.pagingInfoForm.pageNumber,"page-sizes":[20,50,100,200,400],"page-size":t.pagingInfoForm.pageSize,layout:"total, sizes, prev, pager, next, jumper",total:t.page.totalItemCount},on:{"size-change":t.handlePaginationSizeChange,"current-change":t.handlePaginationCurrentChange}}):t._e()],1)],1)},staticRenderFns:[]};e.a=n},"3f40":function(t,e){},"4qOc":function(t,e){},CbDO:function(t,e,i){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var n=i("tvR6"),o=(i.n(n),i("qBF2")),r=i.n(o),a=i("7+uW"),l=i("uN2V"),s=(i.n(l),i("q+7M")),u=i("/rEA"),c=i("/7iZ"),d=i.n(c);a.default.config.productionTip=!1,a.default.use(r.a),a.default.use(u.a),a.default.use(d.a),new a.default({el:"#app",render:function(t){return t(s.a)}})},JnLJ:function(t,e){},MHM4:function(t,e){},MUOT:function(t,e,i){"use strict";var n=i("VsUZ"),o=i("M4fF"),r=i.n(o);e.a={data:function(){return{isLoading:!1,editorOption:{placeholder:"请输入消息"},page:{list:null,totalItemCount:null,totalPageCount:null},isSearchCriteriaFormExpand:!1,searchCriteriaForm:{keyword:null,toUserID:null,creationDate:null,creationDateBegin:null,creationDateEnd:null},pagingInfoForm:{pageNumber:1,pageSize:20,isExcludeMetaData:!1,sortInfo:{sort:"NotificationID",sortDir:"DESC"}},removeActive:null,removeConfirmDialogVisible:!1,editActive:null,mainFormDialogVisible:!1,mainForm:{notificationID:null,title:null,message:null},mainFormRules:{title:[{required:!0,message:"请输入标题",trigger:"blur"},{max:100,message:"最多支持100个字符",trigger:"blur"}],message:[{required:!0,message:"请输入消息",trigger:"blur"},{max:1e3,message:"最多支持1000个字符",trigger:"blur"}]}}},mounted:function(){this.getPage()},computed:{emptyText:function(){return this.isLoading?"加载中...":"暂无数据"}},watch:{},methods:{getPage:function(){var t=this;this.isLoading=!0;var e=r.a.extend({},this.pagingInfoForm,this.searchCriteriaForm);n.a.getNotificationsForManager(e).then(function(e){t.isLoading=!1,t.page=e.data.page},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})},handlePaginationSizeChange:function(t){this.pagingInfoForm.pageSize=t,this.pagingInfoForm.pageNumber=1,this.getPage()},handlePaginationCurrentChange:function(t){this.pagingInfoForm.pageNumber=t,this.getPage()},handleSearchAll:function(){this.pagingInfoForm.pageNumber=1,this.searchCriteriaForm.keyword=null,this.getPage()},handleSearch:function(){this.pagingInfoForm.pageNumber=1,this.searchCriteriaForm.creationDate&&2===this.searchCriteriaForm.creationDate.length&&(this.searchCriteriaForm.creationDateBegin=this.searchCriteriaForm.creationDate[0],this.searchCriteriaForm.creationDateEnd=this.searchCriteriaForm.creationDate[1]),this.getPage()},handleAdd:function(){var t=this;this.editActive=null,this.mainFormDialogVisible=!0,this.mainForm.notificationID=null,this.mainForm.title=null,this.mainForm.message=null,this.$nextTick(function(){t.clearValidate("mainForm")})},handleEdit:function(t){var e=this;console.log("handleEdit",t),this.editActive=t,this.mainFormDialogVisible=!0,this.mainForm.notificationID=t.notificationID,this.mainForm.title=t.title,this.mainForm.message=t.message,this.$nextTick(function(){e.clearValidate("mainForm")})},handleMainFormSure:function(t){console.log("handleMainFormSure",t),t?this.editActive?this.edit():this.add():this.mainFormDialogVisible=!1},handleRemove:function(t){this.removeActive=t,this.removeConfirmDialogVisible=!0},handleRemoveSure:function(t){this.removeConfirmDialogVisible=!1,t?this.remove():this.removeActive=null},add:function(){var t=this;this.$refs.mainForm.validate(function(e){if(!e)return!1;t.isLoading=!0;var i=t.mainForm;n.a.addNotification(i).then(function(e){t.isLoading=!1,t.mainFormDialogVisible=!1,t.getPage()},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})})},edit:function(){var t=this;this.editActive?this.$refs.mainForm.validate(function(e){if(!e)return!1;t.isLoading=!0;var i=t.mainForm;n.a.editNotification(i).then(function(e){t.isLoading=!1,t.editActive=null,t.mainFormDialogVisible=!1,t.getPage()},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})}):this.showErrorMessage("异常：无编辑目标")},remove:function(){var t=this;if(this.removeActive){var e={notificationID:this.removeActive.notificationID};this.isLoading=!0,n.a.removeNotification(e).then(function(e){t.isLoading=!1,t.removeActive=null,t.getPage()},function(e){t.isLoading=!1,t.showErrorMessage(e.message)})}},resetForm:function(t){this.$refs[t].resetFields()},clearValidate:function(t){this.$refs[t].clearValidate()},showErrorMessage:function(t){this.$message({message:t,type:"error"})},handleRowClick:function(t,e,i){"el-table_1_column_1"!==i.id&&this.$refs.mainTable.toggleRowExpansion(t)},handleSortChange:function(t){this.pagingInfoForm.sortInfo.sort=t.prop,this.pagingInfoForm.sortInfo.sortDir="descending"===t.order?"DESC":"ASC",this.pagingInfoForm.pageNumber=1,this.getPage()}}}},V84S:function(t,e,i){"use strict";i("4qOc"),i("+cgv"),i("3f40"),window.Quill||(window.Quill=i("yPE/")),e.a={name:"quill-editor",data:function(){return{_content:"",defaultModules:{toolbar:[["bold","italic","underline","strike"],["blockquote","code-block"],[{header:1},{header:2}],[{list:"ordered"},{list:"bullet"}],[{script:"sub"},{script:"super"}],[{indent:"-1"},{indent:"+1"}],[{direction:"rtl"}],[{size:["small",!1,"large","huge"]}],[{header:[1,2,3,4,5,6,!1]}],[{color:[]},{background:[]}],[{font:[]}],[{align:[]}],["clean"],["link","image","video"]]}}},props:{content:String,value:String,disabled:Boolean,options:{type:Object,required:!1,default:function(){return{}}}},mounted:function(){this.initialize()},beforeDestroy:function(){this.quill=null},methods:{initialize:function(){if(this.$el){var t=this;t.options.theme=t.options.theme||"snow",t.options.boundary=t.options.boundary||document.body,t.options.modules=t.options.modules||t.defaultModules,t.options.modules.toolbar=void 0!==t.options.modules.toolbar?t.options.modules.toolbar:t.defaultModules.toolbar,t.options.placeholder=t.options.placeholder||"Insert text here ...",t.options.readOnly=void 0!==t.options.readOnly&&t.options.readOnly,t.quill=new Quill(t.$refs.editor,t.options),(t.value||t.content)&&t.quill.pasteHTML(t.value||t.content),t.quill.on("selection-change",function(e){e?t.$emit("focus",t.quill):t.$emit("blur",t.quill)}),t.quill.on("text-change",function(e,i,n){var o=t.$refs.editor.children[0].innerHTML,r=t.quill.getText();"<p><br></p>"===o&&(o=""),t._content=o,t.$emit("input",t._content),t.$emit("change",{editor:t.quill,html:o,text:r})}),this.disabled&&this.quill.enable(!1),t.$emit("ready",t.quill)}}},watch:{content:function(t,e){this.quill&&(t&&t!==this._content?(this._content=t,this.quill.pasteHTML(t)):t||this.quill.setText(""))},value:function(t,e){this.quill&&(t&&t!==this._content?(this._content=t,this.quill.pasteHTML(t)):t||this.quill.setText(""))},disabled:function(t,e){this.quill&&this.quill.enable(!t)}}}},VsUZ:function(t,e,i){"use strict";var n=i("7+uW");e.a={login:function(t){return n.default.httpClient.post("/admin/login",t)},logout:function(){return n.default.httpClient.post("/admin/logout")},getProfile:function(){return n.default.httpClient.get("/admin/getProfile")},changeProfile:function(t){return n.default.httpClient.post("/admin/changeProfile",t)},changePassword:function(t){return n.default.httpClient.post("/admin/changePassword",t)},getMenus:function(){return n.default.httpClient.get("/admin/getMenus")},getServerInfo:function(){return n.default.httpClient.get("/admin/getServerinfo")},getSiteConfig:function(){return n.default.httpClient.get("/admin/getSiteconfig")},editSiteConfig:function(t){return n.default.httpClient.post("/admin/editSiteconfig",t)},getBulletin:function(){return n.default.httpClient.get("/admin/getBulletin")},editBulletin:function(t){return n.default.httpClient.post("/admin/editBulletin",t)},getPermissions:function(){return n.default.httpClient.get("/admin/getPermissions")},getModuleConfig:function(){return n.default.httpClient.get("/admin/getModuleConfig")},extractModulePermissions:function(){return n.default.httpClient.get("/admin/extractModulePermissions")},clearModulePermissions:function(){return n.default.httpClient.get("/admin/clearModulePermissions")},getRoles:function(){return n.default.httpClient.get("/admin/getRoles")},addRole:function(t){return n.default.httpClient.post("/admin/addRole",t)},editRole:function(t){return n.default.httpClient.post("/admin/editRole",t)},removeRole:function(t){return n.default.httpClient.post("/admin/removeRole",t)},moveRole:function(t){return n.default.httpClient.post("/admin/moveRole",t)},saveRoleName:function(t){return n.default.httpClient.post("/admin/saveRoleName",t)},getGroupTree:function(){return n.default.httpClient.get("/admin/getGroupTree")},addGroup:function(t){return n.default.httpClient.post("/admin/addGroup",t)},editGroup:function(t){return n.default.httpClient.post("/admin/editGroup",t)},removeGroup:function(t){return n.default.httpClient.post("/admin/removeGroup",t)},moveGroup:function(t){return n.default.httpClient.post("/admin/moveGroup",t)},getUsers:function(t){return n.default.httpClient.post("/admin/getUsers",t)},addUser:function(t){return n.default.httpClient.post("/admin/addUser",t)},editUser:function(t){return n.default.httpClient.post("/admin/editUser",t)},removeUser:function(t){return n.default.httpClient.post("/admin/removeUser",t)},getUserStatus:function(){return n.default.httpClient.get("/admin/getUserStatus")},getNotificationsForManager:function(t){return n.default.httpClient.post("/admin/getNotificationsForManager",t)},addNotification:function(t){return n.default.httpClient.post("/admin/addNotification",t)},editNotification:function(t){return n.default.httpClient.post("/admin/editNotification",t)},removeNotification:function(t){return n.default.httpClient.post("/admin/removeNotification",t)},getNotifications:function(t){return n.default.httpClient.post("/admin/getNotifications",t)},readNotifications:function(t){return n.default.httpClient.post("/admin/readNotifications",t)},deleteNotifications:function(t){return n.default.httpClient.post("/admin/deleteNotifications",t)},getNewestNotification:function(t){return n.default.httpClient.post("/admin/getNewestNotification",t)},getGroups:function(){return n.default.httpClient.get("/admin/getGroups")},getRoleBases:function(){return n.default.httpClient.get("/admin/getRoleBases")},getPermissionTree:function(){return n.default.httpClient.get("/admin/getPermissionTree")},directlyCall:function(t){return n.default.httpClient.get(t)},download:function(t,e){return n.default.httpClient.post(t,e,{responseType:"arraybuffer"})}}},bJDK:function(t,e,i){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var n=i("V84S"),o=i("hGrS"),r=function(t){i("MHM4")},a=i("VU/8")(n.a,o.a,!1,r,null,null);e.default=a.exports},bzuE:function(t,e,i){"use strict";i.d(e,"a",function(){return n}),i.d(e,"b",function(){return o}),i.d(e,"c",function(){return r});var n="/manager/api",o="",r=""},hGrS:function(t,e,i){"use strict";var n={render:function(){var t=this.$createElement,e=this._self._c||t;return e("div",{staticClass:"quill-editor"},[this._t("toolbar"),this._v(" "),e("div",{ref:"editor"})],2)},staticRenderFns:[]};e.a=n},"q+7M":function(t,e,i){"use strict";var n=i("MUOT"),o=i("0eOK"),r=function(t){i("JnLJ")},a=i("VU/8")(n.a,o.a,!1,r,null,null);e.a=a.exports},tvR6:function(t,e){},uN2V:function(t,e){}},["CbDO"]);
//# sourceMappingURL=notificationmanage.js.map