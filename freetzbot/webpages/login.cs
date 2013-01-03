﻿using System;
using FritzBot;
using FritzBot.Core;

namespace webpages
{
    class login : IWebInterface
    {
        public string Url { get { return "/login"; } }

        public HtmlResponse GenPage(HtmlRequest request)
        {
            HtmlResponse theresponse = new HtmlResponse();
            bool LoginSuccesfull = false;
            theresponse.page += "<!DOCTYPE html><html><body>";
            if (request.postdata.Count > 0)
            {
                string name = request.postdata["name"];
                string passwort = request.postdata["pw"];
                if (UserManager.GetInstance().Exists(name))
                {
                    if (UserManager.GetInstance()[name].CheckPassword(passwort))
                    {
                        UserManager.GetInstance()[name].GetModulUserStorage("login").SetVariable("authcookiedate", DateTime.Now);
                        string hash = toolbox.Crypt(name + UserManager.GetInstance()[name].GetModulUserStorage("login").GetVariable("authcookiedate") + request.useradress.ToString());
                        theresponse.cookies["username"] = name;
                        theresponse.cookies["logindata"] = hash;
                        request.cookies.Add(new System.Net.Cookie("username", name));
                        request.cookies.Add(new System.Net.Cookie("logindata", hash));
                        LoginSuccesfull = true;
                    }
                }
                theresponse.page += index.GenMenu(request);
                if (LoginSuccesfull)
                {
                    theresponse.page += "Du bist nun eingeloggt " + name;
                }
                else
                {
                    theresponse.page += "Entweder der angegebene Benutzer konnte nicht gefunden werden oder dein Passwort ist falsch";
                }
            }
            else
            {
                theresponse.page += index.GenMenu(request);
                theresponse.page += "<div style=\"position: absolute;top: 35%;left: 35%;border:1px;border-style:dotted;padding:10px\">Bitte einloggen!<br>";
                theresponse.page += "<form action=\"login\" method=\"POST\"><table>";
                theresponse.page += "<tr><td>IRC-Nick:</td><td><input type=\"text\" name=\"name\"></td></tr>";
                theresponse.page += "<tr><td>Passwort:</td><td><input type=\"password\" name=\"pw\"></td></tr>";
                theresponse.page += "<tr><td><input type=\"submit\" value=\"Login\"></td></tr>";
                theresponse.page += "</table></form></div>";
            }
            theresponse.page += "</div></body></html>";
            theresponse.status_code = 200;
            return theresponse;
        }

        public static string CheckLogin(HtmlRequest request)
        {
            string name = "";
            string hash = "";
            if (request.cookies["username"] != null && request.cookies["logindata"] != null)
            {
                name = request.cookies["username"].Value;
                hash = request.cookies["logindata"].Value;
            }
            if (UserManager.GetInstance().Exists(name))
            {
                string calchash = toolbox.Crypt(name + UserManager.GetInstance()[name].GetModulUserStorage("login").GetVariable("authcookiedate") + request.useradress.ToString());
                if (calchash == hash)
                {
                    return name;
                }
            }
            return "";
        }
    }
}