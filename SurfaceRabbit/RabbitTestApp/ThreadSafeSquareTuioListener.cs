using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUIO.SquareTUI;
using System.Windows.Threading;
using System.Threading;

namespace RabbitTestApp
{
  class ThreadSafeSquareTuioListener : SquareTuioListener
  {

    private SquareTuioListener listener;
    private Dispatcher dispatcher;

    public ThreadSafeSquareTuioListener(SquareTuioListener listener, Dispatcher dispatcher)
    {
      this.listener = listener;
      this.dispatcher = dispatcher;
    }

    public void addSquareTuio(SquareTuioObject squareTui)
    {
      if (dispatcher.CheckAccess())
      {
        listener.addSquareTuio(squareTui);
      }
      else
      {
        object[] e = new object[] { squareTui };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { addSquareTuioImpl(e); }, e);
      }
    }

    private void addSquareTuioImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        addSquareTuio((SquareTuioObject)e[0]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { addSquareTuioImpl(e); }, e);
      }
    }

    public void buttonPressed(SquareTuioObject squareTui, SquareTUIButton button)
    {
      if (dispatcher.CheckAccess())
      {
        listener.buttonPressed(squareTui, button);
      }
      else
      {
        object[] e = new object[] { squareTui, button };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { buttonPressedImpl(e); }, e);
      }
    }

    public void buttonPressedImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        buttonPressed((SquareTuioObject)e[0], (SquareTUIButton)e[1]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { buttonPressedImpl(e); }, e);
      }
    }

    public void buttonReleased(SquareTuioObject squareTui, SquareTUIButton button)
    {
      if (dispatcher.CheckAccess())
      {
        listener.buttonReleased(squareTui, button);
      }
      else
      {
        object[] e = new object[] { squareTui, button };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { buttonReleasedImpl(e); }, e);
      }
    }

    public void buttonReleasedImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        buttonReleased((SquareTuioObject)e[0], (SquareTUIButton)e[1]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { buttonReleasedImpl(e); }, e);
      }
    }

    public void refresh(TUIO.TuioTime ftime)
    {
      if (dispatcher.CheckAccess())
      {
        listener.refresh(ftime);
      }
      else
      {
        object[] e = new object[] { ftime };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { refreshImpl(e); }, e);
      }
    }

    public void refreshImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        refresh((TUIO.TuioTime)e[0]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { refreshImpl(e); }, e);
      }
    }

    public void removeSquareTuio(SquareTuioObject squareTui)
    {
      if (dispatcher.CheckAccess())
      {
        listener.removeSquareTuio(squareTui);
      }
      else
      {
        object[] e = new object[] { squareTui };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { removeSquareTuioImpl(e); }, e);
      }
    }

    public void removeSquareTuioImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        removeSquareTuio((SquareTuioObject)e[0]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { removeSquareTuioImpl(e); }, e);
      }
    }

    public void updateSquareTuio(SquareTuioObject squareTui)
    {
      if (dispatcher.CheckAccess())
      {
        listener.updateSquareTuio(squareTui);
      }
      else
      {
        object[] e = new object[] { squareTui };
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { updateSquareTuioImpl(e); }, e);
      }
    }

    private void updateSquareTuioImpl(object[] e)
    {
      if (dispatcher.CheckAccess())
      {
        updateSquareTuio((SquareTuioObject)e[0]);
      }
      else
      {
        dispatcher.Invoke(DispatcherPriority.DataBind, (SendOrPostCallback)delegate { updateSquareTuioImpl(e); }, e);
      }
    }

  }

}
