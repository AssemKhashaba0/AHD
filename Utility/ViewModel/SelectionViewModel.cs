using Models;
using System.Collections.Generic;

namespace AHD.ViewModels
{
    public class SelectionViewModel
    {
        public List<City> Cities { get; set; }
        public List<Mosque> Mosques { get; set; }
        public List<Cemetery> Cemeteries { get; set; }
        public int? SelectedCityId { get; set; }
        public List<int> SelectedMosques { get; set; } = new();
        public List<int> SelectedCemeteries { get; set; } = new();
    }
}
