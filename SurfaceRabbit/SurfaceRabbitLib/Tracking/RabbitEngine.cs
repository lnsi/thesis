using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using SquareTUI_Core;
using System.Configuration;
using System.Collections.ObjectModel;

namespace SurfaceRabbit.Tracking
{

  public class RabbitEngine
  {

    public virtual event RabbitAdded RabbitAdded;
    public virtual event RabbitUpdated RabbitUpdated;
    public virtual event RabbitRemoved RabbitRemoved;

    private RabbitEngineSettings settings;
    private ObservableCollection<Rabbit> currentRabbits;
    private Int32 rabbitIdCons = 0;

    private SquareTUIType currentType;
    private float contourThreshold;
    private float axisLength;
    private float locationThreshold;
    private float minArea;
    private float maxArea;

    public ObservableCollection<Rabbit> Rabbits
    {
      get { return currentRabbits; }
    }

    public RabbitEngine(ApplicationSettingsBase rtSettings)
    {
      currentRabbits = new ObservableCollection<Rabbit>();
      settings = new RabbitEngineSettings();
      settings.Load(rtSettings);
    }

    private void OnRabbitAdded(Rabbit rabbit)
    {
      if (RabbitAdded != null)
        RabbitAdded(this, new RabbitEngineEventArgs(rabbit, RabbitEngineEventType.Added));
    }

    private void OnRabbitUpdated(Rabbit rabbit)
    {
      if (RabbitUpdated != null)
        RabbitUpdated(this, new RabbitEngineEventArgs(rabbit, RabbitEngineEventType.Updated));
    }

    private void OnRabbitRemoved(Rabbit rabbit)
    {
      if (RabbitRemoved != null)
        RabbitRemoved(this, new RabbitEngineEventArgs(rabbit, RabbitEngineEventType.Removed));
    }

    public void ProcessImage(Bitmap cImage)
    {
      ProcessImage(new Image<Gray, byte>(cImage));
    }

    public void ProcessImage(Image<Gray, Byte> image)
    {
      IList<Rabbit> fRabbits = new List<Rabbit>();
      IList<Rabbit> rRabbits = new List<Rabbit>();
      IList<SquareTUI> foundLTUIs = null, foundPTUIs = null, foundTUIs = new List<SquareTUI>();

      if (settings.supportLTUIs)
      {
        currentType = SquareTUIType.LightTUI;
        contourThreshold = settings.lContourThreshold;
        axisLength = settings.lAxisLength;
        locationThreshold = settings.lLocationThreshold;
        minArea = settings.lMinArea;
        maxArea = settings.lMaxArea;
        foundLTUIs = PerformDetection(image);
      }

      if (settings.supportPTUIs)
      {
        currentType = SquareTUIType.PaperTUI;
        contourThreshold = settings.pContourThreshold;
        axisLength = settings.pAxisLength;
        locationThreshold = settings.pLocationThreshold;
        minArea = settings.pMinArea;
        maxArea = settings.pMaxArea;
        foundPTUIs = PerformDetection(image);
      }

      if (foundLTUIs != null) 
        foundTUIs = foundLTUIs;
      if (foundPTUIs != null)
      {
        foreach (SquareTUI sTUI in foundPTUIs)
          foundTUIs.Add(sTUI);
      }

      foreach (SquareTUI foundTUI in foundTUIs)
      {
        float rabbitLoctaionThreshold = (float)(settings.pAxisLength / 1.2);

        //The same code in the "same" location
        Rabbit existingR = currentRabbits.SingleOrDefault(tmp => tmp.ObjectCode.Value == foundTUI.Value && tmp.ObjectCode.GetDistance(foundTUI) < rabbitLoctaionThreshold);
        //One in the "same" location
        Rabbit closestR = currentRabbits.OrderBy(tmp => tmp.ObjectCode.GetDistance(foundTUI)).FirstOrDefault(tmp => tmp.ObjectCode.GetDistance(foundTUI) < rabbitLoctaionThreshold);

        if (existingR == null && closestR != null)
          existingR = closestR;

        if (existingR == null)
        {
          //Then it's new and has to be added
          Rabbit newRabbit = new Rabbit(++rabbitIdCons, foundTUI);
          newRabbit.SourceImageWidth = image.Width;
          newRabbit.SourceImageHeight = image.Height;

          currentRabbits.Add(newRabbit);
          fRabbits.Add(newRabbit);
          OnRabbitAdded(newRabbit);
          continue;
        }

        //It already existed
        existingR.ObjectCode = foundTUI;
        fRabbits.Add(existingR);
        OnRabbitUpdated(existingR);
        continue;
      }

      foreach (Rabbit formerR in currentRabbits)
      {
        if (!fRabbits.Contains(formerR))
          rRabbits.Add(formerR);
      }

      foreach (Rabbit formerR in rRabbits)
      {
        currentRabbits.Remove(formerR);
        OnRabbitRemoved(formerR);
      }
    }

