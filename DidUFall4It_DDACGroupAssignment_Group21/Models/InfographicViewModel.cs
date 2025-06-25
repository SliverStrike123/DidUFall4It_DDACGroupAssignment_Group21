using AspNetCoreGeneratedDocument;
using Microsoft.IdentityModel.Tokens;

namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class InfographicViewModel
    {
        private List<InfographicModel> InfographicsList { get; set; } = new List<InfographicModel>();

        private int TotalCount
        {
            get
            {
                return InfographicsList != null ? InfographicsList.Count : 0;
            }
        }

        public InfographicViewModel(List<InfographicModel> model)
        {
            InfographicsList = model;
        }

        public void Add(InfographicModel model)
        {
            if (model != null)
            {
                InfographicsList.Add(model);
            }
        }

        public List<InfographicModel> GetList()
        {
            return InfographicsList;
        }
        public void SetList(List<InfographicModel> model)
        {
            InfographicsList = model;
        }
        public int GetTotalCount()
        {
            return TotalCount;
        }

    }
}
