using System;
using System.Drawing;
using System.Windows.Forms;
using TaskbarControl.Forms.Win32;

namespace TaskbarControl.Forms
{
    public class TaskbarControl : Form
    {
        // Area with all of the application icons
        private const string AppIconArea = "Shell_TrayWnd";
        // Area to the far right with notification icons
        private const string NotifyArea = "TrayNotifyWnd";
        // Area between start button and the notification icons on the far right
        private const string ToolbarArea = "ReBarWindow32";

        private struct TaskbarArea
        {
            public IntPtr Handle;
            public (uint ThreadId, uint ProcessId) WindowIds;
            public (Rect ClientRelative, Rect DesktopRelative) AreaRects;
        }

        private static TaskbarArea _appIconArea;
        private static TaskbarArea _toolbarArea;
        private static TaskbarArea _notifyArea;

        private bool _taskbarMoved;
        private bool _taskbarRearranged;

        private readonly IntPtr _taskbarEventHook;
        private readonly Win32Native.WinEventDelegate _taskbarEventCallback;

        /// <summary>
        /// Event triggered when the taskbar has changed in a significant way.
        /// </summary>
        /// <remarks>
        /// In particular, we're paying attention to available space changing to fit into our control size.
        /// </remarks>
        public event Action TaskbarChanged;

        /// <summary>
        /// Gets or Sets: The Maximum TaskbarControl size.
        /// </summary>
        public Size MaxSize { get; set; }

        /// <summary>
        /// Gets or Sets: The size of the <see cref="TaskbarControl"/>
        /// </summary>
        public new Size Size
        {
            get => base.Size;
            set
            {
                this.OnResize(EventArgs.Empty);
                base.Size = value;
            }
        }

        /// <summary>
        /// Static ctor. Gathers taskbar information from the OS.
        /// </summary>
        static TaskbarControl()
        {
            _appIconArea = new TaskbarArea
            {
                Handle = Win32Native.FindWindowEx(IntPtr.Zero, IntPtr.Zero, AppIconArea, string.Empty)
            };
            _notifyArea = new TaskbarArea
            {
                Handle = Win32Native.FindWindowEx(_appIconArea.Handle, IntPtr.Zero, NotifyArea, string.Empty)
            };
            _toolbarArea = new TaskbarArea
            {
                Handle = Win32Native.FindWindowEx(_appIconArea.Handle, IntPtr.Zero, ToolbarArea, string.Empty),
            };
            
            _appIconArea.WindowIds = Win32Native.GetWindowThreadProcessId(_appIconArea.Handle);
            _toolbarArea.WindowIds = Win32Native.GetWindowThreadProcessId(_toolbarArea.Handle);
            _notifyArea.WindowIds = Win32Native.GetWindowThreadProcessId(_notifyArea.Handle);
        }

        /// <summary>
        /// Creates an instance of <see cref="TaskbarControl"/>.
        /// </summary>
        /// <remarks>
        /// Re-parents this form to the taskbar. Using <see cref="Form.Parent"/> won't work the way you'd expect.
        /// </remarks>
        public TaskbarControl()
        {
            _taskbarEventCallback += TaskbarEventCallback;

            MaxSize = Size.Empty;
            Win32Native.SetParent(this.Handle, _appIconArea.Handle);
            IntPtr currentModule = Win32Native.GetModuleHandle(string.Empty);
            _taskbarEventHook = Win32Native.SetWinEventHook(OsEvent.Min, OsEvent.Max,
                currentModule, this._taskbarEventCallback,
                _toolbarArea.WindowIds.ProcessId, _toolbarArea.WindowIds.ThreadId, 0);
            Load += OnLoad;
        }

        private void TaskbarEventCallback(IntPtr winEventHook, OsEvent eventType, int objectId,
            int childId, uint eventThreadId, uint eventTime)
        {
            switch (eventType)
            {
                case OsEvent.SystemMoveSizeEnd:
                    _taskbarMoved = true;
                    break;
                case OsEvent.ObjectReorder when _taskbarMoved:
                    _taskbarRearranged = true;
                    break;
            }

            if (!_taskbarMoved || !_taskbarRearranged) return;

            this.TaskbarChanged?.Invoke();    
            _taskbarMoved = false;
            _taskbarRearranged = false;
        }

        protected Size RequestAvailableControlSize(Size newSizeRequest)
        {
            UpdateTaskbarWindowAreas();

            if (!newSizeRequest.IsEmpty)
            {
                return new Size(_appIconArea.AreaRects.ClientRelative.Bottom,
                    _appIconArea.AreaRects.DesktopRelative.Right - _notifyArea.AreaRects.DesktopRelative.Left);
            }

            var ratio = (float) newSizeRequest.Width / newSizeRequest.Height;

            if (_appIconArea.AreaRects.ClientRelative.Bottom >= newSizeRequest.Height)
            {
                return new Size(newSizeRequest.Width, (int) (newSizeRequest.Width / ratio));
            }

            var height = _appIconArea.AreaRects.ClientRelative.Bottom;

            return new Size((int)(height * ratio), height);

            // Default: Give the available space between icons and the notify bar.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Win32Native.UnhookWinEvent(_taskbarEventHook);
            }

            base.Dispose(disposing);
        }
        
        private void OnLoad(object sender, EventArgs e)
        {
            Resize += OnResize;
        }

        private void OnResize(object sender, EventArgs e)
        {
            UpdateSizeAndPosition();
        }

        private void UpdateTaskbarWindowAreas()
        {
            _notifyArea.AreaRects =
                (Win32Native.GetNativeRect(_notifyArea.Handle, NativeRectType.DesktopRelative),
            Win32Native.GetNativeRect(_notifyArea.Handle, NativeRectType.InternalClientArea));
            _appIconArea.AreaRects =
                (Win32Native.GetNativeRect(_appIconArea.Handle, NativeRectType.DesktopRelative),
                    Win32Native.GetNativeRect(_appIconArea.Handle, NativeRectType.InternalClientArea));
            _toolbarArea.AreaRects =
                (Win32Native.GetNativeRect(_toolbarArea.Handle, NativeRectType.DesktopRelative),
                    Win32Native.GetNativeRect(_toolbarArea.Handle, NativeRectType.InternalClientArea));
        }

        private void UpdateSizeAndPosition()
        {
            UpdateTaskbarWindowAreas();

            var taskbarControlRect = new Rect
            {
                Left = _notifyArea.AreaRects.DesktopRelative.Left - this.Size.Width,
                Right = this.Size.Width,
                Top = 0,
                Bottom = this.Size.Height
            };

            Win32Native.SetWindowPos(this.Handle, IntPtr.Zero,
                taskbarControlRect.Left, taskbarControlRect.Top,
                taskbarControlRect.Right, taskbarControlRect.Bottom,
                SizePositionFlag.ShowWindow);
            Win32Native.SetWindowPos(_toolbarArea.Handle,
                IntPtr.Zero,
                _toolbarArea.AreaRects.DesktopRelative.Left,
                0,
                taskbarControlRect.Left - _toolbarArea.AreaRects.DesktopRelative.Left,
                _toolbarArea.AreaRects.ClientRelative.Bottom, SizePositionFlag.NoOwnerZOrder);
        }
    }
}
