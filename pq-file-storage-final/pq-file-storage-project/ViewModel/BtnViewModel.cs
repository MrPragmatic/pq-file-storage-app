using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace pq_file_storage_project
{
    public partial class BtnViewModel : BindableObject
    {
        public BtnViewModel()
        {

        }

        private void BtnOnPointerEntered()
        {
            System.Diagnostics.Debug.WriteLine("------> BtnOnPointerEntered");
        }

        private void BtnOnPointerMoved()
        {
            System.Diagnostics.Debug.WriteLine("------> BtnOnPointerMoved");
        }

        private void BtnOnPointerExit()
        {
            System.Diagnostics.Debug.WriteLine("------> BtnOnPointerExit");
        }
    }
}
