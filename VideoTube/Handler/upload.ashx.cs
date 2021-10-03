using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace VideoTube.Handler
{
    /// <summary>
    /// Summary description for upload
    /// </summary>
    public class upload : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            UploadFiles(context);
        }
        public void UploadFiles(HttpContext context)
        {


            string _uploadFolder = "~/uploads/Temp";

            var queryString = context.Request.Form;

            if (queryString.Count == 0) return;


            // Read parameters

            var uploadToken = queryString.Get("upload_Token");
            //if(uploadToken==null || uploadToken=="")
            //    uploadToken = Guid.NewGuid().ToString().Replace("-", "");
            //context.Response.Write("upload_Token:"+uploadToken);

            int resumableChunkNumber = int.Parse(queryString.Get("resumableChunkNumber"));

            var resumableFilename = queryString.Get("resumableFilename");

            var resumableIdentifier = queryString.Get("resumableIdentifier");

            int resumableChunkSize = int.Parse(queryString.Get("resumableChunkSize"));

            long resumableTotalSize = long.Parse(queryString.Get("resumableTotalSize"));

            var filePath = string.Format("{0}/{1}/{1}.part{2}", _uploadFolder, uploadToken, resumableChunkNumber.ToString("0000"));

            var localFilePath = HttpContext.Current.Server.MapPath(filePath);

            var directory = System.IO.Path.GetDirectoryName(localFilePath);

            if (!System.IO.Directory.Exists(localFilePath))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            if (context.Request.Files.Count == 1)
            {
                // save chunk
                if (!System.IO.File.Exists(localFilePath))
                {

                    context.Request.Files[0].SaveAs(localFilePath);
                }

                // Check if all chunks are ready and save file
                var files = System.IO.Directory.GetFiles(directory);

                if ((files.Length + 1) * (long)resumableChunkSize >= resumableTotalSize)
                {
                    filePath = string.Format("{0}/{1}{2}", _uploadFolder, uploadToken, resumableFilename);

                    localFilePath = HttpContext.Current.Server.MapPath(filePath);

                    using (var fs = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        foreach (string file in files.OrderBy(x => x))
                        {
                            var buffer = System.IO.File.ReadAllBytes(file);

                            fs.Write(buffer, 0, buffer.Length);

                            System.IO.File.Delete(file);
                        }
                        fs.Close();
                        System.IO.Directory.Delete(directory);
                    }
                    BindData(resumableTotalSize, resumableFilename, localFilePath,(uploadToken+ resumableFilename));
                }
            }
            else
            {
                // log error
                context.Response.Write("Something Went Wrong");
            }

        }
        private byte[] ReadData(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
        private void BindData(long resumableTotalSize, string resumableFilename, string localFilePath,string fileName) => System.Web.HttpContext.Current.Session["UploadData"] = new Dictionary<string, string>()
            {
                { "name" , Path.GetFileName(fileName).Split('.')[0] },
                { "tmp_name" , resumableFilename },
                { "extension" , Path.GetExtension(fileName) },
                { "size",resumableTotalSize.ToString()},
                { "filepath",localFilePath}
            };

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}