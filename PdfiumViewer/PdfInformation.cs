using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    /// <summary>
    /// Contains text from metadata of the document.
    /// </summary>
    public class PdfInformation
    {
        public string Author { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Keywords { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Producer { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }

        public Dictionary<string, string> toDictionary()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("title", this.Title);
            dic.Add("subject", this.Subject);
            dic.Add("author", this.Author);
            dic.Add("creator", this.Creator);

            if (this.CreationDate == null)
                dic.Add("creation_date", string.Empty);
            else
                dic.Add("creation_date", ((DateTime)this.CreationDate).ToString("yyyyMMddHHmmss"));

            if (this.ModificationDate == null)
                dic.Add("modification_date", string.Empty);
            else
                dic.Add("modification_date", ((DateTime)this.ModificationDate).ToString("yyyyMMddHHmmss"));

            dic.Add("keywords", this.Keywords);
            dic.Add("producer", this.Producer);
            return dic;
        }
    }
}
