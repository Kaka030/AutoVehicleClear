using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;

namespace kaka.AutoVehicleClear
{
    public class AutoVehicleClear : RocketPlugin<AutoVehicleClearConfiguration>
    {
        public static AutoVehicleClear Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} 已加载!", ConsoleColor.Yellow);
            InvokeRepeating("SendWarningMessage", Configuration.Instance.ClearInterval, Configuration.Instance.ClearInterval);
        }

        protected override void Unload()
        {
            CancelInvoke("ClearVehicles");
            CancelInvoke("SendWarningMessage");
        }

        private void SendWarningMessage()
        {
            if (Configuration.Instance.AutoClear)
            {
                if (Configuration.Instance.SendWarningMessage)
                {
                    UnturnedChat.Say(Translate("AutoVehicleWarning", Configuration.Instance.WarningTime), Color.yellow);
                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        ClearVehicles();
                    }, Configuration.Instance.WarningTime);
                }
                ClearVehicles();
            }
        }


        public void ClearVehicles()
        {
            Rocket.Core.Logging.Logger.Log("清理车辆中...");

            int cleared = 0;

            List<InteractableVehicle> vehicles = VehicleManager.vehicles;

            for (int i = vehicles.Count - 1; i >= 0; i--)
            {
                InteractableVehicle interactablevehicle = vehicles[i];
                if (CanClearVehicle(interactablevehicle))
                {
                    VehicleManager.instance.channel.send("tellVehicleDestroy", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, interactablevehicle.instanceID); // 清理车
                    cleared++;
                }
            }

            Rocket.Core.Logging.Logger.Log($"清理了{cleared}辆车", ConsoleColor.White);

            if (Configuration.Instance.SendClearedMessage)
            {
                UnturnedChat.Say(Translate("AutoVehicleCleared", cleared), Color.red);
            }
        }

        private bool CanClearVehicle(InteractableVehicle vehicle)
        {
            if (vehicle.passengers.Any((Passenger p) => p.player != null) || vehicle.asset.engine == EEngine.TRAIN)
            {
                return false;
            }
            if (Configuration.Instance.ClearAll)
            {
                return true;
            }
            if (Configuration.Instance.ClearExploded && vehicle.isExploded)
            {
                return true;
            }
            if (Configuration.Instance.ClearDrowned && vehicle.isDrowned && vehicle.transform.FindChild("Buoyancy") == null)
            {
                return true;
            }
            if (Configuration.Instance.ClearNoTires)
            {
                var tires = vehicle.transform.FindChild("Tires");
                if (tires != null)
                {
                    int AllTires = vehicle.transform.FindChild("Tires").childCount;
                    if (AllTires == 0)
                    {
                        return false;
                    }

                    int LeftTires = 0;
                    for (int i = 0; i < AllTires; i++)
                    {
                        if (tires.GetChild(i).gameObject.activeSelf)
                        {
                            LeftTires++;
                        }
                    }
                    if (LeftTires == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"AutoVehicleCleared", "已清理 {0} 辆车!"},
            {"AutoVehicleWarning", "将在{0}秒后清理车辆!"},
            {"NoPermission","你没有此权限!" },
            {"VehicleClearSuccess" ,"成功清理地图车辆"}
        };
    }
}