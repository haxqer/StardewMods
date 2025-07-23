using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace modtools;

/// <summary>
/// 时间处理器 / Time handler
/// </summary>
public class TimeHandler
{
    private readonly ModConfig config;
    private int timeSkipCounter = 0;

    public TimeHandler(ModConfig config)
    {
        this.config = config;
    }

    /// <summary>
    /// 处理时间变化 / Handle time changes
    /// </summary>
    public void OnTimeChanged(TimeChangedEventArgs e)
    {
        if (config.TimeRateMultiplier >= 0.99f)
        {
            timeSkipCounter = 0;
            return; // 正常速度 / normal speed
        }
        
        // 只允许每N次事件推进时间 / Only allow time to advance every Nth event
        int skip = (int)Math.Round(1.0 / config.TimeRateMultiplier);
        timeSkipCounter++;
        if (timeSkipCounter < skip)
        {
            // 回退时间 / Revert time
            Game1.timeOfDay = e.OldTime;
        }
        else
        {
            timeSkipCounter = 0;
        }
    }
} 