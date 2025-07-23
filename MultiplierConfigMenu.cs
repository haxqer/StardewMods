using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace modtools;

/// <summary>
/// 自定义Multiplier配置菜单 / Custom Multiplier configuration menu
/// </summary>
public class MultiplierConfigMenu : IClickableMenu
{
    private ModConfig config;
    private Action saveAction;
    private IModHelper helper;
    
    // Resource multiplier buttons
    private ClickableComponent plusButton;
    private ClickableComponent minusButton;
    
    // Fish difficulty buttons
    private ClickableComponent plusFishButton;
    private ClickableComponent minusFishButton;
    private ClickableComponent plusFishAddButton;
    private ClickableComponent minusFishAddButton;
    private ClickableComponent plusBarHeightButton;
    private ClickableComponent minusBarHeightButton;
    private ClickableComponent plusFishSpeedButton;
    private ClickableComponent minusFishSpeedButton;
    
    // Time rate buttons
    private ClickableComponent plusTimeButton;
    private ClickableComponent minusTimeButton;
    
    // Toggle buttons
    private ClickableComponent easierFishingButton;
    private ClickableComponent largerBarButton;
    private ClickableComponent slowerFishButton;
    private ClickableComponent alwaysPerfectButton;
    private ClickableComponent alwaysTreasureButton;
    private ClickableComponent instantCatchButton;
    private ClickableComponent instantTreasureButton;
    private ClickableComponent infiniteBaitButton;
    private ClickableComponent infiniteTackleButton;
    private ClickableComponent autoHookButton;

    // Layout constants
    private const int WindowWidth = 1200;
    private const int WindowHeight = 800;
    private const int SectionSpacing = 100;
    private const int ButtonSize = 60;
    private const int ToggleButtonWidth = 200;
    private const int ToggleButtonHeight = 40;

