using System;
using System.Xml;

namespace JT.MusicApp
{
    public class MusicConfig
    {
        private readonly string MusicConfigPath = "MusicApp/MusicConfig.xml";

        private XmlDocument GetXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //加载xml文件
            xmlDoc.Load(MusicConfigPath); //从指定的位置加载xml文档
            return xmlDoc;
        }
        public MusicDefault GetConfig()
        {
            XmlDocument xmlDoc = GetXmlDocument();
            XmlElement xmlRoot = xmlDoc.DocumentElement; //DocumentElement获取文档的跟
            //遍历节点
            MusicDefault musicDefault = new MusicDefault();
            foreach (XmlNode node in xmlRoot.ChildNodes)
            {
                musicDefault.Name = node["Name"].InnerText;
                musicDefault.Path = node["Path"].InnerText;
                musicDefault.IsStart = Convert.ToInt32(node["IsStart"].InnerText);
                musicDefault.Volume = Convert.ToInt32(node["Volume"].InnerText);
            }
            return musicDefault;
        }

        public bool SetStart(bool isStart)
        {
            try
            {
                XmlDocument xmlDoc = GetXmlDocument();
                XmlElement xmlRoot = xmlDoc.DocumentElement;
                foreach (XmlNode node in xmlRoot.ChildNodes)
                {
                    node["IsStart"].InnerText = (isStart ? 1 : 0).ToString();
                }
                xmlDoc.Save(MusicConfigPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
