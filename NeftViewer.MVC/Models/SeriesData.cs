using Newtonsoft.Json;

namespace NeftViewer.MVC.Models
{
    public class SeriesData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Stack { get; set; }
        public List<int> Data { get; set; }

        public SeriesData(string name, string type, string stack, List<int> data)
        {
            Name = name;
            Type = type;
            Stack = stack;
            Data = data;
        }

        public string ToString()
        {
            return JsonConvert.SerializeObject(new { name = Name, type = Type, stack = Stack, data = Data });
        }
    }
}
