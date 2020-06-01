using RR.Presenters;
using RR.Properties;

namespace RR.Views
{
    public class GlobalNavigationView : BaseView<GlobalNavigationPresenter, GlobalNavigationProperties>
    {
        public override void Initialize()
        {

        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 

        }

        private void RemoveListeners()
        {

        }
    }
}
