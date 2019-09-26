using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Apprenticeships.Interfaces.Models
{
    public interface IBulkUploadError
    {
        int LineNumber { get; set; }
        string Header { get; set; }
        string Error { get; set; }
    }
}
