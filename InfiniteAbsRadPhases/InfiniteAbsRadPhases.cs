using Modding;
using System;
using SFCore.Utils;
using Satchel.BetterMenus;

namespace InfiniteAbsRadPhases
{
    public static class ModMenu
    {
        private static Menu? MenuRef;
        public static MenuScreen CreateModMenu(MenuScreen modlistmenu)
        {
            MenuRef ??= new Menu("AbsRad Phase Options", new Element[]
            {
            new CustomSlider(
                "Phase (5 does nothing)",
                f =>
                {
                    InfiniteAbsRadPhasesMod.globalSettings.InfinitePhase = (int)f;
                    MenuRef?.Update();
                },
                () => InfiniteAbsRadPhasesMod.globalSettings.InfinitePhase,
                1f,
                6f,
                true,
                Id:"Phases")
            });

            return MenuRef.GetMenuScreen(modlistmenu);
        }
    }

    public class InfiniteAbsRadPhasesMod : Mod, ICustomMenuMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        private static InfiniteAbsRadPhasesMod? _instance;

        internal static InfiniteAbsRadPhasesMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(InfiniteAbsRadPhasesMod)} was never constructed");
                }
                return _instance;
            }
        }
        public static LocalSettings localSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;
        public static GlobalSettings globalSettings { get; private set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public InfiniteAbsRadPhasesMod() : base("InfiniteAbsRadPhases")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            On.PlayMakerFSM.OnEnable += OnFsmEnable;

            Log("Initialized");
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Phase Control")
            {
                self.ChangeFsmTransition("Pause 1", "FINISHED", globalSettings.InfinitePhase == 1 ? "Idle 1" : globalSettings.InfinitePhase != 5 ? "Set Phase 2" : "Check 1");
                self.ChangeFsmTransition("Pause 2", "FINISHED", globalSettings.InfinitePhase == 2 ? "Idle 2" : globalSettings.InfinitePhase != 5 ? "Set Phase 3" : "Check 2");
                self.ChangeFsmTransition("Pause 3", "FINISHED", globalSettings.InfinitePhase == 3 ? "Idle 3" : globalSettings.InfinitePhase != 5 ? "Stun 1" : "Check 3");
                self.ChangeFsmTransition("Pause 4", "FINISHED", globalSettings.InfinitePhase == 4 ? "Idle 4" : globalSettings.InfinitePhase != 5 ? "Set Ascend" : "Check 4");
            }
            else if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Final Idle", globalSettings.InfinitePhase == 6 ? "NEXT" : null);
            }
        }
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) => ModMenu.CreateModMenu(modListMenu);

        public bool ToggleButtonInsideMenu => false;
    }
    public class LocalSettings
    {
    }
    public class GlobalSettings
    {
        public int InfinitePhase = 5;
    }
}
