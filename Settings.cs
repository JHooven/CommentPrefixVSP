using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CommentPrefixVSP
{
    [Serializable]
    public class Settings
    {
        public string Prefix { get; set; }

        public string DateFormat { get; set; }

        public List<CommentToken> CommentTokens { get; set; }

        public Settings()
        {
            LoadDefaultSettings();
        }

        public void Save()
        {
            string fileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommentPrefix");
            string fileName = Path.Combine(fileFolder, "Settings.xml");

            XmlSerializer xs = new XmlSerializer(typeof(Settings));

            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                xs.Serialize(fs, this);
            }
        }

        private void LoadDefaultSettings()
        {
            // JEH 17-Dec-2016 <-- default date format.
            DateFormat = "dd-MMM-yyyy ";
            Prefix = "JEH ";

            if (CommentTokens == null)
            {
                CommentTokens = new List<CommentToken>();
            }

            CommentTokens.Add(new CommentToken(".c", "/* */"));
            CommentTokens.Add(new CommentToken(".cpp", "//"));
            CommentTokens.Add(new CommentToken(".h", "//"));
            CommentTokens.Add(new CommentToken(".hpp", "//"));
            CommentTokens.Add(new CommentToken(".htm", "<!-- -->"));
            CommentTokens.Add(new CommentToken(".html", "<!-- -->"));
            CommentTokens.Add(new CommentToken(".xaml", "<!-- -->"));
            CommentTokens.Add(new CommentToken(".xml", "<!-- -->"));
            CommentTokens.Add(new CommentToken(".py", "#"));
            CommentTokens.Add(new CommentToken("<default>", "//"));
        }
    }
}
