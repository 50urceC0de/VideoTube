using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using System.IO;
using System.Diagnostics;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
    public class VideoProcessor
    {

        private IConnectionConfiguration con;
        private long sizeLimit = 500000000;
        private int scopeId;
        private string[] allowedTypes = new string[] { "mp4", "flv", "webm", "mkv", "vob", "ogv", "ogg", "avi", "wmv", "mov", "mpeg", "mpg" };

        // *** UNCOMMENT ONE OF THESE DEPENDING ON YOUR COMPUTER ***
        private string ffmpegPath = "ffmpeg/mac/regular-xampp/ffmpeg"; // *** MAC (USING REGULAR XAMPP) ***
                                                                       //private ffmpegPath = "ffmpeg/mac/xampp-VM/ffmpeg"; // *** MAC (USING XAMPP VM) ***
                                                                       // private ffmpegPath = "ffmpeg/linux/ffmpeg"; // *** LINUX ***
                                                                       // private ffmpegPath; //  *** WINDOWS (UNCOMMENT CODE IN CONSTRUCTOR) ***

        // *** ALSO UNCOMMENT ONE OF THESE DEPENDING ON YOUR COMPUTER ***
        private string ffprobePath = "ffmpeg/mac/regular-xampp/ffprobe"; // *** MAC (USING REGULAR XAMPP) ***
                                                                         //private ffprobePath = "ffmpeg/mac/xampp-VM/ffprobe"; // *** MAC (USING XAMPP VM) ***
                                                                         // private ffprobePath = "ffmpeg/linux/ffprobe"; // *** LINUX ***
                                                                         // private ffprobePath; //  *** WINDOWS (UNCOMMENT CODE IN CONSTRUCTOR) ***

        public VideoProcessor(IConnectionConfiguration con)
        {
            this.con = con;

            // *** UNCOMMENT IF USING WINDOWS *** 
            this.ffmpegPath = System.Web.HttpContext.Current.Server.MapPath("ffmpeg/windows/ffmpeg.exe");
            this.ffprobePath = System.Web.HttpContext.Current.Server.MapPath("ffmpeg/windows/ffprobe.exe");
        }
        public string uniqid()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double t = ts.TotalMilliseconds / 1000;
            int a = (int)Math.Floor(t);
            int b = (int)((t - Math.Floor(t)) * 1000000);
            return a.ToString("x8") + b.ToString("x5");
        }
        public async Task<bool> upload(VideoUploadData videoUploadData)
        {

            string targetDir = "uploads/videos/";
            var videoData = videoUploadData.videoDataArray;

            string tempFilePath = videoData["filepath"].ToString();

            string isValidData = this.processData(videoData, tempFilePath);

            if (isValidData != "Successfull")
            {
                return false;
            }
            //Directory.Move(videoData["tmp_name"].ToString(), tempFilePath);
            //{
            var finalFilePath = targetDir + videoData["name"].ToString()+".mp4";

            if (!await this.insertVideoData(videoUploadData, finalFilePath))
            {
                return false;
            }

            if (!this.convertVideoToMp4(tempFilePath, finalFilePath))
            {
                return false;
            }

            if (!this.deleteFile(tempFilePath))
            {
                return false;
            }

            if (!await this.generateThumbnails(finalFilePath))
            {
                return false;
            }

            return true;

            //}
        }

        private string processData(Dictionary<string, string> videoData, string filePath)
        {
            var videoType = Path.GetExtension(filePath).Replace(".", "");

            if (!this.isValidSize(videoData))
            {
                return "File too large. Can't be more than " + this.sizeLimit + " bytes";
            }
            else if (!this.isValidType(videoType))
            {
                return "Invalid file type";
            }


            return "Successfull";
        }

        private bool isValidSize(Dictionary<string, string> data)
        {
            return (Convert.ToDouble(data["size"])) <= this.sizeLimit;
        }

        private bool isValidType(string type)
        {
            var lowercased = type.ToLower();
            return Array.Exists(this.allowedTypes, a => a == lowercased);
        }

        private bool hasError(Dictionary<string, string> data)
        {
            return (Convert.ToInt32(data["error"])) != 0;
        }

        private async Task<bool> insertVideoData(VideoUploadData uploadData, string filePath)
        {
            string query = ("set dateformat dmy;INSERT INTO videos(title, uploadedBy, description, privacy, category, filePath,uploadDate) VALUES('" + uploadData.title + "', '" + uploadData.uploadedBy + "', '" + uploadData.description + "', '" + uploadData.privacy + "', '" + uploadData.category + "', '" + filePath + "',GetDate());select SCOPE_IDENTITY();");

            using (var conn = new SqlConnection(con.Value))
            {

                var a = await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                scopeId = Convert.ToInt32(a);

                return scopeId > 0;
            }

        }

        public bool convertVideoToMp4(string tempFilePath, string finalFilePath)
        {

            finalFilePath = System.Web.HttpContext.Current.Server.MapPath(finalFilePath);

            //var args = String.Format("-i \"{0}\" \"{1}\"", tempFilePath, finalFilePath);
            //var args = String.Format("-i \"{0}\" \"{1}\" 2>&1", tempFilePath, finalFilePath);

            //-i "D:\_EDUONIX\VideoTube\VideoTube\uploads\Temp\69a8-3d49-42f0-b33c-ac0bfrag_bunny.mp4" -movflags frag_keyframe+empty_moov+default_base_moof "D:\_EDUONIX\VideoTube\VideoTube\uploads\Temp\out1.mp4"
            var args = String.Format("-i \"{0}\" -movflags frag_keyframe+empty_moov+default_base_moof \"{1}\"", tempFilePath, finalFilePath);

            try
            {
               var outputLog=Execute(this.ffmpegPath, args);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        private static string Execute(string exePath, string parameters)
        {
            string result = String.Empty;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = parameters;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                while (!p.HasExited)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                p.WaitForExit();

                result = p.StandardError.ReadToEnd();
            }

            return result;
        }

        private bool deleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> generateThumbnails(string filePath)
        {
            filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
            var thumbnailSize = "210x118";
            int numThumbnails = 3;
            string pathToThumbnail = "uploads/videos/thumbnails";

            double duration = this.getVideoDuration(filePath);

            int videoId = scopeId;
            this.updateDuration(duration, videoId);

            for (int num = 1; num <= numThumbnails; num++)
            {
                string imageName = uniqid() + ".jpg";
                double interval = (duration * 0.8) / numThumbnails * num;
                string fullThumbnailPath =( $"{pathToThumbnail}/{videoId}-{imageName}");
                string fullThumbnailPathWithServer =System.Web.HttpContext.Current.Server.MapPath(fullThumbnailPath);

                // string cmd =  String.Format( "-i {0} -ss {1} -s {2} -vframes 1 {3} 2>&1", filePath, interval, thumbnailSize,fullThumbnailPath);
                string cmd =  String.Format("-i {0} -ss {1} -s {2} -vframes 1 {3}", filePath, interval, thumbnailSize, fullThumbnailPathWithServer);

                var outputLog = Execute(this.ffmpegPath, cmd);
                //exec(cmd, outputLog, returnCode);

                //using (Process process = new Process())
                //{
                //    process.StartInfo.FileName = this.ffmpegPath;
                //    process.StartInfo.UseShellExecute = false;
                //    process.StartInfo.RedirectStandardOutput = true;
                //    process.StartInfo.RedirectStandardError = true;

                //    process.StartInfo.Arguments = cmd;


                //    process.Start();

                //    StreamReader reader = process.StandardOutput;
                //    var outputLog = reader.ReadToEnd();
                //    process.WaitForExit(); 
                //}
                int success = 0;
                int selected = num == 1 ? 1 : 0;
                string query = ("INSERT INTO thumbnails(videoId, filePath, selected)  VALUES(" + videoId + ", '" + fullThumbnailPath + "', " + selected + ")");
                using (var conn = new SqlConnection(con.Value))
                {
                    success = await conn.ExecuteAsync(query, commandType: CommandType.Text);
                }
                if (success == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private double getVideoDuration(string filePath)
        {
            var cmd =   " -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 " + filePath;
            var outputLog="";
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = this.ffprobePath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.Arguments = cmd;

                    process.Start();

                    while (!process.HasExited)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    StreamReader reader = process.StandardError;
                     outputLog = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                }
            }
            catch (Exception)
            {

            }
            return Convert.ToDouble(outputLog.Trim());
            // return (int)shell_exec();
        }

        private async void updateDuration(double duration, int videoId)
        {

            double hours = Convert.ToDouble(duration / 3600);
            double mins = Convert.ToDouble((duration - (hours * 3600)) / 60);
            double secs = Convert.ToDouble(duration % 60);

            string _hours = (hours < 1) ? "" : hours + ":";
            string _mins = (mins < 10) ? "0" + mins + ":" : mins + ":";
            string _secs = (secs < 10) ? "0" + secs : secs.ToString();

            string _duration = _hours + _mins + _secs;

            string query = ("UPDATE videos SET duration='" + _duration + "' WHERE id=" + videoId);
            using (var conn = new SqlConnection(con.Value))
            {
                await conn.ExecuteAsync(query, commandType: CommandType.Text);
            }
        }
    }
}