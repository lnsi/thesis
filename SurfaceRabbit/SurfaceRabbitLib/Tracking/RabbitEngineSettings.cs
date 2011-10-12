using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SurfaceRabbit.Tracking
{

  public class RabbitEngineSettings
  {
    public float pContourThreshold;
    public float pAxisLength;
    public float pLocationThreshold;
    public float pMinArea;
    public float pMaxArea;

    public float lContourThreshold;
    public float lAxisLength;
    public float lLocationThreshold;
    public float lMinArea;
    public float lMaxArea;

    public bool supportPTUIs;
    public bool supportLTUIs;

    public RabbitEngineSettings()
    {
      supportLTUIs = true;
      supportPTUIs = true;

      pContourThreshold = 70;
      pAxisLength = 85;
      pLocationThreshold = pAxisLength / 10;
      pMinArea = (float)(5 * 5 * Math.PI);
      pMaxArea = (float)(8 * 8 * Math.PI);

      lContourThreshold = 60;
      lAxisLength = 97;
      lLocationThreshold = lAxisLength / 10;
      lMinArea = (float)(5 * 5 * Math.PI);
      lMaxArea = (float)(8 * 8 * Math.PI);
    }

    public void Load(ApplicationSettingsBase settings)
    {
      supportPTUIs = (bool)settings["SupportPaper"];
      pContourThreshold = (float)settings["pContourThreshold"];
      pAxisLength = (float)settings["pAxisLength"];
      pLocationThreshold = (float)settings["pLocationThreshold"];
      pMinArea = (float)settings["pMinArea"];
      pMaxArea = (float)settings["pMaxArea"];

      supportLTUIs = (bool)settings["SupportLight"];
      lContourThreshold = (float)settings["lContourThreshold"];
      lAxisLength = (float)settings["lAxisLength"];
      lLocationThreshold = (float)settings["lLocationThreshold"];
      lMinArea = (float)settings["lMinArea"];
      lMaxArea = (float)settings["lMaxArea"];
    }

  }

}