    private IList<SquareTUI> PerformDetection(Image<Gray, Byte> gray)
    {
      gray = gray.ThresholdBinary(new Gray(contourThreshold), new Gray(400));

      Contour<Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
        Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
      CircleF[] contourCircles = FindPossibleCircles(contours);
      IList<SquareTUI> foundTUIs = LookForTUIs(contourCircles);

      foreach (SquareTUI sTUI in foundTUIs)
        FindMatrixValues(sTUI, contours);

      return foundTUIs;
    }

    private CircleF[] FindPossibleCircles(Contour<Point> contours)
    {
      if (contours == null)
        return null;

      ResetContoursNavigation(ref contours);

      IList<CircleF> circles = new List<CircleF>();
      for (; contours.HNext != null; contours = contours.HNext)
      {
        if (contours.Area >= minArea && contours.Area <= maxArea)
          circles.Add(new CircleF(
            new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
              contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
              contours.BoundingRectangle.Width / 2));
      }

      if (contours.Area >= minArea && contours.Area <= maxArea)
        circles.Add(new CircleF(
          new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
            contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
            contours.BoundingRectangle.Width / 2));

      return circles.ToArray();
    }

    private static void ResetContoursNavigation(ref Contour<Point> contours)
    {
      if (contours == null)
        return;

      //go back to the begining
      while (contours.HPrev != null)
        contours = contours.HPrev;
    }

    private IList<SquareTUI> LookForTUIs(CircleF[] openCvCircles)
    {
      IList<SquareTUI> tuis = new List<SquareTUI>();

      if (openCvCircles == null)
        return tuis;

      IList<TUICircle> circles = new List<TUICircle>();
      foreach (CircleF openCvCircle in openCvCircles)
      {
        TUICircle circle = new TUICircle();
        circle.Circle = openCvCircle;
        circle.Grouped = false;
        circle.IsPivot = false;
        circles.Add(circle);
      }

      foreach (TUICircle circle in circles)
      {
        if (circle.Grouped)
          continue;

        AnchoringSet validAnchor = circle.FindAnchoringSet(circles, axisLength, locationThreshold);
        if (validAnchor == null)
          continue;

        tuis.Add(new SquareTUI() { Anchor = validAnchor, Type = currentType, AxisLenght = axisLength, LocationThreshold = locationThreshold });
      }

      return tuis;
    }

    private void FindMatrixValues(SquareTUI sTUI, Contour<Point> contours)
    {
      for (int row = 0; row < 2; row++)
      {
        for (int col = 2; col < 6; col++)
        {
          PointF point = sTUI.Anchor.CalculatePoint(row, col, axisLength);
          sTUI.MatrixTUI[row, col] = ExistPoint(point, contours);
        }
      }

      for (int row = 2; row < 6; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          PointF point = sTUI.Anchor.CalculatePoint(row, col, axisLength);
          sTUI.MatrixTUI[row, col] = ExistPoint(point, contours);
        }
      }

      for (int row = 6; row < 8; row++)
      {
        for (int col = 2; col < 6; col++)
        {
          PointF point = sTUI.Anchor.CalculatePoint(row, col, axisLength);
          sTUI.MatrixTUI[row, col] = ExistPoint(point, contours);
        }
      }

      sTUI.CalculateValues();
    }

    private bool ExistPoint(PointF point, Contour<Point> contours)
    {
      ResetContoursNavigation(ref contours);
      for (; contours != null; contours = contours.HNext)
      {
        if (contours.Area > axisLength * axisLength)
          continue;
        if (contours.InContour(point) >= 0)
          return true;
      }
      return false;
    }

  }

}
