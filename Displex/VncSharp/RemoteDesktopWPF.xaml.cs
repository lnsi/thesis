﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using VncSharp.Resources;

namespace VncSharp
{
  /// <summary>
  /// Interaction logic for RemoteDesktopWPF.xaml
  /// </summary>
  public partial class RemoteDesktopWPF : UserControl
  {

    [Description("Raised after a successful call to the Connect() method.")]
    /// <summary>
    /// Raised after a successful call to the Connect() method.  Includes information for updating the local display in ConnectEventArgs.
    /// </summary>
    public event ConnectCompleteHandler ConnectComplete;

    [Description("Raised when the VNC Host drops the connection.")]
    /// <summary>
    /// Raised when the VNC Host drops the connection.
    /// </summary>
    public event EventHandler ConnectionLost;

    [Description("Raised when the VNC Host sends text to the client's clipboard.")]
    /// <summary>
    /// Raised when the VNC Host sends text to the client's clipboard. 
    /// </summary>
    public event EventHandler ClipboardChanged;

    /// <summary>
    /// Points to a Function capable of obtaining a user's password.  By default this means using the PasswordDialog.GetPassword() function; however, users of RemoteDesktop can replace this with any function they like, so long as it matches the delegate type.
    /// </summary>
    public AuthenticateDelegate GetPassword;

    private System.Drawing.Bitmap desktopBM;						     // Internal representation of remote image.
    private WriteableBitmap desktopWBM;
    private VncClient vnc;						     // The Client object handling all protocol-level interaction
    private int port = 5900;					     // The port to connect to on remote host (5900 is default)
    private bool passwordPending = false;		     // After Connect() is called, a password might be required.
    private bool fullScreenRefresh = false;		     // Whether or not to request the entire remote screen be sent.
    private VncDesktopTransformPolicy desktopPolicy;
    private RuntimeState state = RuntimeState.Disconnected;

    private System.Windows.Threading.Dispatcher _dispatcher;

    private enum RuntimeState
    {
      Disconnected,
      Disconnecting,
      Connected,
      Connecting
    }

    public RemoteDesktopWPF()
    {
      InitializeComponent();
      _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

      // Users of the control can choose to use their own Authentication GetPassword() method via the delegate above.  This is a default only.
      GetPassword = new AuthenticateDelegate(PasswordDialog.GetPassword);
    }

    [DefaultValue(5900)]
    [Description("The port number used by the VNC Host (typically 5900)")]
    /// <summary>
    /// The port number used by the VNC Host (typically 5900).
    /// </summary>
    public int VncPort
    {
      get
      {
        return port;
      }
      set
      {
        // Ignore attempts to use invalid port numbers
        if (value < 1 | value > 65535) value = 5900;
        port = value;
      }
    }

    /// <summary>
    /// True if the RemoteDesktop is connected and authenticated (if necessary) with a remote VNC Host; otherwise False.
    /// </summary>
    public bool IsConnected
    {
      get
      {
        return state == RuntimeState.Connected;
      }
    }

    [Description("The name of the remote desktop.")]
    /// <summary>
    /// The name of the remote desktop, or "Disconnected" if not connected.
    /// </summary>
    public string Hostname
    {
      get
      {
        return vnc == null ? "Disconnected" : vnc.HostName;
      }
    }

    public Image ImageRDF 
    {
      get { return iRDF; }
    }

    public int serverWidth
    {
        get { return vnc.Framebuffer.Width; }
    }

    public int serverHeight
    {
        get { return vnc.Framebuffer.Height; }
    }

    /// <summary>
    /// Get a complete update of the entire screen from the remote host.
    /// </summary>
    /// <remarks>You should allow users to call FullScreenUpdate in order to correct
    /// corruption of the local image.  This will simply request that the next update be
    /// for the full screen, and not a portion of it.  It will not do the update while
    /// blocking.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void FullScreenUpdate()
    {
      InsureConnection(true);
      fullScreenRefresh = true;
    }

