using System;
using System.Windows.Input;
using System.Windows.Interop;
using WindowsInput.Native;

namespace VirtualMechKeyboard;

public class GlobalHotKey
{
    private readonly int _hotkeyId;
    private readonly IntPtr _handle;
    private readonly Action _action;
    private readonly ModifierKeys _modifierKeys;
    private readonly Key _key;

    public GlobalHotKey(ModifierKeys modifierKeys, Key key, IntPtr handle, Action action)
    {
        _handle = handle;
        _action = action;
        _modifierKeys = modifierKeys;
        _key = key;
        _hotkeyId = GetHashCode();
        RegisterHotKey();
        ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
    }

    public void RegisterHotKey()
    {
        var virtualKeyCode = KeyInterop.VirtualKeyFromKey(_key);
        var modifier = (int)_modifierKeys;
        if (modifier == 0) modifier = 1; // NoMod
        if (modifier == 2) modifier = 3; // Ctrl
        if (modifier == 8) modifier = 9; // Alt
        if (!NativeMethods.RegisterHotKey(_handle, _hotkeyId, modifier, virtualKeyCode))
            throw new InvalidOperationException("Couldn't register the hotkey.");
    }

    public void UnregisterHotKey()
    {
        ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
        NativeMethods.UnregisterHotKey(_handle, _hotkeyId);
    }

    private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
    {
        if (handled) return;
        if (msg.message != NativeMethods.WmHotKey || (int)(msg.wParam) != _hotkeyId) return;
        _action?.Invoke();
        handled = true;
    }
}
