using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;

namespace VideoTube.Data
{
   public class SettingsFormProvider {

    public string createUserDetailsForm(string firstName,string lastName,string email) {
        string firstNameInput = this.createFirstNameInput(firstName);
        string lastNameInput = this.createLastNameInput(lastName);
        string emailInput = this.createEmailInput(email);
        string saveButton = this.createSaveUserDetailsButton();

        return "<form action='settings' method='POST' enctype='multipart/form-data'><span class='title'>User details</span>"+ firstNameInput+ lastNameInput+ emailInput+ saveButton +"</form>";
    }

    public string createPasswordForm() {
        string oldPasswordInput = this.createPasswordInput("oldPassword", "Old password");
        string newPassword1Input = this.createPasswordInput("newPassword", "New password");
        string newPassword2Input = this.createPasswordInput("newPassword2", "Confirm new password");

        string saveButton = this.createSavePasswordButton();

        return "<form action='settings' method='POST' enctype='multipart/form-data'> <span class='title'>Update password</span>"+oldPasswordInput+newPassword1Input+newPassword2Input+saveButton+"</form>";
    }

    private string createFirstNameInput(string value) {
        if(value == null) value = "";
        return "<div class='form-group'> <input class='form-control' type='text' placeholder='First name' name='firstName' value='"+value+"' required></div>";
    }

    private string createLastNameInput(string value) {
        if(value == null) value = "";
        return "<div class='form-group'><input class='form-control' type='text' placeholder='Last name' name='lastName' value='"+value+"' required> </div>";
    }

    private string createEmailInput(string value) {
        if(value == null) value = "";
        return "<div class='form-group'> <input class='form-control' type='email' placeholder='Email' name='email' value='"+value+"' required></div>";
    }

    private string createSaveUserDetailsButton() {
        return "<button type='submit' class='btn btn-primary' name='saveDetailsButton'>Save</button>";
    }

    private string createSavePasswordButton() {
        return "<button type='submit' class='btn btn-primary' name='savePasswordButton'>Save</button>";
    }

    private string createPasswordInput(string name,string placeholder) {
        
        return "<div class='form-group'> <input class='form-control' type='password' placeholder='"+placeholder+"' name='"+name+"' required> </div>";
    }
}
}