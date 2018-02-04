using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CKSource.CKFinder.Connector.Core;
using CKSource.CKFinder.Connector.Config;
using CKSource.CKFinder.Connector.Core.Builders;
using CKSource.CKFinder.Connector.Host.Owin;
using CKSource.FileSystem.Local;
using Owin;
using CKSource.CKFinder.Connector.Core.Acl;
using CKSource.CKFinder.Connector.Core.ResizedImages;
using System.Drawing;
using CKSource.CKFinder.Connector.Core.KeyValueStores;

namespace Lavie.CKFinder
{
    public class ConnectorConfig
    {
        public static void SetupConnector(IAppBuilder builder)
        {
            var connectorBuilder = new ConnectorBuilder();
            var connector = connectorBuilder
                .SetAuthenticator(new GeneralAuthenticator())
                .SetLicense("192.168.0.104", "QVUVC69BEDVXTFMRYAEKJI2DN0DAJ") // VEA2VFXJ6NA
                //.LoadConfig()
                //.SetVerboseLogging(true)
                //.SetCsrfProtection(true)
                .SetRequestConfiguration(
                    (request, config) =>
                    {
                        var userName = request.Principal?.Identity?.Name;

                        //config.LoadConfig();
                        // images
                        config.SetMaxImageSize(new Size(1600, 1200));
                        config.SetImageResizeThreshold(80, 10);
                        config.SetDefaultImageQuality(new ImageQuality(80));
                        config.SetSizeDefinitions(
                            new SizeDefinition("small", 480, 320, new ImageQuality(80)),
                            new SizeDefinition("medium", 600, 480, new ImageQuality(80)),
                            new SizeDefinition("large", 800, 600, new ImageQuality(80))
                            );

                        // thumbnails
                        config.SetThumbnailBackend("CKFinderPrivate", "thumbs");
                        config.SetThumbnailSizes(new SizeAndQuality(new Size(150, 150), new ImageQuality(80)));
                        config.SetThumbnailSizes(new SizeAndQuality(new Size(300, 300), new ImageQuality(80)));
                        config.SetThumbnailSizes(new SizeAndQuality(new Size(500, 500), new ImageQuality(80)));

                        // backends
                        // config.AddBackend("CKFinderPrivate", new LocalStorage("App_Data"));                       // 默认
                        // config.AddProxyBackend("default", new LocalStorage("userfiles", "/ckfinder/userfiles/")); // 默认
                        config.AddBackend("CKFinderPrivate", new LocalStorage("userfiles/" + userName));
                        config.AddBackend("default", new LocalStorage("userfiles/" + userName, "/userfiles/" + userName + "/"));
                        config.AddBackend("public", new LocalStorage("userfiles/public", "/userfiles/public/"));

                        // resourceTypes
                        var imageExtensions = "gif,jpeg,jpg,png".Split(',');
                        var fullExtensions = "7z,aiff,asf,avi,bmp,csv,doc,docx,fla,flv,gif,gz,gzip,jpeg,jpg,mid,mov,mp3,mp4,mpc,mpeg,mpg,ods,odt,pdf,png,ppt,pptx,pxd,qt,ram,rar,rm,rmi,rmvb,rtf,sdc,sitd,swf,sxc,sxw,tar,tgz,tif,tiff,txt,vsd,wav,wma,wmv,xls,xlsx,zip".Split(',');
                        config.AddResourceType("1. 我的文档", resourceBuilder => {
                            resourceBuilder.SetBackend("default", "files");
                            resourceBuilder.SetAllowedExtensions(fullExtensions);
                        });
                        config.AddResourceType("2. 我的图片", resourceBuilder => {
                            resourceBuilder.SetBackend("default", "images");
                            resourceBuilder.SetAllowedExtensions(imageExtensions);
                        });
                        config.AddResourceType("3. 我的回收站", resourceBuilder => {
                            resourceBuilder.SetBackend("default", "trash");
                            resourceBuilder.SetAllowedExtensions(fullExtensions);
                        });
                        config.AddResourceType("4. 共享目录", resourceBuilder => {
                            resourceBuilder.SetBackend("public", "");
                            resourceBuilder.SetAllowedExtensions(fullExtensions);
                        });

                        /* Give full access to all resource types at any path for all users. */
                        config.AddAclRule(new AclRule(
                            new StringMatcher("*"), new StringMatcher("*"), new StringMatcher("*"),
                            new Dictionary<Permission, PermissionType>
                            {
                                 { Permission.FolderView, PermissionType.Allow },
                                 { Permission.FolderCreate, PermissionType.Allow },
                                 { Permission.FolderRename, PermissionType.Allow },
                                 { Permission.FolderDelete, PermissionType.Allow },

                                 { Permission.FileView, PermissionType.Allow },
                                 { Permission.FileCreate, PermissionType.Allow },
                                 { Permission.FileRename, PermissionType.Allow },
                                 { Permission.FileDelete, PermissionType.Allow },

                                 { Permission.ImageResize, PermissionType.Allow },
                                 { Permission.ImageResizeCustom, PermissionType.Allow }
                            }));

                        // config.SetKeyValueStoreProvider(new InMemoryKeyValueStoreProvider());
                    })
                .Build(new OwinConnectorFactory());

            builder.UseConnector(connector);
        }
    }
}
