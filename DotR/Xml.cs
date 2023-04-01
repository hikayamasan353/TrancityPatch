using System;
using System.Globalization;
using System.Xml;

namespace Engine
{
    public class Xml
    {
        public static readonly IFormatProvider DoubleFormat = new NumberFormatInfo {NumberDecimalSeparator = "."};
        
        public static XmlDocument TryOpenDocument(string filename)
        {
        	XmlDocument document;
        	try
        	{
        		document = new XmlDocument();
        		document.Load(filename);
        	}
        	catch (Exception exc)
        	{
        		Logger.LogException(exc, "Can not load XML from file " + filename);
        		document = null;
        	}
        	return document;
        }
        
        public static bool TrySaveDocument(XmlDocument document, string filename)
        {
        	bool result;
        	try
        	{
        		document.Save(filename);
        		result = true;
        	}
        	catch (Exception exc)
        	{
        		Logger.LogException(exc, "Can not save XML to file " + filename);
        		result = false;
        	}
        	return result;
        }
        
        public static XmlElement AddElement(XmlDocument document, string name)
        {
        	return AddElement(document, document, name, string.Empty);
        }

        public static XmlElement AddElement(XmlDocument document, XmlNode parent, string name)
        {
        	return AddElement(document, parent, name, string.Empty);
        }

        public static XmlElement AddElement(XmlDocument document, XmlNode parent, string name, double value)
        {
            return AddElement(document, parent, name, value.ToString(DoubleFormat));
        }

        public static XmlElement AddElement(XmlDocument document, XmlNode parent, string name, string text)
        {
            var newChild = document.CreateElement(name);
            if (!string.IsNullOrEmpty(text))
            {
                newChild.InnerText = text;
            }
            parent.AppendChild(newChild);
            return newChild;
        }
        
        public static double GetDouble(XmlNode element)
        {
        	return GetDouble(element, 0.0);
        }

        public static double GetDouble(XmlNode element, double default_value)
        {
        	if (element != null)
        	{
	        	double value = 0.0;
	        	if (double.TryParse(element.InnerText, NumberStyles.Number, DoubleFormat, out value))
	        	{
	        		return value;
	        	}
        	}
            return default_value;
        }
        
        public static string GetString(XmlNode element)
        {
        	return GetString(element, string.Empty);
        }
        
        public static string GetString(XmlNode element, string default_value)
        {
        	if (element != null)
            {
                return element.InnerText;
            }
            return default_value;
        }
        
        public static XmlElement GetChild(XmlNode element, string name)
        {
        	try
        	{
        		return element[name];
        	}
        	catch (Exception e)
        	{
        		Logger.LogException(e, "Trying to get null child. Child name : " + name + ". URI: " + element.BaseURI);
        		return null;
        	}
        }
    }
}

