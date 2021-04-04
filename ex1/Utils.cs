using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using System.Windows;
namespace ex1
{
    public class FileDetails
    {
        public string Path, Name;
        public string FullPath { get { return Path + "\\" + Name; } }
        public FileDetails(string Path, string Name)
        {
            this.Path = Path;
            this.Name = Name;
        }
    }
    class Utils
    {
        class Pair
        {
            public int x, y;
            public Pair(int x, int y) { this.x = x; this.y = y; }
        }
        private static List<XmlElement> GetElementsChilds(XmlNodeList nd, string nameFilter)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (var child in nd)
            {
                XmlElement element = child as XmlElement;
                if (element != null && element.Name == nameFilter)
                {
                    list.Add(child as XmlElement);
                }
            }
            return list;
        }
        private static List<XmlElement> GetElementsChilds(List<XmlElement> elist, string nameFilter)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (var child in elist)
            {
                list.AddRange(GetElementsChilds(child.ChildNodes, nameFilter));
            }
            return list;
        }
        public static string ParseFeaturesLine(string xmlFile)
        {
            // root -> PropertyList -> generic -> input -> chunk -> name
            var xml = new XmlDocument();
            xml.Load(new System.IO.StreamReader(xmlFile)); // root
            var childs = GetElementsChilds(xml.ChildNodes, "PropertyList");
            childs = GetElementsChilds(childs, "generic");
            childs = GetElementsChilds(childs, "input");
            childs = GetElementsChilds(childs, "chunk");
            childs = GetElementsChilds(childs, "name");

            List<string> features = new List<string>();
            // <feature, <x=total, y=current>>
            Dictionary<string, Pair> amountOfFeaure = new Dictionary<string, Pair>();
            foreach (var nameElement in childs)
            {
                features.Add(nameElement.InnerText);
                if (amountOfFeaure.ContainsKey(nameElement.InnerText))
                {
                    amountOfFeaure[nameElement.InnerText].x++;
                }
                else
                {
                    amountOfFeaure.Add(nameElement.InnerText, new Pair(0, 0));
                }
            }


            String[] array = features.ToArray();
            for (int i = 0; i < array.Length; i++)
                if (amountOfFeaure[array[i]].x > 0)
                    array[i] = array[i] + "[" + (amountOfFeaure[array[i]].y++) + "]";

            return String.Join(",", array);
        }

        // example: var x = GetFileDetailsFromUserGUI("xml file", "*.xml");
        public static FileDetails GetFileDetailsFromUserGUI(string descriprion, string extensionFilter)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = descriprion + "|" + extensionFilter;
            openFile.InitialDirectory = Environment.GetEnvironmentVariable("HomeDrive");
            openFile.Title = "Choose " + descriprion;
            openFile.ShowDialog();
            string fullPath = openFile.FileName;
            string[] temp = fullPath.Split('\\');
            if (fullPath == "" || !openFile.CheckFileExists)
                return null;
            return new FileDetails(fullPath + "\\..", temp[temp.Length - 1]);
        }
    }
}
