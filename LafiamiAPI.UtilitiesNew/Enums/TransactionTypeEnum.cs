using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum TransactionTypeEnum
    {
        Credit = 1,
        Debit,
        [Display(Name = "Book Credit")]
        BookCredit
    }
}