    /// <summary>
    /// Insures the state of the connection to the server, either Connected or Not Connected depending on the value of the connected argument.
    /// </summary>
    /// <param name="connected">True if the connection must be established, otherwise False.</param>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is in the wrong state.</exception>
    private void InsureConnection(bool connected)
    {
      // Grab the name of the calling routine:
      string methodName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;

      if (connected)
      {
        System.Diagnostics.Debug.Assert(state == RuntimeState.Connected ||
                        state == RuntimeState.Disconnecting, // special case for Disconnect()
                        string.Format("RemoteDesktop must be in RuntimeState.Connected before calling {0}.", methodName));
        if (state != RuntimeState.Connected && state != RuntimeState.Disconnecting)
        {
          throw new InvalidOperationException("RemoteDesktop must be in Connected state before calling methods that require an established connection.");
        }
      }
      else
      { // disconnected
        System.Diagnostics.Debug.Assert(state == RuntimeState.Disconnected,
                        string.Format("RemoteDesktop must be in RuntimeState.Disconnected before calling {0}.", methodName));
        if (state != RuntimeState.Disconnected && state != RuntimeState.Disconnecting)
        {
          throw new InvalidOperationException("RemoteDesktop cannot be in Connected state when calling methods that establish a connection.");
        }
      }
    }

