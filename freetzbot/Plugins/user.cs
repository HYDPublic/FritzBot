using FritzBot.Core;
using FritzBot.Database;
using FritzBot.DataModel;
using System;
using System.Linq;

namespace FritzBot.Plugins
{
    [Name("user")]
    [Help("Führt Operationen an meiner Benutzerdatenbank aus, Operator Befehl: !user remove, reload, flush, add <name>, box <name> <box>")]
    [ParameterRequired]
    [Authorize]
    class user : PluginBase, ICommand
    {
        public void Run(ircMessage theMessage)
        {
            try
            {
                if (theMessage.CommandArgs[0] == "box")
                {
                    using (var context = new BotContext())
                    {
                        User u = context.GetUser(theMessage.CommandArgs[1]);
                        if (u != null)
                        {
                            BoxManager mgr = new BoxManager(u, context);
                            string boxtoadd = String.Join(" ", theMessage.CommandArgs.Skip(2));
                            if (mgr.HasBox(boxtoadd))
                            {
                                theMessage.Answer("Diese Box gibt es für diesen User bereits!");
                                return;
                            }
                            mgr.AddBox(boxtoadd);
                            theMessage.Answer("Hinzugefügt");
                            return;
                        }
                        theMessage.Answer("User gibbet nicht");
                    }
                }
                if (theMessage.CommandArgs[0] == "boxremove")
                {
                    using (var context = new BotContext())
                    {
                        User u = context.GetUser(theMessage.CommandArgs[1]);
                        if (u != null)
                        {
                            BoxManager mgr = new BoxManager(u, context);
                            if (mgr.RemoveBox(String.Join(" ", theMessage.CommandArgs.Skip(2))))
                            {
                                theMessage.Answer("Erledigt!");
                                return;
                            }
                            theMessage.Answer("Der Suchstring wurde nicht gefunden und deshalb nicht gelöscht");
                            return;
                        }
                        theMessage.Answer("User gibbet nicht");
                    }
                }
            }
            catch (Exception ex)
            {
                toolbox.Logging("Bei einer Datenbank Operation ist eine Exception aufgetreten: " + ex.Message);
                theMessage.Answer("Wups, das hat eine Exception verursacht");
            }
        }
    }
}