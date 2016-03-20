using System.Collections.ObjectModel;

namespace TravellingSalesmanUIwpf.Concretes
{
    public class SelectOperatorList : ObservableCollection<string>
    {
        public SelectOperatorList()
        {
            Add("Tournament selection");
            Add("Proportional selection");
        }
    }
}
