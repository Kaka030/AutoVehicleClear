using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kaka.AutoVehicleClear.Commands
{
    internal class Command : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "clearvehicle";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> {"cv"};

        public List<string> Permissions => new List<string> {};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player;

            if (caller is ConsolePlayer)
            {
                AutoVehicleClear.Instance.ClearVehicles();
                return;
            }

            player = (UnturnedPlayer)caller;

            if (!player.HasPermission($"clearvehicle"))
            {
                UnturnedChat.Say(caller, AutoVehicleClear.Instance.Translate("NoPermission"));
            }
        }
    }
}
