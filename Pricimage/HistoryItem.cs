using System;
using System.Collections.Generic;
using System.Text;

namespace Pricimage
{
    public class HistoryItem
    {
        public string Code { get; set; }

        public double? OldPrice { get; set; }

        public double NewPrice { get; set; }

        public string ImageSource
        { 
            get
            {
                return "/Images/" + Code + ".jpg";
            }
        }

        public string Content
        {
            get
            {
                string result = Code + ": ";
                if (OldPrice.HasValue)
                {
                    result += OldPrice.Value.ToString("n2") + " ~> ";
                }
                result += NewPrice.ToString("n2");

                return result;
            }
        }
    }
}
