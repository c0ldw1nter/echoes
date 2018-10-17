using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace Echoes
{
    public class XmlCacher
    {
        string filename;
        string backupFilename;
        XDocument xml;
        public XmlCacher(string filename)
        {
            this.filename = filename;
            LoadXml();
        }

        public void CleanupCache()
        {
            LoadXml();
            int totalBefore = xml.Root.Descendants().Count();
            var toRem = xml.Root.Descendants().Where(x => !File.Exists(x.Attribute("filename").Value));
            int removeNum = toRem.Count();
            toRem.Remove();
            SaveXml();
            if (removeNum > 0)
            {
                MessageBox.Show(removeNum + "  entries were removed from cache.");
            }
        }

        public void ClearDupes()
        {
            LoadXml();
            var elems = xml.Root.Descendants();
            foreach (XElement elem in elems)
            {
                var dupes = xml.Root.Elements().Where(x => x.Attribute("filename").Value == elem.Attribute("filename").Value && !x.Equals(elem));
                int listened=0;
                int max = 0;
                Int32.TryParse(elem.Attribute("listened").Value, out listened);
                foreach (XElement dup in dupes)
                {
                    if (Int32.TryParse(dup.Attribute("listened").Value, out listened))
                    {
                        if (listened > max) max = listened;
                    }
                }
                dupes.Remove();
            }
            xml.Save(Program.mainWindow.tagsCacheLocation);
        }

        public void AddOrUpdate(List<Track> tracks)
        {
            LoadXml();
            try
            {
                foreach (Track t in tracks)
                {
                    XElement theThing = xml.Root.Descendants().FirstOrDefault(x => x.Attribute("filename") != null && x.Attribute("filename").Value == t.filename);
                    List<XAttribute> newAttributes = new List<XAttribute>();
                    foreach (var prop in t.GetType().GetProperties())
                    {
                        var name = prop.Name;
                        if (name == "num") continue;
                        var val = prop.GetValue(t, null);
                        if (name == "listened" && val == null)
                        {
                            val = "0";
                        }
                        else if (name == "lastOpened")
                        {
                            DateTime dateVal=(DateTime)val;
                            if(dateVal==DateTime.MinValue) continue;
                            newAttributes.Add(new XAttribute(name, dateVal.Ticks.ToString()));
                            continue;
                        }
                        if (val != null)
                        {
                            newAttributes.Add(new XAttribute(name, val.ToString()));
                        }
                    }
                    if (theThing == null)
                    {
                        theThing = new XElement("track", newAttributes.ToArray());
                        xml.Root.Add(theThing);
                    }
                    else {
                        theThing.ReplaceAttributes(newAttributes.ToArray());
                    }
                }
                SaveXml();
            }
            catch (Exception) { }
        }

        public void LoadXml()
        {
            if (xml != null) return;
            try
            {
                xml = XDocument.Load(filename);
                if (xml.Root.Name != "tracks")
                {
                    xml = null;
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                xml = new XDocument();
                xml.Add(new XElement("tracks"));
            }
        }

        public string GetListened(string filename)
        {
            LoadXml();
            var ret = xml.Root.Descendants().FirstOrDefault(x => x.Attribute("filename") != null && x.Attribute("filename").Value == filename);
            if (ret == null)
            {
                return "00:00:00";
            }
            else
            {
                if (ret.Attribute("listened") != null)
                {
                    var listened = ret.Attribute("listened").Value;
                    return listened;
                }
                else
                {
                    return "00:00:00";
                }
            }
        }

        public void ChangeFilename(string oldName, string newName)
        {
            LoadXml();
            var match=xml.Root.Descendants().FirstOrDefault(x => x.Attribute("filename")!=null && x.Attribute("filename").Value == oldName);
            if (match != null)
            {
                match.Attribute("filename").Value = newName;
                SaveXml();
            }
        }

        void LoadAttributesToTrack(XElement x, Track t)
        {
            if (x.Attribute("title") != null) t.title = x.Attribute("title").Value;
            if (x.Attribute("artist") != null) t.artist = x.Attribute("artist").Value;
            if (x.Attribute("album") != null) t.album = x.Attribute("album").Value;
            if (x.Attribute("listened") != null) t.listened = Int32.Parse(x.Attribute("listened").Value);
            if (x.Attribute("bitrate") != null) t.bitrate = Int32.Parse(x.Attribute("bitrate").Value);
            if (x.Attribute("length") != null) t.length = Int32.Parse(x.Attribute("length").Value);
            if (x.Attribute("year") != null) t.year = Int32.Parse(x.Attribute("year").Value);
            if (x.Attribute("genre") != null) t.genre = x.Attribute("genre").Value;
            if (x.Attribute("comment") != null) t.comment = x.Attribute("comment").Value;
            if (x.Attribute("trackNumber") != null) t.trackNumber = Int32.Parse(x.Attribute("trackNumber").Value);
            if (x.Attribute("size") != null) t.size = Int64.Parse(x.Attribute("size").Value);
            try { if (x.Attribute("lastOpened") != null) t.lastOpened = new DateTime(Int64.Parse(x.Attribute("lastOpened").Value)); }
            catch (Exception) { }
        }

        public bool GetCacheInfo(Track t)
        {
            LoadXml();
            XElement match = xml.Root.Descendants().FirstOrDefault(x => x.Attribute("filename") != null && x.Attribute("filename").Value == t.filename);
            if (match != null)
            {
                LoadAttributesToTrack(match, t);
                return true;
            }
            return false;
        }

        public List<Track> GetAllTracks()
        {
            LoadXml();
            var trackProperties = typeof(Track).GetProperties().ToList();
            List<Track> ret = new List<Track>();
            foreach(XElement x in xml.Root.Descendants()) {
                Track t = new Track(x.Attribute("filename").Value, x.Attribute("title").Value);
                LoadAttributesToTrack(x, t);
                ret.Add(t);
            }
            ret.Reverse();
            return ret;
        }

        public void SaveXml()
        {
            try
            {
                if (xml != null)
                {
                    backupFilename=Path.ChangeExtension(filename, "bak");
                    if (File.Exists(backupFilename)) File.Delete(backupFilename);
                    File.Move(filename, backupFilename);
                    xml.Save(filename);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error with saving cache:"+Environment.NewLine+e.Message+Environment.NewLine+e.StackTrace);
                if (File.Exists(filename)) File.Delete(filename);
                File.Move(backupFilename, filename);
            }
        }
    }
}
