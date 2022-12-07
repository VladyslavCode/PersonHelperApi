using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonHelperApi.Classes
{
    public static class Helper
    {
        public static void NumberFromString(string numberstr, out int number, out bool IsDigit)
        {
            IsDigit = numberstr.ToCharArray().All(x => Char.IsDigit(x));
            number = 0;
            IsDigit = IsDigit ? int.TryParse(numberstr, out number) : IsDigit;
        }
    }
}
