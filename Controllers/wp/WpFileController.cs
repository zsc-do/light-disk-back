using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using lightDiskBack.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using lightDiskBack.Vo;

namespace lightDiskBack.Controllers.wp
{

   
    public class WpFileController : Controller
    {


        private readonly IdDBContext idDBContext;
        public WpFileController(IdDBContext idDBContext)
        {
            this.idDBContext = idDBContext;

        }

        public ActionResult fileView()
        {
            return View();
        }


        [Authorize]
        public JsonResult addFolder([FromQuery(Name = "pid")] String pid,
                                    [FromQuery(Name = "folderName")] String folderName)
        {

            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);

           

            WpFile pFolder = idDBContext.wpFile.Where(a => a.isFolder == "1" && a.fileId == int.Parse(pid)).Single();
            var folderList = idDBContext.wpFile.Where(a => a.filePath == pFolder.filePath + pFolder.fileName + "/");

            foreach(WpFile folder1 in folderList)
            {
                if (folderName.Equals(folder1.fileName))
                {
                    return new JsonResult("文件夹名已存在");
                }
            }


            WpFile folder = new WpFile();
            folder.fileName = folderName;
            folder.filePath = pFolder.filePath + pFolder.fileName + "/";
            folder.isFolder = "1";
            folder.delFlag = "0";
            folder.userId = userId;
            folder.diskFileId = 0;


            idDBContext.wpFile.Add(folder);
            idDBContext.SaveChanges();



            return new JsonResult(folder);

        }



        [Authorize]
        public JsonResult showFolder()
        {


            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);


            List<WpFile> folders = idDBContext.wpFile.Where(a => a.isFolder == "1" && a.userId == userId).ToList();

            WpFile curFile = new WpFile();
            WpFile folderTree = getFolderTree(folders, curFile,"");


            return new JsonResult(folderTree);

        }


        private WpFile getFolderTree(List<WpFile> list, WpFile curFile, String filePath)
        {
           foreach(WpFile wpFile in list)
           {
                if (wpFile.filePath.Equals(filePath))
                {
                    WpFile fileIn = getFolderTree(list, wpFile,wpFile.filePath + wpFile.fileName + "/" );

                    curFile.ChildFolder.Add(fileIn);

                }

            }

            return curFile;
        }


        [Authorize]
        public JsonResult showFile([FromQuery(Name = "pid")] String pid)
        {

            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);


            
            WpFile pFolder = idDBContext.wpFile.Where(a => a.isFolder == "1" && a.fileId == int.Parse(pid)).Single();

            var files = idDBContext.wpFile.Where(a => a.isFolder == "0" 
            && a.filePath == pFolder.filePath + pFolder.fileName + "/"
            && a.userId == userId).ToList();
            return new JsonResult(files);
        }



        public JsonResult updateFolder(
           [FromQuery(Name = "newName")] String newName,
           [FromQuery(Name = "id")] String id)
        {


            var folder = idDBContext.wpFile.Where(a => a.fileId == int.Parse(id)).Single();
            folder.fileName = newName;


            idDBContext.SaveChanges();

            return new JsonResult("");
        }



        public JsonResult deleteFolder([FromQuery(Name = "id")] String id)
        {
            var folder = idDBContext.wpFile.Where(a => a.fileId == int.Parse(id)).Single();

            idDBContext.wpFile.Remove(folder);
            idDBContext.SaveChanges();

            return new JsonResult("");
        }


        [Authorize]
        public JsonResult judgeUserStorage([FromQuery(Name = "size")] String fileSize)
        {
            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);

            var storage = idDBContext.Storage.Where(a => a.userId == userId).Single();

            //判断用户空间是否充足
            long availableStorage = long.Parse(storage.simpleSize) + long.Parse(storage.memberSize);
            long useStorage = long.Parse(storage.storageSize) + long.Parse(fileSize);

            if (useStorage > availableStorage)
            {
                return new JsonResult("false");
            }

            return new JsonResult("true");
        }


        [Authorize]
        [DisableRequestSizeLimit]
        public JsonResult uploadFile([FromQuery(Name = "pid")] String pid, IFormFile file)
        {



            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);

            var storage = idDBContext.Storage.Single(a => a.userId == userId);

            //判断用户空间是否充足
            long availableStorage = long.Parse(storage.simpleSize) + long.Parse(storage.memberSize);
            long useStorage = long.Parse(storage.storageSize) + file.Length;

            if(useStorage > availableStorage)
            {
                return new JsonResult("403");
            }


            // 文件后缀
            var fileExtension = Path.GetExtension(file.FileName);
            Guid randomGuid = Guid.NewGuid();

            var saveName = randomGuid + fileExtension;
            var dataTime = DateTime.Now.ToString("yyyy-MM-dd");
            string dir = "D:\\wp\\File\\" + dataTime;
            string filePath = dir + "\\" + saveName;


            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);


            // 保存文件到服务器
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }


            //插入数据库
            DiskFile diskFile = new DiskFile();

            diskFile.diskFileSize = file.Length.ToString();
            diskFile.diskFileType = "1";
            diskFile.diskFileUrl = dataTime + "\\" + saveName;

            idDBContext.DiskFile.Add(diskFile);

            idDBContext.SaveChanges();


            WpFile wpFile = new WpFile();
            wpFile.fileName = file.FileName;

           
            var pFolder = idDBContext.wpFile.Where(a => a.fileId == int.Parse(pid)).Single();
            wpFile.filePath = pFolder.filePath + pFolder.fileName + "/";

            wpFile.isFolder = "0";
            wpFile.delFlag = "0";
            wpFile.userId = userId;
            wpFile.diskFileId = diskFile.diskFileId;

            idDBContext.wpFile.Add(wpFile);


            
            storage.storageSize = (long.Parse(storage.storageSize) + file.Length).ToString();

            idDBContext.SaveChanges();

            return new JsonResult(wpFile);
        }






        public FileResult downloadFile([FromQuery(Name = "id")] String id)
        {
            var wpFile = idDBContext.wpFile.Where(a => a.fileId == int.Parse(id)).Single();
            var diskFile = idDBContext.DiskFile.Where(a => a.diskFileId == wpFile.diskFileId).Single();



            String dir = "D:\\wp\\File\\";
            var stream = System.IO.File.OpenRead(dir + diskFile.diskFileUrl);  //创建文件流

            return File(stream, "application/x-sh", wpFile.fileName);
        }



        [Authorize]
        public JsonResult fileDetail([FromQuery(Name = "fileId")] String fileId)
        {

            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int userId = userId = int.Parse(userId1);

            WpFile wpFile = idDBContext.wpFile.Single(a => a.fileId == int.Parse(fileId) && a.userId == userId);


            DiskFile diskFile = idDBContext.DiskFile.Single(a => a.diskFileId == wpFile.diskFileId);


            WpFileVo fileVo = new WpFileVo();
            fileVo.fileName = wpFile.fileName;
            fileVo.fileSize = diskFile.diskFileSize;


            return new JsonResult(fileVo);
        }

    }
}
