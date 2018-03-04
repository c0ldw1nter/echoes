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
        /*public static void SaveTagsToXml(List<Track> tracks) {
            XDocument xml;
            try
            {
                xml = XDocument.Load(Program.mainWindow.tagsCacheLocation);
            }
            catch (Exception) 
            {
                xml = new XDocument();
                xml.Add(new XElement("tracks"));
            }
            foreach (Track t in tracks)
            {
                if (xml.Root.Descendants().Where(x => x.Value == t.filename).Any()) continue;
                XElement toAdd=new XElement("track");
                foreach (var prop in t.GetType().GetProperties())
                {
                    var name = prop.Name;
                    if (name == "num") continue;
                    var val=prop.GetValue(t, null);
                    
                    if (name == "listened" && val == null)
                    {
                        val = "00:00:00";
                    }
                    if (val != null)
                    {
                        toAdd.Add(new XAttribute(name, val.ToString()));
                    }
                }
                //if (!String.IsNullOrWhiteSpace(t.title)) toAdd.Add(new XAttribute("title", t.title));
                //if (!String.IsNullOrWhiteSpace(t.artist)) toAdd.Add(new XAttribute("artist", t.artist));
                //if (!String.IsNullOrWhiteSpace(t.album)) toAdd.Add(new XAttribute("album", t.album));
                //if (!String.IsNullOrWhiteSpace(t.listened)) toAdd.Add(new XAttribute("listened", t.listened));
                //else toAdd.Add(new XAttribute("listened", "00:00:00"));
                toAdd.Value = t.filename;
                xml.Root.Add(toAdd);
            }
            xml.Save(Program.mainWindow.tagsCacheLocation);
            xml = null;
        }*/
        XDocument xml;
        public XmlCacher(string filename)
        {
            this.filename = filename;
            LoadXml();
        }

        public void CleanupCache()
        {
            XDocument xml;
            try
            {
                xml = XDocument.Load(Program.mainWindow.tagsCacheLocation);
            }
            catch (Exception)
            {
                return;
            }
            var toRem = xml.Root.Descendants().Where(x => !File.Exists(x.Attribute("filename").Value));
            int removeNum = toRem.Count();
            toRem.Remove();
            xml.Save(Program.mainWindow.tagsCacheLocation);
            if (removeNum > 0)
            {
                MessageBox.Show(removeNum + "  entries were removed from cache.");
            }
        }

        public void BringToTop(string filename)
        {

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
                    if (theThing != null)
                    {
                        theThing.Remove();
                    }
                    theThing = new XElement("track");
                    foreach (var prop in t.GetType().GetProperties())
                    {
                        var name = prop.Name;
                        if (name == "num") continue;
                        var val = prop.GetValue(t, null);
                        if (name == "listened" && val == null)
                        {
                            val = "0";
                        }
                        if (val != null)
                        {
                            theThing.Add(new XAttribute(name, val.ToString()));
                        }
                    }
                    xml.Root.Add(theThing);
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
            if (x.Attribute("timesLoaded") != null) t.timesLoaded = Int32.Parse(x.Attribute("timesLoaded").Value);
            if (x.Attribute("playthrough") != null) t.playthrough = float.Parse(x.Attribute("playthrough").Value);
            if (x.Attribute("year") != null) t.year = Int32.Parse(x.Attribute("year").Value);
            if (x.Attribute("genre") != null) t.genre = x.Attribute("genre").Value;
            if (x.Attribute("comment") != null) t.comment = x.Attribute("comment").Value;
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
                /*foreach (XAttribute att in x.Attributes())
                {
                    System.Reflection.PropertyInfo prop=trackProperties.FirstOrDefault(z=>z.Name==att.Name);
                    if (prop!=null)
                    {
                        object val = prop.GetValue(t, null);
                        try
                        {
                            switch (Type.GetTypeCode(prop.GetType()))
                            {
                                case TypeCode.Int32:
                                    prop.SetValue(t, Int32.Parse(att.Value));
                                    break;
                                case TypeCode.Single:
                                    prop.SetValue(t, float.Parse(att.Value));
                                    break;
                                default:
                                    prop.SetValue(t, att.Value);
                                    break;
                            }
                        }
                        catch (Exception) { }
                    }
                }*/
                //GetCacheInfo(t);
                ret.Add(t);
            }
            ret.Reverse();
            return ret;
        }

        //returns a list of tracks whose cache wasn't found
        public List<Track> LoadTagsToPlaylist(List<Track> tracks)
        {
            List<Track> uncachedTracks = new List<Track>();
            LoadXml();
            foreach (Track t in tracks)
            {
                XElement match = xml.Root.Descendants().FirstOrDefault(x => x.Attribute("filename")!=null && x.Attribute("filename").Value == t.filename);
                if (match != null)
                {
                    if(match.Attribute("title")!=null) t.title = match.Attribute("title").Value;
                    if (match.Attribute("artist") != null) t.artist = match.Attribute("artist").Value;
                    if (match.Attribute("album") != null) t.album = match.Attribute("album").Value;
                    if (match.Attribute("listened") != null) t.listened = Int32.Parse(match.Attribute("listened").Value);
                    if (match.Attribute("bitrate") != null) t.bitrate = Int32.Parse(match.Attribute("bitrate").Value);
                    if (match.Attribute("length") != null) t.length = Int32.Parse(match.Attribute("length").Value);
                }
                else uncachedTracks.Add(t);
            }
            return uncachedTracks;
        }

        public void ReleaseXml()
        {
            xml = null;
        }

        public void SaveXml()
        {
            if (xml != null) xml.Save(filename);
        }
    }
}
