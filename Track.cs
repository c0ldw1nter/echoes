using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

namespace Echoes
{
    public class Track
    {
        public void GetTags()
        {
            if (Program.mainWindow == null) return;
            try
            {
                if (Program.mainWindow.supportedModuleTypes.Contains(Path.GetExtension(filename).ToLower()))
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    int track = Bass.BASS_MusicLoad(filename, 0, 0, BASSFlag.BASS_MUSIC_NOSAMPLE | BASSFlag.BASS_MUSIC_PRESCAN, 0);
                    title = Bass.BASS_ChannelGetMusicName(track);
                    artist = Bass.BASS_ChannelGetMusicMessage(track);
                    length = (int)Bass.BASS_ChannelBytes2Seconds(track, (int)Bass.BASS_ChannelGetLength(track));
                    Bass.BASS_Free();
                }
                else if (Program.mainWindow.supportedMidiTypes.Contains(Path.GetExtension(filename).ToLower()))
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    int track = BassMidi.BASS_MIDI_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT, 0);
                    title = Path.GetFileName(filename);
                    length = (int)Bass.BASS_ChannelBytes2Seconds(track, (int)Bass.BASS_ChannelGetLength(track));
                    Bass.BASS_Free();
                }
                else
                {
                    TagLib.File tagFile = TagLib.File.Create(filename);
                    title = tagFile.Tag.Title;
                    if (tagFile.Tag.Performers.Length > 0)
                    {
                        artist = tagFile.Tag.Performers[0];
                    }
                    album = tagFile.Tag.Album;
                    year = unchecked((int)tagFile.Tag.Year);
                    comment = tagFile.Tag.Comment;
                    genre = tagFile.Tag.JoinedGenres;
                    trackNumber = unchecked((int)tagFile.Tag.Track);
                    length = (int)tagFile.Properties.Duration.TotalSeconds;
                    bitrate = tagFile.Properties.AudioBitrate;
                    tagFile.Dispose();
                }
                size = new FileInfo(filename).Length;
            }
            catch (Exception)
            {
                title = filename;
                length = 0;
                size = 0;
            }
            if (String.IsNullOrWhiteSpace(title))
            {
                title = Path.GetFileName(filename);
            }
            try
            {
                listened = Int32.Parse(Program.mainWindow.xmlCacher.GetListened(filename));
            }
            catch (Exception)
            {
                listened = 0;
            }
        }
        public void GetSize()
        {
            try
            {
                size = new FileInfo(filename).Length;
            }
            catch (Exception) { }
        }
        public string filename { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public int year { get; set; }
        public string comment { get; set; }
        public string genre { get; set; }
        public int length { get; set; }
        public int num { get; set; }
        public int listened { get; set; }
        public float timesListened
        {
            get
            {
                if (length <= 0) return 0;
                return (float)listened / (float)length;
            }
        }
        public int bitrate { get; set; }
        public string filesize { get; set; }
        public int trackNumber { get; set; }
        public DateTime lastOpened { get; set; }
        public long size { get; set; }
        public float trueBitrate
        {
            get
            {
                return ((float)size / length)*0.008f;
            }
        }
        public string format
        {
            get
            {
                return Path.GetExtension(filename).ToUpper();
            }
        }
        public Track(string filename, string title)
        {
            this.filename = filename;
            this.title = title;
        }
    }
}
