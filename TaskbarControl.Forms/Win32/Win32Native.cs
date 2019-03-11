using System;
using System.Runtime.InteropServices;

namespace TaskbarControl.Forms.Win32
{
    public enum NativeRectType
    {
        InternalClientArea,
        DesktopRelative
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    internal enum OsEvent : uint
    {
        Min = 0x0001,
        Start = 0xA000,
        End = 0xAFFF,
        ObjectAcceleratorChange = 0x8012,
        ObjectCloaked = 0x8017,
        ObjectContentScrolled = 0x8015,
        ObjectCreate = 0x8000,
        ObjectDefaultActionChanged = 0x8011,
        ObjectDescriptionChanged = 0x800D,
        ObjectDestroyed = 0x8001,
        ObjectDragStart = 0x8021,
        ObjectDragCancel = 0x8022,
        ObjectDragComplete = 0x8023,
        ObjectDragEnter = 0x8024,
        ObjectDragLeave = 0x8025,
        ObjectDragDropped = 0x8026,
        ObjectEnd = 0x80FF,
        ObjectFocus = 0x8005,
        ObjectHelpChange = 0x8010,
        ObjectHide = 0x8003,
        ObjectHostedObjectsInvalidated = 0x8020,
        ObjectImeHide = 0x8028,
        ObjectImeShow = 0x8027,
        ObjectImeChange = 0x8029,
        ObjectInvoked = 0x8013,
        ObjectLiveRegionChanged = 0x8019,
        ObjectLocationChange = 0x800B,
        ObjectNameChange = 0x800C,
        ObjectParentChange = 0x800F,
        ObjectReorder = 0x8004,
        ObjectSelection = 0x8006,
        ObjectSelectionAdd = 0x8007,
        ObjectSelectionRemove = 0x8008,
        ObjectSelectionWithin = 0x8009,
        ObjectShow = 0x8002,
        ObjectStateChange = 0x800A,
        ObjectTextEditConversionTargetChanged = 0x8030,
        ObjectTextSelectionChanged = 0x8014,
        ObjectUncloaked = 0x8018,
        ObjectValueChange = 0x800E,
        OemDefinedStart = 0x0101,
        OemDefinedEnd = 0x01FF,
        SystemAlert = 0x0002,
        SystemArrangementPreview = 0x8016,
        SystemCaptureEnd = 0x0009,
        SystemCaptureStart = 0x0008,
        SystemContextHelpEnd = 0x000D,
        SystemContextHelpStart = 0x000C,
        SystemDesktopSwitch = 0x0020,
        SystemDialogEnd = 0x0011,
        SystemDialogStart = 0x0010,
        SystemDragDropEnd = 0x000F,
        SystemDragDropStart = 0x000E,
        SystemEnd = 0x00FF,
        SystemForeground = 0x0003,
        SystemMenuPopUpEnd = 0x0007,
        SystemMenuPopUpStart = 0x0006,
        SystemMenuEnd = 0x0005,
        SystemMenuStart = 0x0004,
        SystemMinimizeEnd = 0x0017,
        SystemMinimizeStart = 0x0016,
        SystemMoveSizeEnd = 0x000B,
        SystemMoveSizeStart = 0x000A,
        SystemScrollingEnd = 0x0013,
        SystemScrollingStart = 0x0012,
        SystemSound = 0x0001,
        SystemSwitchEnd = 0x0015,
        SystemSwitchStart = 0x0014,
        UIAEventIdStart = 0x4E00,
        UIAEventIdEnd = 0x4EFF,
        UIAPropIdStart = 0x7500,
        UIAPropIdEnd = 0x75FF,
        Max = 0x7FFFFFFF
    }

    [Flags]
    public enum SizePositionFlag : uint
    {
        None = 0x0000,
        
