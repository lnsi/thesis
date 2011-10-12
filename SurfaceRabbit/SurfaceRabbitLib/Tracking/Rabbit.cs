using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareTUI_Core;
using System.ComponentModel;

namespace SurfaceRabbit.Tracking
{

  public class Rabbit : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged;

    public event RabbitButtonPressed ButtonPressed;
    public event RabbitButtonReleased ButtonReleased;

    public event RabbitObjectEntered ObjectEntered;
    public event RabbitObjectLeft ObjectLeft;

    private SquareTUI sTUI;

    public Int32 RabbitCode { get; private set; }
    public SquareTUI ObjectCode
    {
      get { return sTUI; }
      set
      {
        SquareTUI oldValue = sTUI;
        if (sTUI != null)
          sTUI.PropertyChanged -= ObjectCode_PropertyChanged;

        sTUI = value;
        if ((oldValue != null && sTUI == null) ||
          (oldValue != null && oldValue.Value != 0 && sTUI != null && oldValue.Value != sTUI.Value))
          OnObjectLeft(oldValue);

        if (sTUI != null)
        {
          sTUI.PropertyChanged += ObjectCode_PropertyChanged;

          CopyX();
          CopyY();
          CopyAngle();
          CopyStateBitA();
          CopyStateBitB();

          if ((oldValue == null && sTUI.Value != 0) || 
            (oldValue != null && sTUI.Value != 0 && oldValue.Value != sTUI.Value))
            OnObjectEntered(sTUI);
        }

        if ((oldValue == null && sTUI != null)
          || (oldValue != null && sTUI == null)
          || (oldValue != null && sTUI != null && oldValue.Value != sTUI.Value))
          OnPropertyChanged("ObjectCode");
      }
    }

    public float X { get; private set; }
    public float Y { get; private set; }
    public double AngleRadians { get; private set; }
    public double AngleDegrees { get; private set; }

    public float SourceImageWidth { get; set; }
    public float SourceImageHeight { get; set; }

    public float ProportionalX
    {
      get
      {
        if (SourceImageWidth <= 0)
          throw new ArgumentException("SourceImageWidth not set");
        return X / SourceImageWidth;
      }
    }

    public float ProportionalY
    {
      get 
      {
        if(SourceImageHeight <= 0)
          throw new ArgumentException("SourceImageHeight not set");
        return Y / SourceImageHeight;
      }
    }

    public bool ButtonA { get; private set; }
    public bool ButtonB { get; private set; }

    public Rabbit(Int32 rCode, SquareTUI oCode)
    {
      RabbitCode = rCode;
      ObjectCode = oCode;
    }

    void ObjectCode_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "StateBitA":
          CopyStateBitA();
          break;
        case "StateBitB":
          CopyStateBitB();
          break;
        case "X":
          CopyX();
          break;
        case "Y":
          CopyY();
          break;
        case "Angle":
          CopyAngle();
          break;
      }
    }

    private void CopyAngle()
    {
      AngleRadians = ObjectCode.Anchor.AngleRadians;
      AngleDegrees = ObjectCode.Anchor.Angle;
      OnPropertyChanged("AngleRadians");
      OnPropertyChanged("AngleDegrees");
    }

    private void CopyY()
    {
      Y = ObjectCode.CalculateCenter().Y;
      OnPropertyChanged("Y");
      OnPropertyChanged("ProportionalY");
    }

    private void CopyX()
    {
      X = ObjectCode.CalculateCenter().X;
      OnPropertyChanged("X");
      OnPropertyChanged("ProportionalX");
    }

    private void CopyStateBitB()
    {
      bool previous = ButtonB;
      ButtonB = ObjectCode.StateBitB;
      OnPropertyChanged("ButtonB");

      if (previous == ButtonB)
        return;

      if (ButtonB)
        OnButtonPressed(RabbitButtonType.ButtonB);
      else
        OnButtonReleased(RabbitButtonType.ButtonB);
    }

    private void CopyStateBitA()
    {
      bool previous = ButtonA;
      ButtonA = ObjectCode.StateBitA;
      OnPropertyChanged("ButtonA");

      if (previous == ButtonA)
        return;

      if (ButtonA)
        OnButtonPressed(RabbitButtonType.ButtonA);
      else
        OnButtonReleased(RabbitButtonType.ButtonA);
    }

    private void OnButtonReleased(RabbitButtonType type)
    {
      if (ButtonReleased != null)
        ButtonReleased(this, new RabbitButtonEventArgs(type, RabbitButtonEventType.Released));
    }

    private void OnButtonPressed(RabbitButtonType type)
    {
      if (ButtonPressed != null)
        ButtonPressed(this, new RabbitButtonEventArgs(type, RabbitButtonEventType.Pressed));
    }

    protected void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

    private void OnObjectEntered(SquareTUI sTUI)
    {
      if (ObjectEntered != null)
        ObjectEntered(this, new RabbitEventArgs(this, sTUI, RabbitEventType.ObjectEntered));
    }

    private void OnObjectLeft(SquareTUI sTUI)
    {
      if (ObjectLeft != null)
        ObjectLeft(this, new RabbitEventArgs(this, sTUI, RabbitEventType.ObjectLeft));
    }

  }

}
