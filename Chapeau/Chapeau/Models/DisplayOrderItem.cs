using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    //pato
    public class DisplayOrderItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public CourseCategory CourseCategory { get; set; }
        public MenuCard MenuCard { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new();

        public DisplayOrderItem(string name, MenuCard menuCard, int count, CourseCategory courseCategory)
        {
            Name = name;
            MenuCard = menuCard;
            Count = count;
            CourseCategory = courseCategory;
        }

        public DisplayOrderItem() { }
    }
}
