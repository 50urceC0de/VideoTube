using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Data.Services;
using VideoTube.Models;

namespace VideoTube.Data
{
   public class VideoDetailsFormProvider {

    private IConnectionConfiguration con;

    public VideoDetailsFormProvider(IConnectionConfiguration con) {
        this.con = con;
    }

    public async Task<string> createUploadForm() {
        string fileInput = this.createFileInput();
        string titleInput = this.createTitleInput(null);
        string descriptionInput = this.createDescriptionInput(null);
        string privacyInput = this.createPrivacyInput(null);
        string categoriesInput = await this.createCategoriesInput(null);
        string uploadButton = this.createUploadButton();
        return "<form action='processing' method='POST' enctype='multipart/form-data'>"+ fileInput+  titleInput+ descriptionInput+ privacyInput+categoriesInput+uploadButton+"</form>";
    }

    public async Task<string> createEditDetailsForm(Video video) {
       string titleInput = this.createTitleInput(video._video.title);
       string descriptionInput = this.createDescriptionInput(video._video.description);
       string privacyInput = this.createPrivacyInput(video._video.privacy);
       string categoriesInput =await this.createCategoriesInput(video._video.category);
       string saveButton = this.createSaveButton();
        return "<form method='POST'>"+ titleInput+ descriptionInput+ privacyInput+ categoriesInput+ saveButton+"</form>";
    }

    private string createFileInput() {

        //return "<div class='form-group'> <label for='exampleFormControlFile1'>Your file</label>  <input type='file' class='form-control-file' id='exampleFormControlFile1' name='fileInput' required> </div>";
            return "<div class='form-group'> <label for='exampleFormControlFile1'>Your file</label><div class='resumable-drop' ondragenter='jQuery(this).addClass(\"resumable-dragover\"); ' ondragend='jQuery(this).removeClass(\"resumable-dragover\"); ' ondrop='jQuery(this).removeClass(\"resumable-dragover\"); '> Drop video files here to upload or <a class='resumable-browse'><u>select from your computer</u></a> </div> <div class='resumable-progress'> <table> <tr> <td style='width:100%'><div class='progress-container'><div class='progress-bar'></div></div></td> <td class='progress-text' nowrap='nowrap'></td> <td class='progress-pause' nowrap='nowrap'><a href='#' onclick='r.upload(); return(false);' class='progress-resume-link'><img src='../assets/images/icons/resume.png' title='Resume upload' /></a> <img src='' /> <a href='#' onclick='r.pause(); return(false);' class='progress-pause-link'><img src='../assets/images/icons/pause.png' title='Pause upload' /></a> </td> </tr> </table> </div> <ul class='resumable-list'></ul></div>";
    }

    private string createTitleInput(string value) {
        if(value == null) value = "";
        return "<div class='form-group'> <input class='form-control' type='text' placeholder='Title' name='titleInput' value='"+value+"'>  </div>";
    }

    private string createDescriptionInput(string value) {
        if(value == null) value = "";
        return "<div class='form-group'>  <textarea class='form-control' placeholder='Description' name='descriptionInput' rows='3'>"+value+"</textarea>  </div>";
    }

    private string createPrivacyInput(string value) {
        if(value == null) value = "";

        string privateSelected = (value == "0") ? "selected='selected'" : "";
        string publicSelected = (value == "1") ? "selected='selected'" : "";
        return "<div class='form-group'> <select class='form-control' name='privacyInput'> <option value='0' "+privateSelected+">Private</option>  <option value='1' "+publicSelected+">Public</option> </select> </div>";
    }

    private async Task<string> createCategoriesInput(string value) {
        if(value == null) value = "";
        string query = ("SELECT * FROM categories");
            IEnumerable<Categories> category;
            using (var conn = new SqlConnection(con.Value))
            {
                category = await conn.QueryAsync<Categories>("SELECT * FROM categories", commandType: CommandType.Text);
            }

           string html = "<div class='form-group'> <select class='form-control' name='categoryInput'>";
            foreach (var item in category)
            {
                string selected = (item.id.ToString() == value) ? "selected='selected'" : "";

                html+= "<option "+selected+" value='"+item.id+"'>"+item.name+"</option>";
            }
        
        
        html += "</select></div>";

        return html;

    }

    private string createUploadButton() {
        return "<button type='submit' class='btn btn-primary' name='uploadButton'>Upload</button>";
    }

    private string createSaveButton() {
        return "<button type='submit' class='btn btn-primary' name='saveButton'>Save</button>";
    }
}
}