    public MultiplierConfigMenu(ModConfig config, Action saveAction, IModHelper helper)
        : base(Game1.viewport.Width / 2 - WindowWidth / 2, Game1.viewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
        this.config = config;
        this.saveAction = saveAction;
        this.helper = helper;
        
        // Initialize all buttons with placeholder positions
        // They will be positioned properly in the draw method
        plusButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "Plus");
        minusButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "Minus");
        plusFishButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFish");
        minusFishButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFish");
        plusFishAddButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFishAdd");
        minusFishAddButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFishAdd");
        plusBarHeightButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusBarHeight");
        minusBarHeightButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusBarHeight");
        plusFishSpeedButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFishSpeed");
        minusFishSpeedButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFishSpeed");
        plusTimeButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusTime");
        minusTimeButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusTime");
        
        // Toggle buttons
        easierFishingButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "EasierFishing");
        largerBarButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "LargerBar");
        slowerFishButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "SlowerFish");
        alwaysPerfectButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AlwaysPerfect");
        alwaysTreasureButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AlwaysTreasure");
        instantCatchButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InstantCatch");
        instantTreasureButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InstantTreasure");
        infiniteBaitButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InfiniteBait");
        infiniteTackleButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InfiniteTackle");
        autoHookButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AutoHook");
    }

    public override void draw(SpriteBatch b)
    {
        // Draw background window
        IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Title
        string title = helper.Translation.Get("config.title");
        Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
        b.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - titleSize.X / 2, yPositionOnScreen + 20), Color.Black);

        // Layout constants
        int startY = yPositionOnScreen + 80;
        int leftColumnX = xPositionOnScreen + 50;
        int rightColumnX = xPositionOnScreen + width / 2 + 50;
        int labelX = leftColumnX;
        int minusX = labelX + 250;
        int valueX = minusX + ButtonSize + 20;
        int plusX = valueX + 80;
        int toggleX = rightColumnX;
        int toggleY = startY;

        // Section 1: Resource Multiplier
        string resLabel = helper.Translation.Get("config.resource-multiplier");
        b.DrawString(Game1.smallFont, resLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusButton, "-");
        string resValue = config.Multiplier.ToString();
        Vector2 resValueSize = Game1.smallFont.MeasureString(resValue);
        b.DrawString(Game1.smallFont, resValue, new Vector2(valueX + 30 - resValueSize.X / 2, startY + 15), Color.DarkBlue);
        drawButton(b, plusButton, "+");

        // Section 2: Fish Difficulty Multiplier
        startY += 60;
        string fishLabel = helper.Translation.Get("config.fish-difficulty-multiplier");
        b.DrawString(Game1.smallFont, fishLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishButton, "-");
        string fishValue = $"x{config.FishDifficultyMultiplier:F2}";
        Vector2 fishValueSize = Game1.smallFont.MeasureString(fishValue);
        b.DrawString(Game1.smallFont, fishValue, new Vector2(valueX + 30 - fishValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishButton, "+");

        // Section 3: Fish Difficulty Additive
        startY += 60;
        string fishAddLabel = helper.Translation.Get("config.fish-difficulty-additive");
        b.DrawString(Game1.smallFont, fishAddLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishAddButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishAddButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishAddButton, "-");
        string fishAddValue = $"{config.FishDifficultyAdditive:F1}";
        Vector2 fishAddValueSize = Game1.smallFont.MeasureString(fishAddValue);
        b.DrawString(Game1.smallFont, fishAddValue, new Vector2(valueX + 30 - fishAddValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishAddButton, "+");

        // Section 4: Fishing Bar Height
        startY += 60;
        string barLabel = helper.Translation.Get("config.fishing-bar-height");
        b.DrawString(Game1.smallFont, barLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusBarHeightButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusBarHeightButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusBarHeightButton, "-");
        string barValue = config.FishingBarHeight.ToString();
        Vector2 barValueSize = Game1.smallFont.MeasureString(barValue);
        b.DrawString(Game1.smallFont, barValue, new Vector2(valueX + 30 - barValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusBarHeightButton, "+");

        // Section 5: Fish Movement Speed
        startY += 60;
        string speedLabel = helper.Translation.Get("config.fish-movement-speed");
        b.DrawString(Game1.smallFont, speedLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishSpeedButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishSpeedButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishSpeedButton, "-");
        string speedValue = $"x{config.FishMovementSpeedMultiplier:F2}";
        Vector2 speedValueSize = Game1.smallFont.MeasureString(speedValue);
        b.DrawString(Game1.smallFont, speedValue, new Vector2(valueX + 30 - speedValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishSpeedButton, "+");

        // Section 6: Time Rate Multiplier
        startY += 60;
        string timeLabel = helper.Translation.Get("config.time-rate-multiplier");
        b.DrawString(Game1.smallFont, timeLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusTimeButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusTimeButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusTimeButton, "-");
        string timeValue = $"x{config.TimeRateMultiplier:F2}";
        Vector2 timeValueSize = Game1.smallFont.MeasureString(timeValue);
        b.DrawString(Game1.smallFont, timeValue, new Vector2(valueX + 30 - timeValueSize.X / 2, startY + 15), Color.Purple);
        drawButton(b, plusTimeButton, "+");

        // Right column: Toggle options
        // Section 1: Difficulty toggles
        string difficultyTitle = helper.Translation.Get("config.difficulty-options");
        b.DrawString(Game1.smallFont, difficultyTitle, new Vector2(toggleX, toggleY), Color.Black);
        toggleY += 30;

        easierFishingButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, easierFishingButton, helper.Translation.Get("config.easier-fishing"), config.EasierFishing);
        toggleY += 50;

        largerBarButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, largerBarButton, helper.Translation.Get("config.larger-bar"), config.LargerFishingBar);
        toggleY += 50;

        slowerFishButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, slowerFishButton, helper.Translation.Get("config.slower-fish"), config.SlowerFishMovement);
        toggleY += 50;

        // Section 2: Convenience toggles
        toggleY += 20;
        string convenienceTitle = helper.Translation.Get("config.convenience-options");
        b.DrawString(Game1.smallFont, convenienceTitle, new Vector2(toggleX, toggleY), Color.Black);
        toggleY += 30;

        alwaysPerfectButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, alwaysPerfectButton, helper.Translation.Get("config.always-perfect"), config.AlwaysPerfect);
        toggleY += 50;

        alwaysTreasureButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, alwaysTreasureButton, helper.Translation.Get("config.always-treasure"), config.AlwaysFindTreasure);
        toggleY += 50;

        instantCatchButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, instantCatchButton, helper.Translation.Get("config.instant-catch"), config.InstantCatchFish);
        toggleY += 50;

        instantTreasureButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, instantTreasureButton, helper.Translation.Get("config.instant-treasure"), config.InstantCatchTreasure);
        toggleY += 50;

        infiniteBaitButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, infiniteBaitButton, helper.Translation.Get("config.infinite-bait"), config.InfiniteBait);
        toggleY += 50;

        infiniteTackleButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, infiniteTackleButton, helper.Translation.Get("config.infinite-tackle"), config.InfiniteTackle);
        toggleY += 50;

        autoHookButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, autoHookButton, helper.Translation.Get("config.auto-hook"), config.AutoHook);

        // Instructions
        string tip = helper.Translation.Get("config.close-tip");
        Vector2 tipSize = Game1.tinyFont.MeasureString(tip);
        b.DrawString(Game1.tinyFont, tip, new Vector2(xPositionOnScreen + width - tipSize.X - 32, yPositionOnScreen + height - tipSize.Y - 32), Color.Gray);

        // Draw mouse
        drawMouse(b);
    }

    private void drawButton(SpriteBatch b, ClickableComponent button, string text)
    {
        IClickableMenu.drawTextureBox(b, button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, Color.SandyBrown);
        Vector2 textSize = Game1.smallFont.MeasureString(text);
        b.DrawString(Game1.smallFont, text, new Vector2(button.bounds.X + button.bounds.Width / 2 - textSize.X / 2, button.bounds.Y + button.bounds.Height / 2 - textSize.Y / 2), Color.Black);
    }

    private void drawToggleButton(SpriteBatch b, ClickableComponent button, string text, bool isEnabled)
    {
        Color buttonColor = isEnabled ? Color.LightGreen : Color.LightGray;
        IClickableMenu.drawTextureBox(b, button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, buttonColor);
        Vector2 textSize = Game1.smallFont.MeasureString(text);
        b.DrawString(Game1.smallFont, text, new Vector2(button.bounds.X + button.bounds.Width / 2 - textSize.X / 2, button.bounds.Y + button.bounds.Height / 2 - textSize.Y / 2), Color.Black);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // Resource multiplier
        if (plusButton.containsPoint(x, y))
        {
            if (config.Multiplier < 100) // changed from 50 to 100
            {
                config.Multiplier++;
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusButton.containsPoint(x, y))
        {
            if (config.Multiplier > 3)
            {
                config.Multiplier--;
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish difficulty multiplier
        else if (plusFishButton.containsPoint(x, y))
        {
            if (config.FishDifficultyMultiplier < 2.0f)
            {
                config.FishDifficultyMultiplier = MathF.Min(2.0f, config.FishDifficultyMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishButton.containsPoint(x, y))
        {
            if (config.FishDifficultyMultiplier > 0.1f)
            {
                config.FishDifficultyMultiplier = MathF.Max(0.1f, config.FishDifficultyMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish difficulty additive
        else if (plusFishAddButton.containsPoint(x, y))
        {
            if (config.FishDifficultyAdditive < 50.0f)
            {
                config.FishDifficultyAdditive = MathF.Min(50.0f, config.FishDifficultyAdditive + 5.0f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishAddButton.containsPoint(x, y))
        {
            if (config.FishDifficultyAdditive > -50.0f)
            {
                config.FishDifficultyAdditive = MathF.Max(-50.0f, config.FishDifficultyAdditive - 5.0f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fishing bar height
        else if (plusBarHeightButton.containsPoint(x, y))
        {
            if (config.FishingBarHeight < 568)
            {
                config.FishingBarHeight = Math.Min(568, config.FishingBarHeight + 20);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusBarHeightButton.containsPoint(x, y))
        {
            if (config.FishingBarHeight > 60)
            {
                config.FishingBarHeight = Math.Max(60, config.FishingBarHeight - 20);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish movement speed
        else if (plusFishSpeedButton.containsPoint(x, y))
        {
            if (config.FishMovementSpeedMultiplier < 1.0f)
            {
                config.FishMovementSpeedMultiplier = MathF.Min(1.0f, config.FishMovementSpeedMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishSpeedButton.containsPoint(x, y))
        {
            if (config.FishMovementSpeedMultiplier > 0.1f)
            {
                config.FishMovementSpeedMultiplier = MathF.Max(0.1f, config.FishMovementSpeedMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Time rate multiplier
        else if (plusTimeButton.containsPoint(x, y))
        {
            if (config.TimeRateMultiplier < 2.0f)
            {
                config.TimeRateMultiplier = MathF.Min(2.0f, config.TimeRateMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusTimeButton.containsPoint(x, y))
        {
            if (config.TimeRateMultiplier > 0.1f)
            {
                config.TimeRateMultiplier = MathF.Max(0.1f, config.TimeRateMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Toggle buttons
        else if (easierFishingButton.containsPoint(x, y))
        {
            config.EasierFishing = !config.EasierFishing;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (largerBarButton.containsPoint(x, y))
        {
            config.LargerFishingBar = !config.LargerFishingBar;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (slowerFishButton.containsPoint(x, y))
        {
            config.SlowerFishMovement = !config.SlowerFishMovement;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (alwaysPerfectButton.containsPoint(x, y))
        {
            config.AlwaysPerfect = !config.AlwaysPerfect;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (alwaysTreasureButton.containsPoint(x, y))
        {
            config.AlwaysFindTreasure = !config.AlwaysFindTreasure;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (instantCatchButton.containsPoint(x, y))
        {
            config.InstantCatchFish = !config.InstantCatchFish;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (instantTreasureButton.containsPoint(x, y))
        {
            config.InstantCatchTreasure = !config.InstantCatchTreasure;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (infiniteBaitButton.containsPoint(x, y))
        {
            config.InfiniteBait = !config.InfiniteBait;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (infiniteTackleButton.containsPoint(x, y))
        {
            config.InfiniteTackle = !config.InfiniteTackle;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (autoHookButton.containsPoint(x, y))
        {
            config.AutoHook = !config.AutoHook;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else
        {
            base.receiveLeftClick(x, y, playSound);
        }
    }
} 