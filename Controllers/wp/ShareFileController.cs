using lightDiskBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace lightDiskBack.Controllers.wp
{
    public class ShareFileController : Controller
    {

        private readonly IdDBContext idDBContext;
        public ShareFileController(IdDBContext idDBContext)
        {
            this.idDBContext = idDBContext;

        }


        [Authorize]
        public JsonResult shareFile([FromQuery(Name = "fileId")] String fileId,
                                    [FromQuery(Name = "exCode")] String exCode,
                                    [FromQuery(Name = "endTime")] String endTime)
        {

            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);

            Share share = new Share();
            share.userId = userId;
            share.shareStatus = "0";
            Guid randomGuid = Guid.NewGuid();
            share.shareBatchNum = randomGuid + "";
            share.extractionCode = exCode;

            DateTime dataTime = DateTime.ParseExact(endTime, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            share.endTime = dataTime;
            idDBContext.Share.Add(share);

            var wpFolder = idDBContext.wpFile.Where(a => a.fileId == int.Parse(fileId) && a.userId == userId).Single();


            var wpFolderFiles = idDBContext.wpFile.Where(a => a.filePath.StartsWith(wpFolder.filePath + wpFolder.fileName + "/")
                            && a.userId == userId).ToList();


            ShareFile shareFolder = new ShareFile();
            shareFolder.shareFilePath = "/";
            shareFolder.wpFileName = wpFolder.fileName;
            shareFolder.wpIsFolder = wpFolder.isFolder;
            shareFolder.diskFileId = wpFolder.diskFileId;
            shareFolder.shareBatchNum = randomGuid.ToString();
            idDBContext.ShareFile.Add(shareFolder);

            foreach (WpFile wpfile in wpFolderFiles)
            {
                ShareFile sharefile = new ShareFile();

                sharefile.wpFileName = wpfile.fileName;

                sharefile.wpIsFolder = wpfile.isFolder;

                sharefile.diskFileId= wpfile.diskFileId;

                sharefile.shareBatchNum = randomGuid.ToString();

                sharefile.shareFilePath = wpfile.filePath.Replace(wpFolder.filePath,"/");

                idDBContext.ShareFile.Add(sharefile);

            }

            idDBContext.SaveChanges();

            return new JsonResult(randomGuid + "");

        }



        public JsonResult showShareFolder([FromQuery(Name = "shareCode")] String shareCode,
                                          [FromQuery(Name = "exCode")] String exCode)
        {

            //判断是否过期
            var share = idDBContext.Share.Where(a => a.shareBatchNum == shareCode).Single();

            DateTime now = DateTime.Now.Date;

            int result = DateTime.Compare(now, share.endTime);


            if(result > 0)
            {
                return new JsonResult("过期了");
            }


            //判断提取码
            if (!share.extractionCode.Equals(exCode))
            {
                return new JsonResult("提取码错误");
            }

            List<ShareFile> folders = idDBContext.ShareFile.Where(a => a.shareBatchNum == shareCode && a.wpIsFolder == "1").ToList();


            ShareFile curFile = new ShareFile();
            ShareFile folderTree = getFolderTree(folders, curFile, "/");


            return new JsonResult(folderTree);

        }


        private ShareFile getFolderTree(List<ShareFile> list, ShareFile curFile, String filePath)
        {
            foreach (ShareFile wpFile in list)
            {
                if (wpFile.shareFilePath.Equals(filePath))
                {
                    ShareFile fileIn = getFolderTree(list, wpFile, wpFile.shareFilePath + wpFile.wpFileName + "/");

                    curFile.ChildFolder.Add(fileIn);

                }

            }

            return curFile;
        }


        public JsonResult showShareFile([FromQuery(Name = "pid")] String pid,
                                        [FromQuery(Name = "shareCode")] String shareCode)
        {

            ShareFile pFolder = idDBContext.ShareFile.Where(a => a.wpIsFolder == "1" && a.shareFileId == int.Parse(pid)).Single();


            var files = idDBContext.ShareFile.Where(a => a.wpIsFolder == "0"
            && a.shareFilePath == pFolder.shareFilePath + pFolder.wpFileName+ "/"
            && a.shareBatchNum == shareCode).ToList();

            return new JsonResult(files);

        }


        [Authorize]
        public JsonResult saveShareFile([FromQuery(Name = "pid")] String pid,
                                        [FromQuery(Name = "shareCode")] String shareCode)
        {

            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);



             var wpFolder = idDBContext.wpFile.Where(a => a.fileId == int.Parse(pid)).Single();


            var shareFiles = idDBContext.ShareFile.Where(a => a.shareBatchNum == shareCode).ToList();


            foreach(ShareFile shareFile in shareFiles)
            {
                WpFile wpFile = new WpFile();
                wpFile.fileName = shareFile.wpFileName;
                wpFile.isFolder = shareFile.wpIsFolder;
                wpFile.delFlag = "0";
                wpFile.diskFileId = shareFile.diskFileId;
                wpFile.userId = userId;

                wpFile.filePath = wpFolder.filePath + wpFolder.fileName +  shareFile.shareFilePath;

                idDBContext.wpFile.Add(wpFile);

            }

            idDBContext.SaveChanges();

            return new JsonResult("ok");

        }

    }
}
