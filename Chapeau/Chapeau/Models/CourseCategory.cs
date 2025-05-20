using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chapeau.Models
{
    public enum CourseCategory
    {
        Starters,
        Entrements,
        Mains,
        Desserts,
        [Description("Soft Drinks")]
        Soft_Drinks,
        Beers,
        Wines,
        Spirits,
        [Description("Coffee/Tea")]
        Coffee_Tea
    }
}
