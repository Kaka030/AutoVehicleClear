using JetBrains.Annotations;
using Rocket.API;

namespace kaka.AutoVehicleClear
{
    public class AutoVehicleClearConfiguration : IRocketPluginConfiguration
    {
        public bool AutoClear {  get; set; }
        public int ClearInterval { get; set; }
        public int WarningTime {  get; set; }   
        public bool ClearAll {  get; set; } 
        public bool ClearNoTires {  get; set; } 
        public bool ClearExploded {  get; set; }    
        public bool ClearDrowned {  get; set; } 
        public bool SendClearedMessage {  get; set; }   
        public bool SendWarningMessage {  get; set; }   

        public void LoadDefaults()
        {
            AutoClear = true;
            SendClearedMessage = true;//清理完发送消息
            SendWarningMessage = true;//清理前发送消息
            ClearAll = false;//清理所有车辆
            ClearNoTires = false;//清理没有轮胎的车辆
            ClearExploded = true;//清理爆炸的车辆
            ClearDrowned = true;//清理淹没的车辆

            ClearInterval = 30; //几秒清理一次服务器车辆
            WarningTime = 10; //清理车辆几秒前告知
        }
    }
}
