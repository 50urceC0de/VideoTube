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
    public class FormSanitizer {

        public static string sanitizeFormString(string inputText) {
        inputText = strip_tags(inputText);
        inputText = str_replace(" ", "", inputText);
        inputText = strtolower(inputText);
        inputText = ucfirst(inputText);
            return inputText;
        }

        public static string sanitizeFormUsername(string inputText) {
        inputText = strip_tags(inputText);
        inputText = str_replace(" ", "", inputText);
            return inputText;
        }

        public static string sanitizeFormPassword(string inputText) {
        inputText = strip_tags(inputText);
            return inputText;
        }

        public static string sanitizeFormEmail(string inputText) {
        inputText = strip_tags(inputText);
        inputText = str_replace(" ", "", inputText);
            return inputText;
        }

    }
}