    // This event handler deals with Frambebuffer Updates coming from the host. An
    // EncodedRectangle object is passed via the VncEventArgs (actually an IDesktopUpdater
    // object so that *only* Draw() can be called here--Decode() is done elsewhere).
    // The VncClient object handles thread marshalling onto the UI thread.
    protected void VncUpdate(object sender, VncEventArgs e)
    {
      _dispatcher.BeginInvoke(new Action(() =>
      {
        //e.DesktopUpdater.Draw(desktopBM);
        //iRDF.Source = desktopBM.ToBitmapSource();
        e.DesktopUpdater.Draw(desktopWBM);

        if (state == RuntimeState.Connected)
        {
          vnc.RequestScreenUpdate(fullScreenRefresh);
          // Make sure the next screen update is incremental
          fullScreenRefresh = false;
        }
      }));
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host)
    {
      // Use Display 0 by default.
      Connect(host, 0);
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <param name="viewOnly">Determines whether mouse and keyboard events will be sent to the host.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host, bool viewOnly)
    {
      // Use Display 0 by default.
      Connect(host, 0, viewOnly);
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <param name="viewOnly">Determines whether mouse and keyboard events will be sent to the host.</param>
    /// <param name="scaled">Determines whether to use desktop scaling or leave it normal and clip.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host, bool viewOnly, bool scaled)
    {
      // Use Display 0 by default.
      Connect(host, 0, viewOnly, scaled);
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <param name="display">The Display number (used on Unix hosts).</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host, int display)
    {
      Connect(host, display, false);
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <param name="display">The Display number (used on Unix hosts).</param>
    /// <param name="viewOnly">Determines whether mouse and keyboard events will be sent to the host.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host, int display, bool viewOnly)
    {
      Connect(host, display, viewOnly, false);
    }

    /// <summary>
    /// Connect to a VNC Host and determine whether or not the server requires a password.
    /// </summary>
    /// <param name="host">The IP Address or Host Name of the VNC Host.</param>
    /// <param name="display">The Display number (used on Unix hosts).</param>
    /// <param name="viewOnly">Determines whether mouse and keyboard events will be sent to the host.</param>
    /// <param name="scaled">Determines whether to use desktop scaling or leave it normal and clip.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if host is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if display is negative.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Connect(string host, int display, bool viewOnly, bool scaled)
    {
      // TODO: Should this be done asynchronously so as not to block the UI?  Since an event 
      // indicates the end of the connection, maybe that would be a better design.
      InsureConnection(false);

      if (host == null) throw new ArgumentNullException("host");
      if (display < 0) throw new ArgumentOutOfRangeException("display", display, "Display number must be a positive integer.");

      // Start protocol-level handling and determine whether a password is needed
      vnc = new VncClient();
      vnc.ConnectionLost += new EventHandler(VncClientConnectionLost);
      vnc.ServerCutText += new EventHandler(VncServerCutText);

      Console.WriteLine("Connecting...");
      passwordPending = vnc.Connect(host, display, VncPort, viewOnly);

      if (passwordPending)
      {
        // Server needs a password, so call which ever method is refered to by the GetPassword delegate.
        string password = GetPassword();

        if (password == null)
        {
          // No password could be obtained (e.g., user clicked Cancel), so stop connecting
          return;
        }
        else
        {
          Authenticate(password);
        }
      }
      else
      {
        // No password needed, so go ahead and Initialize here
        Initialize();
      }
    }

    /// <summary>
    /// Authenticate with the VNC Host using a user supplied password.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already Connected.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    /// <exception cref="System.NullReferenceException">Thrown if the password is null.</exception>
    /// <param name="password">The user's password.</param>
    public void Authenticate(string password)
    {
      InsureConnection(false);
      if (!passwordPending) throw new InvalidOperationException("Authentication is only required when Connect() returns True and the VNC Host requires a password.");
      if (password == null) throw new NullReferenceException("password");

      passwordPending = false;  // repeated calls to Authenticate should fail.
      if (vnc.Authenticate(password))
      {
        Initialize();
      }
      else
      {
        OnConnectionLost();
      }
    }


    /// <summary>
    /// Changes the input mode to view-only or interactive.
    /// </summary>
    /// <param name="viewOnly">True if view-only mode is desired (no mouse/keyboard events will be sent).</param>
    public void SetInputMode(bool viewOnly)
    {
      vnc.SetInputMode(viewOnly);
    }

    /// <summary>
    /// After protocol-level initialization and connecting is complete, the local GUI objects have to be set-up, and requests for updates to the remote host begun.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is already in the Connected state.  See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>		
    protected void Initialize()
    {
        Console.WriteLine("Initializing..");
      // Finish protocol handshake with host now that authentication is done.
      InsureConnection(false);
      vnc.Initialize();
      SetState(RuntimeState.Connected);

      // Create a buffer on which updated rectangles will be drawn and draw a "please wait..." 
      // message on the buffer for initial display until we start getting rectangles
      SetupDesktop();

      // Tell the user of this control the necessary info about the desktop in order to setup the display
      OnConnectComplete(new ConnectEventArgs(vnc.Framebuffer.Width,
                           vnc.Framebuffer.Height,
                           vnc.Framebuffer.DesktopName));

      // Refresh scroll properties
      //AutoScrollMinSize = desktopPolicy.AutoScrollMinSize;

      // Start getting updates from the remote host (vnc.StartUpdates will begin a worker thread).
      vnc.VncUpdate += new VncUpdateHandler(VncUpdate);
      vnc.StartUpdates();
    }

    private void SetState(RuntimeState newState)
    {
      state = newState;

      // Set mouse pointer according to new state
      switch (state)
      {
        case RuntimeState.Connected:
          // Change the cursor to the "vnc" custor--a see-through dot
          Cursor = new Cursor(Directory.GetCurrentDirectory() +
                System.IO.Path.DirectorySeparatorChar + "Resources" +
                System.IO.Path.DirectorySeparatorChar + "vnccursor.cur");
          break;
        // All other states should use the normal cursor.
        case RuntimeState.Disconnected:
        default:
          Cursor = Cursors.Arrow;
          break;
      }
    }

    /// <summary>
    /// Creates and initially sets-up the local bitmap that will represent the remote desktop image.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not already in the Connected state. See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    protected void SetupDesktop()
    {
      InsureConnection(true);

      // Create a new bitmap to cache locally the remote desktop image.  Use the geometry of the
      // remote framebuffer, and 32bpp pixel format (doesn't matter what the server is sending--8,16,
      // or 32--we always draw 32bpp here for efficiency).
      desktopBM = new System.Drawing.Bitmap(vnc.Framebuffer.Width, vnc.Framebuffer.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
      desktopWBM = new WriteableBitmap(desktopBM.ToBitmapSource());
      iRDF.Source = desktopWBM;

      // Draw a "please wait..." message on the local desktop until the first
      // rectangle(s) arrive and overwrite with the desktop image.
      //DrawDesktopMessage("Connecting to VNC host, please wait...");
    }

    /// <summary>
    /// Stops the remote host from sending further updates and disconnects.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not already in the Connected state. See <see cref="VncSharp.RemoteDesktop.IsConnected" />.</exception>
    public void Disconnect()
    {
      InsureConnection(true);
      vnc.ConnectionLost -= new EventHandler(VncClientConnectionLost);
      vnc.ServerCutText -= new EventHandler(VncServerCutText);
      vnc.Disconnect();
      SetState(RuntimeState.Disconnected);
      OnConnectionLost();
      InvalidateVisual();
    }

    /// <summary>
    /// Fills the remote server's clipboard with the text in the client's clipboard, if any.
    /// </summary>
    public void FillServerClipboard()
    {
      FillServerClipboard(Clipboard.GetText());
    }

    /// <summary>
    /// Fills the remote server's clipboard with text.
    /// </summary>
    /// <param name="text">The text to put in the server's clipboard.</param>
    public void FillServerClipboard(string text)
    {
      vnc.WriteClientCutText(text);
    }

    /// <summary>
    /// RemoteDesktop listens for ConnectionLost events from the VncClient object.
    /// </summary>
    /// <param name="sender">The VncClient object that raised the event.</param>
    /// <param name="e">An empty EventArgs object.</param>
    protected void VncClientConnectionLost(object sender, EventArgs e)
    {
      // If the remote host dies, and there are attempts to write
      // keyboard/mouse/update notifications, this may get called 
      // many times, and from main or worker thread.
      // Guard against this and invoke Disconnect once.
      if (state == RuntimeState.Connected)
      {
        SetState(RuntimeState.Disconnecting);
        Disconnect();
      }
    }

    // Handle the VncClient ServerCutText event and bubble it up as ClipboardChanged.
    protected void VncServerCutText(object sender, EventArgs e)
    {
      OnClipboardChanged();
    }

    protected void OnClipboardChanged()
    {
      if (ClipboardChanged != null)
        ClipboardChanged(this, EventArgs.Empty);
    }

    /// <summary>
    /// Dispatches the ConnectionLost event if any targets have registered.
    /// </summary>
    /// <param name="e">An EventArgs object.</param>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is in the Connected state.</exception>
    protected void OnConnectionLost()
    {
      if (ConnectionLost != null)
      {
        ConnectionLost(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Dispatches the ConnectComplete event if any targets have registered.
    /// </summary>
    /// <param name="e">A ConnectEventArgs object with information about the remote framebuffer's geometry.</param>
    /// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.</exception>
    protected void OnConnectComplete(ConnectEventArgs e)
    {
      if (ConnectComplete != null)
      {
        ConnectComplete(this, e);
      }
    }

    // Handle Mouse Events:		 -------------------------------------------
    // In all cases, the same thing needs to happen: figure out where the cursor
    // is, and then figure out the state of all mouse buttons.
    // TODO: currently we don't handle the case of 3-button emulation with 2-buttons.
    private Point mousePosition;
    private MouseButton? button = null;
    private bool clicking = false;

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!clicking)
        return;

      mousePosition = e.GetPosition(iRDF);

      UpdateRemotePointer();
      Console.Write("-");
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      clicking = true;
      mousePosition = e.GetPosition(iRDF);
      button = e.ChangedButton;

      UpdateRemotePointer();
      Console.Write("MouseDown({0:00.00}, {1:00.00})", mousePosition.X, mousePosition.Y);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      
      clicking = false;
      mousePosition = e.GetPosition(iRDF);
      button = e.ChangedButton;

      UpdateRemotePointer();
      Console.Write("MouseUp({0:00.00}, {1:00.00})\n", mousePosition.X, mousePosition.Y);

      //Hack
      button = null;
      UpdateRemotePointer();
    }

    public void ContactDown(Point contactPos)
    {
      mousePosition = contactPos;
      button = MouseButton.Left;
      UpdateRemotePointer();
    }

    public void ContactUp(Point contactPos)
    {
      // First up
      mousePosition = contactPos;
      button = MouseButton.Left;
      UpdateRemotePointer();

      //Then the hack
      button = null;
      UpdateRemotePointer();
    }

    public void ContactChange(Point contactPos)
    {
      mousePosition = contactPos;
      button = MouseButton.Left;
      UpdateRemotePointer();
    }

    public void OnRightClick()
    {
        mousePosition = new Point(20,20);
        button = MouseButton.Right;
        UpdateRemotePointer();
        UpdateRemotePointer();
        button = null;
        UpdateRemotePointer();
    }

    // TODO: Perhaps overload UpdateRemotePointer to take a flag indicating if mousescroll has occured??
    //protected override void OnMouseWheel(MouseEventArgs mea)
    //{
    //  // HACK: this check insures that while in DesignMode, no messages are sent to a VNC Host
    //  // (i.e., there won't be one--NullReferenceException)			
    //  if (!DesignMode && IsConnected)
    //  {
    //    Point current = PointToClient(MousePosition);
    //    byte mask = 0;

    //    // mouse was scrolled forward
    //    if (mea.Delta > 0)
    //    {
    //      mask += 8;
    //    }
    //    else if (mea.Delta < 0)
    //    { // mouse was scrolled backwards
    //      mask += 16;
    //    }

    //    vnc.WritePointerEvent(mask, desktopPolicy.GetMouseMovePoint(current));
    //  }
    //  base.OnMouseWheel(mea);
    //}

    private void UpdateRemotePointer()
    {
      if (IsConnected)
      {
        System.Drawing.Point current =
          new System.Drawing.Point((int)mousePosition.X, (int)mousePosition.Y);
        byte mask = 0;

        if (button == MouseButton.Left) mask += 1;
        if (button == MouseButton.Middle) mask += 2;
        if (button == MouseButton.Right) mask += 4;

        if (current.X < 0 || current.Y < 0)
          throw new Exception("Crazy value for mouse pointer!");

        vnc.WritePointerEvent(mask, current);
      }
    }

    // Handle Keyboard Events:		 -------------------------------------------
    // These keys don't normally throw an OnKeyDown event. Returning true here fixes this.
    //protected override bool IsInputKey(Keys keyData)
    //{
    //  switch (keyData)
    //  {
    //    case Keys.Tab:
    //    case Keys.Up:
    //    case Keys.Down:
    //    case Keys.Left:
    //    case Keys.Right:
    //    case Keys.Shift:
    //    case Keys.RWin:
    //    case Keys.LWin:
    //      return true;
    //    default:
    //      return base.IsInputKey(keyData);
    //  }
    //}

    // Thanks to Lionel Cuir, Christian and the other developers at 
    // Aulofee.com for cleaning-up my keyboard code, specifically:
    // ManageKeyDownAndKeyUp, OnKeyPress, OnKeyUp, OnKeyDown.
    //private void ManageKeyDownAndKeyUp(KeyEventArgs e, bool isDown)
    //{
    //  UInt32 keyChar;
    //  bool isProcessed = true;
    //  switch (e.KeyCode)
    //  {
    //    case Keys.Tab: keyChar = 0x0000FF09; break;
    //    case Keys.Enter: keyChar = 0x0000FF0D; break;
    //    case Keys.Escape: keyChar = 0x0000FF1B; break;
    //    case Keys.Home: keyChar = 0x0000FF50; break;
    //    case Keys.Left: keyChar = 0x0000FF51; break;
    //    case Keys.Up: keyChar = 0x0000FF52; break;
    //    case Keys.Right: keyChar = 0x0000FF53; break;
    //    case Keys.Down: keyChar = 0x0000FF54; break;
    //    case Keys.PageUp: keyChar = 0x0000FF55; break;
    //    case Keys.PageDown: keyChar = 0x0000FF56; break;
    //    case Keys.End: keyChar = 0x0000FF57; break;
    //    case Keys.Insert: keyChar = 0x0000FF63; break;
    //    case Keys.ShiftKey: keyChar = 0x0000FFE1; break;

    //    // BUG FIX -- added proper Alt/CTRL support (Edward Cooke)
    //    case Keys.Alt: keyChar = 0x0000FFE9; break;
    //    case Keys.ControlKey: keyChar = 0x0000FFE3; break;
    //    case Keys.LControlKey: keyChar = 0x0000FFE3; break;
    //    case Keys.RControlKey: keyChar = 0x0000FFE4; break;

    //    case Keys.Menu: keyChar = 0x0000FFE9; break;
    //    case Keys.Delete: keyChar = 0x0000FFFF; break;
    //    case Keys.LWin: keyChar = 0x0000FFEB; break;
    //    case Keys.RWin: keyChar = 0x0000FFEC; break;
    //    case Keys.Apps: keyChar = 0x0000FFEE; break;
    //    case Keys.F1:
    //    case Keys.F2:
    //    case Keys.F3:
    //    case Keys.F4:
    //    case Keys.F5:
    //    case Keys.F6:
    //    case Keys.F7:
    //    case Keys.F8:
    //    case Keys.F9:
    //    case Keys.F10:
    //    case Keys.F11:
    //    case Keys.F12:
    //      keyChar = 0x0000FFBE + ((UInt32)e.KeyCode - (UInt32)Keys.F1);
    //      break;
    //    default:
    //      keyChar = 0;
    //      isProcessed = false;
    //      break;
    //  }

    //  if (isProcessed)
    //  {
    //    vnc.WriteKeyboardEvent(keyChar, isDown);
    //    e.Handled = true;
    //  }
    //}

    //// HACK: the following overrides do a double check on DesignMode so 
    //// that if still in design mode, no messages are sent for 
    //// mouse/keyboard events (i.e., there won't be Host yet--
    //// NullReferenceException)			
    //protected override void OnKeyPress(KeyPressEventArgs e)
    //{
    //  base.OnKeyPress(e);
    //  if (DesignMode || !IsConnected)
    //    return;

    //  if (e.Handled)
    //    return;

    //  if (Char.IsLetterOrDigit(e.KeyChar) || Char.IsWhiteSpace(e.KeyChar) || Char.IsPunctuation(e.KeyChar) ||
    //    e.KeyChar == '~' || e.KeyChar == '`' || e.KeyChar == '<' || e.KeyChar == '>' ||
    //    e.KeyChar == '|' || e.KeyChar == '=' || e.KeyChar == '+' || e.KeyChar == '$' || e.KeyChar == '^')
    //  {
    //    vnc.WriteKeyboardEvent((UInt32)e.KeyChar, true);
    //    vnc.WriteKeyboardEvent((UInt32)e.KeyChar, false);
    //  }
    //  else if (e.KeyChar == '\b')
    //  {
    //    UInt32 keyChar = ((UInt32)'\b') | 0x0000FF00;
    //    vnc.WriteKeyboardEvent(keyChar, true);
    //    vnc.WriteKeyboardEvent(keyChar, false);
    //  }
    //}

    //protected override void OnKeyDown(KeyEventArgs e)
    //{
    //  if (DesignMode || !IsConnected)
    //    return;

    //  ManageKeyDownAndKeyUp(e, true);
    //  if (e.Handled)
    //    return;

    //  base.OnKeyDown(e);
    //}

    //protected override void OnKeyUp(KeyEventArgs e)
    //{
    //  if (DesignMode || !IsConnected)
    //    return;

    //  ManageKeyDownAndKeyUp(e, false);
    //  if (e.Handled)
    //    return;

    //  base.OnKeyDown(e);
    //}

    ///// <summary>
    ///// Sends a keyboard combination that would otherwise be reserved for the client PC.
    ///// </summary>
    ///// <param name="keys">SpecialKeys is an enumerated list of supported keyboard combinations.</param>
    ///// <remarks>Keyboard combinations are Pressed and then Released, while single keys (e.g., SpecialKeys.Ctrl) are only pressed so that subsequent keys will be modified.</remarks>
    ///// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.</exception>
    //public void SendSpecialKeys(SpecialKeys keys)
    //{
    //  this.SendSpecialKeys(keys, true);
    //}

    ///// <summary>
    ///// Sends a keyboard combination that would otherwise be reserved for the client PC.
    ///// </summary>
    ///// <param name="keys">SpecialKeys is an enumerated list of supported keyboard combinations.</param>
    ///// <remarks>Keyboard combinations are Pressed and then Released, while single keys (e.g., SpecialKeys.Ctrl) are only pressed so that subsequent keys will be modified.</remarks>
    ///// <exception cref="System.InvalidOperationException">Thrown if the RemoteDesktop control is not in the Connected state.</exception>
    //public void SendSpecialKeys(SpecialKeys keys, bool release)
    //{
    //  InsureConnection(true);
    //  // For all of these I am sending the key presses manually instead of calling
    //  // the keyboard event handlers, as I don't want to propegate the calls up to the 
    //  // base control class and form.
    //  switch (keys)
    //  {
    //    case SpecialKeys.Ctrl:
    //      PressKeys(new uint[] { 0xffe3 }, release);	// CTRL, but don't release
    //      break;
    //    case SpecialKeys.Alt:
    //      PressKeys(new uint[] { 0xffe9 }, release);	// ALT, but don't release
    //      break;
    //    case SpecialKeys.CtrlAltDel:
    //      PressKeys(new uint[] { 0xffe3, 0xffe9, 0xffff }, release); // CTRL, ALT, DEL
    //      break;
    //    case SpecialKeys.AltF4:
    //      PressKeys(new uint[] { 0xffe9, 0xffc1 }, release); // ALT, F4
    //      break;
    //    case SpecialKeys.CtrlEsc:
    //      PressKeys(new uint[] { 0xffe3, 0xff1b }, release); // CTRL, ESC
    //      break;
    //    // TODO: are there more I should support???
    //    default:
    //      break;
    //  }
    //}

    ///// <summary>
    ///// Given a list of keysym values, sends a key press for each, then a release.
    ///// </summary>
    ///// <param name="keys">An array of keysym values representing keys to press/release.</param>
    ///// <param name="release">A boolean indicating whether the keys should be Pressed and then Released.</param>
    //private void PressKeys(uint[] keys, bool release)
    //{
    //  System.Diagnostics.Debug.Assert(keys != null, "keys[] cannot be null.");

    //  for (int i = 0; i < keys.Length; ++i)
    //  {
    //    vnc.WriteKeyboardEvent(keys[i], true);
    //  }

    //  if (release)
    //  {
    //    // Walk the keys array backwards in order to release keys in correct order
    //    for (int i = keys.Length - 1; i >= 0; --i)
    //    {
    //      vnc.WriteKeyboardEvent(keys[i], false);
    //    }
    //  }
    //}
  }
}
