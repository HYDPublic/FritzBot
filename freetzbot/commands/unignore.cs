﻿using System;

namespace FritzBot.commands
{
    class unignore : ICommand
    {
        public String[] Name { get { return new String[] { "unignore" }; } }
        public String HelpText { get { return "Die betroffene Person wird von der ignore Liste gestrichen, Operator Befehl: z.b. !unignore Testnick"; } }
        public Boolean OpNeeded { get { return true; } }
        public Boolean ParameterNeeded { get { return true; } }
        public Boolean AcceptEveryParam { get { return false; } }

        public void Destruct()
        {

        }

        public void Run(ircMessage theMessage)
        {
            theMessage.TheUsers[theMessage.CommandArgs[0]].ignored = false;
        }
    }
}