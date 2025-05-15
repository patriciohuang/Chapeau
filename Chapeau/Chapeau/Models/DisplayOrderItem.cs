using Chapeau.Enums;

namespace Chapeau.Models
{
    //pato
    public class DisplayOrderItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public CourseCategory CourseCategory { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new();

        public DisplayOrderItem(string name, int count, CourseCategory courseCategory)
        {
            Name = name;
            Count = count;
            CourseCategory = courseCategory;
        }

        public DisplayOrderItem() { }
    }
}
