namespace Bonkers
{
    using System;
    using System.Collections.Generic;

    using Cysharp.Threading.Tasks;

    using JetBrains.Annotations;

    using UnityEngine;
    
    using QFSW.QC;
    using QFSW.QC.Actions;
    
    using I32   = System.Int32;
    using I32x2 = Unity.Mathematics.int2;

    [PublicAPI]
    [UsedImplicitly]
    [CommandPrefix(prefixName: "screen.")]
    public static class ScreenCommands
    {
        [Command(aliasOverride: "monitor")]
        public static IEnumerator<ICommandAction> ScreenMonitor()
        {
            List<DisplayInfo> __displays = new();
            Screen.GetDisplayLayout(displayLayout: __displays);

            if (__displays.Count > 0)
            {
                // var moveOperation = Screen.MoveMainWindowTo(displays[0], new Vector2Int(displays[0].width / 2, displays[0].height / 2));
                // yield return moveOperation;
                
                List<String> __displayNames = new();

                for (I32 __index = 0; __index < __displays.Count; __index++)
                {
                    DisplayInfo __display = __displays[index: __index];
                    __displayNames.Add(item: __display.name + $" ({__index})");
                }
                
                I32 __currentDisplayIndex = __displays.IndexOf(item: Screen.mainWindowDisplayInfo);
                
                yield return new Value(value: "Pick a display");
                yield return new Choice<String>(choices: __displayNames, onSelect: display => __currentDisplayIndex = __displayNames.IndexOf(item: display));
                
                DisplayInfo __selectedDisplay = __displays[index: __currentDisplayIndex];
                
                yield return new Typewriter(message: "Setting active monitor to " + __selectedDisplay.name + $" ({__currentDisplayIndex})" +"...");
                
                //Screen.MoveMainWindowTo(display: __displays[index: __currentDisplayIndex], position: new Vector2Int(x: __selectedDisplay.width / 2, y: __selectedDisplay.height / 2));
                MoveWindowAsync(index: __currentDisplayIndex, knownDisplays: __displays).Forget();
            }
        }
        
        private static async UniTask MoveWindowTask(I32 index, List<DisplayInfo> knownDisplays = null)
        {
            FullScreenMode __fullScreenMode = Screen.fullScreenMode;
            
            if (knownDisplays == null)
            {
                knownDisplays = new List<DisplayInfo>();
                Screen.GetDisplayLayout(displayLayout: knownDisplays);
            }

            if (index < knownDisplays.Count)
            {
                DisplayInfo __display  = knownDisplays[index: index];
                Vector2Int  __position = new (x: 0, y: 0);
                if (Screen.fullScreenMode != FullScreenMode.Windowed)
                {
                    __position.x += __display.width  / 2;
                    __position.y += __display.height / 2;
                }
                
                AsyncOperation __asyncOperation = Screen.MoveMainWindowTo(display: __display, position: __position);
                while (__asyncOperation.progress < 1f)
                {
                    await UniTask.Yield();
                }
            }
            else
            {
                Debug.LogError(message: $"Display index {index} is out of range!", context: null);
                //await UniTask.CompletedTask;
            }
            
            Screen.fullScreenMode = __fullScreenMode;
        }
 
        private static async UniTask MoveWindowAsync(I32 index, List<DisplayInfo> knownDisplays = null)
        {
            await MoveWindowTask(index: index, knownDisplays: knownDisplays);
        }
        
        [Command(aliasOverride: "resolution")]
        public static Vector2Int Resolution
        {
            get
            {
                Resolution __resolution = Screen.currentResolution;
                return new Vector2Int(x: __resolution.width, y: __resolution.height);
            }
            set
            {
                Screen.SetResolution(width: value.x, height: value.y, fullscreenMode: FullScreenMode.Windowed);
                Debug.Log(message: $"Set resolution to {value.x}×{value.y}.");
            }
        }

        [Command(aliasOverride: "reset")]
        public static async UniTask Reset()
        {
            //Reset back to default resolution of screen 0
            List<DisplayInfo> __displays = new();
            Screen.GetDisplayLayout(displayLayout: __displays);
            
            await MoveWindowTask(index: 0, knownDisplays: __displays);
            
            Resolution __resolution = new ()
            {
                width  = __displays[0].width,
                height = __displays[0].height,
                refreshRateRatio = __displays[0].refreshRate,
            };
            
            Screen.SetResolution(width: __resolution.width, height: __resolution.height, fullscreenMode: FullScreenMode.Windowed, preferredRefreshRate: __resolution.refreshRateRatio);
            Debug.Log(message: $"Reset resolution to {__resolution.width}×{__resolution.height}.");
            
            //Reset back to default fullscreen mode
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        
        [Command(aliasOverride: "fullscreen-mode")]
        public static FullScreenMode FullScreen
        {
            get => Screen.fullScreenMode;
            set
            {
                Screen.fullScreenMode = value;
                Debug.Log(message: $"Set fullscreen mode to {value}.");
            }
        }
    }
}
