using System;
using System.Collections.Generic;
using System.Text;

namespace Pricimage
{
    public class HistoryItem
    {
        public HistoryItem()
        {
            TimeStamp = DateTime.Now;
        }

        public string Code { get; set; }

        public string OldPrice { get; set; }

        public string NewPrice { get; set; }

        public string Note { get; set; }

        public string ImageSource
        { 
            get
            {
                return "Images/" + Code + ".jpg";
            }
        }

        public bool IsSavedToHistory { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Content
        {
            get
            {
                string result = Code + ": ";
                if (!string.IsNullOrEmpty(OldPrice))
                {
                    result += OldPrice + " ~> ";
                }
                result += NewPrice;
                if (!string.IsNullOrEmpty(Note))
                {
                    result += ", " + Note; 
                }

                return result;
            }
        }
    }
}
