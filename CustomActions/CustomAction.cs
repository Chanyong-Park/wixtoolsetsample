using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CreateLogFile(Session session)
        {
            string userName = session["USERNAME"];
            string companyName = session["COMPANYNAME"];

            string path = @"d:\watch";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText($"{System.IO.Path.Combine(path, "log.txt")}", $"{userName}, {companyName}");

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeleteLogFile(Session session)
        {
            string path = @"d:\watch";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.Delete($"{System.IO.Path.Combine(path, "log.txt")}");

            return ActionResult.Success;
        }
    }
}
