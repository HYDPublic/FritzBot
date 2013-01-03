﻿using System;
using FritzBot;
using FritzBot.Core;
using System.Linq;

namespace webpages
{
    class helpdb : IWebInterface
    {
        public string Url { get { return "/helpdb"; } }

        public HtmlResponse GenPage(HtmlRequest request)
        {
            HtmlResponse theresponse = new HtmlResponse();
            string logincheck = login.CheckLogin(request);
            theresponse.page += "<!DOCTYPE html><html><body>";
            theresponse.page += index.GenMenu(request);
            theresponse.page += "<table border=2px>";
            theresponse.page += "<tr><td><b>Befehl</b></td><td><b>Beschreibung</b></td></tr>";
            foreach (ICommand theCommand in PluginManager.GetInstance().Get<ICommand>().Where(x => toolbox.GetAttribute<FritzBot.Module.NameAttribute>(x) != null))
            {
                bool OPNeeded = toolbox.GetAttribute<FritzBot.Module.AuthorizeAttribute>(theCommand) != null;
                if (toolbox.IsOp(logincheck) || !OPNeeded)
                {
                    string names = "";
                    foreach (string name in toolbox.GetAttribute<FritzBot.Module.NameAttribute>(theCommand).Names)
                    {
                        names += ", " + name;
                    }
                    names = names.Remove(0, 2);
                    theresponse.page += "<tr><td>" + names + "</td><td>" + toolbox.GetAttribute<FritzBot.Module.HelpAttribute>(theCommand).Help + "</td></tr>";
                }
            }
            theresponse.page += "</table>";
            theresponse.page += "</body></html>";
            theresponse.status_code = 200;
            return theresponse;
        }
    }
}