        /// <summary>
        /// If the calling thread and the thread that owns the 
        /// window are attached to different input queues, the 
        /// system posts the request to the thread that owns the window. 
        /// This prevents the calling thread from blocking its
        /// execution while other threads process the request.
        /// </summary>
        AsyncWindowPos = 0x4000,
        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message.
        /// </summary>
        DeferErase = 0x2000,
        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window. 
        /// </summary>
        DrawFrame = 0x0020,
        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function.
        /// Sends a WM_NCCALCSIZE message to the window, even if the window's
        /// size is not being changed. If this flag is not specified,
        /// WM_NCCALCSIZE is sent only when the window's size is being changed. 
        /// </summary>
        FrameChanged = 0x0020,
        /// <summary>
        /// Hides the window. 
        /// </summary>
        HideWindow = 0x0080,
        /// <summary>
        /// Does not activate the window. If this flag is not set,
        /// the window is activated and moved to the top of either
        /// the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter). 
        /// </summary>
        NoActivate = 0x0010,
        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified,
        /// the valid contents of the client area are saved and
        /// copied back into the client area after the window is sized or repositioned. 
        /// </summary>
        NoCopyBits = 0x0100,
        /// <summary>
        /// Retains the current position (ignores X and Y parameters). 
        /// </summary>
        NoMove = 0x0002,
        /// <summary>
        /// Does not change the owner window's position in the Z order. 
        /// </summary>
        NoOwnerZOrder = 0x0200,
        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs.
        /// This applies to the client area,
        /// the nonclient area (including the title bar and scroll bars), and any part of
        /// the parent window uncovered as a result of the window being moved.
        /// When this flag is set, the application must explicitly invalidate or redraw
        /// any parts of the window and parent window that need redrawing. 
        /// </summary>
        NoRedraw = 0x0008,
        /// <summary>
        /// Same as the SWP_NOOWNERZORDER flag. 
        /// </summary>
        NoReposition = 0x0200,
        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message. 
        /// </summary>
        NoSendChanging = 0x0400,
        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters). 
        /// </summary>
        NoSize = 0x001,
        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter). 
        /// </summary>
        NoZOrder = 0x0004,
        /// <summary>
        /// Displays the window. 
        /// </summary>
        ShowWindow = 0x0040
    }

    internal class Win32Native
    {
        public static bool IsValidWindow(IntPtr windowHandle)
        {
            return IsWindow(windowHandle);
        }

        public static IntPtr GetWndProcHandle(IntPtr window)
        {
            return GetWindowLongPtr(window, -4);
        }

        internal delegate void WinEventDelegate(
            IntPtr winEventHook,
            OsEvent eventType,
            int objectId,
            int childId,
            uint eventThreadId,
            uint eventTime);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(
            OsEvent        eventMin,
            OsEvent        eventMax,
            IntPtr      winEventProcModule,
            WinEventDelegate winEventCallback,
            uint        processId,
            uint        threadId,
            uint        flags
        );

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr winEventHook);

        internal static (uint ThreadId, uint ProcessId) GetWindowThreadProcessId(IntPtr window)
        {
            uint threadId = GetWindowThreadProcessId(window, out uint processId);
            return (threadId, processId);
        }

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(
            IntPtr hWnd,
            int  nIndex
        );

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent
        );

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int  x,
            int  y,
            int  width,
            int  height,
            SizePositionFlag flags
        );

        public static Rect GetNativeRect(IntPtr nativeHandle, NativeRectType rectType)
        {
            if (rectType == NativeRectType.DesktopRelative)
            {
                return GetWindowRect(nativeHandle);
            }

            return GetClientRect(nativeHandle);
        }

        internal static Rect GetClientRect(IntPtr windowHandle)
        {
            GetClientRect(windowHandle, out Rect result);
            return result;
        }

        internal static Rect GetWindowRect(IntPtr windowHandle)
        {
            GetWindowRect(windowHandle, out Rect result);
            return result;
        }

        [DllImport("user32.dll")]
        private static extern void GetClientRect(IntPtr windowHandle, out Rect rect);

        [DllImport("user32.dll")]
        private static extern void GetWindowRect(IntPtr windowHandle, out Rect rect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(
            IntPtr parentWindow,
            IntPtr childWindow,
            string className,
            string windowName);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr windowHandle);
    }
}
