using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Models.Internals
{
    public class AmountObjectModel
    {
        public decimal Amount { get; set; }
        public MoneyUnitEnum MoneyUnit { get; set; }
    }
}
