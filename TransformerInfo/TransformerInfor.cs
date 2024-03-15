using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Electroimpact
{
  public class TransformerInfo
  {
    static string basicSettingFile = XmlSerialization.Serializer.GenerateDefaultFilename("Electroimpact", "Transformer", "Basic.xml", true);


    /// <summary>
    /// Each cBarrelTransform has 13 arguments: MandrelToSpin(X, Y, Z, rX, rY, rZ), SpinToFRC(X, Y, Z, rX, rY, rZ), and Ax.
    /// </summary>
    public class cBarrelTransform
    {
      public cMandrelToSpin MandrelToSpin = new cMandrelToSpin();
      public double Ax;
      public cSpinToFRC SpinToFRC = new cSpinToFRC();
    }

    /// <summary>
    /// Each cMandrelToSpin has six arguments: X, Y, Z, rX, rY, rZ.
    /// </summary>
    public class cMandrelToSpin
    {
      public double X;
      public double Y;
      public double Z;

      public double rX;
      public double rY;
      public double rZ;
    }

    /// <summary>
    /// Each cSpinToFRC has six arguments: X, Y, Z, rX, rY, rZ.
    /// </summary>
    public class cSpinToFRC
    {
      public double X;
      public double Y;
      public double Z;

      public double rX;
      public double rY;
      public double rZ;
    }

    /// <summary>
    /// Gets the current list of tools on this machine
    /// </summary>
    /// <returns>A list of cTools</returns>
    public static List<cBarrelTransform> getListOfBarrelT()
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }
      if (!File.Exists(getBarrelTFilename()))
      {
        initializeBarrelTransforms();
      }
      List<cBarrelTransform> list = XmlSerialization.Serializer.Load<List<cBarrelTransform>>(getBarrelTFilename());
      return list;
    }

    /// <summary>
    /// Saves a list of cTools
    /// </summary>
    /// <param name="List">A list of all the cTools</param>
    public static void SaveListOfBarrelT(List<cBarrelTransform> List)
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }

      XmlSerialization.Serializer.Save(List, getBarrelTFilename());
    }

    /// <summary>
    /// Each tool consists of a list of cPoints, a toolName, and which M38S it used last.
    /// </summary>
    public class cTool
    {
      public List<cPoint> Points = new List<cPoint>();

      public string toolName;

      public int M38S;
    }

    /// <summary>
    /// Gets the current list of tools on this machine
    /// </summary>
    /// <returns>A list of cTools</returns>
    public static List<cTool> getListOfTools()
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }
      List<cTool> list = XmlSerialization.Serializer.Load<List<cTool>>(getToolFilename());
      if (list == null)
        list = new List<cTool>();
      return list;
    }

    /// <summary>
    /// Saves a list of cTools
    /// </summary>
    /// <param name="List">A list of all the cTools</param>
    public static void SaveListOfTools(List<cTool> List)
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }

      XmlSerialization.Serializer.Save(List, getToolFilename());
    }

    /// <summary>
    /// Consists of the nominal and last measured X, Y, and Z coordinates.
    /// </summary>
    public class cPoint
    {
      public double nomX;
      public double nomY;
      public double nomZ;

      public double LmX;
      public double LmY;
      public double LmZ;

      public bool isUsed;
    }

    /// <summary>
    /// Saves a list of Transforms name's
    /// </summary>
    /// <param name="Transforms">A list of all the transforms name's</param>
    public static void saveTransformNames(List<string> Transforms)
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }

      XmlSerialization.Serializer.Save(Transforms, getTransformFilename());

    }

    /// <summary>
    /// Gets the current list of all transforms name's
    /// </summary>
    /// <returns>a list of strings consisting of all the transform's names</returns>
    public static List<string> getListOfTransforms()
    {
      if (!File.Exists(basicSettingFile))
      {
        XmlSerialization.Serializer.Save(new basicSettings(), basicSettingFile);
      }
      if (!File.Exists(getTransformFilename()))
      {
        initializeTransformNames();
      }
      List<string> list = XmlSerialization.Serializer.Load<List<string>>(getTransformFilename());
      return list;
    }

    /// <summary>
    /// Sets the filenames for the list of tools and transforms name's.
    /// </summary>
    public class basicSettings
    {
      public string toolList = XmlSerialization.Serializer.GenerateDefaultFilename("Electroimpact", "Transformer", "Tools.xml", true);

      public string transformNames = XmlSerialization.Serializer.GenerateDefaultFilename("Electroimpact", "Transformer", "TransformNames.xml", true);

      public string barrelTransforms = XmlSerialization.Serializer.GenerateDefaultFilename("Electroimpact", "Transformer", "BarrelTransforms.xml", true);
    }

    /// <summary>
    /// Gets the file path of where the list of tools is currently being stored.
    /// </summary>
    /// <returns>file path of list of tools</returns>
    public static string getToolFilename()
    {
      basicSettings basicFiles = XmlSerialization.Serializer.Load<basicSettings>(basicSettingFile);
      return basicFiles.toolList;

    }

    /// <summary>
    /// Gets the file path of where the list of tools is currently being stored.
    /// </summary>
    /// <returns>file path of list of tools</returns>
    public static string getBarrelTFilename()
    {
      basicSettings basicFiles = XmlSerialization.Serializer.Load<basicSettings>(basicSettingFile);
      return basicFiles.barrelTransforms;

    }

    /// <summary>
    /// Gets the file path of where the list of transform names is currently being stored.
    /// </summary>
    /// <returns>file path of list of transform names</returns>
    public static string getTransformFilename()
    {
      basicSettings basicFiles = XmlSerialization.Serializer.Load<basicSettings>(basicSettingFile);
      return basicFiles.transformNames;
    }

    private static void initializeTransformNames()
    {
      List<string> TNames = new List<string>();

      for (int i = 0; i < 18; i++)
      {
        TNames.Add("Not Used");
      }
      saveTransformNames(TNames);
    }

    private static void initializeBarrelTransforms()
    {
      List<cBarrelTransform> BTNames = new List<cBarrelTransform>();

      for (int i = 0; i < 8; i++)
      {
        cBarrelTransform BTName = new cBarrelTransform();

        BTName.MandrelToSpin.X =
        BTName.MandrelToSpin.Y =
        BTName.MandrelToSpin.Z =
        BTName.MandrelToSpin.rX =
        BTName.MandrelToSpin.rY =
        BTName.MandrelToSpin.rZ =
        BTName.SpinToFRC.X =
        BTName.SpinToFRC.Y =
        BTName.SpinToFRC.Z =
        BTName.SpinToFRC.rX =
        BTName.SpinToFRC.rY =
        BTName.SpinToFRC.rZ =
        BTName.Ax = 0;

        BTNames.Add(BTName);
      }

      SaveListOfBarrelT(BTNames);
    }
  }
}
